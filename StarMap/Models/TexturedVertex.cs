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

namespace StarMap.Models
{
    public struct TexturedVertex
    {
        private readonly Vector3 _position;
        private readonly Vector2 _textureCoordinate;

        public static int Size { get { return (4 + 2) * 4; } }

        public static int PositionOffsetBytes { get { return 0; } }
        public static int PositionSize { get { return 3; } }
        public static int TexCoordAttribOffsetBytes { get { return PositionSize * 4; } }
        public static int TexCoordAttribSize { get { return 2; } }


        public TexturedVertex(Vector3 position, Vector2 textureCoordinate)
        {
            _position = position;
            _textureCoordinate = textureCoordinate;
        }
    }
}
