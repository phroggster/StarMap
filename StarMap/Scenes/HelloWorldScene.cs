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
using System.ComponentModel;
using System.Drawing;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Scenes
{
    /// <summary>
    /// A crappy scene to draw a static 2D ortho projection triangle using old GL methods.
    /// </summary>
    public class HelloWorldScene : Scene
    {
        public HelloWorldScene(IContainer container) : base(container)
        {
            BackColor = Color.DarkSlateGray;
            Camera = new StaticCamera(Vector3.Zero, Quaternion.Identity, this);
            Name = nameof(HelloWorldScene);
        }

        protected override void OnFirstRender()
        {
            base.OnFirstRender();

            gld.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            gld.PatchParameter(PatchParameterInt.PatchVertices, 3);
            gld.PointSize(3);
            gld.Enable(EnableCap.Blend);
            gld.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            gld.Enable(EnableCap.DepthTest);
            gld.Enable(EnableCap.CullFace);
        }
    }
}
