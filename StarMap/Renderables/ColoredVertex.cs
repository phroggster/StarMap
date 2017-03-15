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
    public struct ColoredVertex
    {
        // 4*4 for Vector4 plus 4*4 for Color4
        public const int Size = (4 + 4)* 4;

        private readonly Vector4 _position;
        private readonly Color4 _colour;

        public ColoredVertex(Vector4 position, Color4 colour)
        {
            _position = position;
            _colour = colour;
        }
    }
}
