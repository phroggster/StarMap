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
    public sealed class ShaderCollection : IDisposable
    {
        #region Public interface

        public bool IsDisposed { get; private set; } = false;
        public bool IsLoaded { get; private set; } = false;

        public PlainPipeShader PlainPipe
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException("ShaderCollection");
                return _Shaders["PlainPipe"] as PlainPipeShader;
            }
        }
        public TexPipeShader TexPipe
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException("ShaderCollection");
                return _Shaders["TexPipe"] as TexPipeShader;
            }
        }
        public TextShader Text
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException("ShaderCollection");
                return _Shaders["Text"] as TextShader;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="ShaderCollection"/>, and links the <see cref="PlainPipeShader"/> for immediate use.
        /// All other shaders can be loaded via <see cref="Load(GLControl)"/> or <see cref="LoadAsync(GLControl)"/>.
        /// </summary>
        public ShaderCollection()
        {
            Debug.WriteLine($"[INFO] Constructing ShaderCollection.");
            // this is used by the loading scene, so let's block for it.
            _Shaders["PlainPipe"] = new PlainPipeShader();
            _Shaders["PlainPipe"].LoadAndLink();
        }

        ~ShaderCollection()
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
        public void Load(GLControl context)
        {
            asyncThreadLoadShaders(context);
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
        public void LoadAsync(GLControl context)
        {
            if (IsLoaded || IsBusy) return;
            IsBusy = true;
            Thread t = new Thread(asyncThreadLoadShaders);
            t.SetApartmentState(ApartmentState.STA);
            t.Name = "ShaderCollection loader";
            t.Start(context);
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

        private Dictionary<string, Shader> _Shaders = new Dictionary<string, Shader>();
        private bool loadAbort = false;

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Debug.WriteLine($"[DEBUG] Disposing of ShaderCollection.");
                if (_Shaders != null)
                {
                    foreach (Shader s in _Shaders.Values)
                        s?.Dispose();
                    if (disposing)
                        _Shaders.Clear();
                }
                _Shaders = null;
                LoadCompleted = null;
            }
        }

        #region Shader compilation

        private void asyncThreadLoadShaders(object objControl)
        {
            GLControl control = objControl as GLControl;

            // Context is shared with whatever GLControl was sent in. Window will not be shown; is merely context.
            GameWindow gw = new GameWindow(control.Width, control.Height, control.GraphicsMode, "Shader load window",
                GameWindowFlags.Default, DisplayDevice.Default, control.GLMajorVersion, control.GLMinorVersion, control.ContextFlags, control.Context);
            gw.MakeCurrent();

            Dictionary<string, Shader> shades = new Dictionary<string, Shader>();
            shades.Add("TexPipe", new TexPipeShader());
            shades.Add("Text", new TextShader());
            foreach (Shader s in shades.Values)
            {
                if (loadAbort)
                    break;
                s.LoadAndLink();
                // XXX: Slow this down a bit during testing...
                //Thread.Sleep(10000);
            }

            // clean up if things fell apart while we had our head buried in the sand.
            if (loadAbort || IsDisposed || _Shaders == null)
            {
                foreach (Shader s in shades.Values)
                    s?.Dispose();
            }
            // Can't dispose of shades if gw was already nuked...
            gw.Dispose();

            if (!loadAbort && !IsDisposed && _Shaders != null)
            {
                lock (_Shaders)
                {
                    foreach (string s in shades.Keys)
                        _Shaders.Add(s, shades[s]);
                }
                IsLoaded = true;

                if (LoadCompleted != null)
                    control.FindForm().BeginInvoke(LoadCompleted);
            }

            LoadCompleted = null;
            IsBusy = false;
            loadAbort = false;
        }

        #endregion // Shader compilation

        #endregion // Private implementation
    }
}
