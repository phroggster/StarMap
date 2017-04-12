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

#region --- using ... ---
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion // --- using ... ---

namespace StarMap.Database
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct NamedSystem
    {
        public long EdsmId { get; private set; }
        public string Name { get; private set; }
        public Vector3 Position { get; private set; }

        public NamedSystem(long edsmId, string name, Vector3 position)
        {
            EdsmId = edsmId;
            Name = name;
            Position = position;
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{nameof(SystemBase)} #{EdsmId}: '{Name}', {Position.ToString()}";
            }
        }
    }
}
