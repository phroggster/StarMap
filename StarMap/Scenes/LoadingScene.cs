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
using StarMap.Objects;
using StarMap.Renderables;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace StarMap.Scenes
{
    public class LoadingScene : AScene
    {
        public override Color BackColor { get; set; } = Color.Black;
        public override Camera Camera { get; set; } = new FirstPersonCamera(new Vector3(0, 0, 2), new Quaternion(Vector3.Zero));
        public override string Name { get { return "LoadingScene"; } }
        public override List<Keys> ToggleKeys { get; set; } = new List<Keys>() { Keys.P };

        public LoadingScene() { }

        public LoadingScene(int width, int height) : base(width, height)
        {
            Config.Instance.GridLineColourChanged += Config_GridLineColourChanged;
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
                if (BackColor.ToArgb() == Config.Instance.GridLineColour.ToArgb())
                    BackColor = Color.NavajoWhite;
                else
                    BackColor = Config.Instance.GridLineColour;
            }
        }

        protected override void OnLoad()
        {
            models = new Dictionary<string, ARenderable>();
            ARenderable axis = new AxisModel(1);
            ARenderable box = new StupidBoxModel(1);
            ARenderable line = new StupidLineModel(1);

            models.Add("Axis", axis);
            models.Add("Box", box);
            models.Add("Line", line);

            Contents.Add(new StupidBox(models["Box"], new Vector4(0, -1f, -2.7f, 0), new Vector4(0, 0, 0.5f, 0), Vector3.One));
            Contents.Add(new StupidBox(models["Box"], new Vector4(0, 0.5f, -2.7f, 0), Vector4.Zero, Vector3.One));
            Contents.Add(new StupidBox(models["Box"], new Vector4(1, 1, -2.7f, 0), Vector4.Zero, Vector3.One));

            Contents.Add(new StupidLine(models["Line"], new Vector4(-30, 0, 0, 0), Vector4.Zero, Vector3.One));
            Contents.Add(new StupidLine(models["Line"], new Vector4(-20, 0, 0, 0), Vector4.Zero, Vector3.One));
            Contents.Add(new StupidLine(models["Line"], new Vector4(-10, 0, 0, 0), Vector4.Zero, Vector3.One));
            //Contents.Add(new StupidLine(models["Line"], new Vector4(  0, 0, 0, 0), Vector4.Zero, Vector3.One));
            Contents.Add(new StupidLine(models["Line"], new Vector4( 10, 0, 0, 0), Vector4.Zero, Vector3.One));
            Contents.Add(new StupidLine(models["Line"], new Vector4( 20, 0, 0, 0), Vector4.Zero, Vector3.One));
            Contents.Add(new StupidLine(models["Line"], new Vector4( 30, 0, 0, 0), Vector4.Zero, Vector3.One));


            Contents.Add(new StupidLine(models["Axis"], Vector4.Zero, Vector4.Zero, new Vector3(10)));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.PointSize(10);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            //Camera.LookAt(Vector3.Zero);
            
        }

        protected override void OnUpdate(double delta)
        {
            rotation += (float)delta;
            float rotspeed = rotation * speed;
            Contents[0].Rotation = -Vector4.UnitZ * rotspeed;
            Contents[1].Rotation = Vector4.UnitZ * rotspeed;
            Contents[2].Rotation = Vector4.UnitY * rotspeed;
            base.OnUpdate(delta);
        }

        private float rotation = 0;
        private const float speed = 0.4f;
        private Dictionary<string, ARenderable> models;

        private void Config_GridLineColourChanged(object sender, Color e)
        {
            BackColor = e;
        }
    }

    public class LoadingSceneBlue : LoadingScene
    {
        public override Color BackColor { get; set; } = Color.LightBlue;
        public override string Name { get { return "LoadingSceneBlue"; } }

        public LoadingSceneBlue() { }

        public LoadingSceneBlue(int width, int height) : base(width, height) { }
    }
}
