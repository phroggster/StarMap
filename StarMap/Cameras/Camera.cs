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
using StarMap.Scenes;
using System;
using System.Diagnostics;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif
#endregion // --- using ... ---

namespace StarMap.Cameras
{
    /// <summary>
    /// An entry-level camera: an object with which to inspect a scene.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class Camera : ICamera
    {
        #region --- protected Camera(Vector3, Quaternion, IScene) ---

        /// <summary>
        /// Constructs a new <see cref="Camera"/> instance.
        /// </summary>
        /// <param name="position">The location of the camera.</param>
        /// <param name="orientation">The orientation of the camera.</param>
        protected Camera(Vector3 position, Quaternion orientation, IScene scene)
        {
            Position = new Vector3(position.X, position.Y, -position.Z);
            Orientation = orientation;
            ParentScene = scene;
        }

        #endregion // --- protected Camera(Vector3, Quaternion, IScene) ---

        #region --- ICamera interface ---

        #region --- Properties ---

        // TODO: this is total rubbish. Have to take orientation into account, maybe raytrace through view matrix in reverse. Ugh.
        public Vector3 FocalPoint
        {
            get
            {
                Vector4 bob = new Vector4(0, 0, 4000, 1) * ViewMatrix;
                return new Vector3(bob.X, bob.Y, -bob.Z);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is the primary input source for this <see cref="Camera"/> (<c>true</c>), or
        /// if it's a static <see cref="Camera"/>, or a <see cref="Camera"/> controlled solely by software (<c>false</c>).
        /// </summary>
        public bool IsUserControlled { get; protected set; } = false;

        /// <summary>
        /// The name of this <see cref="Camera"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The orientation of this camera.
        /// </summary>
        public Quaternion Orientation { get; protected set; }

        /// <summary>
        /// The world-space coordinates of this camera.
        /// </summary>
        public Vector3 Position { get; protected set; }

        /// <summary>
        /// The camera's <see cref="Matrix4"/>. Converts world-space to camera-space (TODO: is that backwards?).
        /// </summary>
        public Matrix4 ViewMatrix { get { return m_ViewMatrix; } }

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
                LerpToFOV = ParentScene.FOV;
            }
        }

        /// <summary>
        /// Animate the position and rotation of the camera.
        /// </summary>
        /// <param name="position">The position of the object to view.</param>
        /// <param name="speed">Normalized (from 0 [slow], to 1 [immedate]), how quickly the animation should complete.</param>
        /// <param name="orientation">The desired final orientation.</param>
        public virtual void BeginLerp(float speed, Vector3 position, Quaternion orientation, float FOV = 45)
        {
            LerpSpeed = speed;

            LerpBeginOrientation = Orientation;
            LerpToOrientation = orientation;

            LerpBeginPosition = Position;
            LerpToPosition = new Vector3(position.X, position.Y, -position.Z);

            LerpBeginFOV = ParentScene.FOV;
            LerpToFOV = FOV;

            IsLerping = true;
        }

        public virtual void LookAt(Vector3 target)
        {
            float cosTheta = Vector3.Dot(Position, target);
            float s = (float)Math.Sqrt((1 + cosTheta) * 2);
            float invs = 1 / s;
            Vector3 rotAxis = Vector3.Cross(Position, target);

            Orientation = new Quaternion(s * 0.5f, rotAxis.X * invs, rotAxis.Y * invs, rotAxis.Z * invs);
            IsViewMatDirty = true;
        }

        /// <summary>
        /// Moves this <see cref="Camera"/> by a world-space offset.
        /// </summary>
        /// <param name="offset">The worldspace offset to adjust this <see cref="Camera"/> by.</param>
        public virtual void Move(Vector3 offset)
        {
            if (!IsLerping)
            {
                Position += offset;
                IsViewMatDirty = true;
            }
        }

        /// <summary>
        /// Immediately move the camera to the provided position.
        /// </summary>
        /// <param name="position">The world coordinates of which the camera should be moved.</param>
        public virtual void MoveTo(Vector3 position)
        {
            Position = position;
            IsViewMatDirty = true;
        }

        /// <summary>
        /// Rotate the camera by the provided amount.
        /// </summary>
        /// <param name="rotation"></param>
        public virtual void Rotate(Quaternion rotation)
        {
            if (!IsLerping)
            {
                Orientation = Quaternion.Multiply(rotation, Orientation).Normalized();
                IsViewMatDirty = true;
            }
        }

        /// <summary>
        /// Immediately rotate the camera to the provided orientation.
        /// </summary>
        /// <param name="orientation"></param>
        public virtual void RotateTo(Quaternion orientation)
        {
            Orientation = orientation;
            IsViewMatDirty = true;
        }

        /// <summary>
        /// Translates this <see cref="Camera"/> by a camera-space offset.
        /// <para><c>X</c>: (-) left, to (+) right; <c>Y</c>: (-) down, to (+) up; <c>Z</c>: (-) forward, to (+) backward.</para>
        /// </summary>
        /// <param name="offset">The camera-space offset to translate the <see cref="Camera"/> by.</param>
        public virtual void Translate(Vector3 offset)
        {
            if (!IsLerping)
            {
                Vector4 off4 = ViewMatrix * new Vector4(offset);
                Position -= off4.Xyz;
                IsViewMatDirty = true;
            }
        }

        /// <summary>
        /// Updates this camera's <see cref="m_ViewMatrix"/> given any user input or animation.
        /// </summary>
        /// <param name="delta">The time in seconds since the last update.</param>
        public bool Update(double delta)
        {
            if (IsLerping)
            {
                IsViewMatDirty = true;
                LerpCompletion += LerpSpeed * (float)delta;
                LerpCompletion = Math.Min(LerpCompletion, 1);
                Position = Vector3.Lerp(Position, LerpToPosition, LerpCompletion);
                Orientation = Quaternion.Slerp(Orientation, LerpToOrientation, LerpCompletion);
                ParentScene.FOV = ObjectExtensions.Lerp(LerpBeginFOV, LerpToFOV, LerpCompletion);

                // TODO: Orientation can get *very* close to LerpToOrientation, yet still not pass the equality check.
                if (LerpCompletion >= 1 ||
                    (Position == LerpToPosition && Orientation == LerpToOrientation && ParentScene.FOV == LerpToFOV))
                {
                    IsLerping = false;
                    LerpCompletion = 0;
                    Program.MainFrm.ProgressPercentage = 0;
                }
                else
                    Program.MainFrm.ProgressPercentage = (int)Math.Round(LerpCompletion * 100);
            }

            if (IsViewMatDirty)
            {
                m_ViewMatrix = Matrix4.CreateTranslation(Position) * Matrix4.CreateFromQuaternion(Orientation);
                IsViewMatDirty = false;
                return true;
            }
            return false;
        }

        #endregion // --- Methods ---

        #endregion // --- ICamera interface ---

        #region --- protected implementation ---

        protected IScene ParentScene { get; set; } = null;

        protected bool IsLerping { get; set; } = false;
        protected float LerpBeginFOV { get; set; } = 45;
        protected Quaternion LerpBeginOrientation { get; set; } = Quaternion.Identity;
        protected Vector3 LerpBeginPosition { get; set; } = Vector3.Zero;
        protected float LerpCompletion { get; set; } = 0;
        protected float LerpSpeed { get; set; } = 0.1f;
        protected float LerpToFOV { get; set; } = 45;
        protected Quaternion LerpToOrientation { get; set; }
        protected Vector3 LerpToPosition { get; set; }

        protected virtual string DebuggerDisplay
        {
            get
            {
                return string.Format("{0}: {1}, Position {2}, Orientation {3}", Name, IsUserControlled ? "Movable" : "Not movable", new Vector3(Position.X, Position.Y, -Position.Z), Orientation);
            }
        }

        #endregion // --- protected implementation ---

        #region --- private implementation ---

        private Matrix4 m_ViewMatrix;
        private bool IsViewMatDirty = true;

        #endregion // --- private implementation ---
    }
}
