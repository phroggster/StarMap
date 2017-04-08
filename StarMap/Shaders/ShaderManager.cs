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
using OpenTK;
using Phroggiesoft.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace StarMap.Shaders
{
    /// <summary>
    /// A collection of all known <see cref="Shader"/> objects.
    /// </summary>
    /// <seealso cref="Program.Shaders"/>
    public sealed class ShaderManager : IDisposable
    {
        #region Public interface

        public bool IsDisposed { get; private set; } = false;
        public bool IsLoaded { get; private set; } = false;

        public GridLineShader GridLine { get; private set; }
        public PlainPipeShader PlainPipe { get; private set; }
        public StarShader Star { get; private set; }
        public TexPipeShader TexPipe { get; private set; }
        public TextShader Text { get; private set; }

        public Shader this[int shaderProg]
        {
            get
            {
                return _Shaders.FindLast(s => s.ProgramID == shaderProg);
            }
        }

        /// <summary>
        /// Constructs a new <see cref="ShaderManager"/>, and links the <see cref="PlainPipeShader"/> for immediate use.
        /// All other shaders can be loaded via <see cref="Load(GLControl)"/> or <see cref="LoadAsync(GLControl)"/>.
        /// </summary>
        public ShaderManager()
        {
            TraceLog.Info($"Constructing {nameof(ShaderManager)}.");
        }

        ~ShaderManager()
        {
#if DEBUG
            Debug.WriteLine("[WARN] ShaderCollection leaked; Did you forget to call Dispose()?");
#endif
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Loading

        /// <summary>
        /// Whether async loading is currently in progress.
        /// </summary>
        /// <seealso cref="LoadAsync(GLControl)"/>
        /// <seealso cref="LoadAsyncCancel"/>
        /// <seealso cref="LoadCompleted"/>
        public bool IsBusy { get; private set; } = false;

        /// <summary>
        /// The asynchonous shader loading has completed event, raised when the thread started by <see cref="LoadAsync"/> has completed.
        /// </summary>
        /// <seealso cref="LoadAsync(GLControl)"/>
        /// <seealso cref="LoadAsyncCancel"/>
        /// <seealso cref="IsBusy"/>
        public event EventHandler LoadCompleted;

        /// <summary>
        /// Load all shaders while blocking the callee. You probably want to use <see cref="LoadAsync(GLControl)"/> instead.
        /// </summary>
        /// <param name="context">A <see cref="GLControl"/> to use for the shared context.</param>
        public void Load(phrogGLControl context)
        {
            if (!IsLoaded)
            {
                GridLine = new GridLineShader();
                GridLine.LoadAndLink();
                _Shaders.Add(GridLine);

                PlainPipe = new PlainPipeShader();
                PlainPipe.LoadAndLink();
                _Shaders.Add(PlainPipe);

                TexPipe = new TexPipeShader();
                TexPipe.LoadAndLink();
                _Shaders.Add(TexPipe);

                Star = new StarShader();
                Star.LoadAndLink();
                _Shaders.Add(Star);

                Text = new TextShader();
                Text.LoadAndLink();
                _Shaders.Add(Text);

                IsLoaded = true;
            }

            //asyncThreadLoadShaders(context);
        }

        /// <summary>
        /// Load all shaders asynchronously. The <see cref="LoadCompleted"/> event will be fired
        /// upon completion. Loading can be cancelled via <see cref="LoadAsyncCancel"/>.
        /// <para>NOTE: the <see cref="LoadCompleted"/> <c>EventHandler</c> will be cleared after invocation. If you cancelled
        /// loading but aren't actually exiting, you will have to subscribe to <see cref="LoadCompleted"/> again.</para>
        /// </summary>
        /// <param name="context">A <see cref="GLControl"/> to use for the shared context.</param>
        /// <seealso cref="LoadCompleted"/>
        /// <seealso cref="LoadAsyncCancel"/>
        /// <seealso cref="IsBusy"/>
        /// <seealso cref="IsLoaded"/>
        public void LoadAsync(phrogGLControl context)
        {
            Load(context);  // XXX
            /* XXX
            if (IsLoaded || IsBusy) return;
            IsBusy = true;
            Thread t = new Thread(asyncThreadLoadShaders);
            t.SetApartmentState(ApartmentState.STA);
            t.Name = "ShaderCollection loader";
            t.Start(context);*/
        }

        /// <summary>
        /// Cancel asynchronous shader loading that was started via <see cref="LoadAsync(GLControl)"/>.
        /// </summary>
        /// <seealso cref="LoadAsync(GLControl)"/>
        /// <seealso cref="LoadCompleted"/>
        /// <seealso cref="IsBusy"/>
        public void LoadAsyncCancel()
        {
            loadAbort = true;
        }

        #endregion // Loading

        #endregion // Public interface

        #region Private implementation

        private List<Shader> _Shaders = new List<Shader>();
        private bool loadAbort = false;

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Debug.WriteLine($"[DEBUG] Disposing of ShaderCollection.");
                foreach (Shader s in _Shaders)
                    s?.Dispose();
                if (disposing)
                    _Shaders.Clear();
                _Shaders = null;
                LoadCompleted = null;
            }
        }

        #region Shader compilation

        [STAThread]
        private void asyncThreadLoadShaders(object objControl)
        {
            Debug.Assert(objControl != null);
            Debug.Assert(objControl is phrogGLControl);
            phrogGLControl control = objControl as phrogGLControl;

            // Context is shared with whatever GLControl was sent in. Window will not be shown; is merely context.
            GameWindow gw = new GameWindow(control.Width, control.Height, control.GraphicsMode, "Shader load window",
                GameWindowFlags.Default, DisplayDevice.Default, control.GLMajorVersion, control.GLMinorVersion, control.ContextFlags, control.Context);
            gw.MakeCurrent();

            // TODO: This does not assign it to the (e.g. CoarseGridLine) properties!
            List<Shader> shades = new List<Shader>();
            shades.Add(new TexPipeShader());
            shades.Add(new TextShader());
            foreach (Shader s in shades)
            {
                if (loadAbort)
                    break;
                s.LoadAndLink(true);
                // XXX: Slow this down a bit during testing...
                //Thread.Sleep(10000);
            }

            // clean up if things fell apart while we had our head buried in the sand.
            if (loadAbort || IsDisposed || _Shaders == null)
            {
                foreach (Shader s in shades)
                    s?.Dispose();
            }
            // Can't dispose of shades if gw was already nuked...
            gw.Dispose();

            if (!loadAbort && !IsDisposed && _Shaders != null)
            {
                lock (_Shaders)
                {
                    foreach (Shader s in shades)
                        _Shaders.Add(s);
                }
                IsLoaded = true;

                if (LoadCompleted != null)
                    control.BeginInvoke(LoadCompleted);
                    //control.FindForm().BeginInvoke(LoadCompleted);
            }

            LoadCompleted = null;
            IsBusy = false;
            loadAbort = false;
        }

        #endregion // Shader compilation

        #endregion // Private implementation
    }
}
