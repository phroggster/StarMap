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

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif
#endregion // --- using ... ---

namespace StarMap.Models
{
    public class LineModel : ColoredModel
    {
        public LineModel(float length, Color4 color) : base(Model(length, color)) { }

        private static ColoredVertex[] Model(float length, Color4 color)
        {
            float hlen = length * .5f;
            //Color4 red = new Color4(1f, 0, 0, 1);

            // Dumb colored line
            ColoredVertex[] vertices = new ColoredVertex[]
            {
                // side 1
                new ColoredVertex(new Vector3(-hlen, 0, 0), color),
                new ColoredVertex(new Vector3( hlen, 0, 0), color)
            };

            return vertices;
        }

        public override void Render()
        {
            gld.DrawArrays(PrimitiveType.Lines, 0, m_VertexCount);
        }
    }
}
