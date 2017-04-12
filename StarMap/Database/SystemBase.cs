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
using System.Diagnostics;
using System.Drawing;
#endregion // --- using ... ---

namespace StarMap.Database
{
    /// <summary>
    /// A de minimus solar system representation for long-term retention in memory.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct SystemBase
    {
        public long EdsmId { get; private set; }
        public Vector3 Position { get; private set; }

        public SystemBase(long edsmId, Vector3 position)
        {
            EdsmId = edsmId;
            Position = position;
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{nameof(SystemBase)} #{EdsmId}: {Position.ToString()}";
            }
        }
    }
}
