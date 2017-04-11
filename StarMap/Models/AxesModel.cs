using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

#if DEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Models
{
    public class AxesModel : ColoredModel
    {
        public AxesModel(float length) : base(Model(length), Program.Shaders.PlainPipe) { }

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
                new ColoredVertex(new Vector3(0, 0, 0), white),
                new ColoredVertex(new Vector3(length, 0, 0), red),
                // Y
                new ColoredVertex(new Vector3(0, 0, 0), white),
                new ColoredVertex(new Vector3(0, length, 0), green),
                // Z
                new ColoredVertex(new Vector3(0, 0, 0), white),
                new ColoredVertex(new Vector3(0, 0, length), blue),
            };

            return vertices;
        }

        public override void Render()
        {
            gld.DrawArrays(PrimitiveType.Lines, 0, m_VertexCount);
        }
    }
}
