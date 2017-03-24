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
    public abstract class AScene : IIsDisposed, IScene
    {
        #region --- public interface ---

        #region --- Constructors ---

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
        public virtual List<AObject> Contents { get; set; } = new List<AObject>();
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
        public virtual List<Keys> ToggleKeys { get; set; } = new List<Keys>();
        /// <summary>
        /// How fast this scene's camera can translate.
        /// </summary>
        public virtual float TranslationSpeed { get; set; } = translationSpeedLow;

        public virtual float RotationSpeed { get; set; } = rotateSpeedLow;
        #endregion // --- Properties ---

        #region --- Methods ---

        /// <summary>
        /// Receives external KeyDown notifications, and raises scene-specific KeyDown and KeyPress events.
        /// </summary>
        /// <param name="e">Provides data for the KeyDown and KeyUp events.</param>
        public void KeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        /// <summary>
        /// Receives external KeyUp notifications, and raises scene-specific KeyUp events.
        /// </summary>
        /// <param name="e">Provides data for the KeyDown and KeyUp events.</param>
        public void KeyUp(KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        /// <summary>
        /// Loads the scene. This will block the calling thread, potentially for a rather long time.
        /// </summary>
        public void Load()
        {
            if (!IsLoaded)
            {
                Trace.WriteLine($"[DEBUG] Loading {Name}.");
                OnLoad();
                IsLoaded = true;
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
        /// Refresh the projection matrix, such as after the canvas is resized.
        /// </summar
        /// <param name="width">The width of the new viewport.</param>
        /// <param name="height">The height of the new viewport.</param>
        public virtual void ResetProjectionMatrix(int width, int height)
        {
            if (IsDisposed) throw new ObjectDisposedException(Name);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV * ((float)Math.PI / 180f), width / (float)height, 1, 100000);
        }

        /// <summary>
        /// Update the scene in preparation for rendering.
        /// </summary>
        /// <param name="delta">The time that has elapsed since the last frame.</param>
        public void Update(double delta)
        {
            if (IsDisposed) throw new ObjectDisposedException(Name);
            OnUpdate(delta);
        }

        #endregion // --- Methods ---

        #endregion // --- IScene interface ---

        #endregion // --- public interface ---

        #region --- protected implementation ---

        protected Matrix4 ProjectionMatrix;
        protected List<KeyEventArgs> keyData = new List<KeyEventArgs>();

        protected virtual string DebuggerDisplay
        {
            get
            {
                return string.Format("{0}: {1} objects, {2}, fov {3}°", Name, Contents.Count, Camera.Position.ToString(), FOV);
            }
        }

        #region --- Methods ---

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Trace.WriteLine($"[DEBUG] Disposing of {Name}.");
                if (disposing)
                {
                    Contents?.Clear();
                    keyData?.Clear();
                }
                Camera = null;
                Contents = null;
                keyData = null;
            }
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
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

        protected virtual void OnKeyPress(KeyEventArgs key) { }

        protected virtual void OnKeyUp(KeyEventArgs e)
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

        #endregion // --- Methods ---

        #endregion // --- protected implementation ---

        #region --- private implementation ---

        private const float rotateSpeedHigh = 1;
        private const float rotateSpeedLow = 0.05f;
        private const float translationSpeedHigh = 5000;
        private const float translationSpeedLow = 500f;

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
