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

#region --- using ... ---

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;

#if DEBUG
using gldebug = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

#endregion // --- using ... ---

namespace StarMap.Shaders
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class Shader : IDisposable
    {
        public int AttribPositionIndex { get; private set; }
        public int AttribColorIndex { get; private set; }
        public int AttribTextureCoordinateIndex { get; private set; }
        public int AttribTextureOffsetIndex { get; private set; }

        public int UBO_ModelDiffuseIndex { get; private set; }
        public int UBO_ProjViewViewportIndex { get; private set; }

        public int UniformModelMatIndex { get; private set; }
        public int UniformDiffuseColorIndex { get; private set; }

        private string DebuggerDisplay
        {
            get
            {
                string status = IsReady ? "Ready" : (IsDisposed ? "Disposed" : "Not ready");
                return $"{Name}: {status}, ID {m_ProgramID:0}";
            }
        }
        public bool IsDisposed { get; private set; } = false;
        public bool IsReady { get; private set; } = false;
        public virtual string Name { get; protected set; }
        protected virtual string RootName { get; set; }
        public int ProgramID
        {
            get
            {
                return m_ProgramID;
            }
        }

#if DEBUG
        protected gldebug gld = new gldebug();
#endif
        protected readonly int m_ProgramID;
        protected readonly List<int> m_SubShaders = new List<int>();

        public Shader()
        {
            m_ProgramID = gld.CreateProgram();
        }

        ~Shader()
        {
#if DEBUG
            Debug.WriteLine($"[WARNING] Shader {GetType().Name} leaked! Did you forget to call Dispose()?");
#endif
            Dispose(false);
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        public void LoadAndLink(bool background = false)
        {
            string loadStyle = background ? "Background loading" : "Foreground loading";
            TraceLog.Debug($"{loadStyle} shader {Name}.");
            Load(Assembly.GetExecutingAssembly(), "StarMap.Shaders." + (string.IsNullOrEmpty(RootName) ? Name : RootName));
            if (Link())
            {
                IsReady = true;
                OnUpdateIndices();
                TraceLog.Debug($"Shader {Name} loaded.");
            }
            else
                Debug.Assert(false, $"Shader {Name} failed to Link()!");
        }

        protected virtual bool Link()
        {
            bool ret = true;

            foreach (var shader in m_SubShaders)
                gld.AttachShader(m_ProgramID, shader);
            gld.LinkProgram(m_ProgramID);
            var info = gld.GetProgramInfoLog(m_ProgramID);
            if (!string.IsNullOrWhiteSpace(info))
            {
                ret = false;
                LogErr(info, "LinkProgram");
            }
            foreach (var shader in m_SubShaders)
            {
                gld.DetachShader(m_ProgramID, shader);
                gld.DeleteShader(shader);
            }
            return ret;
        }

        protected virtual void Load(Assembly ass, string path)
        {
            Tuple<string, ShaderType>[] types = new Tuple<string, ShaderType>[]
            {
                new Tuple<string, ShaderType>("01-Vertex", ShaderType.VertexShader),
                new Tuple<string, ShaderType>("02-TessCtl", ShaderType.TessControlShader),
                new Tuple<string, ShaderType>("03-TessEval", ShaderType.TessEvaluationShader),
                new Tuple<string, ShaderType>("04-Geometry", ShaderType.GeometryShader),
                new Tuple<string, ShaderType>("05-Fragment", ShaderType.FragmentShader),
                new Tuple<string, ShaderType>("06-Compute", ShaderType.ComputeShader)
            };

            foreach (var t in types)
            {
                string fullpath = $"{path}.{t.Item1}.glsl";
                using (Stream stream = ass.GetManifestResourceStream(fullpath))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var subShader = gld.CreateShader(t.Item2);
                            gld.ShaderSource(subShader, reader.ReadToEnd());
                            gld.CompileShader(subShader);
                            var info = gld.GetShaderInfoLog(subShader);
                            if (!string.IsNullOrWhiteSpace(info))
                            {
                                if (info.ToLowerInvariant().Contains("error"))
                                    TraceLog.Fatal($"GLDebug.CompileShader {Name} {t.Item1} reported: '{info}'.");
                                else
                                    TraceLog.Notice($"GLDebug.CompileShader {Name} {t.Item1} reported: '{info}'.");
                            }
                            m_SubShaders.Add(subShader);
                        }
                    }
                    else if (t.Item2 == ShaderType.VertexShader || t.Item2 == ShaderType.FragmentShader)
                        TraceLog.Fatal($"GLDebug.CompileShader {Name} required sub-shader type {t.Item1} is missing!");
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsReady = false;
                if (disposing)
                {
                    m_SubShaders.Clear();
                }
                gld.DeleteProgram(m_ProgramID);
                IsDisposed = true;
            }
        }

        protected void LogErr(string loginfo, string procName)
        {
            if (loginfo.Contains("error"))
                Debug.WriteLine($"*** [ERROR] *** Shader '{Name}', {procName} reported: '{loginfo}'.");
            else if (loginfo.Contains("warn"))
                Debug.WriteLine($"[WARN] Shader '{Name}', {procName} reported: '{loginfo}'.");
            else
                Debug.WriteLine($"[NOTICE] Shader '{Name}', {procName} reported: '{loginfo}'.");
        }

        protected virtual void OnUpdateIndices()
        {
            AttribPositionIndex = gld.GetAttribLocation(m_ProgramID, "position");
            AttribColorIndex = gld.GetAttribLocation(m_ProgramID, "color");
            AttribTextureCoordinateIndex = gld.GetAttribLocation(m_ProgramID, "textureCoordinate");
            AttribTextureOffsetIndex = gld.GetAttribLocation(m_ProgramID, "textureOffset");

            UniformModelMatIndex = gld.GetUniformLocation(m_ProgramID, "modelMatrix");
            UniformDiffuseColorIndex = gld.GetUniformLocation(m_ProgramID, "diffuseColor");

            UBO_ProjViewViewportIndex = gld.GetUniformBlockIndex(m_ProgramID, "ProjectionView");
        }
    }
}
