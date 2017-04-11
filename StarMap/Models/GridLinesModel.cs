using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using StarMap.Shaders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
using gldebug = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Models
{
    public class GridLinesModel : Model
    {
        private Config _config;

        public GridLinesModel() : base(Program.Shaders.GridLine, coarseVertCount + fineVertCount)
        {
            _config = Config.Instance;
            gld.NamedBufferStorage(m_gl_vboId, 4 * 3 * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);

            // attrib 0: position (3 floats [12 bytes], no offset)
            gld.VertexArrayAttribBinding(m_gl_vaoId, Shader.AttribPositionIndex, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, Shader.AttribPositionIndex);
            gld.VertexArrayAttribFormat(m_gl_vaoId, Shader.AttribPositionIndex, 3, VertexAttribType.Float, false, 0);

            gld.VertexArrayVertexBuffer(m_gl_vaoId, 0, m_gl_vboId, IntPtr.Zero, 4 * 3); // 4*3 = Vector3 position

            gld.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public override void Render()
        {
            GL.Uniform4(Shader.UniformDiffuseColorIndex, Config.Instance.GridLineColour);
            gld.DrawArrays(PrimitiveType.Lines, 0, coarseVertCount);

            GL.Uniform4(Shader.UniformDiffuseColorIndex, Config.Instance.FineGridLineColour);
            gld.DrawArrays(PrimitiveType.Lines, coarseVertCount, fineVertCount);
        }

        #region private static Vector3[]'s

        private static Vector3[] vertices
        {
            get
            {
                var ret = new Vector3[coarseVertCount + fineVertCount];
                CoarseVertices.CopyTo(ret, 0);
                FineVertices.CopyTo(ret, coarseVertCount);
                return ret;
            }
        }

        // Vector3[40] (10 horiz, 10 vert lines @ 10kLY spacing)
        private const int coarseVertCount = 38;
        private static Vector3[] CoarseVertices
        {
            get
            {
                const float left = -40000;
                const float right = 40000;
                const float bottom = -20000;
                const float top = 70000;

                Vector3[] ret = new Vector3[coarseVertCount]
                {
                    // 10 horizontal lines (20 verts) at 10kLY distance, bottom to top.
                    new Vector3(left, bottom, 0), new Vector3(right, bottom, 0),
                    new Vector3(left, -10000, 0), new Vector3(right, -10000, 0),
                    new Vector3(left,      0, 0), new Vector3(right,      0, 0), // Sol intersect
                    new Vector3(left,  10000, 0), new Vector3(right,  10000, 0),
                    new Vector3(left,  20000, 0), new Vector3(right,  20000, 0),
                    new Vector3(left,  30000, 0), new Vector3(right,  30000, 0),
                    new Vector3(left,  40000, 0), new Vector3(right,  40000, 0),
                    new Vector3(left,  50000, 0), new Vector3(right,  50000, 0),
                    new Vector3(left,  60000, 0), new Vector3(right,  60000, 0),
                    new Vector3(left,    top, 0), new Vector3(right,    top, 0),

                    // 9 vertical lines (18 verts) at 10kLY distance, left to right.
                    new Vector3(  left, bottom, 0), new Vector3(  left, top, 0),
                    new Vector3(-30000, bottom, 0), new Vector3(-30000, top, 0),
                    new Vector3(-20000, bottom, 0), new Vector3(-20000, top, 0),
                    new Vector3(-10000, bottom, 0), new Vector3(-10000, top, 0),
                    new Vector3(     0, bottom, 0), new Vector3(     0, top, 0), // Sol intersect
                    new Vector3( 10000, bottom, 0), new Vector3( 10000, top, 0),
                    new Vector3( 20000, bottom, 0), new Vector3( 20000, top, 0),
                    new Vector3( 30000, bottom, 0), new Vector3( 30000, top, 0),
                    new Vector3( right, bottom, 0), new Vector3( right, top, 0)
                };

                return ret;
            }
        }

        // Vector3[72] (18 horiz, 18 vert lines @ 1kLY spacing)
        private const int fineVertCount = 72;
        private static Vector3[] FineVertices
        {
            get
            {
                int halflen = 10000;

                Vector3[] ret = new Vector3[fineVertCount]
                {
                    new Vector3(-halflen, -9000, 0), new Vector3(halflen, -9000, 0),
                    new Vector3(-halflen, -8000, 0), new Vector3(halflen, -8000, 0),
                    new Vector3(-halflen, -7000, 0), new Vector3(halflen, -7000, 0),
                    new Vector3(-halflen, -6000, 0), new Vector3(halflen, -6000, 0),
                    new Vector3(-halflen, -5000, 0), new Vector3(halflen, -5000, 0),
                    new Vector3(-halflen, -4000, 0), new Vector3(halflen, -4000, 0),
                    new Vector3(-halflen, -3000, 0), new Vector3(halflen, -3000, 0),
                    new Vector3(-halflen, -2000, 0), new Vector3(halflen, -2000, 0),
                    new Vector3(-halflen, -1000, 0), new Vector3(halflen, -1000, 0),

                    new Vector3(-halflen,  1000, 0), new Vector3(halflen,  1000, 0),
                    new Vector3(-halflen,  2000, 0), new Vector3(halflen,  2000, 0),
                    new Vector3(-halflen,  3000, 0), new Vector3(halflen,  3000, 0),
                    new Vector3(-halflen,  4000, 0), new Vector3(halflen,  4000, 0),
                    new Vector3(-halflen,  5000, 0), new Vector3(halflen,  5000, 0),
                    new Vector3(-halflen,  6000, 0), new Vector3(halflen,  6000, 0),
                    new Vector3(-halflen,  7000, 0), new Vector3(halflen,  7000, 0),
                    new Vector3(-halflen,  8000, 0), new Vector3(halflen,  8000, 0),
                    new Vector3(-halflen,  9000, 0), new Vector3(halflen,  9000, 0),

                    new Vector3(-9000, -halflen, 0), new Vector3(-9000, halflen, 0),
                    new Vector3(-8000, -halflen, 0), new Vector3(-8000, halflen, 0),
                    new Vector3(-7000, -halflen, 0), new Vector3(-7000, halflen, 0),
                    new Vector3(-6000, -halflen, 0), new Vector3(-6000, halflen, 0),
                    new Vector3(-5000, -halflen, 0), new Vector3(-5000, halflen, 0),
                    new Vector3(-4000, -halflen, 0), new Vector3(-4000, halflen, 0),
                    new Vector3(-3000, -halflen, 0), new Vector3(-3000, halflen, 0),
                    new Vector3(-2000, -halflen, 0), new Vector3(-2000, halflen, 0),
                    new Vector3(-1000, -halflen, 0), new Vector3(-1000, halflen, 0),

                    new Vector3( 1000, -halflen, 0), new Vector3( 1000, halflen, 0),
                    new Vector3( 2000, -halflen, 0), new Vector3( 2000, halflen, 0),
                    new Vector3( 3000, -halflen, 0), new Vector3( 3000, halflen, 0),
                    new Vector3( 4000, -halflen, 0), new Vector3( 4000, halflen, 0),
                    new Vector3( 5000, -halflen, 0), new Vector3( 5000, halflen, 0),
                    new Vector3( 6000, -halflen, 0), new Vector3( 6000, halflen, 0),
                    new Vector3( 7000, -halflen, 0), new Vector3( 7000, halflen, 0),
                    new Vector3( 8000, -halflen, 0), new Vector3( 8000, halflen, 0),
                    new Vector3( 9000, -halflen, 0), new Vector3( 9000, halflen, 0)
                };

                return ret;
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            _config = null;
            base.Dispose(disposing);
        }
    }
}
