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
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Runtime.CompilerServices;
#endregion // --- using ... ---

namespace StarMap
{
#if GLDEBUG
    /// <summary>
    /// Provides a thin logging wrapper for all calls to <see cref="GL"/> functions.
    /// It also aids in CPU/GPU profiling, by giving Visual Studio something to show
    /// for CPU time, minus the hassles of using OpenTK.pdb.
    /// </summary>
    /// <example>
    /// <code>using OpenTK.Graphics.OpenGL4; // Still needed for type, etc, references
    /// 
    /// #if GLDEBUG
    /// using gld = StarMap.GLDebug;
    /// #else
    /// using gld = OpenTK.Graphics.OpenGL.GL;
    /// #endif
    /// 
    /// namespace GLDebugExample1
    /// {
    ///     public class FooForm : Form
    ///     {
    ///         private int m_BufferId;
    /// 
    ///         public FooForm()
    ///         {
    ///             InitializeComponent();
    ///         }
    /// 
    ///         public override void Dispose(bool disposing)
    ///         {
    ///             if (disposing)
    ///             {
    ///                 components?.Dispose();
    ///                 gld.DeleteBuffer(m_BufferId);
    ///             }
    ///             base.Dispose(disposing);
    ///         }
    ///         
    ///         phrogGLControl1_Load(object sender, ...)
    ///         {
    ///             m_BufferId = gld.GenBuffer();
    ///             phrogGLControl1.Load -= phrogGLControl1_Load;
    ///             phrogGLControl1.Paint += phrogGLControl1_Paint;
    ///         }
    ///         
    ///         phrogGLControl1_Paint(object sender, EventArgs e)
    ///         {
    ///             throw new NotImplementedException();
    /// 
    ///             gld.ClearColor(Color4.Black);
    ///             gld.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    // TODO: Inject moar profiling somehow.
    public static class GLDebug
    {
        public static void Attach()
        {
            if (!IsAttached)
            {
                IsAttached = !IsAttached;

                string moreInfo = string.Empty;
                if (!Program.MainFrm.phrogGLControl1.ContextFlags.HasFlag(GraphicsContextFlags.Debug))
                    moreInfo = $" Note: {nameof(Program)}.{nameof(Program.MainFrm)}.{nameof(Program.MainFrm.phrogGLControl1)} lacks debugging context; available information will be lacking.";
                TraceLog.Info($"{nameof(GLDebug)} Attaching GL logging shim...{moreInfo}");
                GL.Enable(EnableCap.DebugOutput);
                DumpLog("(pre-existing log information)", "(unknown)");
            }
        }

        public static void Detach()
        {
            if (IsAttached)
            {
                IsAttached = !IsAttached;
                TraceLog.Info($"{nameof(GLDebug)} Detaching GL logging shim...");
                GL.Disable(EnableCap.DebugOutput);
            }
        }

        public static string TryGetErrors()
        {
            const string separator = ", ";
            StringBuilder sb = new StringBuilder();
            ErrorCode e = GL.GetError();

            while (e != ErrorCode.NoError)
            {
                sb.Append(e.ToString());
                sb.Append(separator);
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1 - separator.Length, separator.Length);
                string ret = sb.ToString();
                TraceLog.Error($"{nameof(GLDebug)} reporting the following error(s): {ret}.");
                return ret;
            }   
            else
                return null;
        }

        #region --- 1:1 GL wrappers ---

        public static void AttachShader(int program, int shader)
        {
            using (new Glock())
                GL.AttachShader(program, shader);
        }

        public static void BindBuffer(BufferTarget target, int buffer)
        {
            using (new Glock())
                GL.BindBuffer(target, buffer);
        }

        public static void BindBufferBase(BufferRangeTarget target, int index, int buffer)
        {
            using (new Glock())
                GL.BindBufferBase(target, index, buffer);
        }

        public static void BufferSubData<T>(BufferTarget target, IntPtr offset, int size, ref T data) where T : struct
        {
            using (new Glock())
                GL.BufferSubData(target, offset, size, ref data);
        }

        public static void BindTexture(TextureTarget target, int texture)
        {
            using (new Glock())
                GL.BindTexture(target, texture);
        }

        public static void BindVertexArray(int array)
        {
            using (new Glock())
                GL.BindVertexArray(array);
        }

        public static void BlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor)
        {
            using (new Glock())
                GL.BlendFunc(sfactor, dfactor);
        }

        public static void BufferData(BufferTarget target, int size, IntPtr data, BufferUsageHint usage)
        {
            using (new Glock())
                GL.BufferData(target, size, data, usage);
        }

        public static void BufferData<T2>(BufferTarget target, int size, ref T2 data, BufferUsageHint usage) where T2 : struct
        {
            using (new Glock())
                GL.BufferData(target, size, ref data, usage);
        }

        public static void BufferData<T2>(BufferTarget target, int size, T2[] data, BufferUsageHint usage) where T2 : struct
        {
            using (new Glock())
                GL.BufferData(target, size, data, usage);
        }

        public static void Clear(ClearBufferMask mask)
        {
            using (new Glock())
                GL.Clear(mask);
        }

        public static void ClearColor(Color color)
        {
            using (new Glock())
                GL.ClearColor(color);
        }

        public static void CompileShader(int shader)
        {
            using (new Glock())
                GL.CompileShader(shader);
        }

        public static int CreateProgram()
        {
            using (new Glock())
                return GL.CreateProgram();
        }

        public static int CreateShader(ShaderType type)
        {
            using (new Glock())
                return GL.CreateShader(type);
        }

        public static void CreateTextures(TextureTarget target, int n, out int textures)
        {
            using (new Glock())
                GL.CreateTextures(target, n, out textures);
        }

        public static void DeleteBuffer(int buffers)
        {
            using (new Glock())
                GL.DeleteBuffer(buffers);
        }

        public static void DeleteProgram(int program)
        {
            using (new Glock())
                GL.DeleteProgram(program);
        }

        public static void DeleteShader(int shader)
        {
            using (new Glock())
                GL.DeleteShader(shader);
        }

        public static void DeleteTexture(int textures)
        {
            using (new Glock())
                GL.DeleteTexture(textures);
        }

        public static void DeleteVertexArray(int arrays)
        {
            using (new Glock())
                GL.DeleteVertexArray(arrays);
        }

        public static void DetachShader(int program, int shader)
        {
            using (new Glock())
                GL.DetachShader(program, shader);
        }

        public static void DrawArrays(PrimitiveType mode, int first, int count)
        {
            using (new Glock())
                GL.DrawArrays(mode, first, count);
        }

        public static void Enable(EnableCap cap)
        {
            using (new Glock())
                GL.Enable(cap);
        }

        public static void EnableVertexArrayAttrib(int vaobj, int index)
        {
            using (new Glock())
                GL.EnableVertexArrayAttrib(vaobj, index);
        }

        public static int GenBuffer()
        {
            using (new Glock())
                return GL.GenBuffer();
        }

        public static int GenVertexArray()
        {
            using (new Glock())
                return GL.GenVertexArray();
        }

        public static int GetAttribLocation(int program, string name)
        {
            using (new Glock())
                return GL.GetAttribLocation(program, name);
        }

        public static ErrorCode GetError()
        {
            using (new Glock())
                return GL.GetError();
        }

        public static string GetProgramInfoLog(int program)
        {
            using (new Glock())
                return GL.GetProgramInfoLog(program);
        }

        public static string GetShaderInfoLog(int shader)
        {
            using (new Glock())
                return GL.GetShaderInfoLog(shader);
        }

        public static int GetUniformBlockIndex(int program, string uniformBlockName)
        {
            using (new Glock())
                return GL.GetUniformBlockIndex(program, uniformBlockName);
        }

        public static int GetUniformLocation(int program, string name)
        {
            using (new Glock())
                return GL.GetUniformLocation(program, name);
        }

        public static void LineWidth(float width)
        {
            using (new Glock())
                GL.LineWidth(width);
        }

        public static void LinkProgram(int program)
        {
            using (new Glock())
                GL.LinkProgram(program);
        }

        public static void NamedBufferStorage<T>(int buffer, int size, T[] data, BufferStorageFlags flags) where T : struct
        {
            using (new Glock())
                GL.NamedBufferStorage(buffer, size, data, flags);
        }

        public static void PatchParameter(PatchParameterInt pname, int value)
        {
            using (new Glock())
                GL.PatchParameter(pname, value);
        }

        public static void PointSize(float size)
        {
            using (new Glock())
                GL.PointSize(size);
        }

        public static void PolygonMode(MaterialFace face, PolygonMode mode)
        {
            using (new Glock())
                GL.PolygonMode(face, mode);
        }

        public static void ProgramUniform4(int program, int location, Color4 color)
        {
            using (new Glock())
                GL.ProgramUniform4(program, location, color);
        }

        public static void ProgramUniformMatrix4(int program, int location, bool transpose, ref Matrix4 matrix)
        {
            using (new Glock())
                GL.ProgramUniformMatrix4(program, location, transpose, ref matrix);
        }

        public static void ShaderSource(int shader, string @string)
        {
            using (new Glock())
                GL.ShaderSource(shader, @string);
        }

        public static void TextureParameterI(int texture, All pname, ref int @params)
        {
            using (new Glock())
                GL.TextureParameterI(texture, pname, ref @params);
        }

        public static void TextureStorage2D(int texture, int levels, SizedInternalFormat internalformat, int width, int height)
        {
            using (new Glock())
                GL.TextureStorage2D(texture, levels, internalformat, width, height);
        }

        public static void TextureSubImage2D<T>(int texture, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, T[] pixels) where T : struct
        {
            using (new Glock())
                GL.TextureSubImage2D(texture, level, xoffset, yoffset, width, height, format, type, pixels);
        }

        public static void Uniform4(int location, Color4 color)
        {
            using (new Glock())
                GL.Uniform4(location, color);
        }

        public static void UniformBlockBinding(int program, int uniformBlockIndex, int uniformBlockBinding)
        {
            using (new Glock())
                GL.UniformBlockBinding(program, uniformBlockIndex, uniformBlockBinding);
        }

        public static void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix)
        {
            using (new Glock())
                GL.UniformMatrix4(location, transpose, ref matrix);
        }

        public static void UseProgram(int program)
        {
            using (new Glock())
                GL.UseProgram(program);
        }

        public static void VertexArrayAttribBinding(int vaobj, int attribindex, int bindingindex)
        {
            using (new Glock())
                GL.VertexArrayAttribBinding(vaobj, attribindex, bindingindex);
        }

        public static void VertexArrayAttribFormat(int vaobj, int attribindex, int size, VertexAttribType type, bool normalized, int relativeoffset)
        {
            using (new Glock())
                GL.VertexArrayAttribFormat(vaobj, attribindex, size, type, normalized, relativeoffset);
        }

        public static void VertexArrayVertexBuffer(int vaobj, int bindingindex, int buffer, IntPtr offset, int stride)
        {
            using (new Glock())
                GL.VertexArrayVertexBuffer(vaobj, bindingindex, buffer, offset, stride);
        }

        public static void Viewport(Rectangle rectangle)
        {
            using (new Glock())
                GL.Viewport(rectangle);
        }

        #endregion // --- 1:1 GL wrappers ---

        #region --- private implementation ---

        private static bool IsAttached = false;

        private static void DumpLog(string callerDesc, string calledDesc)
        {
            int id = -1;
            int len = 0;

            DebugSeverity severity;
            DebugSource source;
            DebugType type;
            StringBuilder sb = new StringBuilder(512);

            while (GL.GetDebugMessageLog(1, 512, out source, out type, out id, out severity, out len, sb) == 1)
            {
                string output = $"{nameof(GLDebug)} {callerDesc}, {calledDesc}:  {id} - {sb.ToString()}";
                if (type == DebugType.DebugTypeError)
                    TraceLog.Error(output);
                else if (type == DebugType.DebugTypeDeprecatedBehavior || type == DebugType.DebugTypeUndefinedBehavior || severity == DebugSeverity.DebugSeverityHigh)
                    TraceLog.Warn(output);
                else if (severity == DebugSeverity.DebugSeverityMedium)
                    TraceLog.Notice(output);
                else
                    TraceLog.Debug(output);
            }
        }

        // Yes, they're decent guns, but I want an actual trigger safety as well as a magazine safety.
        // That said, this name is way cooler than GLDebugLogInvokeWrapper or something similar.
        private class Glock : IDisposable
        {
            public string MethodName { get; private set; }

            public StackFrame CallingFrame { get; private set; } = new StackFrame(2);

            public string ParentInfo { get; private set; }

            public Glock([CallerMemberName] string methodName = null)
            {
                MethodName = methodName;
                ParentInfo = $"{CallingFrame.GetMethod().DeclaringType.Name}:{CallingFrame.GetMethod().Name}";
            }

            public void Dispose()
            {
                DumpLog(ParentInfo, MethodName);
                MethodName = null;
                CallingFrame = null;
                ParentInfo = null;
                GC.SuppressFinalize(this);
            }
        }

        #endregion // --- private implementation ---
    }
#endif
}
