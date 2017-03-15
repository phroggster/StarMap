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
using System.Diagnostics;

namespace StarMap.Renderables
{
    public abstract class ARenderable : IDisposable
    {
        protected ARenderable(Shader shader, int vertexCount)
        {
            Shader = shader;
            VertexCount = vertexCount;
            VertexArray = GL.GenVertexArray();
            Buffer = GL.GenBuffer();

            GL.BindVertexArray(VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer);
        }

        ~ARenderable()
        {
#if DEBUG
            Debug.WriteLine($"[WARN] ARenderable {GetType().ToString()} leaked! Did you forget to call Dispose()?");
#endif
            Dispose(false);
        }


        public virtual void Bind()
        {
            GL.UseProgram(Shader.ProgramID);
            GL.BindVertexArray(VertexArray);
        }

        public virtual void Render()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
        }


        #region IDisposable interface

        public bool IsDisposed { get; private set; } = false;

        public void Dispose()
        {
            Debug.WriteLine($"[DEBUG] Disposing of {GetType().Name}.");
            if (!IsDisposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }   
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                GL.DeleteVertexArray(VertexArray);
                GL.DeleteBuffer(Buffer);
            }
        }

        #endregion // IDisposable interface

        public readonly Shader Shader;
        protected readonly int Buffer;
        protected readonly int VertexArray;
        protected readonly int VertexCount;
    }
}
