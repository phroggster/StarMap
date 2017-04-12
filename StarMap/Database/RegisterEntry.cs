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
using System;
using System.Drawing;

namespace StarMap.Database
{
    public class RegisterEntry
    {
        public bool ValueBool { get { return ValueLong != 0; } }

        public Color ValueColor { get { return Color.FromArgb((int)ValueLong); } }

        public int ValueInt { get { return (int)ValueLong; } }

        public double ValueDouble { get; private set; }
        public long ValueLong { get; private set; }
        public string ValueString { get; private set; }

        protected RegisterEntry() { }

        public RegisterEntry(string stringval = null, long longval = 0, double floatval = Double.NaN)
        {
            ValueString = stringval;
            ValueLong = longval;
            ValueDouble = floatval;
        }
    }
}
