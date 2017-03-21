using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace StarMap.Renderables
{
    public class AxisModel : ColoredRenderable
    {
        public AxisModel(float length) : base(Model(length), Program.Shaders.PlainPipe) { }

        private static ColoredVertex[] Model(float length)
        {
            Color4 white    = new Color4(1f, 1, 1, 1);
            Color4 red      = new Color4(1f, 0, 0, 1);
            Color4 green    = new Color4(0f, 1, 0, 1);
            Color4 blue     = new Color4(0f, 0, 1, 1);

            // Colored lines extending from origin
            ColoredVertex[] vertices = new ColoredVertex[]
            {
                // X
                new ColoredVertex(new Vector4(0, 0, 0, 1), white),
                new ColoredVertex(new Vector4(length, 0, 0, 1), red),
                // Y
                new ColoredVertex(new Vector4(0, 0, 0, 1), white),
                new ColoredVertex(new Vector4(0, length, 0, 1), green),
                // Z
                new ColoredVertex(new Vector4(0, 0, 0, 1), white),
                new ColoredVertex(new Vector4(0, 0, length, 1), blue),
            };

            return vertices;
        }

        public override void Render()
        {
            GL.DrawArrays(PrimitiveType.Lines, 0, VertexCount);
        }
    }
}
