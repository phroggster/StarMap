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
using OpenTK.Graphics.OpenGL;
using StarMap.Cameras;
using StarMap.Database;
using StarMap.Objects;
using StarMap.Renderables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StarMap.Scenes
{
    public class LoadingScene : AScene
    {
        public const float halfPI = (float)Math.PI / 2;

        public override Color BackColor { get; set; } = Color.Black;

        public override ICamera Camera { get; set; } = new FirstPersonCamera(new Vector3(0, 0, -4000), Quaternion.Identity);
        //public override ICamera Camera { get; set; } = new FirstPersonCamera(new Vector3(0, 0, -20), new Quaternion(Vector3.Zero));

        public override string Name { get { return "LoadingScene"; } }
        public override List<Keys> ToggleKeys { get; set; } = new List<Keys>() { Keys.P };

        protected virtual Color colorA { get; set; } = Color.Black;
        protected virtual Color colorB { get; set; } = Color.NavajoWhite;
        protected bool ColorFlip { get; set; } = false;

        public LoadingScene() { }

        public LoadingScene(int width, int height) : base(width, height)
        {
            Config.Instance.GridLineColourChanged += Config_GridLineColourChanged;
        }

        public void AddSystem(SystemBase system)
        {
            Contents.Add(new StupidBoxObject(models["box"], new Vector4(system.Position, 0), Quaternion.Identity, new Vector3(100)));
        }

        IList<SystemBase> _systems;
        public void AddSystems(IList<SystemBase> systems)
        {
            _systems = systems;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                foreach (var m in models.Values)
                    m?.Dispose();

                if (disposing)
                {
                    models.Clear();
                    Config.Instance.GridLineColourChanged -= Config_GridLineColourChanged;
                }

                models = null;
                base.Dispose(disposing);
            }
        }

        protected override void OnKeyPress(KeyEventArgs e)
        {
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
            models = new Dictionary<string, ARenderable>();
            //ARenderable axis = new AxisModel(1);
            ARenderable box = new StupidBoxModel(1);
            ARenderable redline = new StupidLineModel(1, new OpenTK.Graphics.Color4(1f, 0, 0, 1));
            ARenderable greenline = new StupidLineModel(1, new OpenTK.Graphics.Color4(0f, 1, 0, 1));

            //models.Add(nameof(axis), axis);
            models.Add("box", box);
            models.Add(nameof(redline), redline);
            models.Add(nameof(greenline), greenline);

            for (int i = -20000; i <= 70000; i+= 10000)
                Contents.Add(new StupidLine(models[nameof(redline)], new Vector4(0, i, 0, 0), Quaternion.Identity, new Vector3(80000, 1, 1), nameof(redline)));
            for (int i = -40000; i <= 40000; i += 10000)
                Contents.Add(new StupidLine(models[nameof(greenline)], new Vector4(i, 25000, 0, 0), new Quaternion(halfPI, 0, 0), new Vector3(1, 90000, 1), nameof(greenline)));

            //Contents.Add(new StupidLine(models[nameof(axis)], Vector4.Zero, Quaternion.Identity, new Vector3(100), nameof(axis)));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.LineWidth(2);
            GL.PointSize(2);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            //Camera.LookAt(Vector3.Zero);
            
        }


        protected override void OnUpdate(double delta)
        {
            if (_systems != null)
            {
                lock (_systems)
                {
                    lock (models)
                    {
                        models.Add("stars", new StupidStars(_systems));
                    }
                    _systems = null;
                }
                lock (Contents)
                {
                    Contents.RemoveAll(c => c is StupidBoxObject);
                    Contents.Add(new StarsObject(models["stars"]));
                }
            }
            base.OnUpdate(delta);
        }

        //private const float starrotspeed = 0.75f;
        private Dictionary<string, ARenderable> models;

        private void Config_GridLineColourChanged(object sender, Color e)
        {
            BackColor = e;
        }
    }

    public class LoadingSceneBlue : LoadingScene
    {
        private bool colorFlip = false;
        public override Color BackColor { get; set; } = Color.LightBlue;
        protected override Color colorA { get { return Color.LightBlue; } }
        protected override Color colorB { get { return Color.NavajoWhite; } }
        public override string Name { get { return "LoadingSceneBlue"; } }
        
        public LoadingSceneBlue() { }

        public LoadingSceneBlue(int width, int height) : base(width, height) { }

        protected override void OnKeyPress(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
            {
                if (colorFlip)
                    BackColor = colorA;
                else
                    BackColor = colorB;

                colorFlip = !colorFlip;
            }
        }
    }
}
