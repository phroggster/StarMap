﻿#region --- Apache v2.0 license ---
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

#region --- using's ---
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif
#endregion // --- using's ---

namespace StarMap.Shaders
{
    public class GridLineShader : Shader
    {
        public GridLineShader()
        {
            Name = nameof(GridLineShader);
        }

        protected override void OnUpdateIndices()
        {
            base.OnUpdateIndices();

            //  layout(std140) uniform ProjectionView{..};
            Debug.Assert(ProjectionView >= 0);
            //  layout(std140) uniform Model{..};
            Debug.Assert(Model >= 0);
            //  layout(std140) uniform GridLineData{..};
            Debug.Assert(GridLineData >= 0);

            //  in vec4 position;
            Debug.Assert(AttribPosition >= 0);
        }
    }
}
