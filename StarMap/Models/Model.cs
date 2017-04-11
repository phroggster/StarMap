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

#if DEBUG
using gldebug = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Models
{
    public abstract class Model : IDisposable
    {
        public Shader Shader { get { return m_Shader; } }

        public virtual void Bind()
        {
            gld.BindVertexArray(m_gl_vaoId);
        }

        public virtual void Render()
        {
            gld.DrawArrays(PrimitiveType.Triangles, 0, m_VertexCount);
        }


        #region IDisposable interface

        public bool IsDisposed { get; private set; } = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                if (disposing)
                    TraceLog.Debug($"Disposing of {GetType().Name}.");
                else
                    TraceLog.Warn($"{GetType().Name} leaked! Did you forget to call `{nameof(Dispose)}()`?");
                gld.DeleteVertexArray(m_gl_vaoId);
                gld.DeleteBuffer(m_gl_vboId);
            }
        }

        #endregion // IDisposable interface

        protected readonly int m_gl_vaoId;
        protected readonly int m_gl_vboId;
        protected readonly int m_VertexCount;
#if DEBUG
        protected gldebug gld = new gldebug();
#endif

        protected Model(Shader shader, int vertexCount)
        {
            m_Shader = shader;
            m_VertexCount = vertexCount;
            m_gl_vaoId = gld.GenVertexArray();
            m_gl_vboId = gld.GenBuffer();

            gld.BindVertexArray(m_gl_vaoId);
            gld.BindBuffer(BufferTarget.ArrayBuffer, m_gl_vboId);
        }


        private Shader m_Shader;

        ~Model()
        {
            Dispose(false);
        }
    }
}
