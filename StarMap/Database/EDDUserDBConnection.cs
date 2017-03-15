/*
 * Copyright © 2017 phroggie <phroggster@gmail.com>, StarMap development team
 * Copyright © 2015 - 2016 EDDiscovery development team
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
using System.Collections.Generic;
using System.IO;

namespace StarMap.Database
{
    public class EDDUserDBConnection : ADBConnection<EDDUserDBConnection>
    {
        public EDDUserDBConnection() : base(DBSelection.EDDUser) { }

        public EDDUserDBConnection(DBAccessMode mode = DBAccessMode.Readonly) : base(DBSelection.EDDUser) { }

        public EDDUserDBConnection(bool initializing, DBAccessMode mode = DBAccessMode.Readonly) : base(DBSelection.EDDUser, initializing: initializing) { }

        public static void Initialize()
        {
            // This is used to upgrade the database schemas. I don't want to, not this DB.
            InitializeIfNeeded(() => { });
        }

        public static void EarlyReadRegister()
        {
            EarlyRegister = EarlyGetRegister();
        }


        private static Dictionary<string, RegisterEntry> EarlyGetRegister()
        {
            Dictionary<string, RegisterEntry> reg = new Dictionary<string, RegisterEntry>();
            if (File.Exists(GetSQLiteDBFile(DBSelection.EDDUser)))
                using (EDDUserDBConnection conn = new EDDUserDBConnection(true, DBAccessMode.Readonly))
                    conn.GetRegister(reg);
            return reg;
        }
    }
}
