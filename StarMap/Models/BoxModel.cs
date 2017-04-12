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
#endregion // --- using ... ---

namespace StarMap.Models
{
    public class BoxModel : ColoredModel
    {
        public BoxModel(float size) : base(Model(size)) { }

        private static ColoredVertex[] Model(float size)
        {
            float side = size / 2;
            Color4 white = new Color4(1f, 1, 1, 1);
            Color4 red = white; // new Color4(1f, 0, 0, 1);
            Color4 green = white; //new Color4(0f, 1, 0, 1);
            Color4 blue = white;//new Color4(0f, 0, 1, 1);
            Color4 purple = white;// new Color4(.636f, .285f, .64f, 1);
            
            // Multicolored cube... who knows.
            ColoredVertex[] vertices = new ColoredVertex[]
            {
                // side 1
                new ColoredVertex(new Vector3(-side, -side, -side), blue),
                new ColoredVertex(new Vector3(-side, -side,  side), red),
                new ColoredVertex(new Vector3(-side,  side, -side), purple),
                new ColoredVertex(new Vector3(-side,  side, -side), purple),
                new ColoredVertex(new Vector3(-side, -side,  side), red),
                new ColoredVertex(new Vector3(-side,  side,  side), green),
                // side 2
                new ColoredVertex(new Vector3( side, -side, -side), green),
                new ColoredVertex(new Vector3( side,  side, -side), red),
                new ColoredVertex(new Vector3( side, -side,  side), purple),
                new ColoredVertex(new Vector3( side, -side,  side), purple),
                new ColoredVertex(new Vector3( side,  side, -side), red),
                new ColoredVertex(new Vector3( side,  side,  side), blue),
                // side 3
                new ColoredVertex(new Vector3(-side, -side, -side), blue),
                new ColoredVertex(new Vector3( side, -side, -side), green),
                new ColoredVertex(new Vector3(-side, -side,  side), red),
                new ColoredVertex(new Vector3(-side, -side,  side), red),
                new ColoredVertex(new Vector3( side, -side, -side), green),
                new ColoredVertex(new Vector3( side, -side,  side), purple),
                // side 4
                new ColoredVertex(new Vector3(-side,  side, -side), purple),
                new ColoredVertex(new Vector3(-side,  side,  side), green),
                new ColoredVertex(new Vector3( side,  side, -side), red),
                new ColoredVertex(new Vector3( side,  side, -side), red),
                new ColoredVertex(new Vector3(-side,  side,  side), green),
                new ColoredVertex(new Vector3( side,  side,  side), blue),
                // down
                new ColoredVertex(new Vector3(-side, -side, -side), blue),
                new ColoredVertex(new Vector3(-side,  side, -side), purple),
                new ColoredVertex(new Vector3( side, -side, -side), green),
                new ColoredVertex(new Vector3( side, -side, -side), green),
                new ColoredVertex(new Vector3(-side,  side, -side), purple),
                new ColoredVertex(new Vector3( side,  side, -side), red),
                // up
                new ColoredVertex(new Vector3(-side, -side,  side), red),
                new ColoredVertex(new Vector3( side, -side,  side), purple),
                new ColoredVertex(new Vector3(-side,  side,  side), green),
                new ColoredVertex(new Vector3(-side,  side,  side), green),
                new ColoredVertex(new Vector3( side, -side,  side), purple),
                new ColoredVertex(new Vector3( side,  side,  side), blue)
            };

            return vertices;
        }
    }
}
