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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace StarMap.Scenes
{
    public class LoadingScene : AScene
    {
        public override Color BackColor { get; set; } = Color.Black;
        public override Camera Camera { get; set; } = new FirstPersonCamera(new Vector3(0, -1, 2), -Vector3.UnitY);
        public override string Name { get { return "LoadingScene"; } }
        public override Keys ToggleKeys { get { return Keys.P; } }

        public LoadingScene() { }

        public LoadingScene(int width, int height) : base(width, height) { }

        protected override void Dispose(bool disposing)
        {
            rob?.Dispose();
            rob = null;
            base.Dispose(disposing);
        }

        protected override void OnKeyPress(Keys key)
        {
            if (key.HasFlag(Keys.P))
            {
                if (BackColor == Color.Black)
                    BackColor = Color.NavajoWhite;
                else
                    BackColor = Color.Black;
            }
        }

        protected override void OnLoad()
        {
            rob = new StupidBoxModel(0.5f);
            Contents.Add(new StupidBox(rob, new Vector4(0, -1f, -2.7f, 0), new Vector4(0, 0, 0.5f, 0), Vector3.One));
            Contents.Add(new StupidBox(rob, new Vector4(0, 0.5f, -2.7f, 0), Vector4.Zero, Vector3.One));
            Contents.Add(new StupidBox(rob, new Vector4(1, 1, -2.7f, 0), Vector4.Zero, Vector3.One));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.PointSize(3);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            Camera.LookAt(Vector3.Zero);
            
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
        private StupidBoxModel rob;
    }
}
