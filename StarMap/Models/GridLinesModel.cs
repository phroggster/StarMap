#region --- Apache v2.0 license ---
/*
 * Copyright © 2017 phroggie <phroggster@gmail.com>, StarMap development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */
#endregion // --- Apache v2.0 license ---

#region --- using ... ---
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

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif
#endregion // --- using ... ---

namespace StarMap.Models
{
    public class GridLinesModel : Model
    {
        public GridLinesModel() : base(Program.Shaders.GridLine, coarseVertCount + fineVertCount)
        {
            if (Shader.AttribPosition < 0)
                throw new InvalidOperationException($"{Shader.Name} lacks position attribute!");

            gld.NamedBufferStorage(m_gl_vboId, 4 * 3 * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);

            // attrib 0: position (3 floats [12 bytes], no offset)
            gld.VertexArrayAttribBinding(m_gl_vaoId, Shader.AttribPosition, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, Shader.AttribPosition);
            gld.VertexArrayAttribFormat(m_gl_vaoId, Shader.AttribPosition, 3, VertexAttribType.Float, false, 0);

            gld.VertexArrayVertexBuffer(m_gl_vaoId, 0, m_gl_vboId, IntPtr.Zero, 4 * 3); // 4*3 = Vector3 position

            gld.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public override void Render()
        {
            gld.DrawArrays(PrimitiveType.Lines, 0, coarseVertCount + fineVertCount);
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

        private const int gLeft = -43000;
        private const int gRight = 41000;
        private const int gBottom = -17000;
        private const int gTop = 66000;

        // Vector3[38] (10 horiz, 9 vert lines @ 10kLY spacing)
        public const int coarseVertCount = 34;
        private static Vector3[] CoarseVertices
        {
            get
            {
                Vector3[] ret = new Vector3[coarseVertCount]
                {
                    // 10 horizontal lines (20 verts) at 10kLY distance, bottom to top.
                    new Vector3(gLeft, -10000, 0), new Vector3(gRight, -10000, 0),
                    new Vector3(gLeft,      0, 0), new Vector3(gRight,      0, 0), // Sol intersect
                    new Vector3(gLeft,  10000, 0), new Vector3(gRight,  10000, 0),
                    new Vector3(gLeft,  20000, 0), new Vector3(gRight,  20000, 0),
                    new Vector3(gLeft,  30000, 0), new Vector3(gRight,  30000, 0),
                    new Vector3(gLeft,  40000, 0), new Vector3(gRight,  40000, 0),
                    new Vector3(gLeft,  50000, 0), new Vector3(gRight,  50000, 0),
                    new Vector3(gLeft,  60000, 0), new Vector3(gRight,  60000, 0),

                    // 9 vertical lines (18 verts) at 10kLY distance, left to right.
                    new Vector3(-40000, gBottom, 0), new Vector3(-40000, gTop, 0),
                    new Vector3(-30000, gBottom, 0), new Vector3(-30000, gTop, 0),
                    new Vector3(-20000, gBottom, 0), new Vector3(-20000, gTop, 0),
                    new Vector3(-10000, gBottom, 0), new Vector3(-10000, gTop, 0),
                    new Vector3(     0, gBottom, 0), new Vector3(     0, gTop, 0), // Sol intersect
                    new Vector3( 10000, gBottom, 0), new Vector3( 10000, gTop, 0),
                    new Vector3( 20000, gBottom, 0), new Vector3( 20000, gTop, 0),
                    new Vector3( 30000, gBottom, 0), new Vector3( 30000, gTop, 0),
                    new Vector3( 40000, gBottom, 0), new Vector3( 40000, gTop, 0)
                };

                return ret;
            }
        }

        // Vector3[304] (81 horiz, 76 vert lines @ 1kLY spacing)
        private const int fineVertCount = 304;
        private static Vector3[] FineVertices
        {
            get
            {
                int index = 0;

                Vector3[] ret = new Vector3[fineVertCount];

                // 81 horizontal lines (162 verts)
                for (int y = gBottom; y <= gTop; y += 1000)
                {
                    if (y % 10000 == 0)
                        continue;
                    ret[index++] = new Vector3(gLeft, y, 0);
                    ret[index++] = new Vector3(gRight, y, 0);
                }

                // 72 vertical lines (144 verts)
                for (int x = gLeft; x <= gRight; x += 1000)
                {
                    if (x % 10000 == 0)
                        continue;
                    ret[index++] = new Vector3(x, gBottom, 0);
                    ret[index++] = new Vector3(x, gTop, 0);
                }
                Debug.Assert(index == fineVertCount);
                return ret;
            }
        }

        #endregion
    }
}
