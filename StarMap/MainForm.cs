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

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using StarMap.Database;
using StarMap.Scenes;
using StarMap.Shaders;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace StarMap
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            TraceLog.Info("Constructing Main Form.");
            InitializeComponent();
#if GLDEBUG
            phrogGLControl1.ContextFlags |= GraphicsContextFlags.Debug;
#endif
            if (components == null)
                components = new Container();
            Text = string.Format("{0} v{1}", Text, Program.AppVersion);
            _sceneTrans = new SceneTransitions();
        }

        #region --- private implementation ---

        #region fields

        // Startup state awareness
        private bool _databasesLoaded = false;

        // others.
        private ConfigBindingList _cbl;
        private Config _config;
        private IScene _scene;
        private SceneTransitions _sceneTrans;

        #endregion // fields


        #region methods

        private void InitializeDatabases()
        {
            toolStripStatusLabel1.Text = "Initializing databases...";
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
            toolStripStatusLabel1.Text = "Loading configuration...";
            _config = Config.Instance;
            _config.Load(EDDUserDBConnection.EarlyRegister, SMDBConnection.EarlyRegister);
        }

        private void TryLoadMainScene()
        {
            // TODO: XXX: Async is broken.
            if (_databasesLoaded && !IsDisposed && Visible)
            {
                _sceneTrans.Immediate(components, phrogGLControl1, ref _scene, new MainScene(components));
                _scene.FPSUpdate += scene_FPSUpdate;
            }
        }

        private void scene_FPSUpdate(object sender, StringChangedEventArgs e)
        {
            toolStripStatusLabel1.Text = e.Value;
        }

        #endregion // methods


        #region Upstream event handlers

        /// <summary>
        /// The <see cref="Form.FormClosed"/> event handler.
        /// </summary>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TraceLog.Info(nameof(MainForm_FormClosed));
            _cbl?.Clear();
        }

        /// <summary>
        /// The <see cref="Form.FormClosing"/> event handler.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TraceLog.Info(nameof(MainForm_FormClosing));

            _scene?.Stop();

            if (bgSysLoadWorker.IsBusy)
                bgSysLoadWorker.CancelAsync();
        }

        /// <summary>
        /// The <see cref="Form.Load"/> event handler.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            TraceLog.Info(nameof(MainForm_Load));
            InitializeDatabases();
            LoadConfig();
#if !DONT_LOAD_SYSTEMS
            bgSysLoadWorker.RunWorkerAsync();
#endif
        }

        #endregion // Upstream event handlers


        #region Downstream event handlers

        #region bgSysLoadWorker

        // The background worker is used to load systems from the database.
        // Right now, it loads _every_ system minimal info to RAM at start.
        // TODO: keep the bgw on-hand to fetch details as-needed.
        private void bgSysLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SystemsManager.LoadBGW(sender as BackgroundWorker);
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
                        toolStripStatusLabel1.Text = $"Loading systems; please wait... {x.ReadSystems.ToString("N0")} of {x.TotalSystems.ToString("N0")}";
                        //(_scene as LoadingScene).AddSystem(x.MostRecentSystem);
                    }   
                    else
                        toolStripStatusLabel1.Text = $"Loaded {x.TotalSystems.ToString("N0")} systems. Initiating pre-flight checks ...";
                }
            }
        }

        // TODO: keep the bgw on-hand to fetch details as-needed.
        private void bgSysLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // We may be disposed already, or disposing, or loading was cancelled, or it's just a Tuesday.
            if (IsDisposed || e == null || e.Cancelled || SystemsManager.SystemsList == null)
                return;

            _databasesLoaded = true;
            TraceLog.Debug("Finished loading {0:0,0} systems from database.", SystemsManager.SystemsList.Count);

            // kill the loading demo scene and switch to the main scene
            TryLoadMainScene();
        }

        #endregion // bgSysLoadWorker

        #region phrogGLControl1

        private void phrogGLControl1_Load(object sender, EventArgs e)
        {
            TraceLog.Info(nameof(phrogGLControl1_Load));
#if GLDEBUG
            GLDebug.Attach();
#endif

            _cbl = new ConfigBindingList();
            _cbl.BindToControl(phrogGLControl1, nameof(phrogGLControl1.VSync), nameof(_config.VSync));

            Program.Shaders = new ShaderManager();
            Program.Shaders.Load();

            _scene = new LoadingScene(components);
            _scene.Load(phrogGLControl1);
            _scene.Start();

            phrogGLControl1.Load -= phrogGLControl1_Load;
        }

        #endregion // phrogGLControl1

        #region menuStrip1

                private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    TraceLog.Info(nameof(aboutToolStripMenuItem_Click));
                    using (var abt = new AboutBoxForm())
                        abt.ShowDialog(this);
                }

                private void exitToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    TraceLog.Info(nameof(exitToolStripMenuItem_Click));
                    Close();
                }

                private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    TraceLog.Info(nameof(settingsToolStripMenuItem_Click));
                    using (var settingsForm = new SettingsForm())
                        settingsForm.ShowDialog(this);
                }

                private void sysListToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    TraceLog.Info(nameof(sysListToolStripMenuItem_Click));
                    SystemListForm frm = new SystemListForm();
                    frm.Show();
                }

        #endregion // menuStrip1

        private void scene_FPSUpdate(object sender, string frameRate)
        {
            toolStripStatusLabel1.Text = frameRate;
        }

        #endregion // Downstream event handlers

        #endregion // --- private implementation ---
    }
}
