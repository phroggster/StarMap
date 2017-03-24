using OpenTK;
using StarMap.Renderables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarMap.Objects
{
    public class StarsObject : AObject
    {
        public StarsObject(ARenderable model)
            : base(model, Vector4.Zero, Quaternion.Identity, Vector3.One) { }
    }
}
