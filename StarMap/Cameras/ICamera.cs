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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion // --- using ... ---

namespace StarMap.Cameras
{
    public interface ICamera
    {
        #region --- Properties ---

        Vector3 FocalPoint { get; }

        /// <summary>
        /// Gets a value indicating whether the user is the primary input source for this <see cref="ICamera"/> (<c>true</c>), or
        /// if it's a static <see cref="ICamera"/>, or an <see cref="ICamera"/> controlled solely by software (<c>false</c>).
        /// </summary>
        bool IsUserControlled { get; }

        /// <summary>
        /// The name of this <see cref="ICamera"/>.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The orientation of this camera.
        /// </summary>
        Quaternion Orientation { get; }

        /// <summary>
        /// The world-space coordinates of this camera.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// The camera's <see cref="Matrix4"/>. Converts world-space to camera-space (TODO: is that backwards?).
        /// This is typically built from <see cref="Orientation"/> and <see cref="Position"/>, but may vary.
        /// </summary>
        Matrix4 ViewMatrix { get; }

        #endregion // --- Properties ---


        #region --- Methods ---

        #region --- Lerpage ---

        /// <summary>
        /// Cancels any in progress movement/rotation animations.
        /// </summary>
        /// <seealso cref="BeginLerp(Vector3, float, Quaternion)"/>
        void AbortLerp();

        /// <summary>
        /// Animate the position and rotation of the camera.
        /// </summary>
        /// <param name="position">The position of the object to view.</param>
        /// <param name="speed">Normalized (from 0 [slow], to 1 [immedate]), how quickly the animation should complete.</param>
        /// <param name="orientation">The desired final orientation.</param>
        /// <seealso cref="AbortLerp"/>
        void BeginLerp(float speed, Vector3 position, Quaternion orientation, float FOV);

        #endregion // --- Lerpage ---

        #region --- Camera-space transformations ---

        /// <summary>
        /// Rotate the camera by the provided amount.
        /// </summary>
        void MouseLook(Vector2 input);

        /// <summary>
        /// Translate an <see cref="ICamera"/> by a camera-space offset.
        /// <para><c>X</c>: (-) left, to (+) right; <c>Y</c>: (-) down, to (+) up; <c>Z</c>: (-) forward, to (+) backward.</para>
        /// </summary>
        /// <param name="offset">The camera-space offset to apply.</param>
        void Translate(Vector3 offset);

        #endregion // --- Camera-space transformations ---

        #region --- World-space transformations ---

        /// <summary>
        /// Move and rotate an <see cref="ICamera"/> to view the provided world-space location.
        /// </summary>
        /// <param name="target">The world-space location to look at.</param>
        void LookAt(Vector3 target);

        /// <summary>
        /// Move an <see cref="ICamera"/> by a world-space offset.
        /// </summary>
        /// <param name="offset">The world-space offset to apply.</param>
        void Move(Vector3 offset);

        /// <summary>
        /// Immediately move an <see cref="ICamera"/> to the provided position.
        /// </summary>
        /// <param name="position">The world-space coordinates to which the camera should be moved.</param>
        void MoveTo(Vector3 position);

        /// <summary>
        /// Immediately rotate the camera to the provided orientation.
        /// </summary>
        void RotateTo(Quaternion orientation);

        #endregion // --- World-space transformations ---

        /// <summary>
        /// Updates this <see cref="ICamera"/> based on the elapsed time since last update.
        /// </summary>
        /// <param name="delta">The time in seconds since the last update.</param>
        bool Update(double delta);

        #endregion // --- Methods ---
    }
}
