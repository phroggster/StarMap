using OpenTK;
using OpenTK.Graphics;
using StarMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
