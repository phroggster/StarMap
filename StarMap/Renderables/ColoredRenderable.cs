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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarMap.Renderables
{
    public class ColoredRenderable : ARenderable
    {
        public ColoredRenderable(ColoredVertex[] vertices, Shader shader)
            : base(shader, vertices.Length)
        {
            GL.NamedBufferStorage(Buffer, ColoredVertex.Size * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);

            // attrib 0: position (4 floats [16 bytes], no offset)
            GL.VertexArrayAttribBinding(VertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 0);
            GL.VertexArrayAttribFormat(VertexArray, 0, 4, VertexAttribType.Float, false, 0);

            // attrib 1: colour (4 floats [16 bytes], 16 byte offset)
            GL.VertexArrayAttribBinding(VertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 1);
            GL.VertexArrayAttribFormat(VertexArray, 1, 4, VertexAttribType.Float, false, 16);

            GL.VertexArrayVertexBuffer(VertexArray, 0, Buffer, IntPtr.Zero, ColoredVertex.Size);
        }
    }
}
