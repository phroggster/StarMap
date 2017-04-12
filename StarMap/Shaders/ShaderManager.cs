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
using Phroggiesoft.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
#endregion // --- using ... ---

namespace StarMap.Shaders
{
    /// <summary>
    /// A manager of <see cref="Shader"/> objects. Accessible via <see cref="Program.Shaders"/>.
    /// </summary>
    public sealed class ShaderManager : IDisposable
    {
        #region --- public ctor() ---

        /// <summary>
        /// Constructs a new <see cref="ShaderManager"/>. After construction, load shaders via <see cref="Load(GLControl)"/>.
        /// </summary>
        public ShaderManager()
        {
            TraceLog.Info($"Constructing {nameof(ShaderManager)}.");
        }

        #endregion // --- public ctor() ---

        #region --- public interface ---

        #region --- properties ---

        public bool IsDisposed { get; private set; } = false;
        public bool IsLoaded { get; private set; } = false;

        public GridLineShader GridLine { get; private set; }
        public PlainPipeShader PlainPipe { get; private set; }
        public StarShader Star { get; private set; }
        public TexPipeShader TexPipe { get; private set; }
        public TextShader Text { get; private set; }

        #endregion // --- properties ---

        public Shader this[int shaderProg]
        {
            get
            {
                return m_Shaders.FindLast(s => s.ProgramID == shaderProg);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region --- public void Load() ---

        /// <summary>
        /// Load all shaders while blocking the callee. You probably want to use <see cref="LoadAsync(GLControl)"/> instead.
        /// </summary>
        /// <param name="context">A <see cref="GLControl"/> to use for the shared context.</param>
        public void Load()
        {
            if (!IsLoaded)
            {
                Star = new StarShader();
                Star.LoadAndLink();
                m_Shaders.Add(Star);

                PlainPipe = new PlainPipeShader();
                PlainPipe.LoadAndLink();
                m_Shaders.Add(PlainPipe);

                /*TexPipe = new TexPipeShader();
                TexPipe.LoadAndLink();
                _Shaders.Add(TexPipe);*/

                GridLine = new GridLineShader();
                GridLine.LoadAndLink();
                m_Shaders.Add(GridLine);

                /*Text = new TextShader();
                Text.LoadAndLink();
                _Shaders.Add(Text);*/

                IsLoaded = true;
            }
        }

        #endregion // --- public void Load() ---

        #endregion // --- public interface ---

        #region --- private implementation ---

        private List<Shader> m_Shaders = new List<Shader>();

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                TraceLog.Debug($"Disposing of {nameof(ShaderManager)}.");
                IsDisposed = true;

                if (m_Shaders != null && m_Shaders.Count > 0)
                {
                    foreach (Shader s in m_Shaders)
                        s?.Dispose();
                    if (disposing)
                        m_Shaders.Clear();
                    else
                        TraceLog.Warn($"{nameof(ShaderManager)} leaked; Did you forget to call Dispose()?");
                }
                
                m_Shaders = null;
            }
        }

        ~ShaderManager()
        {
            Dispose(false);
        }

        #endregion // --- private implementation ---
    }
}
