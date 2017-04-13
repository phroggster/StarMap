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
#endregion // --- using ... ---

namespace StarMap.Models
{
    public class StarsModel : Model
    {
        private readonly Color4 StarColor = new Color4(1f, 1, 1, 1);

        public StarsModel(IList<SystemBase> systems) : base(Program.Shaders.Star, systems.Count)
        {
            ColoredVertex[] model = new ColoredVertex[systems.Count];

            lock (systems)
            {
                for (int i = 0; i < systems.Count; i++)
                    model[i++] = new ColoredVertex(systems[i].Position, StarColor);
            }

            gld.NamedBufferStorage(m_gl_vboId, ColoredVertex.SizeInBytes * model.Length, model, BufferStorageFlags.MapWriteBit);

            // attrib a: Position
            gld.VertexArrayAttribBinding(m_gl_vaoId, Shader.Position, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, Shader.Position);
            gld.VertexArrayAttribFormat(m_gl_vaoId, Shader.Position, ColoredVertex.PositionSize,
                VertexAttribType.Float, false, ColoredVertex.PositionOffsetInBytes);

            // attrib b: Color
            gld.VertexArrayAttribBinding(m_gl_vaoId, Shader.Color, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, Shader.Color);
            gld.VertexArrayAttribFormat(m_gl_vaoId, Shader.Color, ColoredVertex.ColorSize,
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
