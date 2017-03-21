#region License
//
// The Open Toolkit Library License
//
// Author:
//       phroggie <phroggster@gmail.com>
//
// Copyright (c) 2006-2013 Stefanos Apostolopoulos
// Copyright (c) 2017 phroggie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using Phroggiesoft.EditorControls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Phroggiesoft.Controls
{
    /// <summary>
    /// An extended OpenTK.GLControl (an OpenGL-aware WinForms control) fork allowing
    /// for additional design-time features such as specifying GL major.minor modes.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [Description("An extended OpenTK.GLControl fork allowing for additional design and run-time features.")]
    [Designer(typeof(Design.GLControlDesigner))]
    [DefaultEvent("Paint")]
    [DisplayName("GLControl")]
    [InitializationEvent("Load")]
    [ToolboxBitmap(typeof(GLControl), "Resources.GLControl.Toolbox.bmp")]
    [ToolboxItem(true)]
    public partial class GLControl : Control, ISupportInitialize
    {
        #region --- Public interface ---

        #region --- Constructor ---

        /// <summary>
        /// Constructs a new <see cref="GLControl"/> instance.
        /// </summary>
        public GLControl()
        {
            design_mode = DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            InitializeComponent();

            if (!design_mode)
            {
                Toolkit.Init(new ToolkitOptions
                {
                    Backend = PlatformBackend.PreferNative
                });

                SetStyle(ControlStyles.Opaque, true);
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                DoubleBuffered = false;
            }
        }

        #endregion // --- Constructor ---

        #region --- Properties ---

        #region --- Designer-visible properties ---

        #region --- public float AspectRatio { get; } ---

        /// <summary>
        /// Gets the aspect ratio of this GLControl.
        /// </summary>
        [Browsable(true)]
        [Category("Misc.")]
        [Description("The aspect ratio of the client area of this GLControl.")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public float AspectRatio
        {
            get
            {
                return ClientSize.Width / (float)ClientSize.Height;
            }
        }

        #endregion // --- public float AspectRatio { get; } ---

        #region --- public ContextFlags ContextFlags { get; set; } ---

        /// <summary>
        /// The <see cref="GraphicsContextFlags"/> for the OpenGL <see cref="OpenTK.Graphics.GraphicsContext"/>.
        /// Do not set this property during run-time.
        /// </summary>
        [Browsable(true)]
        [Category("OpenGL")]
        [DefaultValue(GraphicsContextFlags.Default)]
        [Description("Allows for utilizing advanced features, such as debugging, of this control.")]
        [Editor(typeof(FlagsListBoxUIEditor), typeof(UITypeEditor))]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public GraphicsContextFlags ContextFlags
        {
            get { return _ContextFlags; }
            set
            {
                if (_ContextFlags != value)
                {
                    _ContextFlags = value;
                    if (!design_mode && !Initializing)
                    {
                        // TODO: Test this.
                        DestroyHandle();
                        RecreateHandle();
                    }
                }
            }
        }
        private GraphicsContextFlags _ContextFlags = GraphicsContextFlags.Default;

        #endregion // --- public ContextFlags ContextFlags { get; set; } ---

        #region --- public int GLMajorVersion { get; set; } ---

        /// <summary>
        /// The major version for the OpenGL GraphicsContext.
        /// Do not set this property during run-time.
        /// </summary>
        [Browsable(true)]
        [Category("OpenGL")]
        [DefaultValue(1)]
        [Description("The major version for the OpenGL GraphicsContext.")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int GLMajorVersion
        {
            get { return _GLMajorVersion; }
            set
            {
                if (_GLMajorVersion != value)
                {
                    _GLMajorVersion = value;
                    if (!Initializing && !design_mode)
                    {
                        // TODO: Test this.
                        DestroyHandle();
                        RecreateHandle();
                    }
                }
            }
        }
        private int _GLMajorVersion = 1;

        #endregion // --- public int GLMajorVersion { get; set; } ---

        #region --- public int GLMinorVersion { get; set; } ---

        /// <summary>
        /// The minor version for the OpenGL GraphicsContext.
        /// Do not set this property during run-time.
        /// </summary>
        [Browsable(true)]
        [Category("OpenGL")]
        [DefaultValue(0)]
        [Description("The minor version for the OpenGL GraphicsContext.")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int GLMinorVersion
        {
            get { return _GLMinorVersion; }
            set
            {
                if (_GLMinorVersion != value)
                {
                    _GLMinorVersion = value;
                    if (!Initializing && !design_mode)
                    {
                        // TODO: Test this.
                        DestroyHandle();
                        RecreateHandle();
                    }
                }
            }
        }
        private int _GLMinorVersion = 0;

        #endregion // --- public int GLMinorVersion { get; set; } ---

        #region --- public bool VSync { get; set; } ---

        /// <summary>
        /// Gets or sets a value indicating whether vsync is active for this <see cref="GLControl"/>.
        /// Setting this property requires a valid <see cref="Context"/>.
        /// </summary>
        /// <seealso cref="Context"/>
        /// <seealso cref="MakeCurrent"/>
        [Browsable(true)]
        [Category("OpenGL")]
        [DefaultValue(false)]
        [Description("Indicates whether GLControl updates are synced to the monitor's refresh rate.")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool VSync
        {
            get { return _VSync; }
            set
            {
                _VSync = value;
                if (!Initializing && !design_mode && IsHandleCreated)
                {
                    ValidateContext("VSync");
                    Context.SwapInterval = value ? 1 : 0;
                }
            }
        }
        private bool _VSync = false;

        #endregion // --- public bool VSync { get; set; } ---

        #endregion // --- Designer-visible properties ---

        #region --- Run-time properties ---

        #region --- public IGraphicsContext Context { get; private set; } ---

        /// <summary>
        /// <para>Gets the <see cref="IGraphicsContext"/> instance that is associated with this <see cref="GLControl"/>.
        /// The associated <see cref="IGraphicsContext"/> is updated whenever the <see cref="GLControl"/> handle is
        /// created or recreated.</para>
        /// <para>When using multiple <see cref="GLControl"/>s, ensure that <c>Context</c> is current by calling
        /// <see cref="MakeCurrent"/> before performing any OpenGL operations.</para>
        /// </summary>
        /// <seealso cref="MakeCurrent"/>
        [Browsable(false)]
        public IGraphicsContext Context
        {
            get
            {
                ValidateState();
                return _Context;
            }
            private set
            {
                _Context = value;
            }
        }
        private IGraphicsContext _Context;

        #endregion // --- public IGraphicsContext Context { get; private set; } ---

        #region --- public bool IsIdle { get; } ---

        /// <summary>
        /// Gets a value indicating whether the current thread contains pending system messages.
        /// </summary>
        [Browsable(false)]
        public bool IsIdle
        {
            get
            {
                ValidateState();
                return Implementation.IsIdle;
            }
        }

        #endregion // --- public bool IsIdle { get; } ---

        #region --- public GraphicsMode GraphicsMode { get; set; } ---

        /// <summary>
        /// Gets or sets the <see cref="GraphicsMode"/> of the <see cref="IGraphicsContext"/>
        /// associated with this <see cref="GLControl"/>.
        /// </summary>
        [Bindable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public GraphicsMode GraphicsMode
        {
            get
            {
                if (!design_mode)
                    return Context.GraphicsMode;
                else
                    return DesiredGraphicsMode;
            }
            set
            {
                if (!DesiredGraphicsMode.Equals(value))
                {
                    // TODO: Recreate somehow?
                    DesiredGraphicsMode = value;
                }
            }
        }

        #endregion // --- public GraphicsMode GraphicsMode { get; } ---

        #region --- public IWindowInfo WindowInfo { get; } ---

        /// <summary>
        /// Gets the <see cref="IWindowInfo"/> for this instance.
        /// </summary>
        [Browsable(false)]
        public IWindowInfo WindowInfo
        {
            get
            {
                return Implementation.WindowInfo;
            }
        }

        #endregion // --- public IWindowInfo WindowInfo { get; } ---

        #endregion // --- Run-time properties ---

        #endregion // --- Properties ---

        #region --- Events ---

        /// <summary>
        /// Occurs once before the control is first displayed.
        /// </summary>
        public event EventHandler Load;

        #endregion // --- Events ---

        #region --- Methods ---

        #region --- public void SwapBuffers() ---

        /// <summary>
        /// Swaps the front and back buffers, presenting the rendered scene to the screen. This method
        /// will have no effect on a single-buffered <see cref="GraphicsMode"/>.
        /// </summary>
        public void SwapBuffers()
        {
            Context.SwapBuffers();
        }

        #endregion // --- public void SwapBuffers() ---

        #region --- public void MakeCurrent() ---

        /// <summary>
        /// <para> Makes <see cref="Context"/> current in the calling thread. All OpenGL commands issued
        /// are hereafter interpreted by this <see cref="Context"/>.</para>
        /// <para>When using multiple <see cref="GLControl"/>s, calling <see cref="MakeCurrent"/> on one
        /// control will make all other controls non-current in the calling thread.</para>
        /// <para>A <see cref="GLControl"/> can only be current in one thread at a time. To make a control
        /// non-current, call <c>glControl1.Context.MakeCurrent(null)</c>.</para>
        /// </summary>
        /// <seealso cref="Context"/>
        /// <seealso cref="IGraphicsContext"/>
        /// <seealso cref="IGraphicsContext.MakeCurrent"/>
        public void MakeCurrent()
        {
            Context.MakeCurrent(Implementation.WindowInfo);
        }

        #endregion // --- public void MakeCurrent() ---

        #endregion // --- Methods ---

        #region --- ISupportInitialize ---

        /// <summary>
        /// Whether or not this control is undergoing batch initialization from the <see cref="ISupportInitialize"/> interface.
        /// If <c>true</c>, no events will be generated from this control. If <c>false</c>, normal event dispatching is in place.
        /// </summary>
        /// <seealso cref="ISupportInitialize"/>
        /// <seealso cref="BeginInit"/>
        /// <seealso cref="EndInit"/>
        [Browsable(false)]
        public bool Initializing { get; protected set; } = false;

        /// <summary>
        /// Begins the batch initialization routine.
        /// </summary>
        /// <seealso cref="ISupportInitialize"/>
        /// <seealso cref="EndInit"/>
        /// <seealso cref="Initializing"/>
        public void BeginInit()
        {
            Initializing = true;
        }

        /// <summary>
        /// Ends the batch initialization routine.
        /// </summary>
        /// <seealso cref="ISupportInitialize"/>
        /// <seealso cref="BeginInit"/>
        /// <seealso cref="Initializing"/>
        public void EndInit()
        {
            Initializing = false;
        }

        #endregion // --- ISupportInitialize ---

        #endregion // --- Public interface ---


        #region --- Protected implementation ---

        #region --- Delegates ---

        /// <summary>
        /// Delays the invocation on Mac OS X, and inline actions aren't supported in .NET 2.
        /// </summary>
        public delegate void DelayUpdate();

        #endregion // --- Delegates ---

        #region --- Properties ---

        #region --- protected override CreateParams CreateParams { get; }

        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.CreateParams"/> for this <see cref="GLControl"/> instance.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_VREDRAW = 0x1;
                const int CS_HREDRAW = 0x2;
                const int CS_OWNDC = 0x20;

                var cp = base.CreateParams;
                if (Configuration.RunningOnWindows)
                    cp.ClassStyle |= CS_VREDRAW | CS_HREDRAW | CS_OWNDC;
                return cp;
            }
        }

        #endregion // --- protected override CreateParams CreateParams { get; }

        #region --- protected virtual GraphicsMode DesiredGraphicsMode { get; set; } ---

        /// <summary>
        /// This control's desired <see cref="OpenTK.Graphics.GraphicsMode"/>.
        /// </summary>
        protected virtual GraphicsMode DesiredGraphicsMode
        {
            get
            {
                return _DesiredGraphicsMode;
            }
            set
            {
                if (!_DesiredGraphicsMode.Equals(value))
                {
                    _DesiredGraphicsMode = value;
                    if (!Initializing && !design_mode && IsHandleCreated)
                    {
                        DestroyHandle();
                        RecreateHandle();
                    }
                }
            }
        }
        private GraphicsMode _DesiredGraphicsMode = GraphicsMode.Default;

        #endregion // --- protected virtual GraphicsMode DesiredGraphicsMode { get; set; } ---

        #endregion // --- Properties ---

        #region --- Methods ---

        #region --- protected override void OnHandleCreated(EventArgs e) ---

        /// <summary>
        /// Raises the <see cref="Control.HandleCreated"/> event.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            if (!design_mode)
            {
                if (_Context != null)
                    _Context.Dispose();

                if (_Implementation != null)
                    _Implementation.WindowInfo.Dispose();

                _Implementation = new GLControlFactory().CreateGLControl(_DesiredGraphicsMode, this);
                _Context = _Implementation.CreateContext(_GLMajorVersion, _GLMinorVersion, _ContextFlags);
                MakeCurrent();
                ((IGraphicsContextInternal)_Context).LoadAll();
                _Context.SwapInterval = _VSync ? 1 : 0;
            }

            base.OnHandleCreated(e);

            if (!load_event_fired)
                OnLoad(EventArgs.Empty);

            if (resize_event_suppresed)
            {
                OnResize(EventArgs.Empty);
                resize_event_suppresed = false;
            }
        }

        #endregion // --- protected override void OnHandleCreated(EventArgs e) ---

        #region --- protected override void OnHandleDestroyed(EventArgs e) ---

        /// <summary>
        /// Raises the <see cref="Control.HandleDestroyed"/> event.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            // Ensure that context is still alive when passing to events
            // => This allows to perform cleanup operations in OnHandleDestroyed handlers
            base.OnHandleDestroyed(e);

            if (!design_mode)
            {
                if (_Context != null)
                {
                    _Context.Dispose();
                    _Context = null;
                }

                if (_Implementation != null)
                {
                    _Implementation.WindowInfo.Dispose();
                    _Implementation = null;
                }
            }
        }

        #endregion // --- protected override void OnHandleDestroyed(EventArgs e) ---

        #region --- protected virtual void OnLoad(EventArgs e) ---

        /// <summary>
        /// Raises the <see cref="Load"/> event.
        /// </summary>
        /// <param name="e">Not used.</param>
        /// <remarks>This class does not inherit from <see cref="UserControl"/>, so it lacks the official
        /// <see cref="UserControl.Load"/> (or <see cref="Form.Load"/>) event. Instead, it attempts to
        /// mimic that behavior by raising the event once, shortly after a handle is created.</remarks>
        protected virtual void OnLoad(EventArgs e)
        {
            load_event_fired = true;
            Load?.Invoke(this, e);
        }

        #endregion // --- protected virtual void OnLoad(EventArgs e) ---

        #region --- protected override void OnPaint(PaintEventArgs e) ---

        /// <summary>
        /// Raises the <see cref="Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Initializing && !design_mode)
                ValidateState();
            base.OnPaint(e);
        }

        #endregion // --- protected override void OnPaint(PaintEventArgs e) ---

        #region --- protected override void OnParentChanged(EventArgs e) ---

        /// <summary>
        /// Raises the <see cref="Control.ParentChanged"/> event.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnParentChanged(EventArgs e)
        {
            if (_Context != null && !Initializing && !design_mode)
                _Context.Update(Implementation.WindowInfo);

            base.OnParentChanged(e);
        }

        #endregion // --- protected override void OnParentChanged(EventArgs e) ---

        #region --- protected override void OnResize(EventArgs e) ---

        /// <summary>
        /// Raises the <see cref="Control.Resize"/> event.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            if (!design_mode)
            {
                if (!IsHandleCreated || Initializing)
                {
                    resize_event_suppresed = true;
                    return;
                }

                if (Configuration.RunningOnMacOS)
                {
                    DelayUpdate delay = PerformContextUpdate;
                    BeginInvoke(delay); //Need the native window to resize first otherwise our control will be in the wrong place.
                }
                else if (_Context != null)
                    _Context.Update(Implementation.WindowInfo);
            }
            base.OnResize(e);
        }

        #endregion // --- protected override void OnResize(EventArgs e) ---

        #region --- protected void PerformContextUpdate() ---

        /// <summary>
        /// Performs the delayed context update.
        /// </summary>
        protected void PerformContextUpdate()
        {
            if (!Initializing && _Context != null)
                _Context.Update(Implementation.WindowInfo);
        }

        #endregion // --- protected void PerformContextUpdate() ---

        #endregion // --- Methods ---

        #endregion // --- Protected implementation ---


        #region --- Private implementation ---

        private IGLControl Implementation
        {
            get
            {
                ValidateState();
                return _Implementation;
            }
        }
        private IGLControl _Implementation;

        private readonly bool design_mode;
        private bool load_event_fired = false;
        private bool resize_event_suppresed;
        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"GLControl {Name}: OpenGL v{GLMajorVersion}.{GLMinorVersion}");
            }
        }

        [Conditional("DEBUG")]
        private void ValidateContext(string message)
        {
#if DEBUG
            if (!_Context.IsCurrent)
                Debug.Print($"[GLControl] Attempting to access {message} and context isn't there. Revalidating.");
            if (!Context.IsCurrent)
                Debug.Print($"[GLControl] Attempted to access {message} on a non-current context. Results undefined.");
#endif
        }

        private void ValidateState()
        {
            if (!Initializing)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                if (!IsHandleCreated)
                    CreateControl();

                if (_Implementation == null || _Context == null || _Context.IsDisposed)
                    RecreateHandle();
            }
        }

        #endregion // --- Private implementation ---
    }
}
