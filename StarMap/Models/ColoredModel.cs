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
using OpenTK.Graphics.OpenGL4;
using StarMap.Shaders;
using System;

#if DEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Models
{
    public class ColoredModel : Model
    {
        public ColoredModel(ColoredVertex[] vertices, Shader shader)
            : base(shader, vertices.Length)
        {
            gld.NamedBufferStorage(m_gl_vboId, ColoredVertex.SizeInBytes * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);

            int attrib = 0;
            // attrib 0: position
            gld.VertexArrayAttribBinding(m_gl_vaoId, attrib, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, attrib);
            gld.VertexArrayAttribFormat(m_gl_vaoId, attrib, ColoredVertex.PositionSize, VertexAttribType.Float, false, ColoredVertex.PositionOffsetInBytes);

            attrib++;
            // attrib 1: colour
            gld.VertexArrayAttribBinding(m_gl_vaoId, attrib, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, attrib);
            gld.VertexArrayAttribFormat(m_gl_vaoId, attrib, ColoredVertex.ColorSize, VertexAttribType.Float, false, ColoredVertex.ColorOffsetInBytes);

            gld.VertexArrayVertexBuffer(m_gl_vaoId, 0, m_gl_vboId, IntPtr.Zero, ColoredVertex.SizeInBytes);

            gld.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
