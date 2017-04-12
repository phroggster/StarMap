using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using StarMap.Shaders;
using StarMap.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Models
{
    public class StarsModel : Model
    {
        private readonly Color4 StarColor = new Color4(1f, 1, 1, 1);

        public StarsModel(IList<SystemBase> systems) : base(Program.Shaders.Star, systems.Count)
        {
            if (Shader.AttribPosition < 0)
                throw new InvalidOperationException($"{Shader.Name} lacks position attribute!");
            else if (Shader.AttribColor < 0)
                throw new InvalidOperationException($"{Shader.Name} lacks color attribute!");

            ColoredVertex[] model = new ColoredVertex[systems.Count];

            lock (systems)
            {
                for (int i = 0; i < systems.Count; i++)
                    model[i++] = new ColoredVertex(systems[i].Position, StarColor);
            }

            gld.NamedBufferStorage(m_gl_vboId, ColoredVertex.SizeInBytes * model.Length, model, BufferStorageFlags.MapWriteBit);

            // attrib 0: position
            gld.VertexArrayAttribBinding(m_gl_vaoId, Shader.AttribPosition, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, Shader.AttribPosition);
            gld.VertexArrayAttribFormat(m_gl_vaoId, Shader.AttribPosition, ColoredVertex.PositionSize,
                VertexAttribType.Float, false, ColoredVertex.PositionOffsetInBytes);

            // attrib 1: colour
            gld.VertexArrayAttribBinding(m_gl_vaoId, Shader.AttribColor, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, Shader.AttribColor);
            gld.VertexArrayAttribFormat(m_gl_vaoId, Shader.AttribColor, ColoredVertex.ColorSize,
                VertexAttribType.Float, false, ColoredVertex.ColorOffsetInBytes);

            gld.VertexArrayVertexBuffer(m_gl_vaoId, 0, m_gl_vboId, IntPtr.Zero, ColoredVertex.SizeInBytes);

            gld.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public override void Bind()
        {
            base.Bind();
        }

        public override void Render()
        {
            gld.DrawArrays(PrimitiveType.Points, 0, m_VertexCount);
        }
    }
}
