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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace StarMap.Shaders
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class Shader : IDisposable
    {
        // layout(location = 0) in vec4 position;
        public virtual int AttribPosition { get { return 0; } }
        // layout(location = 1) in vec4 color;
        public virtual int AttribColor { get { return 1; } }
        // layout(location = 20) uniform mat4 projection;
        public virtual int UniformProjection { get { return 20; } }
        // layout(location = 21) uniform mat4 view;
        public virtual int UniformView { get { return 21; } }
        // layout(location = 22) uniform mat4 model;
        public virtual int UniformModel { get { return 22; } }

        public string DebuggerDisplay { get { return string.Format("Shader {0}: {1}, {2}ID {3}", Name, IsReady ? "Ready" : "Not ready", IsDisposed ? "Disposed, " : "", Program); } }
        public bool IsDisposed { get; private set; } = false;
        public bool IsReady { get; private set; } = false;
        public abstract string Name { get; }
        public int ProgramID
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().Name);
                return Program;
            }
        }

        protected readonly int Program;
        protected readonly List<int> SubShaders = new List<int>();

        public Shader()
        {
            Program = GL.CreateProgram();
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
            string loadStyle = background ? "Background loading" : "Loading";
            Trace.WriteLine($"[DEBUG] {loadStyle} shader {Name}.");
            Load(Assembly.GetExecutingAssembly(), "StarMap.Shaders." + Name);
            Link();
            IsReady = true;
            Trace.WriteLine($"[DEBUG] Shader {Name} loaded.");
        }

        protected virtual void Link()
        {
            foreach (var shader in SubShaders)
                GL.AttachShader(Program, shader);
            GL.LinkProgram(Program);
            var info = GL.GetProgramInfoLog(Program);
            if (!string.IsNullOrWhiteSpace(info))
            {
                LogErr(info, "LinkProgram");
            }
            foreach (var shader in SubShaders)
            {
                GL.DetachShader(Program, shader);
                GL.DeleteShader(shader);
            }
        }

        protected abstract void Load(Assembly ass, string path);

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsReady = false;
                if (disposing)
                {
                    SubShaders.Clear();
                }
                GL.DeleteProgram(Program);
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
    }
}
