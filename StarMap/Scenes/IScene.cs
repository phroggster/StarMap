using StarMap.Cameras;
using StarMap.Objects;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace StarMap.Scenes
{
    /// <summary>
    /// The public interface for a scene, which is presently backed by an <see cref="AScene"/>.
    /// </summary>
    public interface IScene : IIsDisposed
    {
        #region Properties

        /// <summary>
        /// The background <see cref="Color"/> of this scene.
        /// </summary>
        Color BackColor { get; set; }

        /// <summary>
        /// The <see cref="Cameras.Camera"/> that this scene uses.
        /// </summary>
        ICamera Camera { get; set; }

        /// <summary>
        /// All of the <see cref="AObject"/>s that will be rendered in this scene.
        /// </summary>
        List<AObject> Contents { get; set; }

        /// <summary>
        /// The <see cref="Camera"/>'s field of view for this scene.
        /// </summary>
        float FOV { get; set; }

        /// <summary>
        /// Whether or not this <see cref="IScene"/> is fully loaded.
        /// </summary>
        /// <seealso cref="Load"/>
        bool IsLoaded { get; }

        /// <summary>
        /// The name of this <see cref="IScene"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The toggle keys that this scene is concerned with.
        /// </summary>
        List<Keys> ToggleKeys { get; set; }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Receives external KeyDown notifications, and raises scene-specific KeyDown and KeyPress events.
        /// </summary>
        /// <param name="e">Provides data for the KeyDown and KeyUp events.</param>
        void KeyDown(KeyEventArgs e);

        /// <summary>
        /// Receives external KeyUp notifications, and raises scene-specific KeyUp events.
        /// </summary>
        /// <param name="e">Provides data for the KeyDown and KeyUp events.</param>
        void KeyUp(KeyEventArgs e);

        /// <summary>
        /// Loads the scene. This will block the calling thread, potentially for a rather long time.
        /// </summary>
        void Load();

        /// <summary>
        /// Renders the scene.
        /// </summary>
        void Render();

        /// <summary>
        /// Refresh the projection matrix, such as after the canvas is resized.
        /// </summar
        /// <param name="width">The width of the new viewport.</param>
        /// <param name="height">The height of the new viewport.</param>
        void ResetProjectionMatrix(int width, int height);

        /// <summary>
        /// Updates the scene in preparation for rendering.
        /// </summary>
        /// <param name="delta">The time that has elapsed since the last frame.</param>
        void Update(double delta);

        #endregion // Methods
    }
}
