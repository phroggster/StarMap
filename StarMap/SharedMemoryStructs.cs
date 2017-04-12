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
using StarMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion // --- using ... ---

namespace StarMap
{
    public static class BindingPosts
    {
        private static int _index = 0;

        /// <summary>
        /// Designed to be shared amongst all objects, this is the binding post for a <see cref="ProjectionView"/> uniform buffer.
        /// </summary>
        public static int ProjectionView { get { return 0; } }

        public static int GenBindingPost()
        {
            return ++_index;
        }
    }

    /*  layout(std140) uniform GridLineData
     *  {
     *      vec4 coarseColor;
     *      vec4 fineColor;
     *      int coarseVertCount;
     *  };
     */
    public struct GridLineData
    {
        public Color4 CoarseColor;  // (4*4*         16 bytes
        public Color4 FineColor;    //     *2)+      16 bytes
        public int CoarseVertCount; //        +4      4 bytes
        // -----------------------------------------------------
        public const int SizeInBytes = (4*4*2)+4; // 36 bytes
        // =====================================================

        public GridLineData(Color4 coarseColor, Color4 fineColor, int coarseVertCount)
        {
            CoarseColor = coarseColor;
            FineColor = fineColor;
            CoarseVertCount = coarseVertCount;
        }

        public static GridLineData Identity
        {
            get
            {
                return new GridLineData(Config.Instance.GridLineColour, Config.Instance.FineGridLineColour, GridLinesModel.coarseVertCount);
            }
        }
    }

    /*  layout(std140) uniform ProjectionView
     *  {
     *      mat4 projectionMatrix;
     *      mat4 viewMatrix;
     *      vec2 viewportSize;
     *  };
     */
    public struct ProjectionView
    {
        public Matrix4 ProjectionMatrix;// (4*4*4*              64 bytes
        public Matrix4 ViewMatrix;      //       *2)+           64 bytes
        public Vector2 ViewportSize;    //          +(4*2)       8 bytes
        // --------------------------------------------------------------
        public const int SizeInBytes =     (4*4*4*2)+(4*2); // 136 bytes
        // ==============================================================

        public ProjectionView(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector2 viewportSize)
        {
            ProjectionMatrix = projectionMatrix;
            ViewMatrix = viewMatrix;
            ViewportSize = viewportSize;
        }

        public static ProjectionView Identity
        {
            get
            {
                return new ProjectionView(Matrix4.Identity, Matrix4.Identity, new Vector2(640, 480));
            }
        }
    }
}
