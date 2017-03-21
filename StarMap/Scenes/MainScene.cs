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
// TODO: get rid of crappy GL2 triangle
using OpenTK.Graphics.OpenGL;
using StarMap.Cameras;
using StarMap.Renderables;
using System.Collections.Generic;
using System.Drawing;

namespace StarMap.Scenes
{
    public class MainScene : AScene
    {
        public override Color BackColor { get; set; } = Color.FromArgb(16, 16, 16);

        // 20k LY above earth and looking down? Maybe?
        public override Camera Camera { get; set; } = new FirstPersonCamera(new Vector3(0, 0, 20000), Quaternion.Identity);

        public override string Name { get; } = "MainScene";

        private Dictionary<string, ARenderable> Models = new Dictionary<string, ARenderable>();

        public MainScene() { }

        public MainScene(int width, int height) : base(width, height) { }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var m in Models)
                {
                    m.Value.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public void UpdateCoarseGridColour(Color colour)
        {
            //TODO.
        }

        public void UpdateFineGridColour(Color colour)
        {
            //TODO.
        }

        protected override void OnLoad()
        {
            // What crap this is.
            Models.Add("galaxy", new TexturedRenderable(GalaxyGenerator.Galaxy(25f, 4500, 4500), Program.Shaders.TexPipe, Properties.Resources.Galaxy_L));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.PointSize(3);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }
    }
}
