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
using OpenTK.Graphics.OpenGL4;
using Phroggiesoft.Controls;
using StarMap.Cameras;
using StarMap.Models;
using StarMap.SceneObjects;
using StarMap.Shaders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

#endregion // --- using ... --- 

namespace StarMap.Scenes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial class Scene : Component, IIsDisposed, IScene
    {
        #region --- protected ctor's ---

        protected Scene()
        {
            InitializeComponent();
        }

        protected Scene(IContainer container)
        {
            InitializeComponent();
            container.Add(this);
        }

        #endregion // --- protected ctor's ---


        #region --- public interfaces ---

        #region --- IIsDisposed interface ---

        public bool IsDisposed { get { return m_IsDisposed; } }

        // `public void Dispose(){}` is in Scene.Designer.cs, since this is a Component.

        #endregion // --- IIsDisposed interface ---


        #region --- IScene interface ---

        #region --- events ---

        public event EventHandler<StringEventArgs> FPSUpdate;

        #endregion // --- events ---

        #region --- properties ---

        /// <summary>
        /// The background <see cref="Color"/> of this scene.
        /// </summary>
        public Color BackColor { get; set; } = Color.Black;
        /// <summary>
        /// The <see cref="ICamera"/> that this scene uses.
        /// </summary>
        public ICamera Camera { get; protected set; }
        /// <summary>
        /// All of the <see cref="AObject"/>s that will be rendered in this scene.
        /// </summary>
        public virtual IList<ISceneObject> Contents { get; } = new List<ISceneObject>();
        private float m_FOV = 45;
        public float FOV
        {
            get
            {
                return m_FOV;
            }
            set
            {
                float newval = ObjectExtensions.Clamp(value, 0.000001f, 179.9999f);
                if (m_FOV != newval && !float.IsNaN(newval))
                {
                    m_FOV = newval;
                    _IsProjMatDirty = true;
                }
            }
        }
        /// <summary>
        /// The frame rate (frames per second) that this scene is rendering at.
        /// </summary>
        public string FrameRate { get; private set; } = "144 FPS";
        /// <summary>
        /// Whether this scene is fully loaded, and merely awaiting a transition.
        /// </summary>
        public bool IsLoaded { get; private set; } = false;
        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string Name { get; protected set; } = nameof(Scene);
        public virtual phrogGLControl Parent { get; private set; }
        /// <summary>
        /// The toggle keys that this scene is concerned with.
        /// </summary>
        public IList<Keys> ToggleKeys { get; private set; } = new List<Keys>();
        /// <summary>
        /// How fast this scene's camera can translate.
        /// </summary>
        public virtual float TranslationSpeed { get; set; } = translationSpeedLow;

        public virtual float RotationSpeed { get; set; } = rotateSpeedLow;
        #endregion // --- properties ---

        #region --- methods ---

        public void Load(phrogGLControl parent)
        {
            if (!m_IsDisposed && !IsLoaded)
            {
                Parent = parent;

                TraceLog.Debug($"Loading {Name}.");

                OnLoad();

                // if multiple items use the same shader, they should be rendered congruently.
                var x = (from c in Contents
                            orderby c.Model.Shader.ProgramID
                            select c).ToList();
                Contents.Clear();
                foreach (var c in x)
                    Contents.Add(c);

                // Generate a Uniform Buffer Object sized appropriately, but null (intptr.zero).
                gl_UBO_ProjViewViewPort_ID = gld.GenBuffer();
                gld.BindBuffer(BufferTarget.UniformBuffer, gl_UBO_ProjViewViewPort_ID);
                gld.BufferData(BufferTarget.UniformBuffer, ProjectionView.SizeInBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                gld.BindBuffer(BufferTarget.UniformBuffer, 0);

                int bindPost = BindingPosts.GenBindingPost();
                // bind the UBO to an arbitrary but unique binding post
                gld.BindBufferBase(BufferRangeTarget.UniformBuffer, bindPost, gl_UBO_ProjViewViewPort_ID);

                // Bind all usable shaders to the binding post
                foreach (var s in Contents.Select(c => c.Model.Shader).Distinct())
                {
                    if (s.ProjectionView >= 0)
                        gld.UniformBlockBinding(s.ProgramID, s.ProjectionView, bindPost);
                }

                TraceLog.Debug($"Done loading {Name}.");
                IsLoaded = true;
            }
            else if (m_IsDisposed)
                throw new ObjectDisposedException(Name);
        }

        // Stage 0
        public void Start()
        {
            if (Parent != null && !Parent.IsDisposed && !m_IsDisposed)
            {
                if (!m_Enabled)
                {
                    TraceLog.Info($"{Name} Starting up...");
                    m_Enabled = true;
                    ModifyEvents(m_Enabled);
                    m_keyData.Clear();
                    if (!m_Watch.IsRunning)
                        m_Watch.Start();
                }
                else
                    TraceLog.Notice($"{Name}.{nameof(Start)}() needlessly called.");
            }
            else if (m_IsDisposed)
                throw new ObjectDisposedException(Name);
            else if (Parent == null)
                throw new InvalidOperationException($"{nameof(Parent)} is null; unable to {Name}.Start().");
            else if (Parent.IsDisposed)
                throw new ObjectDisposedException($"{nameof(Parent)} is null; unable to {Name}.Start().");
        }

        // render hotloop stage 1
        public void Update(double delta)
        {
            if (m_Enabled && !m_IsDisposed && Parent != null && !Parent.IsDisposed)
            {
                OnUpdate(delta);
            }
            else if (m_IsDisposed)
                throw new ObjectDisposedException(Name);
            else if (Parent == null)
                throw new InvalidOperationException($"{nameof(Parent)} is null; unable to Update().");
            else if (Parent.IsDisposed)
                throw new ObjectDisposedException(nameof(Parent));
            else if (!m_Enabled)
                TraceLog.Warn($"{Name}.Update() called when not enabled. Did you forget to call {Name}.Start()?");
        }

        // render hotloop stage 2
        public void Render()
        {
            if (m_Enabled && !m_IsDisposed)
            {
                OnRenderClear();
                OnRender();
            }
            else if (m_IsDisposed)
                throw new ObjectDisposedException(Name);
            else if (!m_Enabled)
                TraceLog.Warn($"{Name}.Render() called when not enabled. Did you forget to call {Name}.Start()?");
        }

        // stage -1
        public void Stop()
        {
            if (!m_IsDisposed && m_Enabled)
            {
                TraceLog.Info($"{Name} Shutting down...");

                m_Enabled = false;
                ModifyEvents(m_Enabled);

                if (m_Watch != null && m_Watch.IsRunning)
                    m_Watch.Stop();

                m_keyData.Clear();
            }
            else if (!m_Enabled)
                TraceLog.Notice($"{Name}.{nameof(Stop)}() needlessly called.");
            else
                throw new ObjectDisposedException(Name);
        }

        #endregion // --- methods ---

        #endregion // --- IScene interface ---

        #endregion // --- public interfaces ---


        #region --- implementation ---

        #region --- protected implementation ---

        #region --- fields and properties ---

        protected List<KeyEventArgs> m_keyData = new List<KeyEventArgs>();

        protected virtual string DebuggerDisplay
        {
            get
            {
                return string.Format("{0}: {1} objects, {2}, fov {3}°", Name, Contents.Count, Camera.Position.ToString(), FOV);
            }
        }

        #endregion // --- fields and properties ---

        #region --- Methods ---

        protected virtual void OnFirstRender() { }

        protected virtual void OnFPSUpdate(string fpsText)
        {
            FPSUpdate?.Invoke(this, new StringEventArgs(fpsText));
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (ToggleKeys.Contains(e.KeyCode))
                OnKeyPress(e);

            if (e.Shift)
            {
                RotationSpeed = rotateSpeedHigh;
                TranslationSpeed = translationSpeedHigh;
            }
            else
            {
                RotationSpeed = rotateSpeedLow;
                TranslationSpeed = translationSpeedLow;
            }

            if (!m_keyData.Exists(k => k.KeyCode == e.KeyCode))
                m_keyData.Add(e);
        }

        protected virtual void OnKeyPress(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                m_keyData.Clear();
                RotationSpeed = rotateSpeedLow;
                TranslationSpeed = translationSpeedLow;
                Camera.BeginLerp(0.25f, new Vector3(0, 0, 4000), Quaternion.Identity, 45);
            }
        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            m_keyData.RemoveAll(k => k.KeyCode == e.KeyCode);

            if (m_keyData.FindIndex(k => k.Shift) == -1)
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

        protected virtual void OnLoad()
        {
            ToggleKeys.Add(Keys.Space);
        }

        protected virtual void OnRender()
        {
            int lastProg = -1;

            foreach (var c in Contents)
            {
                Shader s = c.Model.Shader;
                if (s.ProgramID != lastProg)
                {
                    lastProg = s.ProgramID;
                    gld.UseProgram(lastProg);
                }
                c.Render();
            }
            gld.BindVertexArray(0);
        }

        protected virtual void OnRenderClear()
        {
            gld.ClearColor(BackColor);
            gld.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        protected virtual void OnUpdate(double delta)
        {
            if (m_keyData.Count > 0 && Camera.IsUserControlled)
            {
                float offset = (float)delta * TranslationSpeed;
                float rotoff = (float)delta * RotationSpeed;

                foreach (var key in m_keyData)
                {
                    switch (key.KeyCode)
                    {
                        case Keys.D:
                            Camera.Translate(new Vector3(offset, 0, 0));
                            break;
                        case Keys.A:
                            Camera.Translate(new Vector3(-offset, 0, 0));
                            break;

                        case Keys.E:
                            Camera.Translate(new Vector3(0, offset, 0));
                            break;
                        case Keys.Q:
                            Camera.Translate(new Vector3(0, -offset, 0));
                            break;

                        case Keys.S:
                            Camera.Translate(new Vector3(0, 0, offset));
                            break;
                        case Keys.W:
                            Camera.Translate(new Vector3(0, 0, -offset));
                            break;

                            /* TODO: fix these back up...
                        // test out some roll control...
                        case Keys.NumPad8:
                            Camera.MouseLook(new Quaternion(0, 0, -rotoff));
                            break;
                        case Keys.NumPad2:
                            Camera.MouseLook(new Quaternion(0, 0, rotoff));
                            break;

                        case Keys.NumPad4:
                            Camera.MouseLook(new Quaternion(0, -rotoff, 0));
                            break;
                        case Keys.NumPad6:
                            Camera.MouseLook(new Quaternion(0, rotoff, 0));
                            break;

                        case Keys.NumPad7:
                            Camera.MouseLook(new Quaternion(-rotoff, 0, 0));
                            break;
                        case Keys.NumPad9:
                            Camera.MouseLook(new Quaternion(rotoff, 0, 0));
                            break;*/

                        default:
                            break;
                    }
                }
            }

            IsCamMatDirty = Camera.Update(delta);

            foreach (var obj in Contents)
            {
                obj.Update(delta);
            }
            if (IsCamMatDirty || _IsProjMatDirty || _IsViewPortSzDirty)
                OnUpdateProjectionView();
        }

        protected virtual void OnUpdateProjectionView()
        {
            if (IsCamMatDirty)
                m_ProjViewViewPort.ViewMatrix = Camera.ViewMatrix;

            if (_IsProjMatDirty)
                m_ProjViewViewPort.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                    m_FOV * ((float)Math.PI / 180f),// FOV in rads
                    Parent.AspectRatio,             // Aspect ratio
                    .000001f,                       // Near clip plane
                    100000);                        // Far clip plane

            if (_IsViewPortSzDirty)
            {
                m_ProjViewViewPort.ViewportSize.X = Parent.Width;
                m_ProjViewViewPort.ViewportSize.Y = Parent.Height;
            }

            IsCamMatDirty = _IsViewPortSzDirty = _IsProjMatDirty = false;
            gld.BindBuffer(BufferTarget.UniformBuffer, gl_UBO_ProjViewViewPort_ID);
            gld.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, ProjectionView.SizeInBytes, ref m_ProjViewViewPort);
            gld.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        #endregion // --- Methods ---

        #endregion // --- protected implementation ---


        #region --- private implementation ---

        #region --- fields ---

        private bool m_Enabled = false;
        private bool m_IsDisposed = false;
        private Stopwatch m_Watch = new Stopwatch();

        private ProjectionView m_ProjViewViewPort = ProjectionView.Identity;
        private int gl_UBO_ProjViewViewPort_ID = -1;
        protected bool IsCamMatDirty = true;
        private bool _IsProjMatDirty = true;
        private bool _IsViewPortSzDirty = true;

        private const float rotateSpeedHigh = 1;
        private const float rotateSpeedLow = 0.05f;
        private const float translationSpeedHigh = 5000;
        private const float translationSpeedLow = 500f;

        // FPS counting...
        private double _accumulator;
        private uint _idleCounter = 0;
        private double _lastUpdate;
        private double _thisUpdate;
        private double _updateDelta;

        // Mousing around...
        private MouseButtons _MouseBtns = MouseButtons.None;
        private Point _MouseLastPos;

        #endregion // --- fields ---

        #region --- methods ---

        #region --- private void ModifyEvents(bool attach = false) ---

        /// <summary>
        /// Detach then (optionally) attach concerned event handlers to <see cref="Application"/>/<see cref="Config"/>/<see cref="Parent"/>/etc.
        /// </summary>
        /// <param name="attach">If <c>true</c>, event handlers will be detached then (re-)attached; if <c>false</c>, all event handlers will merely be detached.</param>
        private void ModifyEvents(bool attach = false)
        {
            phrogGLControl parent = Parent;
            Application.Idle -= Application_Idle;

            if (parent != null)
            {
                parent.KeyDown -= parent_KeyDown;
                parent.KeyUp -= parent_KeyUp;
                parent.MouseDown -= parent_MouseDown;
                parent.MouseMove -= parent_MouseMove;
                parent.MouseUp -= parent_MouseUp;
                parent.MouseWheel -= parent_MouseWheel;
                parent.Paint -= parent_Paint;
                parent.Resize -= parent_Resize;
            }

            if (attach)
            {
                Application.Idle += Application_Idle;
                if (parent != null)
                {
                    parent.KeyDown += parent_KeyDown;
                    parent.KeyUp += parent_KeyUp;
                    parent.MouseDown += parent_MouseDown;
                    parent.MouseMove += parent_MouseMove;
                    parent.MouseUp += parent_MouseUp;
                    parent.MouseWheel += parent_MouseWheel;
                    parent.Paint += parent_Paint;
                    parent.Resize += parent_Resize;
                }
            }
        }

        #endregion // --- private void ModifyEvents(bool attach = false) ---

        #endregion // --- methods ---

        #region --- event handlers ---

        // The following event handlers should only be referenced twice: both from ModifyEvents().
        // Any more or less and something is likely wrong.

        #region --- Application_Idle: invalidate Parent to force painting, count FPS ---

        /// <summary>
        /// Updates the framerate counter and invalidates <see cref="Parent"/> so that it actually paints
        /// at the next update..
        /// </summary>
        private void Application_Idle(object sender, EventArgs e)
        {
            _idleCounter++;
            _accumulator += _updateDelta;
            if (_accumulator > 1)
            {
                FrameRate = $"{_idleCounter} FPS";
                OnFPSUpdate(FrameRate);
                _accumulator -= 1;
                _idleCounter = 0;
            }

            Parent.Invalidate();
        }

        #endregion // --- Application_Idle: invalidates Parent to force painting, count FPS ---

        #region --- keyboard ---

        private void parent_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        private void parent_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        #endregion // --- keyboard ---

        #region --- mouse ---

        private void parent_MouseClick(object sender, MouseEventArgs e)
        {
            // TODO: Parent_MouseClick
            throw new NotImplementedException();
        }

        private void parent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // TODO: Parent_MouseDoubleClick
            throw new NotImplementedException();
        }

        private void parent_MouseDown(object sender, MouseEventArgs e)
        {
            if (Parent.ContainsFocus || Parent.ClientRectangle.Contains(e.Location))
            {
                _MouseBtns |= e.Button;
            }
        }

        private void parent_MouseMove(object sender, MouseEventArgs e)
        {
            Vector2 mousediff = new Vector2(e.X - _MouseLastPos.X, e.Y - _MouseLastPos.Y);

            if (_MouseBtns.HasFlag(MouseButtons.Right))
            {
                // Move by world-space Z-axis ± (up/down)
                Camera.Move(new Vector3(0, 0, mousediff.Y));
            }   
            if (_MouseBtns.HasFlag(MouseButtons.Left))
            {
                // Mouse look: transform camera pitch/yaw to reflect the mouse.
                Camera.MouseLook(new Vector2(mousediff.X, mousediff.Y));
            }   
            _MouseLastPos = e.Location;
        }

        private void parent_MouseUp(object sender, MouseEventArgs e)
        {
            if (_MouseBtns != MouseButtons.None)
                _MouseBtns &= ~e.Button;
        }

        private void parent_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                float fovChange = (e.Delta > 0 ? - 0.25f : 0.25f);
                if (m_keyData.Exists(k => k.Shift))
                    fovChange *= 4;
                FOV = m_FOV + fovChange;
            }
        }

        #endregion // --- mouse ---

        #region --- paint & resize ---

        private void parent_Paint(object sender, PaintEventArgs e)
        {
            _lastUpdate = _thisUpdate;
            _thisUpdate = m_Watch.Elapsed.TotalSeconds;
            _updateDelta = _thisUpdate - _lastUpdate;
            Update(_updateDelta);
            Render();
            Parent.SwapBuffers();
        }

        private void parent_Resize(object sender, EventArgs e)
        {
            gld.Viewport(Parent.ClientRectangle);
            _IsProjMatDirty = true;
            // Application.Idle isn't called when resizing a window, so do this here.
            Parent.Invalidate();
        }

        #endregion // --- paint & resize ---

        #endregion // --- event handlers ---

        #endregion // --- private implementation ---

        #endregion // --- implementation ---
    }
}
