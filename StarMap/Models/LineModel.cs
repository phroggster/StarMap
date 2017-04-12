using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Models
{
    public class LineModel : ColoredModel
    {
        public LineModel(float length, Color4 color) : base(Model(length, color)) { }

        private static ColoredVertex[] Model(float length, Color4 color)
        {
            float hlen = length * .5f;
            //Color4 red = new Color4(1f, 0, 0, 1);

            // Dumb colored line
            ColoredVertex[] vertices = new ColoredVertex[]
            {
                // side 1
                new ColoredVertex(new Vector3(-hlen, 0, 0), color),
                new ColoredVertex(new Vector3( hlen, 0, 0), color)
            };

            return vertices;
        }

        public override void Render()
        {
            gld.DrawArrays(PrimitiveType.Lines, 0, m_VertexCount);
        }
    }
}
