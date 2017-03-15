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
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace StarMap.Database
{
    public class SMDBConnection : ADBConnection<SMDBConnection>
    {
        #region Public interface

        public SMDBConnection() : base(DBSelection.StarMap) { }

        public SMDBConnection(DBAccessMode mode = DBAccessMode.Indeterminate) : base(DBSelection.StarMap) { }

        public static void Initialize()
        {
            InitializeIfNeeded(() =>
            {
                using (SMDBConnection conn = new SMDBConnection(true, DBAccessMode.ReadWrite))
                    UpgradeStarMapDB(conn);
            });
        }

        public static void EarlyReadRegister()
        {
            EarlyRegister = EarlyGetRegister();
        }

        #endregion // Public interface


        #region Protected implementation


        protected static bool UpgradeStarMapDB(SMDBConnection conn)
        {
            int dbver;
            try
            {
                conn.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS Register (ID TEXT PRIMARY KEY NOT NULL, ValueInt INTEGER, ValueDouble DOUBLE, ValueString TEXT, ValueBlob BLOB)");
                dbver = conn.GetSettingIntCN("DBVer", 1);

                //CreateUserDBTableIndexes(conn);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("UpgradeStarMapDB error: " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
            }
        }


        protected SMDBConnection(bool initializing, DBAccessMode mode = DBAccessMode.Indeterminate) : base(DBSelection.StarMap, initializing) { }


        #endregion // Protected implementation



        private static Dictionary<string, RegisterEntry> EarlyGetRegister()
        {
            Dictionary<string, RegisterEntry> reg = new Dictionary<string, RegisterEntry>();
            if (File.Exists(GetSQLiteDBFile(DBSelection.EDDUser)))
                using (SMDBConnection conn = new SMDBConnection(true, DBAccessMode.Readonly))
                    conn.GetRegister(reg);
            return reg;
        }
    }
}
