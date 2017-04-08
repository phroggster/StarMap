using Phroggiesoft.Controls;
using StarMap.Cameras;
using StarMap.SceneObjects;
using StarMap.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace StarMap.Scenes
{
    /// <summary>
    /// The public interface for a <see cref="Scene"/> component..
    /// </summary>
    public interface IScene : IComponent, IIsDisposed
    {
        event EventHandler<string> FPSUpdate;

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
        IList<ISceneObject> Contents { get; set; }

        /// <summary>
        /// The frame rate (frames per second) that this scene is rendering at.
        /// </summary>
        string FrameRate { get; }

        /// <summary>
        /// Whether or not this <see cref="IScene"/> is fully loaded.
        /// </summary>
        /// <seealso cref="Load"/>
        bool IsLoaded { get; }

        /// <summary>
        /// The name of this <see cref="IScene"/>.
        /// </summary>
        string Name { get; }
        phrogGLControl Parent { get; }

        /// <summary>
        /// The toggle keys that this scene is concerned with.
        /// </summary>
        IList<Keys> ToggleKeys { get; set; }

        /// <summary>
        /// How fast this scene's camera can translate.
        /// </summary>
        float TranslationSpeed { get; set; }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Load the scene resources from disk, etc, to vRAM. This will block the calling thread, potentially for a rather long time.
        /// <para>See also: <see cref="SceneTransitions.LoadAsync(GLControl, IScene)"/></para>
        /// </summary>
        void Load(phrogGLControl parent);

        /// <summary>
        /// Renders the scene.
        /// </summary>
        void Render();

        void Start();

        void Stop();

        /// <summary>
        /// Updates the scene in preparation for rendering.
        /// </summary>
        /// <param name="delta">The time that has elapsed since the last frame.</param>
        void Update(double delta);

        #endregion // Methods
    }
}
