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
using OpenTK.Graphics.OpenGL4;
using Phroggiesoft.Controls;
using StarMap.Cameras;
using StarMap.Database;
using StarMap.SceneObjects;
using StarMap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#if DEBUG
using gldebug = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Scenes
{
    public class LoadingScene : Scene
    {
        #region --- public LoadingScene(IContainer) ---

        public LoadingScene(IContainer container) : base(container)
        {
            BackColor = Color.Black;
            Camera = new FirstPersonCamera(new Vector3(0, 0, 4000), Quaternion.Identity, this);
            Name = nameof(LoadingScene);
        }

        #endregion // --- public LoadingScene(IContainer) ---

        #region --- public interface ---

        #region --- base property overrides ---

        public override IList<Keys> ToggleKeys { get; set; } = new List<Keys>() { Keys.P };

        #endregion // --- base property overrides ---

        #endregion // --- public interface ---

        #region --- protected implementation ---

        #region --- downstream ---

        protected virtual Color colorA { get; set; } = Color.Black;
        protected virtual Color colorB { get; set; } = Color.NavajoWhite;
        protected bool ColorFlip { get; set; } = false;

        #endregion // --- downstream ---

        #region --- upstream overrides ---

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (models != null)
                {
                    foreach (var m in models.Values)
                        m?.Dispose();
                    if (disposing)
                        models.Clear();
                }

                if (disposing)
                {
                    ToggleKeys?.Clear();
                }

                models = null;
                ToggleKeys = null;
                base.Dispose(disposing);
            }
        }

        protected override void OnFirstUpdate()
        {
            base.OnFirstUpdate();
            gld.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            gld.PatchParameter(PatchParameterInt.PatchVertices, 3);
            gld.LineWidth(3);
            gld.PointSize(2);
            gld.Enable(EnableCap.Blend);
            gld.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            gld.Enable(EnableCap.CullFace);
            gld.Enable(EnableCap.DepthTest);
            gld.Enable(EnableCap.LineSmooth);
        }

        protected override void OnKeyPress(KeyEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyCode == Keys.P)
            {
                if (ColorFlip)
                    BackColor = colorA;
                else
                    BackColor = colorB;
                ColorFlip = !ColorFlip;
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            models = new Dictionary<string, Model>();

            GridLinesModel gridLines = new GridLinesModel();
            models.Add(nameof(gridLines), gridLines);
            Contents.Add(new GridLinesObject(gridLines));

            AxesModel axes = new AxesModel(1);
            models.Add(nameof(axes), axes);
            Contents.Add(new AxesObject(axes, new Vector3(2000)));
        }

        #endregion // --- upstream overrides ---

        #endregion // --- protected implementation ---

        #region --- private implementation ---

        //private const float starrotspeed = 0.75f;
        private Dictionary<string, Model> models;

        #endregion // --- private implementation ---
    }
}
