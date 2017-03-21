using OpenTK;
using StarMap.Renderables;

namespace StarMap.Objects
{
    public class AxisObject : AObject
    {
        public AxisObject(ARenderable model, Vector3 scale)
            : base(model, Vector4.Zero, Vector4.Zero, scale) { }
    }
}
