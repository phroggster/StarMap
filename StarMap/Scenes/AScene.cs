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
using Phroggiesoft.Controls;
using StarMap.Cameras;
using StarMap.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace StarMap.Scenes
{
    /// <summary>
    /// <para>Abstract base class for a scene object, which is responsible for managing everything that is or
    /// will be rendered inside of a <see cref="GLControl"/>.</para>
    /// <para>This includes the scene <see cref="Contents"/>, <see cref="Camera"/>,
    /// animation (<see cref="Update(double)"/>), rendering (<see cref="Render"/>), and user interaction, etc.</para>
    /// </summary>
    [Obsolete]
    public abstract class AScene : IIsDisposed, IScene
    {
        #region --- public interface ---

        #region --- Constructors ---

        /// <summary>
        /// Constructs a new <see cref="AScene"/> instance.
        /// </summary>
        protected AScene() { }

        #endregion // --- Constructors ---

        #region --- IIsDisposed interface ---

        public bool IsDisposed { get; private set; } = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // --- IIsDisposed interface ---

        #region --- IScene interface ---

        #region --- Properties ---

        /// <summary>
        /// The background <see cref="Color"/> of this scene.
        /// </summary>
        public virtual Color BackColor { get; set; } = Color.Black;
        /// <summary>
        /// The <see cref="ICamera"/> that this scene uses.
        /// </summary>
        public virtual ICamera Camera { get; set; } = new StaticCamera(Vector3.Zero, Quaternion.Identity);
        /// <summary>
        /// All of the <see cref="AObject"/>s that will be rendered in this scene.
        /// </summary>
        public virtual IList<IObject> Contents { get; set; } = new List<IObject>();
        /// <summary>
        /// The camera's field of view for this scene.
        /// </summary>
        public virtual float FOV { get; set; } = 45f;
        public string FrameRate { get { return string.Empty; } }
        public virtual bool IsHandlingInput { get; private set; } = false;
        /// <summary>
        /// Whether this scene is fully loaded, and merely awaiting a transition.
        /// </summary>
        public bool IsLoaded { get; private set; } = false;
        /// <summary>
        /// The name of the scene.
        /// </summary>
        public abstract string Name { get; }
        public virtual GLControl Parent { get; private set; }
        /// <summary>
        /// The toggle keys that this scene is concerned with.
        /// </summary>
        public virtual IList<Keys> ToggleKeys { get; set; } = new List<Keys>();
        /// <summary>
        /// How fast this scene's camera can translate.
        /// </summary>
        public virtual float TranslationSpeed { get; set; } = translationSpeedLow;

        public virtual float RotationSpeed { get; set; } = rotateSpeedLow;
        #endregion // --- Properties ---

        #region --- Methods ---

        /// <summary>
        /// Load the scene resources from disk, etc, to vRAM. This will block the calling thread, potentially for a rather long time.
        /// <para>See also: <see cref="SceneTransitions.LoadAsync(GLControl, IScene)"/></para>
        /// </summary>
        public void Load(GLControl control)
        {
            if (!IsLoaded)
            {
                Parent = control;
                Trace.WriteLine($"[DEBUG] Loading {Name}.");
                OnLoad();
                IsLoaded = true;
                Trace.WriteLine($"[DEBUG] Done loading {Name}.");

                Parent.KeyDown += Parent_KeyDown;
                Parent.KeyUp += Parent_KeyUp;
                Parent.Resize += Parent_Resize;
            }
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        public void Render()
        {
            if (IsDisposed) throw new ObjectDisposedException(Name);
            GL.ClearColor(BackColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            OnRender();
        }

        /// <summary>
        /// Update the scene in preparation for rendering.
        /// </summary>
        /// <param name="delta">The time that has elapsed since the last frame.</param>
        public void Update(double delta)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(Name);

            if (!IsSetup)
            {
                OnFirstUpdate();
                IsSetup = true;
            }

            OnUpdate(delta);
        }

        #endregion // --- Methods ---

        #endregion // --- IScene interface ---

        #endregion // --- public interface ---

        #region --- protected implementation ---

        protected Matrix4 ProjectionMatrix;
        protected List<KeyEventArgs> keyData = new List<KeyEventArgs>();

        #region --- Methods ---

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Trace.WriteLine($"[DEBUG] Disposing of {Name}.");
                if (disposing)
                {
                    if (Parent != null)
                    {
                        Parent.KeyDown -= Parent_KeyDown;
                        Parent.KeyUp -= Parent_KeyUp;
                        Parent.Resize -= Parent_Resize;
                    }
                    Contents?.Clear();
                    keyData?.Clear();
                    ToggleKeys?.Clear();
                }
                Camera = null;
                Contents = null;
                keyData = null;
                Parent = null;
            }
        }

        protected virtual void OnFirstUpdate()
        {
            keyData.Clear();
            IsHandlingInput = true;
            ResetProjectionMatrix(Parent.Width, Parent.Height);
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (IsHandlingInput && !IsDisposed)
            {
                if (ToggleKeys.Contains(e.KeyCode))
                    OnKeyPress(e);

                if (e.Shift)
                {
                    RotationSpeed = rotateSpeedLow;
                    TranslationSpeed = translationSpeedHigh;
                }
                else
                {
                    RotationSpeed = rotateSpeedHigh;
                    TranslationSpeed = translationSpeedLow;
                }

                if (!keyData.Exists(k => k.KeyCode == e.KeyCode))
                    keyData.Add(e);
            }
        }

        protected virtual void OnKeyPress(KeyEventArgs key) { }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (IsHandlingInput && !IsDisposed)
            {
                keyData.RemoveAll(k => k.KeyCode == e.KeyCode);

                if (keyData.FindIndex(k => k.Shift) == -1)
                {
                    RotationSpeed = rotateSpeedLow;
                    TranslationSpeed = translationSpeedLow;
                }
                else
                {
                    RotationSpeed = rotateSpeedHigh;
                    TranslationSpeed = translationSpeedHigh;
                }
            }
        }

        protected abstract void OnLoad();

        /// <summary>
        /// Raises the Render event.
        /// </summary>
        protected virtual void OnRender()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(Name);
            if (Contents == null || Contents.Count < 1)
                return;

            int lastprogram = -1;

            foreach (var obj in Contents)
            {
                if (obj.Model.Shader.ProgramID != lastprogram)
                {
                    GL.UniformMatrix4(obj.Model.Shader.UniformProjection, false, ref ProjectionMatrix);
                    Camera.BindViewMatrix(obj.Model.Shader.UniformView);
                }
                lastprogram = obj.Model.Shader.ProgramID;
                obj.Render();
            }
        }

        /// <summary>
        /// Raises the Update event.
        /// </summary>
        /// <param name="delta">The time since the last update.</param>
        protected virtual void OnUpdate(double delta)
        {
            if (Camera.IsUserControlled && keyData.Count > 0)
            {
                float offset = (float)delta * TranslationSpeed;
                float rotoff = (float)delta * RotationSpeed;

                foreach (var key in keyData)
                {
                    switch (key.KeyCode)
                    {
                        case Keys.D:
                            Camera.Move(new Vector3(offset, 0, 0));
                            break;
                        case Keys.A:
                            Camera.Move(new Vector3(-offset, 0, 0));
                            break;

                        case Keys.E:
                            Camera.Move(new Vector3(0, offset, 0));
                            break;
                        case Keys.Q:
                            Camera.Move(new Vector3(0, -offset, 0));
                            break;

                        case Keys.S:
                            Camera.Move(new Vector3(0, 0, offset));
                            break;
                        case Keys.W:
                            Camera.Move(new Vector3(0, 0, -offset));
                            break;

                        // test out some roll control...
                        case Keys.NumPad8:
                            Camera.Rotate(new Quaternion(0, 0, -rotoff));
                            break;
                        case Keys.NumPad2:
                            Camera.Rotate(new Quaternion(0, 0, rotoff));
                            break;

                        case Keys.NumPad4:
                            Camera.Rotate(new Quaternion(0, -rotoff, 0));
                            break;
                        case Keys.NumPad6:
                            Camera.Rotate(new Quaternion(0, rotoff, 0));
                            break;

                        case Keys.NumPad7:
                            Camera.Rotate(new Quaternion(-rotoff, 0, 0));
                            break;
                        case Keys.NumPad9:
                            Camera.Rotate(new Quaternion(rotoff, 0, 0));
                            break;

                        default:
                            break;
                    }
                }
            }

            Camera.Update(delta);
            foreach (var obj in Contents)
                obj.Update(delta);
        }

        /// <summary>
        /// Refresh the projection matrix, such as after the canvas is resized.
        /// </summar
        /// <param name="width">The width of the new viewport.</param>
        /// <param name="height">The height of the new viewport.</param>
        protected virtual void ResetProjectionMatrix(int width, int height)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(Name);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV * ((float)Math.PI / 180f), width / (float)height, 1, 100000);
        }

        #endregion // --- Methods ---

        #endregion // --- protected implementation ---

        #region --- private implementation ---

        private bool IsSetup = false;
        private const float rotateSpeedHigh = 1;
        private const float rotateSpeedLow = 0.05f;
        private const float translationSpeedHigh = 5000;
        private const float translationSpeedLow = 500f;

        private void Parent_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        private void Parent_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        private void Parent_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, Parent.Width, Parent.Height);
            ResetProjectionMatrix(Parent.Width, Parent.Height);
            // Application.Idle invalidation doesn't work during a resize.
            Parent.Invalidate();
        }

        ~AScene()
        {
#if DEBUG
            Debug.WriteLine($"[WARN] Leaked scene {Name}; Did you forget to call Dispose()?");
#endif
            Dispose(false);
        }

        #endregion // --- private implementation ---
    }
}
