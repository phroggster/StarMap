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
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace StarMap.Shaders
{
    public class PlainPipeShader : Shader
    {
        public override string Name { get { return "PlainPipe"; } }

        public PlainPipeShader() { }

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
                    Debug.WriteLine($"[NOTICE] GL.CompileShader {Name} Vertex reported: '{info}'.");
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
                    Debug.WriteLine($"[NOTICE] GL.CompileShader {Name} Fragment reported: '{info}'.");
                SubShaders.Add(shader);
            }
        }
    }
}
