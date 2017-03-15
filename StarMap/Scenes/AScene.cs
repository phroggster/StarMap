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
using OpenTK.Graphics.OpenGL4;
using StarMap.Cameras;
using StarMap.Objects;
using StarMap.Renderables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace StarMap.Scenes
{
    /// <summary>
    /// <para>Abstract base class for scene objects, which are responsible for managing everything that
    /// is or will be rendered to the screen.</para>
    /// <para>This includes the scene <see cref="Contents"/>, <see cref="Camera"/>,
    /// animation (<see cref="Update(double)"/>), rendering (<see cref="Render"/>), and user interaction
    /// (<see cref="KeyDown(KeyEventArgs)"/>, <see cref="KeyUp(KeyEventArgs)"/>), etc.</para>
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class AScene : IDisposable
    {
        /// <summary>
        /// The background <see cref="Color"/> of this scene.
        /// </summary>
        public virtual Color BackColor { get; set; } = Color.Black;
        /// <summary>
        /// The <see cref="Camera"/> that this scene uses.
        /// </summary>
        public virtual Camera Camera { get; set; } = new StaticCamera(Vector3.Zero, Vector3.Zero);
        /// <summary>
        /// All of the <see cref="AObject"/>s that will be rendered in this scene.
        /// </summary>
        public virtual List<AObject> Contents { get; set; } = new List<AObject>();
        public virtual string DebuggerDisplay
        {
            get
            {
                return string.Format("Scene {0}: {1} objects, {2}, fov {3}°", Name, Contents.Count, Camera.DebuggerDisplay, FOV);
            }
        }
        /// <summary>
        /// The camera's field of view for this scene.
        /// </summary>
        public virtual float FOV { get; set; } = 45f;
        /// <summary>
        /// Whether this scene is fully loaded, and merely awaiting a transition.
        /// </summary>
        public bool IsLoaded { get; private set; } = false;
        /// <summary>
        /// The name of the scene.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// The toggle keys that this scene is concerned with.
        /// </summary>
        public virtual Keys ToggleKeys { get; set; } = Keys.None;

        public Keys keyData = Keys.None;

        protected Matrix4 ProjectionMatrix;

        /// <summary>
        /// Constructs a new <see cref="AScene"/> instance, but does NOT establish the
        /// <c>ProjectionMatrix</c>. You MUST call <see cref="ResetProjectionMatrix"/>
        /// prior to calling <see cref="Render"/> if using this constructor.
        /// </summary>
        protected AScene() { }

        /// <summary>
        /// Constructs a new <see cref="AScene"/> instance, and establishes the <c>ProjectionMatrix</c>.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio of the viewport.</param>
        protected AScene(int width, int height)
        {
            ResetProjectionMatrix(width, height);
        }

        ~AScene()
        {
#if DEBUG
            Debug.WriteLine($"[WARN] Leaked scene {Name}; Did you forget to call Dispose()?");
#endif
            Dispose(false);
        }

        /// <summary>
        /// Receives external KeyDown notifications, and raises scene-specific KeyDown and KeyPress events.
        /// </summary>
        /// <param name="e">Provides data for the KeyDown and KeyUp events.</param>
        public void KeyDown(KeyEventArgs e)
        {
            if (!keyData.HasFlag(e.KeyData) && ToggleKeys.HasFlag(e.KeyData))
                OnKeyPress(e.KeyData);
            keyData = e.KeyData;
            OnKeyDown(e.KeyData);
        }

        /// <summary>
        /// Receives external KeyUp notifications, and raises scene-specific KeyUp events.
        /// </summary>
        /// <param name="e">Provides data for the KeyDown and KeyUp events.</param>
        public void KeyUp(KeyEventArgs e)
        {
            keyData -= e.KeyData;
            OnKeyUp(e.KeyData);
        }

        public void Load()
        {
            Trace.WriteLine($"[DEBUG] Loading {Name}.");
            OnLoad();
            IsLoaded = true;
        }

        /// <summary>
        /// Raises the Render event.
        /// </summary>
        public void Render()
        {
            if (IsDisposed) throw new ObjectDisposedException(Name);
            GL.ClearColor(BackColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            OnRender();
        }

        /// <summary>
        /// Raises the Update event.
        /// </summary>
        /// <param name="delta">The time that has elapsed since the last frame.</param>
        public void Update(double delta)
        {
            if (IsDisposed) throw new ObjectDisposedException(Name);
            OnUpdate(delta);
        }

        /// <summary>
        /// Refresh the projection matrix, such as after the canvas is resized.
        /// </summar
        /// <param name="width">The width of the new viewport.</param>
        /// <param name="height">The height of the new viewport.</param>
        public virtual void ResetProjectionMatrix(int width, int height)
        {
            if (IsDisposed) throw new ObjectDisposedException(Name);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV * ((float)Math.PI / 180f), width / (float)height, 0.001f, 200f);
        }

        #region IDisposable implementation

        public bool IsDisposed { get; private set; } = false;

        protected virtual void Dispose(bool disposing)
        {
            Debug.WriteLine($"[DEBUG] Disposing of {Name}, disposing = {disposing}.");
            if (!IsDisposed)
            {
                if (disposing && Contents != null)
                    Contents.Clear();

                Camera = null;
                Contents = null;
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion //  IDisposable implementation

        protected virtual void OnKeyDown(Keys key) { }
        protected virtual void OnKeyPress(Keys key) { }
        protected virtual void OnKeyUp(Keys key) { }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        protected virtual void OnRender()
        {
            if (Contents == null || Contents.Count < 1)
                return;

            int lastprogram = -1;

            foreach (var obj in Contents)
            {
                if (obj.Model.Shader.ProgramID != lastprogram)
                {
                    GL.UniformMatrix4(obj.Model.Shader.UniformProjection, false, ref ProjectionMatrix);
                    GL.UniformMatrix4(obj.Model.Shader.UniformView, false, ref Camera.ViewMatrix);
                }   
                lastprogram = obj.Model.Shader.ProgramID;
                obj.Render();
            }
        }

        protected abstract void OnLoad();

        private float cameraspeed = 0.5f;

        /// <summary>
        /// Updates the scene, such as object movement, animations, etc.
        /// </summary>
        /// <param name="delta">The time since the last update.</param>
        protected virtual void OnUpdate(double delta)
        {
            if (Camera.IsUserMovable && keyData != Keys.None)
            {
                if (keyData.HasFlag(Keys.D))
                    Camera.Move(Vector3.UnitX * (float)delta * cameraspeed);
                if (keyData.HasFlag(Keys.A))
                    Camera.Move(-Vector3.UnitX * (float)delta * cameraspeed);

                if (keyData.HasFlag(Keys.W))
                    Camera.Move(-Vector3.UnitZ * (float)delta * cameraspeed);
                if (keyData.HasFlag(Keys.S))
                    Camera.Move(Vector3.UnitZ * (float)delta * cameraspeed);

                if (keyData.HasFlag(Keys.E))
                    Camera.Move(Vector3.UnitY * (float)delta * cameraspeed);
                if (keyData.HasFlag(Keys.Q))
                    Camera.Move(-Vector3.UnitY * (float)delta * cameraspeed);
            }

            Camera.Update(delta);
            foreach (var obj in Contents)
                obj.Update(delta);
        }
    }
}
