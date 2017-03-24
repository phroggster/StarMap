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
    public class StarShader : Shader
    {
        public override string Name { get { return "Star"; } }

        public StarShader() { }

        protected override void Load(Assembly ass, string path)
        {
            using (Stream stream = ass.GetManifestResourceStream(path + "V.c"))
            using (StreamReader reader = new StreamReader(stream))
            {
                var shader = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(shader, reader.ReadToEnd());
                GL.CompileShader(shader);
                var info = GL.GetShaderInfoLog(shader);
                if (!string.IsNullOrWhiteSpace(info))
                    LogErr(info, "CompileShader (vertex)");
                SubShaders.Add(shader);
            }

            using (Stream stream = ass.GetManifestResourceStream(path + "F.c"))
            using (StreamReader reader = new StreamReader(stream))
            {
                var shader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(shader, reader.ReadToEnd());
                GL.CompileShader(shader);
                var info = GL.GetShaderInfoLog(shader);
                if (!string.IsNullOrWhiteSpace(info))
                    LogErr(info, "CompileShader (fragment)");
                SubShaders.Add(shader);
            }
        }
    }
}
