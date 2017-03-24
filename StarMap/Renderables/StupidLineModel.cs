using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace StarMap.Renderables
{
    public class StupidLineModel : ColoredRenderable
    {
        public StupidLineModel(float length, Color4 color) : base(Model(length, color), Program.Shaders.PlainPipe) { }

        private static ColoredVertex[] Model(float length, Color4 color)
        {
            float hlen = length * .5f;
            //Color4 red = new Color4(1f, 0, 0, 1);

            // Dumb colored line
            ColoredVertex[] vertices = new ColoredVertex[]
            {
                // side 1
                new ColoredVertex(new Vector4(-hlen, 0, 0, 1), color),
                new ColoredVertex(new Vector4( hlen, 0, 0, 1), color)
            };

            return vertices;
        }

        public override void Render()
        {
            GL.DrawArrays(PrimitiveType.Lines, 0, VertexCount);
        }
    }
}
