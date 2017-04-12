#region --- Apache v2.0 license ---
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
#endregion // --- Apache v2.0 license ---

namespace StarMap.Database
{
    public class EDDSystemsDBConnection : ADBConnection<EDDSystemsDBConnection>
    {
        public EDDSystemsDBConnection() : base(DBSelection.EDDSystem) { }

        public EDDSystemsDBConnection(DBAccessMode mode = DBAccessMode.Readonly) : this() { }

        protected EDDSystemsDBConnection(bool initializing, DBAccessMode mode = DBAccessMode.Readonly) : base(DBSelection.EDDSystem, initializing: initializing) { }

        public static void Initialize()
        {
            // This is used to upgrade the database schemas. I don't want to, not this DB.
            InitializeIfNeeded(() => { });
        }
    }
}
