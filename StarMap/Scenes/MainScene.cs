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
using Phroggiesoft.Controls;
using StarMap.Cameras;
using StarMap.Database;
using StarMap.SceneObjects;
using StarMap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

#if DEBUG
using gldebug = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Scenes
{
    public class MainScene : Scene
    {
        #region --- boring constructors ---

        public MainScene() : base() { }

        public MainScene(IContainer container) : base(container) { }

        #endregion // --- boring constructors ---

        #region --- public property overrides ---

        public override Color BackColor { get; set; } = Color.FromArgb(16, 16, 16);

        public override ICamera Camera { get; set; } = new FirstPersonCamera(new Vector3(0, 0, -4000), Quaternion.Identity);

        public override string Name { get { return nameof(MainScene); } }

        #endregion // --- public property overrides ---

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (m_Models != null && m_Models.Count > 0)
                    {
                        foreach (var m in m_Models.Values)
                            m?.Dispose();
                        m_Models.Clear();
                    }
                }

                Camera = null;
                m_Models = null;

                base.Dispose(disposing);
            }
        }

        protected override void OnFirstUpdate()
        {
            base.OnFirstUpdate();
            gld.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            gld.PatchParameter(PatchParameterInt.PatchVertices, 3);
            gld.LineWidth(2);
            gld.PointSize(2);
            gld.Enable(EnableCap.Blend);
            gld.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            gld.Enable(EnableCap.DepthTest);
            gld.Enable(EnableCap.CullFace);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            m_Models = new Dictionary<string, Model>();

            //Models.Add("galaxy", new TexturedRenderable(GalaxyGenerator.Galaxy(25f, 4500, 4500), Program.Shaders.TexPipe, Properties.Resources.Galaxy_L));
            StarsModel systems = new StarsModel(SystemsManager.SystemsList);
            m_Models.Add(nameof(systems), systems);

            GridLinesModel gridLines = new GridLinesModel();
            m_Models.Add(nameof(gridLines), gridLines);

            Contents.Add(new GridLinesObject(gridLines));
            Contents.Add(new StarsObject(systems));
        }

        #region --- private implementation ---

        private Dictionary<string, Model> m_Models;

        #endregion // --- private implementation ---
    }
}
