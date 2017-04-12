using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Runtime.CompilerServices;

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
        private static bool IsAttached = false;

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
            using (new glock())
                GL.AttachShader(program, shader);
        }

        public static void BindBuffer(BufferTarget target, int buffer)
        {
            using (new glock())
                GL.BindBuffer(target, buffer);
        }

        public static void BindBufferBase(BufferRangeTarget target, int index, int buffer)
        {
            using (new glock())
                GL.BindBufferBase(target, index, buffer);
        }

        public static void BufferSubData<T>(BufferTarget target, IntPtr offset, int size, ref T data) where T : struct
        {
            using (new glock())
                GL.BufferSubData(target, offset, size, ref data);
        }

        public static void BindTexture(TextureTarget target, int texture)
        {
            using (new glock())
                GL.BindTexture(target, texture);
        }

        public static void BindVertexArray(int array)
        {
            using (new glock())
                GL.BindVertexArray(array);
        }

        public static void BlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor)
        {
            using (new glock())
                GL.BlendFunc(sfactor, dfactor);
        }

        public static void BufferData(BufferTarget target, int size, IntPtr data, BufferUsageHint usage)
        {
            using (new glock())
                GL.BufferData(target, size, data, usage);
        }

        public static void BufferData<T2>(BufferTarget target, int size, ref T2 data, BufferUsageHint usage) where T2 : struct
        {
            using (new glock())
                GL.BufferData(target, size, ref data, usage);
        }

        public static void BufferData<T2>(BufferTarget target, int size, T2[] data, BufferUsageHint usage) where T2 : struct
        {
            using (new glock())
                GL.BufferData(target, size, data, usage);
        }

        public static void Clear(ClearBufferMask mask)
        {
            using (new glock())
                GL.Clear(mask);
        }

        public static void ClearColor(Color color)
        {
            using (new glock())
                GL.ClearColor(color);
        }

        public static void CompileShader(int shader)
        {
            using (new glock())
                GL.CompileShader(shader);
        }

        public static int CreateProgram()
        {
            using (new glock())
                return GL.CreateProgram();
        }

        public static int CreateShader(ShaderType type)
        {
            using (new glock())
                return GL.CreateShader(type);
        }

        public static void CreateTextures(TextureTarget target, int n, out int textures)
        {
            using (new glock())
                GL.CreateTextures(target, n, out textures);
        }

        public static void DeleteBuffer(int buffers)
        {
            using (new glock())
                GL.DeleteBuffer(buffers);
        }

        public static void DeleteProgram(int program)
        {
            using (new glock())
                GL.DeleteProgram(program);
        }

        public static void DeleteShader(int shader)
        {
            using (new glock())
                GL.DeleteShader(shader);
        }

        public static void DeleteTexture(int textures)
        {
            using (new glock())
                GL.DeleteTexture(textures);
        }

        public static void DeleteVertexArray(int arrays)
        {
            using (new glock())
                GL.DeleteVertexArray(arrays);
        }

        public static void DetachShader(int program, int shader)
        {
            using (new glock())
                GL.DetachShader(program, shader);
        }

        public static void DrawArrays(PrimitiveType mode, int first, int count)
        {
            using (new glock())
                GL.DrawArrays(mode, first, count);
        }

        public static void Enable(EnableCap cap)
        {
            using (new glock())
                GL.Enable(cap);
        }

        public static void EnableVertexArrayAttrib(int vaobj, int index)
        {
            using (new glock())
                GL.EnableVertexArrayAttrib(vaobj, index);
        }

        public static int GenBuffer()
        {
            using (new glock())
                return GL.GenBuffer();
        }

        public static int GenVertexArray()
        {
            using (new glock())
                return GL.GenVertexArray();
        }

        public static int GetAttribLocation(int program, string name)
        {
            using (new glock())
                return GL.GetAttribLocation(program, name);
        }

        public static ErrorCode GetError()
        {
            using (new glock())
                return GL.GetError();
        }

        public static string GetProgramInfoLog(int program)
        {
            using (new glock())
                return GL.GetProgramInfoLog(program);
        }

        public static string GetShaderInfoLog(int shader)
        {
            using (new glock())
                return GL.GetShaderInfoLog(shader);
        }

        public static int GetUniformBlockIndex(int program, string uniformBlockName)
        {
            using (new glock())
                return GL.GetUniformBlockIndex(program, uniformBlockName);
        }

        public static int GetUniformLocation(int program, string name)
        {
            using (new glock())
                return GL.GetUniformLocation(program, name);
        }

        public static void LineWidth(float width)
        {
            using (new glock())
                GL.LineWidth(width);
        }

        public static void LinkProgram(int program)
        {
            using (new glock())
                GL.LinkProgram(program);
        }

        public static void NamedBufferStorage<T>(int buffer, int size, T[] data, BufferStorageFlags flags) where T : struct
        {
            using (new glock())
                GL.NamedBufferStorage(buffer, size, data, flags);
        }

        public static void PatchParameter(PatchParameterInt pname, int value)
        {
            using (new glock())
                GL.PatchParameter(pname, value);
        }

        public static void PointSize(float size)
        {
            using (new glock())
                GL.PointSize(size);
        }

        public static void PolygonMode(MaterialFace face, PolygonMode mode)
        {
            using (new glock())
                GL.PolygonMode(face, mode);
        }

        public static void ProgramUniform4(int program, int location, Color4 color)
        {
            using (new glock())
                GL.ProgramUniform4(program, location, color);
        }

        public static void ProgramUniformMatrix4(int program, int location, bool transpose, ref Matrix4 matrix)
        {
            using (new glock())
                GL.ProgramUniformMatrix4(program, location, transpose, ref matrix);
        }

        public static void ShaderSource(int shader, string @string)
        {
            using (new glock())
                GL.ShaderSource(shader, @string);
        }

        public static void TextureParameterI(int texture, All pname, ref int @params)
        {
            using (new glock())
                GL.TextureParameterI(texture, pname, ref @params);
        }

        public static void TextureStorage2D(int texture, int levels, SizedInternalFormat internalformat, int width, int height)
        {
            using (new glock())
                GL.TextureStorage2D(texture, levels, internalformat, width, height);
        }

        public static void TextureSubImage2D<T>(int texture, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, T[] pixels) where T : struct
        {
            using (new glock())
                GL.TextureSubImage2D(texture, level, xoffset, yoffset, width, height, format, type, pixels);
        }

        public static void Uniform4(int location, Color4 color)
        {
            using (new glock())
                GL.Uniform4(location, color);
        }

        public static void UniformBlockBinding(int program, int uniformBlockIndex, int uniformBlockBinding)
        {
            using (new glock())
                GL.UniformBlockBinding(program, uniformBlockIndex, uniformBlockBinding);
        }

        public static void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix)
        {
            using (new glock())
                GL.UniformMatrix4(location, transpose, ref matrix);
        }

        public static void UseProgram(int program)
        {
            using (new glock())
                GL.UseProgram(program);
        }

        public static void VertexArrayAttribBinding(int vaobj, int attribindex, int bindingindex)
        {
            using (new glock())
                GL.VertexArrayAttribBinding(vaobj, attribindex, bindingindex);
        }

        public static void VertexArrayAttribFormat(int vaobj, int attribindex, int size, VertexAttribType type, bool normalized, int relativeoffset)
        {
            using (new glock())
                GL.VertexArrayAttribFormat(vaobj, attribindex, size, type, normalized, relativeoffset);
        }

        public static void VertexArrayVertexBuffer(int vaobj, int bindingindex, int buffer, IntPtr offset, int stride)
        {
            using (new glock())
                GL.VertexArrayVertexBuffer(vaobj, bindingindex, buffer, offset, stride);
        }

        public static void Viewport(Rectangle rectangle)
        {
            using (new glock())
                GL.Viewport(rectangle);
        }

        #endregion // --- 1:1 GL wrappers ---

        #region --- private implementation ---

        private static void DumpLog(string callerDesc, string calledDesc)
        {
            int id = -1;
            int len = 0;
            int logRepeat = 0;

            DebugSeverity severity;
            DebugSource source;
            DebugType type;
            StringBuilder sb = new StringBuilder(512);
            string lastLogMsg = string.Empty;

            while (GL.GetDebugMessageLog(1, 512, out source, out type, out id, out severity, out len, sb) == 1)
            {
                string output = $"{nameof(GLDebug)} {callerDesc}, {calledDesc}:  {id} - {sb.ToString()}";
                if (output.CompareTo(lastLogMsg) != 0)
                {
                    if (logRepeat > 0)
                    {
                        TraceLog.Info($"{nameof(GLDebug)} Above message repeated {logRepeat} times.");
                        logRepeat = 0;
                    }
                    lastLogMsg = output.ToString();
                    if (type == DebugType.DebugTypeError)
                        TraceLog.Error(lastLogMsg);
                    else if (type == DebugType.DebugTypeDeprecatedBehavior || type == DebugType.DebugTypeUndefinedBehavior || severity == DebugSeverity.DebugSeverityHigh)
                        TraceLog.Warn(lastLogMsg);
                    else if (severity == DebugSeverity.DebugSeverityNotification)
                        TraceLog.Info(lastLogMsg);
                    else if (severity == DebugSeverity.DebugSeverityMedium)
                        TraceLog.Debug(lastLogMsg);
                    else
                        TraceLog.Debug(lastLogMsg);
                }
                else
                {
                    logRepeat++;
                }
            }
        }

        // Yes, they're decent guns, but I want an actual trigger safety as well as a magazine safety.
        // That said, this name is way cooler than GLDebugLogInvokeWrapper or something similar.
        private class glock : IDisposable
        {
            public string MethodName { get; private set; }

            public StackFrame CallingFrame { get; private set; } = new StackFrame(2);

            public string ParentInfo { get; private set; }

            public glock([CallerMemberName] string methodName = null)
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
