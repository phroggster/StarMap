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

namespace StarMap.Database
{
    /// <summary>
    /// A small class for reporting systems loading progress.
    /// </summary>
    public class SystemsLoadingProgress
    {
        public bool AllComplete { get; private set; }
        public string MostRecentSystem { get; private set; }
        public float ReadSystems { get; private set; }
        public float TotalSystems { get; private set; }

        public SystemsLoadingProgress(float index, float total, string mostRecent, bool complete = false)
        {
            AllComplete = complete;
            ReadSystems = index;
            TotalSystems = total;
            MostRecentSystem = mostRecent;
        }
    }

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
    }

    public static class Systems
    {
        public static bool IsLoaded { get; private set; } = false;
        public static bool IsLoading { get; private set; } = false;
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

                using (DbCommand cmd = cn.CreateCommand("SELECT s.EdsmId, n.Name, s.X, s.Y, s.Z FROM EdsmSystems s JOIN SystemNames n ON s.EdsmId=n.EdsmId;"))
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    lock (SystemsList)
                    {
                        while (!bgw.CancellationPending && reader.Read())
                        {
                            SystemsList.Add(new SystemBase((long)reader[0], (string)reader[1], new Vector3((float)((long)reader[2] / 128), (float)((long)reader[3] / 128), (float)((long)reader[4] / 128))));
                            processedCount++;
                            if (processedCount % 50000 == 0) // Modulus is pretty arbitrary. Currently around 7.5M systems, 75k ~= 1%...
                                bgw.ReportProgress((int)((processedCount / totalCount) * 100), new SystemsLoadingProgress(processedCount, totalCount, (string)reader[1]));
                        }
                    }
                }
            }

            if (!bgw.CancellationPending)
            {
                IsLoaded = true;
                bgw.ReportProgress(100, new SystemsLoadingProgress(totalCount, totalCount, SystemsList[SystemsList.Count - 1].Name, true));
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
