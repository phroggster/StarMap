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
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
#endregion // --- using ... ---

namespace StarMap.Shaders
{
    public class TexPipeShader : Shader
    {
        public TexPipeShader()
        {
            Name = nameof(TexPipeShader);
        }

        protected override void OnUpdateIndices()
        {
            base.OnUpdateIndices();

            //  layout(std140) uniform ProjectionView{..};
            if (ProjectionView < 0)
                throw new ApplicationException($"{Name} lacks {nameof(ProjectionView)} uniform binding.");

            //  layout(std140) uniform Model{..};
            if (Model < 0)
                throw new ApplicationException($"{Name} lacks {nameof(Model)} uniform binding.");

            //  in vec4 Position;
            if (Position < 0)
                throw new ApplicationException($"{Name} lacks {nameof(Position)} attribute.");

            //  in vec2 TexCoord;
            if (TexCoord < 0)
                throw new ApplicationException($"{Name} lacks {nameof(TexCoord)} attribute.");
        }
    }
}
