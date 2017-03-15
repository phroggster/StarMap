﻿/*
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
        private AScene _scene;
        private ShaderCollection _shaders;
        private SceneTransitions _sceneTrans;

        public MainForm()
        {
            Debug.WriteLine($"[INFO] constructing MainForm.");
            InitializeComponent();
        }

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

        private void LoadAndBindConfig()
        {
            StatusText = "Loading configuration...";
            Config.Instance.Load();
            Config.Instance.GridLineColourChanged -= Config_GridLineColourChanged;
            Config.Instance.GridLineColourChanged += Config_GridLineColourChanged;
            Config.Instance.FineGridLineColourChanged -= Config_FineGridLineColourChanged;
            Config.Instance.FineGridLineColourChanged += Config_FineGridLineColourChanged;
            _cbl = new ConfigBindingList();
            _cbl.Bind(glControl1, "VSync", "VSync");
        }

        private void TryLoadMainScene()
        {
            if (_databasesLoaded && _glLoaded && _shadersLoaded)
            {
                // TODO: Figure out the users current position from last FTL jump journal. For now, just use the chosen home system.
                Vector3 pos = Systems.SystemsList.Find(s => s.Name == Config.Instance.HomeSystem).Position;
                SceneTransitions.Immediate(glControl1, ref _scene, new MainScene());
                _scene.Camera.BeginLerp(pos, 0.1f, Vector3.UnitY);
            }
        }

        #region Upstream events

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
                    if (_databasesLoaded && _glLoaded && _shadersLoaded)
                        StatusText = _idleCounter.ToString() + " FPS";
                    _accumulator -= 1;
                    _idleCounter = 0;
                }

                glControl1.Invalidate();
            }
        }

        /// <summary>
        /// The <see cref="Config.FineGridLineColourChanged"/> event handler.
        /// </summary>
        /// <param name="e">The new <see cref="Color"/> set to <see cref="Config.FineGridLineColour"/>.</param>
        private void Config_FineGridLineColourChanged(object sender, Color e)
        {
            if (_scene is MainScene)
            {
                (_scene as MainScene).UpdateFineGridColour(e);
                glControl1.Invalidate();
            }
        }

        /// <summary>
        /// The <see cref="Config.GridLineColourChanged"/> event handler.
        /// </summary>
        /// <param name="e">The new <see cref="Color"/> set to <see cref="Config.GridLineColour"/>.</param>
        private void Config_GridLineColourChanged(object sender, Color e)
        {
            if (_scene is MainScene)
            {
                (_scene as MainScene).UpdateCoarseGridColour(e);
                glControl1.Invalidate();
            }
        }

        /// <summary>
        /// The <see cref="Form.FormClosed"/> event handler.
        /// </summary>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.WriteLine($"[INFO] MainForm_FormClosed");
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
            Config.Instance.GridLineColourChanged -= Config_GridLineColourChanged;
            Config.Instance.FineGridLineColourChanged -= Config_FineGridLineColourChanged;

            _cbl?.Clear();

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
            InitializeDatabases();
            LoadAndBindConfig();
            //bgSysLoadWorker.RunWorkerAsync();
            _sceneTrans = new SceneTransitions();
            _sceneTrans.LoadAsyncCompleted += SceneTransitions_LoadAsyncCompleted;
        }

        #endregion // Upstream events


        #region Downstream events

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

        private void SceneTransitions_LoadAsyncCompleted(object sender, AScene e)
        {
            SceneTransitions.Immediate(glControl1, ref _scene, e);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[INFO] settingsToolStripMenuItem_Click.");
            using (var settingsForm = new SettingsForm())
                settingsForm.ShowDialog(this);
        }

        private void shaderCollection_LoadCompleted(object sender, EventArgs e)
        {
            Debug.Assert(!InvokeRequired);
            _shadersLoaded = true;
            TryLoadMainScene();
        }

        private void sysListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[INFO] sysListToolStripMenuItem_Click.");
            SystemListForm frm = new SystemListForm();
            frm.Show();
        }


        // The background worker is used to load systems from the database.
        private void bgSysLoadWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Systems.LoadBGW(sender as BackgroundWorker);
        }

        private void bgSysLoadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (toolStripProgressBar1 != null && !toolStripProgressBar1.IsDisposed && e != null)
            {
                toolStripProgressBar1.Value = e.ProgressPercentage;
                if (e.UserState != null)
                {
                    var x = e.UserState as SystemsLoadingProgress;
                    if (!x.AllComplete)
                        StatusText = $"Loading systems; please wait... {x.ReadSystems.ToString("N0")} of {x.TotalSystems.ToString("N0")}: {x.MostRecentSystem}";
                    else
                        StatusText = $"Loaded {x.TotalSystems.ToString("N0")} systems. Initiating pre-flight checks ...";
                }
            }
        }

        private void bgSysLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // We may be disposed already, or disposing, or loading was cancelled, or it's just a Tuesday.
            if (IsDisposed || e == null || e.Cancelled || Systems.SystemsList == null) return;

            // TODO: there's a bit of work needed here.
            // Start loading nearby stars, get ready to display them.

            // kill the loading demo scene and switch to the main scene
            _databasesLoaded = true;
            TryLoadMainScene();
        }

        #endregion // Downstream events


        #region GL Control Events

        private void glControl1_Load(object sender, EventArgs e)
        {
            if (!_glLoaded)
            {
                Debug.WriteLine($"[INFO] glControl1_Load.");
                _glLoaded = true;

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
            // TODO
        }

        private void glControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // TODO
        }
        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            // TODO
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            // TODO
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            // TODO
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

        #endregion // GL Control Events
    }
}
