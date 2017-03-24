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
using OpenTK.Graphics;

namespace StarMap.Renderables
{
    public class StupidBoxModel : ColoredRenderable
    {
        public StupidBoxModel(float size) : base(Model(size), Program.Shaders.PlainPipe) { }

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
                new ColoredVertex(new Vector4(-side, -side, -side, 1), blue),
                new ColoredVertex(new Vector4(-side, -side,  side, 1), red),
                new ColoredVertex(new Vector4(-side,  side, -side, 1), purple),
                new ColoredVertex(new Vector4(-side,  side, -side, 1), purple),
                new ColoredVertex(new Vector4(-side, -side,  side, 1), red),
                new ColoredVertex(new Vector4(-side,  side,  side, 1), green),
                // side 2
                new ColoredVertex(new Vector4( side, -side, -side, 1), green),
                new ColoredVertex(new Vector4( side,  side, -side, 1), red),
                new ColoredVertex(new Vector4( side, -side,  side, 1), purple),
                new ColoredVertex(new Vector4( side, -side,  side, 1), purple),
                new ColoredVertex(new Vector4( side,  side, -side, 1), red),
                new ColoredVertex(new Vector4( side,  side,  side, 1), blue),
                // side 3
                new ColoredVertex(new Vector4(-side, -side, -side, 1), blue),
                new ColoredVertex(new Vector4( side, -side, -side, 1), green),
                new ColoredVertex(new Vector4(-side, -side,  side, 1), red),
                new ColoredVertex(new Vector4(-side, -side,  side, 1), red),
                new ColoredVertex(new Vector4( side, -side, -side, 1), green),
                new ColoredVertex(new Vector4( side, -side,  side, 1), purple),
                // side 4
                new ColoredVertex(new Vector4(-side,  side, -side, 1), purple),
                new ColoredVertex(new Vector4(-side,  side,  side, 1), green),
                new ColoredVertex(new Vector4( side,  side, -side, 1), red),
                new ColoredVertex(new Vector4( side,  side, -side, 1), red),
                new ColoredVertex(new Vector4(-side,  side,  side, 1), green),
                new ColoredVertex(new Vector4( side,  side,  side, 1), blue),
                // down
                new ColoredVertex(new Vector4(-side, -side, -side, 1), blue),
                new ColoredVertex(new Vector4(-side,  side, -side, 1), purple),
                new ColoredVertex(new Vector4( side, -side, -side, 1), green),
                new ColoredVertex(new Vector4( side, -side, -side, 1), green),
                new ColoredVertex(new Vector4(-side,  side, -side, 1), purple),
                new ColoredVertex(new Vector4( side,  side, -side, 1), red),
                // up
                new ColoredVertex(new Vector4(-side, -side,  side, 1), red),
                new ColoredVertex(new Vector4( side, -side,  side, 1), purple),
                new ColoredVertex(new Vector4(-side,  side,  side, 1), green),
                new ColoredVertex(new Vector4(-side,  side,  side, 1), green),
                new ColoredVertex(new Vector4( side, -side,  side, 1), purple),
                new ColoredVertex(new Vector4( side,  side,  side, 1), blue)
            };

            return vertices;
        }
    }
}
