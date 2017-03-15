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
    public abstract class Camera
    {
        #region Properties

        /// <summary>
        /// The galaxy-space to camera-space matrix.
        /// </summary>
        public Matrix4 ViewMatrix;
        public virtual string DebuggerDisplay
        {
            get
            {
                return string.Format("Camera {0}: {1}, Position {2}, Rotation {3}", Name, IsUserMovable ? "Movable" : "Not movable", Position, Rotation);
            }
        }
        /// <summary>
        /// Returns whether or not this camera can be directly moved by the user.
        /// </summary>
        public virtual bool IsUserMovable { get; protected set; } = false;
        public abstract string Name { get; }
        /// <summary>
        /// The world-space coordinates of this camera.
        /// </summary>
        public virtual Vector3 Position { get; protected set; } = new Vector3();
        /// <summary>
        /// The orientation of this camera.
        /// </summary>
        public virtual Vector3 Rotation { get; protected set; } = new Vector3();

        protected bool IsLerping { get; set; } = false;
        protected Vector3 LerpBeginPosition;
        protected Vector3 LerpBeginRotation;
        protected float LerpSpeed { get; set; } = 0.1f;
        protected Vector3 LerpToPosition { get; set; }
        protected Vector3 LerpToRotation { get; set; }
        protected float LerpCompletion = 0;
        protected bool hasmoved = true;

        #endregion

        /// <summary>
        /// Constructs a new <see cref="Camera"/>.
        /// </summary>
        /// <param name="position">The location of the camera, in world-coordinates.</param>
        /// <param name="orientation">The orientation of the camera.</param>
        public Camera(Vector3 position, Vector3 up)
        {
            Position = position;
            Rotation = up;
        }

        /// <summary>
        /// Animate the position and rotation of the camera.
        /// </summary>
        /// <param name="position">The position of the object to view.</param>
        /// <param name="speed">Normalized (from 0 [slow], to 1 [immedate]), how quickly the animation should complete.</param>
        /// <param name="rotation">The desired final orientation.
        /// to be aimed directly at <paramref name="position"/>.</param>
        public virtual void BeginLerp(Vector3 position, float speed, Vector3 rotation)
        {
            LerpBeginPosition = Position;
            LerpToPosition = position;
            LerpBeginRotation = Rotation;
            LerpToRotation = rotation;
            LerpSpeed = speed;
            IsLerping = true;
        }

        public virtual void LookAt(Vector3 target)
        {
            ViewMatrix = Matrix4.LookAt(
                Position,
                target,
                Rotation);
        }

        /// <summary>
        /// Immediately move the camera to the provided position.
        /// </summary>
        /// <param name="position">The world coordinates of which the camera should be moved.</param>
        public virtual void MoveTo(Vector3 position)
        {
            Position = position;
            hasmoved = true;
        }

        /// <summary>
        /// Move the camera by the provided camera-space offset.
        /// </summary>
        /// <param name="offset">The camera-space offset to move the camera by.</param>
        public virtual void Move(Vector3 offset)
        {
            // TODO: This doesn't work.
            Position += offset * Rotation;
            hasmoved = true;
        }

        bool hasbroken = false;

        /// <summary>
        /// Updates this camera's <see cref="ViewMatrix"/> given any user input or animation.
        /// </summary>
        /// <param name="delta">The time in seconds since the last update.</param>
        public virtual void Update(double delta)
        {
            // TODO: Test lerp
            if (IsLerping)
            {
                hasmoved = true;
                LerpCompletion += Math.Max(LerpSpeed * (float)delta, 1);
                Position = Vector3.Lerp(Position, LerpToPosition, LerpCompletion);
                Rotation = Vector3.Lerp(Rotation, LerpToRotation, LerpCompletion);
                if (LerpCompletion >= 1)
                {
                    IsLerping = false;
                    LerpCompletion = 0;
                }
            }

            if (hasmoved)
            {
                ViewMatrix = Matrix4.LookAt(Position, -Vector3.UnitZ * Rotation, Vector3.UnitY);
                hasmoved = false;
            }
        }
    }
}
