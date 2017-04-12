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

#region --- using ... ---
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif
#endregion // --- using ... ---

namespace StarMap.Models
{
    public class AxesModel : ColoredModel
    {
        public AxesModel(float length) : base(Model(length)) { }

        private static ColoredVertex[] Model(float length)
        {
            Color4 white    = new Color4(1f, 1, 1, 1);
            Color4 red      = new Color4(1f, 0, 0, 1);
            Color4 green    = new Color4(0f, 1, 0, 1);
            Color4 blue     = new Color4(0f, 0, 1, 1);

            // Colored lines extending from origin
            ColoredVertex[] vertices = new ColoredVertex[]
            {
                // X
                new ColoredVertex(new Vector3(0, 0, 0), white),
                new ColoredVertex(new Vector3(length, 0, 0), red),
                // Y
                new ColoredVertex(new Vector3(0, 0, 0), white),
                new ColoredVertex(new Vector3(0, length, 0), green),
                // Z
                new ColoredVertex(new Vector3(0, 0, 0), white),
                new ColoredVertex(new Vector3(0, 0, length), blue),
            };

            return vertices;
        }

        public override void Render()
        {
            gld.DrawArrays(PrimitiveType.Lines, 0, m_VertexCount);
        }
    }
}
