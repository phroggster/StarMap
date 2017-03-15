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
using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using StarMap.Cameras;

namespace StarMap.Scenes
{
    /// <summary>
    /// A crappy scene to draw a static 2D ortho projection triangle using old GL methods.
    /// </summary>
    public class HelloWorldScene : AScene
    {
        public override Color BackColor { get; set; } = Color.DarkSlateGray;

        public override Camera Camera { get; set; } = new StaticCamera(Vector3.Zero, Vector3.Zero);

        public override string Name { get { return "HelloWorldScene"; } }

        public HelloWorldScene() { }

        public HelloWorldScene(int width, int height) : base(width, height) { }

        protected override void OnLoad()
        {
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
