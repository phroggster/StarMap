using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarMap.Cameras
{
    public interface ICamera
    {
        #region --- Properties ---

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
        void BeginLerp(Vector3 position, float speed, Quaternion orientation);

        void LookAt(Vector3 target);

        /// <summary>
        /// Move the camera by the provided camera-space offset.
        /// </summary>
        /// <param name="offset">The camera-space offset to move the camera by.</param>
        void Move(Vector3 offset);

        /// <summary>
        /// Immediately move the camera to the provided position.
        /// </summary>
        /// <param name="position">The world coordinates of which the camera should be moved.</param>
        void MoveTo(Vector3 position);

        /// <summary>
        /// Rotate the camera by the provided amount.
        /// </summary>
        void Rotate(Quaternion rotation);

        /// <summary>
        /// Immediately rotate the camera to the provided orientation.
        /// </summary>
        void RotateTo(Quaternion orientation);

        /// <summary>
        /// Updates this <see cref="ICamera"/> based on the elapsed time since last update.
        /// </summary>
        /// <param name="delta">The time in seconds since the last update.</param>
        bool Update(double delta);

        #endregion // --- Methods ---
    }
}
