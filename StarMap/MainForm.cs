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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using StarMap.Database;
using StarMap.Scenes;
using StarMap.Shaders;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StarMap
{
    public partial class MainForm : Form
    {
        public string StatusText
        {
            get
            {
                return toolStripStatusLabel1.Text;
            }
            set
            {
                if (!InvokeRequired)
                    toolStripStatusLabel1.Text = value;
                else
                {
                    Invoke(new Action(() =>
                    {
                        StatusText = value;
                    }));
                }
            }
        }

        GraphicsMode gm { get { return glControl1.GraphicsMode; } }

        public MainForm()
        {
            Debug.WriteLine($"[INFO] constructing MainForm.");
            InitializeComponent();
        }

        #region --- private implementation ---

        #region fields

        // Startup state awareness
        private bool _databasesLoaded = false;
        private bool _glLoaded = false;
        private bool _shadersLoaded = false;

        // FPS counting...
        private double _accumulator;
        private uint _idleCounter = 0;
        private double _lastUpdate;
        private double _thisUpdate;
        private double _updateLen;
        private readonly Stopwatch _watch = new Stopwatch();

        // others.
        private ConfigBindingList _cbl;
        private Config _config;
        private IScene _scene;
        private ShaderCollection _shaders;
        private SceneTransitions _sceneTrans;

        #endregion // fields


        #region methods

        private void InitializeDatabases()
        {
            StatusText = "Initializing databases...";
            string appFolderPath = Environment.ExpandEnvironmentVariables(Path.Combine("%LOCALAPPDATA%", "StarMap"));
            if (!Directory.Exists(appFolderPath))
                Directory.CreateDirectory(appFolderPath);

            EDDUserDBConnection.EarlyReadRegister();
            SMDBConnection.EarlyReadRegister();

            // Initialize the databases, possibly updating the schemas.
            EDDSystemsDBConnection.Initialize();
            EDDUserDBConnection.Initialize();
            SMDBConnection.Initialize();
        }

        private void LoadConfig()
        {
            StatusText = "Loading configuration...";
            _config = Config.Instance;
            _config.Load(EDDUserDBConnection.EarlyRegister, SMDBConnection.EarlyRegister);
        }

        private void TryLoadMainScene()
        {
            if (_databasesLoaded && _glLoaded && _shadersLoaded)
            {
                // TODO: Figure out the users current position from last FTL jump journal. For now, just use the chosen home system, or Sol.
                SystemBase home = Systems.GetSystem(_config.HomeSystem);
                if (float.IsNaN(home.Position.LengthFast))
                    home = Systems.Sol;

                _sceneTrans.Immediate(glControl1, ref _scene, new LoadingSceneBlue());//TODO: Change to MainScene()
                //_scene.Camera.BeginLerp(new Vector3(home.Position.X, home.Position.Y, home.Position.Z), 0.1f, Vector3.UnitY);
                //_scene.Camera.BeginLerp(new Vector3(0, -1, 4), 0.01f, new Quaternion(0, 1, 0, .25f));
            }
        }

        #endregion // methods


        #region Upstream event handlers

        /// <summary>
        /// Updates the framerate counter and invalidates the GLControl so that it actually paints
        /// at the next update..
        /// </summary>
        private void Application_Idle(object sender, EventArgs e)
        {
            if (_glLoaded)
            {
                _idleCounter++;
                _accumulator += _updateLen;
                if (_accumulator > 1)
                {
                    if (_glLoaded && _shadersLoaded && _databasesLoaded)
                        StatusText = _idleCounter.ToString() + " FPS";
                    _accumulator -= 1;
                    _idleCounter = 0;
                }

                glControl1.Invalidate();
            }
        }

        /// <summary>
        /// The <see cref="Form.FormClosed"/> event handler.
        /// </summary>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.WriteLine($"[INFO] MainForm_FormClosed");
            _cbl?.Clear();
        }

        /// <summary>
        /// The <see cref="Form.FormClosing"/> event handler.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine($"[INFO] MainForm_FormClosing.");

            if (_shaders.IsBusy)
                _shaders.LoadAsyncCancel();

            // TODO: Some semblance of cancellation...
            if (_sceneTrans != null)
                _sceneTrans.LoadAsyncCompleted -= SceneTransitions_LoadAsyncCompleted;

            Application.Idle -= Application_Idle;

            if (_watch != null && _watch.IsRunning)
                _watch.Stop();

            if (bgSysLoadWorker.IsBusy)
                bgSysLoadWorker.CancelAsync();
        }

        /// <summary>
        /// The <see cref="Form.Load"/> event handler.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            Debug.WriteLine($"[INFO] MainForm_Load.");
            if (_sceneTrans == null)
                _sceneTrans = new SceneTransitions();

            InitializeDatabases();
            LoadConfig();
#if !DONT_LOAD_SYSTEMS
            bgSysLoadWorker.RunWorkerAsync();
#endif
            _sceneTrans.LoadAsyncCompleted += SceneTransitions_LoadAsyncCompleted;
        }

        #endregion // Upstream event handlers


        #region Downstream event handlers

        #region bgSysLoadWorker

        // The background worker is used to load systems from the database.
        // Right now, it loads _every_ system minimal info to RAM at start.
        // TODO: keep the bgw on-hand to fetch details as-needed.
        private void bgSysLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Systems.LoadBGW(sender as BackgroundWorker);
        }

        private void bgSysLoadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (toolStripProgressBar1 != null && !toolStripProgressBar1.IsDisposed && e != null)
            {
                toolStripProgressBar1.Value = e.ProgressPercentage;
                var x = e.UserState as SystemsLoadProgressEventArgs;

                if (x != null)
                {
                    if (!x.AllComplete)
                    {
                        StatusText = $"Loading systems; please wait... {x.ReadSystems.ToString("N0")} of {x.TotalSystems.ToString("N0")}: {x.MostRecentSystem.Name}";
                        (_scene as LoadingScene).AddSystem(x.MostRecentSystem);
                    }   
                    else
                        StatusText = $"Loaded {x.TotalSystems.ToString("N0")} systems. Initiating pre-flight checks ...";
                }
            }
        }

        // TODO: keep the bgw on-hand to fetch details as-needed.
        private void bgSysLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // We may be disposed already, or disposing, or loading was cancelled, or it's just a Tuesday.
            if (IsDisposed || e == null || e.Cancelled || Systems.SystemsList == null)
                return;

            _databasesLoaded = true;
            Debug.WriteLine(string.Format("[DEBUG] Finished loading {00,0} systems. Now transferring to GPU.", Systems.SystemsList.Count));

            // TODO: there's a bit of work needed here.
            // Start loading nearby stars, get ready to display them.
            var scen = _scene as LoadingScene;
            scen.AddSystems(Systems.SystemsList);

            // kill the loading demo scene and switch to the main scene
            //TryLoadMainScene();
        }

        #endregion // bgSysLoadWorker

        #region glControl1

        private void glControl1_Load(object sender, EventArgs e)
        {
            if (!_glLoaded)
            {
                Debug.WriteLine($"[INFO] glControl1_Load.");
                _glLoaded = true;

                _cbl = new ConfigBindingList();
                _cbl.BindToControl(glControl1, nameof(glControl1.VSync), nameof(_config.VSync));

                _scene = new LoadingScene(glControl1.Width, glControl1.Height);

                Program.Shaders = _shaders = new ShaderCollection();
                _shaders.LoadCompleted += shaderCollection_LoadCompleted;
                _shaders.LoadAsync(glControl1);

                // Most shaders are not yet loaded. this scene cannot do much.
                _scene.Load();

                _watch.Start();
                Application.Idle += Application_Idle;
            }
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            _scene.KeyDown(e);
        }

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
            _scene.KeyUp(e);
        }

        private void glControl1_MouseClick(object sender, MouseEventArgs e)
        {
            // TODO: glControl1_MouseClick
        }

        private void glControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // TODO: glControl1_MouseDoubleClick
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            // TODO: glControl1_MouseDown
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            // TODO: glControl1_MouseMove
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            // TODO: glControl1_MouseUp
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (_glLoaded && _scene != null)
            {
                _lastUpdate = _thisUpdate;
                _thisUpdate = _watch.Elapsed.TotalSeconds;
                _updateLen = _thisUpdate - _lastUpdate;
                _scene.Update(_updateLen);
                _scene.Render();
                glControl1.SwapBuffers();
            }
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!_glLoaded) return;
            //Debug.WriteLine($"[INFO] glControl1_Resize.");
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            _scene.ResetProjectionMatrix(glControl1.Width, glControl1.Height);
            // Idle invalidation doesn't work during resize.
            glControl1.Invalidate();
        }

        #endregion // glControl1

        #region menuStrip1

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[INFO] aboutToolStripMenuItem_Click.");
            using (var abt = new AboutBoxForm())
                abt.ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[INFO] exitToolStripMenuItem_Click.");
            Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[INFO] settingsToolStripMenuItem_Click.");
            using (var settingsForm = new SettingsForm())
                settingsForm.ShowDialog(this);
        }

        private void sysListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[INFO] sysListToolStripMenuItem_Click.");
            SystemListForm frm = new SystemListForm();
            frm.Show();
        }

        #endregion // menuStrip1

        private void SceneTransitions_LoadAsyncCompleted(object sender, IScene e)
        {
            _sceneTrans.Immediate(glControl1, ref _scene, e);
        }

        private void shaderCollection_LoadCompleted(object sender, EventArgs e)
        {
            Debug.Assert(!InvokeRequired);
            _shadersLoaded = true;
            TryLoadMainScene();
        }

        #endregion // Downstream event handlers

        #endregion // --- private implementation ---
    }
}
