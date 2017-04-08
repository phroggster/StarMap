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

namespace StarMap.Models
{
    public struct ColoredVertex
    {
        private readonly Vector3 _position;
        private readonly Color4 _colour;

        // 4*3 for Vector3 plus 4*4 for Color4
        public static int SizeInBytes { get { return PositionSize*4 + ColorSize*4; } }

        public static int ColorOffsetInBytes { get { return PositionSize * 4; } }
        public static int ColorSize { get { return 4; } }

        public static int PositionOffsetInBytes { get { return 0; } }
        public static int PositionSize { get { return 3; } }

        public ColoredVertex(Vector3 position, Color4 colour)
        {
            _position = position;
            _colour = colour;
        }

        public static ColoredVertex Identity { get { return new ColoredVertex(Vector3.Zero, Color4.PaleGreen); } }
    }
}
