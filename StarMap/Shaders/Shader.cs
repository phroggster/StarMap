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
using System.Linq;
using System.Reflection;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

#endregion // --- using ... ---

namespace StarMap.Shaders
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class Shader : IIsDisposed
    {
        #region --- protected Shader() ---

        protected Shader()
        {
            m_ProgramID = gld.CreateProgram();
        }

        #endregion // --- protected Shader() ---


        #region --- public interfaces ---

        #region --- GLSL attribute/uniform/etc indices ---

        #region --- Attribute (per-vertex) indices ---

        /// <summary>
        /// The per-vertex color attribute index: <code>in vec4 color;</code>
        /// </summary>
        public int AttribColor { get; private set; }

        /// <summary>
        /// The per-vertex position attribute index: <code>in vec4 position;</code>
        /// </summary>
        public int AttribPosition { get; private set; }

        /// <summary>
        /// The per-vertex TextureCoordinate (UV) attribute index: <code>in vec2 texCoord;</code>
        /// </summary>
        public int AttribTexCoord { get; private set; }

        /// <summary>
        /// The per-vertex TextureOffset (UV) attribute index: <code>in vec2 texOffset;</code>
        /// </summary>
        public int AttribTexOffset { get; private set; }

        #endregion // --- Attribute (per-vertex) indices ---

        #region --- UBO (per-shader) binding indices ---

        /// <summary>
        /// The per-shader Model uniform buffer object (UBO) binding index:
        /// <para><code>layout(std140) uniform Model {
        ///     mat4 modelMatrix;
        /// };</code></para>
        /// </summary>
        public int Model { get; private set; }

        /// <summary>
        /// The per-shader GridLineData uniform buffer object (UBO) binding index:
        /// <para><code>layout(std140) uniform GridLineData {
        ///     vec4 coarseColor;
        ///     vec4 fineColor;
        ///     int coarseVertCount;
        /// };</code></para>
        /// </summary>
        public int GridLineData { get; private set; }

        /// <summary>
        /// The per-shader ProjectionView uniform buffer object (UBO) binding index:
        /// <para><code>layout(std140) uniform ProjectionView {
        ///     mat4 projectionMatrix;
        ///     mat4 viewMatrix;
        ///     vec2 viewportSize;
        /// };</code></para>
        /// </summary>
        public int ProjectionView { get; private set; }

        #endregion // --- UBO (per-shader) binding indices ---

        #endregion // --- GLSL attribute/uniform/etc indices ---


        #region --- Other properties ---

        public bool IsReady { get; private set; } = false;

        public string Name { get; protected set; }

        public int ProgramID
        {
            get
            {
                return m_ProgramID;
            }
        }

        #endregion // --- Other properties ---


        #region --- IIsDisposed interface ---

        public bool IsDisposed { get; private set; } = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // --- IIsDisposed interface ---


        #region --- methods ---

        public void LoadAndLink()
        {
            if (!IsReady)
            {
                TraceLog.Debug($"Loading shader {Name}.");
                OnLoad(Assembly.GetExecutingAssembly(), $"StarMap.Shaders.{Name}");
                if (OnLink())
                {
                    IsReady = true;
                    OnUpdateIndices();
                    TraceLog.Debug($"Shader {Name} loaded.");
                }
                else
                {
                    string err = $"Shader {Name} failed to Link()!";
                    TraceLog.Fatal(err);
                    Debug.Assert(false, err);
                }
            }
        }

        #endregion // --- methods ---

        #endregion --- public interfaces ---


        #region --- protected implementation ---

        protected readonly int m_ProgramID;
        protected List<int> m_SubShaders = new List<int>();


        protected virtual string DebuggerDisplay
        {
            get
            {
                string status = IsReady ? "Ready" : (IsDisposed ? "Disposed" : "Not ready");
                return $"{Name}: {status}, ID {m_ProgramID:0}";
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                IsReady = false;

                if (disposing)
                    TraceLog.Info($"Disposing of {Name}.");
                else
                    TraceLog.Warn($"{Name} leaked! Did you forget to call `Dispose()`?");

                gld.DeleteProgram(m_ProgramID);
                if (disposing)
                {
                    m_SubShaders.Clear();
                }
                m_SubShaders = null;
            }
        }

        protected virtual bool OnLink()
        {
            bool ret = true;

            foreach (var shader in m_SubShaders)
                gld.AttachShader(m_ProgramID, shader);
            gld.LinkProgram(m_ProgramID);
            var info = gld.GetProgramInfoLog(m_ProgramID);
            if (!string.IsNullOrWhiteSpace(info))
            {
                ret = false;
                LogShaderMsg(info, "LinkProgram");
            }
            foreach (var shader in m_SubShaders)
            {
                gld.DetachShader(m_ProgramID, shader);
                gld.DeleteShader(shader);
            }
            return ret;
        }

        protected virtual void OnLoad(Assembly ass, string path)
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

                if (ass.GetManifestResourceNames().Contains(fullpath))
                {
                    using (StreamReader reader = new StreamReader(ass.GetManifestResourceStream(fullpath)))
                    {
                        var subShader = gld.CreateShader(t.Item2);
                        gld.ShaderSource(subShader, reader.ReadToEnd());
                        gld.CompileShader(subShader);
                        var info = gld.GetShaderInfoLog(subShader);
                        if (!string.IsNullOrWhiteSpace(info))
                        {
                            LogShaderMsg(info, $"CompileShader {t.Item1}");
                        }
                        m_SubShaders.Add(subShader);
                    }
                }
                else if (t.Item2 == ShaderType.VertexShader || t.Item2 == ShaderType.FragmentShader)
                    TraceLog.Fatal($"CompileShader {Name} required sub-shader type {t.Item1} is missing!");
            }
        }

        protected virtual void OnUpdateIndices()
        {
            GridLineData = gld.GetUniformBlockIndex(m_ProgramID, nameof(GridLineData));
            Model = gld.GetUniformBlockIndex(m_ProgramID, nameof(Model));
            ProjectionView = gld.GetUniformBlockIndex(m_ProgramID, nameof(ProjectionView));

            AttribColor = gld.GetAttribLocation(m_ProgramID, "color");
            AttribPosition = gld.GetAttribLocation(m_ProgramID, "position");
            AttribTexCoord = gld.GetAttribLocation(m_ProgramID, "texCoord");
            AttribTexOffset = gld.GetAttribLocation(m_ProgramID, "texOffset");
        }

        #endregion // --- protected implementation ---


        #region --- private implementation ---

        private void LogShaderMsg(string loginfo, string procName)
        {
            string msg = $"Shader '{Name}', {procName} reported: '{loginfo}'.";
            string infolow = loginfo.ToLower();

            if (infolow.Contains("error"))
                TraceLog.Fatal(msg);
            else if (infolow.Contains("warn"))
                TraceLog.Warn(msg);
            else
                TraceLog.Notice(msg);
        }

        ~Shader()
        {
            Dispose(false);
        }

        #endregion // --- private implementation ---
    }
}
