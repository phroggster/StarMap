using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
