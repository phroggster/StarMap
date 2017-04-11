using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


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

            //  layout(location = 0) in vec4 position;
            Debug.Assert(AttribPositionIndex != -1);

            //  layout(std140, binding = 0) uniform ProjectionView{..};
            Debug.Assert(UBO_ProjViewViewportIndex != -1);

            //  uniform mat4 modelMatrix;
            Debug.Assert(UniformModelMatIndex != -1);

            //  uniform vec4 diffuseColor;
            Debug.Assert(UniformDiffuseColorIndex != -1);
        }
    }
}
