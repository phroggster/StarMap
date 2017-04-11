#region --- Apache v2.0 license ---
/*
 * Copyright © 2017 phroggie <phroggster@gmail.com>, StarMap development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */
#endregion // --- Apache v2.0 license ---

using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarMap.Database
{
    #region --- public class SystemsLoadProgressEventArgs : EventArgs ---

    /// <summary>
    /// A small class for reporting systems loading progress.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SystemsLoadProgressEventArgs : EventArgs
    {
        public bool AllComplete { get; private set; }
        public SystemBase MostRecentSystem { get; private set; }
        public float ReadSystems { get; private set; }
        public float TotalSystems { get; private set; }

        public SystemsLoadProgressEventArgs(float index, float total, SystemBase mostRecent, bool complete = false)
        {
            TotalSystems = total;
            ReadSystems = index;

            AllComplete = complete;
            MostRecentSystem = mostRecent;
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format("SystemsLoadProgress: {0:0,0} loaded of {1:0,0} ({2:0%})", ReadSystems, TotalSystems, ReadSystems / TotalSystems);
            }
        }
    }

    #endregion // --- public class SystemsLoadProgressEventArgs : EventArgs ---

    #region --- public class SystemsManager ---

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SystemsManager
    {
        public static bool IsLoaded { get; private set; } = false;
        public static bool IsLoading { get; private set; } = false;
        public static SystemBase Sol { get { return _sol; } }
        public static List<SystemBase> SystemsList { get; private set; }

        public static void LoadBGW(BackgroundWorker bgw)
        {
            if (IsLoaded || IsLoading)
                return;

            SystemsList = new List<SystemBase>();
            IsLoading = true;

            float processedCount = 0;
            float totalCount = 0;
            using (EDDSystemsDBConnection cn = new EDDSystemsDBConnection())
            {
                using (DbCommand cmd = cn.CreateCommand("SELECT COUNT(EdsmId) FROM EdsmSystems"))
                    totalCount = Convert.ToSingle(cmd.ExecuteScalar());

                lock (SystemsList)
                {
                    using (DbCommand cmd = cn.CreateCommand("SELECT EdsmId, X, Y, Z FROM EdsmSystems"))
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (!bgw.CancellationPending && reader.Read())
                        {
                            SystemBase sb = new SystemBase((long)reader[0], new Vector3((float)((long)reader[1] / 128), (float)((long)reader[3] / 128), -(float)((long)reader[2] / 128)));
                            SystemsList.Add(sb);
                            processedCount++;
                            // Modulus is pretty arbitrary. Currently around 7.8M located systems, 78k ≈ 1%...
                            if (processedCount % 65535 == 0 || sb.EdsmId == 27)
                            {
                                bgw.ReportProgress((int)((processedCount / totalCount) * 100), new SystemsLoadProgressEventArgs(processedCount, totalCount, sb));
                            }
                        }
                    }
                }
            }

            if (!bgw.CancellationPending)
            {
                IsLoaded = true;
                bgw.ReportProgress(100, new SystemsLoadProgressEventArgs(totalCount, totalCount, SystemsList[SystemsList.Count - 1], true));
            }
            else
            {
                SystemsList.Clear();
                GC.Collect();
                SystemsList = null;
            }
            IsLoading = false;
        }

        public static NamedSystem GetSystem(string systemName)
        {
            if (!IsLoaded || IsLoading || SystemsList == null)
                throw new InvalidOperationException("SystemBase.TryGetSystem() cannot be used before loading has finished.");

            NamedSystem ns = new NamedSystem();

            using (EDDSystemsDBConnection cn = new EDDSystemsDBConnection())
            using (DbCommand cmd = cn.CreateCommand("SELECT a.EdsmId, b.Name, a.X, a.Y, a.Z FROM EdsmSystems a JOIN SystemNames b ON a.EdsmId=b.EdsmId WHERE b.Name=@name LIMIT 1"))
            {
                cmd.AddDirectionalParam("@name", ParameterDirection.Input, DbType.String, systemName);
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ns = new NamedSystem((long)reader[0], (string)reader[1], new Vector3((float)((long)reader[2] / 128), (float)((long)reader[4] / 128), (float)((long)reader[3] / 128)));
                    }
                }
            }

            return ns;
        }


        protected virtual string DebuggerDisplay
        {
            get
            {
                string loaded = IsLoaded ? "Loaded" : (IsLoading ? "Loading" : string.Empty);
                int sysCount = SystemsList != null ? SystemsList.Count : -1;
                return string.Format("{0}: {1}, {2:0,0} systems", nameof(SystemsManager), loaded, sysCount);
            }
        }

        private static SystemBase _sol = new SystemBase(27, Vector3.Zero);
    }

    #endregion // --- public class SystemsManager ---
}
