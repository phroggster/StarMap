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
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;

namespace StarMap.Cameras
{
    /// <summary>
    /// An entry-level camera: an object with which to inspect a scene.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class Camera : ICamera
    {
        #region --- protected Camera(Vector3 position, Quaternion orientation) ---

        /// <summary>
        /// Constructs a new <see cref="Camera"/> instance.
        /// </summary>
        /// <param name="position">The location of the camera.</param>
        /// <param name="orientation">The orientation of the camera.</param>
        protected Camera(Vector3 position, Quaternion orientation)
        {
            Position = position;
            Orientation = orientation;
            UpdateViewMatrix();
        }

        #endregion // --- protected Camera(Vector3 position, Quaternion orientation) ---

        #region --- ICamera interface ---

        #region --- Properties ---

        /// <summary>
        /// Gets a value indicating whether the user is the primary input source for this <see cref="Camera"/> (<c>true</c>), or
        /// if it's a static <see cref="Camera"/>, or a <see cref="Camera"/> controlled solely by software (<c>false</c>).
        /// </summary>
        public virtual bool IsUserControlled { get; protected set; } = false;

        /// <summary>
        /// The name of this <see cref="Camera"/>.
        /// </summary>
        public virtual string Name { get; set; } = nameof(Camera);

        /// <summary>
        /// The orientation of this camera.
        /// </summary>
        public virtual Quaternion Orientation { get; protected set; } = Quaternion.Identity;

        /// <summary>
        /// The world-space coordinates of this camera.
        /// </summary>
        public virtual Vector3 Position { get; protected set; } = Vector3.Zero;

        /// <summary>
        /// The camera's <see cref="Matrix4"/>. Converts world-space to camera-space (TODO: is that backwards?).
        /// </summary>
        public virtual Matrix4 ViewMatrix { get { return m_ViewMatrix; } }

        #endregion // --- Properties ---

        #region --- Methods ---

        public virtual void AbortLerp()
        {
            if (IsLerping)
            {
                IsLerping = false;
                LerpCompletion = 0;
                LerpToOrientation = Orientation;
                LerpToPosition = Position;
            }
        }

        /// <summary>
        /// Animate the position and rotation of the camera.
        /// </summary>
        /// <param name="position">The position of the object to view.</param>
        /// <param name="speed">Normalized (from 0 [slow], to 1 [immedate]), how quickly the animation should complete.</param>
        /// <param name="orientation">The desired final orientation.</param>
        public virtual void BeginLerp(Vector3 position, float speed, Quaternion orientation)
        {
            LerpBeginOrientation = Orientation;
            LerpBeginPosition = Position;
            LerpToOrientation = orientation;
            LerpToPosition = position;
            LerpSpeed = speed;
            IsLerping = true;
        }

        public virtual void BindViewMatrix(int uniform)
        {
            GL.UniformMatrix4(uniform, false, ref m_ViewMatrix);
        }

        public virtual void LookAt(Vector3 target)
        {
            float cosTheta = Vector3.Dot(Position, target);
            float s = (float)Math.Sqrt((1 + cosTheta) * 2);
            float invs = 1 / s;
            Vector3 rotAxis = Vector3.Cross(Position, target);

            Orientation = new Quaternion(s * 0.5f, rotAxis.X * invs, rotAxis.Y * invs, rotAxis.Z * invs);
            UpdateViewMatrix();
        }

        /// <summary>
        /// Move the camera by the provided camera-space offset.
        /// </summary>
        /// <param name="offset">The camera-space offset to move the camera by.</param>
        public virtual void Move(Vector3 offset)
        {
            // TODO: This doesn't work. Yaw/pitch/roll +/- 90° and forward/strafe is inverted; 180° and it's fine...
            Position -= Orientation * offset;
            UpdatePosMatrix();
        }

        /// <summary>
        /// Immediately move the camera to the provided position.
        /// </summary>
        /// <param name="position">The world coordinates of which the camera should be moved.</param>
        public virtual void MoveTo(Vector3 position)
        {
            Position = position;
            UpdatePosMatrix();
        }

        /// <summary>
        /// Rotate the camera by the provided amount.
        /// </summary>
        /// <param name="rotation"></param>
        public virtual void Rotate(Quaternion rotation)
        {
            Orientation = Quaternion.Multiply(Orientation, rotation).Normalized();
            UpdateRotMatrix();
        }

        /// <summary>
        /// Immediately rotate the camera to the provided orientation.
        /// </summary>
        /// <param name="orientation"></param>
        public virtual void RotateTo(Quaternion orientation)
        {
            Orientation = orientation;
            UpdateRotMatrix();
        }

        /// <summary>
        /// Updates this camera's <see cref="m_ViewMatrix"/> given any user input or animation.
        /// </summary>
        /// <param name="delta">The time in seconds since the last update.</param>
        public virtual void Update(double delta)
        {
            // TODO: Test lerping
            if (IsLerping)
            {
                LerpCompletion += LerpSpeed * (float)delta;
                LerpCompletion = Math.Min(LerpCompletion, 1);
                Position = Vector3.Lerp(Position, LerpToPosition, LerpCompletion);
                Orientation = Quaternion.Slerp(Orientation, LerpToOrientation, LerpCompletion);
                if (LerpCompletion >= 1)
                {
                    IsLerping = false;
                    LerpCompletion = 0;
                }
                UpdateViewMatrix();
            }
        }

        #endregion // --- Methods ---

        #endregion // --- ICamera interface ---

        #region --- protected implementation ---

        protected bool hasmoved { get; set; } = true;
        protected bool IsLerping { get; set; } = false;
        protected Quaternion LerpBeginOrientation { get; set; } = Quaternion.Identity;
        protected Vector3 LerpBeginPosition { get; set; } = Vector3.Zero;
        protected float LerpCompletion { get; set; } = 0;
        protected float LerpSpeed { get; set; } = 0.1f;
        protected Quaternion LerpToOrientation { get; set; }
        protected Vector3 LerpToPosition { get; set; }

        protected virtual string DebuggerDisplay
        {
            get
            {
                return string.Format("{0}: {1}, Position {2}, Orientation {3}", Name, IsUserControlled ? "Movable" : "Not movable", Position, Orientation);
            }
        }

        #endregion // --- protected implementation ---

        #region --- private implementation ---

        private Matrix4 m_ViewMatrix;
        private Matrix4 m_RotationMatrix = Matrix4.Identity;
        private Matrix4 m_TranslateMatrix = Matrix4.Identity;


        private void UpdatePosMatrix()
        {
            m_TranslateMatrix = Matrix4.CreateTranslation(Position);
            m_ViewMatrix = m_TranslateMatrix * m_RotationMatrix;
        }

        private void UpdateRotMatrix()
        {
            m_RotationMatrix = Matrix4.CreateFromQuaternion(Orientation);
            m_ViewMatrix = m_TranslateMatrix * m_RotationMatrix;
        }

        private void UpdateViewMatrix()
        {
            m_RotationMatrix = Matrix4.CreateFromQuaternion(Orientation);
            m_TranslateMatrix = Matrix4.CreateTranslation(Position);
            m_ViewMatrix = m_TranslateMatrix * m_RotationMatrix;
        }

        #endregion // --- private implementation ---
    }
}
