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
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;

namespace StarMap.Database
{
    /// <summary>
    /// A small class for reporting systems loading progress.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SystemsLoadProgressEventArgs : EventArgs
    {
        public bool AllComplete { get; private set; }
        public bool IsSol { get; private set; }
        public SystemBase MostRecentSystem { get; private set; }
        public float ReadSystems { get; private set; }
        public float TotalSystems { get; private set; }

        public SystemsLoadProgressEventArgs(float index, float total, SystemBase mostRecent, bool complete = false, bool isSol = false)
        {
            TotalSystems = total;
            ReadSystems = index;

            AllComplete = complete;
            MostRecentSystem = mostRecent;
            IsSol = isSol;
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format("SystemsLoadProgress: {0:0,0} loaded of {1:0,0} ({2:0%})", ReadSystems, TotalSystems, ReadSystems / TotalSystems);
            }
        }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct SystemBase
    {
        public long EdsmId { get; private set; }
        public string Name { get; private set; }
        public Vector3 Position { get; private set; }

        public SystemBase(long edsmId, string name, Vector3 position)
        {
            EdsmId = edsmId;
            Name = name;
            Position = position;
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"System #{EdsmId}: '{Name}', {Position.ToString()}";
            }
        }
    }

    public static class Systems
    {
        public static bool IsLoaded { get; private set; } = false;
        public static bool IsLoading { get; private set; } = false;
        private static SystemBase _sol = new SystemBase(-1, "Sol", Vector3.Zero);
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
                using (DbCommand cmd = cn.CreateCommand("SELECT COUNT(EdsmId) FROM EdsmSystems;"))
                    totalCount = Convert.ToSingle(cmd.ExecuteScalar());

                lock (SystemsList)
                {
                    using (DbCommand cmd = cn.CreateCommand("SELECT s.EdsmId, n.Name, s.X, s.Y, s.Z FROM EdsmSystems s JOIN SystemNames n ON s.EdsmId=n.EdsmId;"))
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (!bgw.CancellationPending && reader.Read())
                        {
                            SystemBase sb = new SystemBase((long)reader[0], (string)reader[1], new Vector3((float)((long)reader[2] / 128), (float)((long)reader[4] / 128), -(float)((long)reader[3] / 128)));
                            SystemsList.Add(sb);
                            processedCount++;
                            if (sb.EdsmId == 27)
                            {
                                _sol = sb;
                                bgw.ReportProgress((int)((processedCount / totalCount) * 100), new SystemsLoadProgressEventArgs(processedCount, totalCount, _sol, isSol: true));
                            }
                            else if (processedCount % 65535 == 0) // Modulus is pretty arbitrary. Currently around 7.8M located systems, 78k ≈ 1%...
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

        public static SystemBase GetSystem(string systemName)
        {
            if (!IsLoaded || IsLoading || SystemsList == null)
                throw new InvalidOperationException("SystemBase.TryGetSystem() cannot be used before loading has finished.");

            return SystemsList.FindLast(s => s.Name.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
