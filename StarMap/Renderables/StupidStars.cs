using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarMap.Shaders;
using StarMap.Database;
using OpenTK;

namespace StarMap.Renderables
{
    public class StupidStars : ARenderable
    {
        public StupidStars(IList<SystemBase> systems) : base(Program.Shaders.Star, systems.Count)
        {
            Vector4[] sysv4s = new Vector4[systems.Count];
            int i = 0;
            foreach (var s in systems)
            {
                sysv4s[i++] = new Vector4(s.Position, 1);
            }
            GL.NamedBufferStorage(Buffer, 4 * 4 * systems.Count, sysv4s, BufferStorageFlags.MapWriteBit);

            GL.VertexArrayAttribBinding(VertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 0);
            GL.VertexArrayAttribFormat(VertexArray, 0, 4, VertexAttribType.Float, false, 0);

            GL.VertexArrayVertexBuffer(VertexArray, 0, Buffer, IntPtr.Zero, 4 * 4);
        }

        public override void Render()
        {
            GL.DrawArrays(PrimitiveType.Points, 0, VertexCount);
        }
    }
}
