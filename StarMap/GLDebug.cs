using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarMap.SceneObjects;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace StarMap
{
    /*public struct ModelDiffuse_UBOData
    {
        public Matrix4 ModelMatrix; // 4*4*4 +          64 bytes
        public Color4 DiffuseColor; //       + 4*4      16 bytes
        // -----------------------------------------------------
        public const int SizeInBytes = 4*4*4 + 4*4; //  80 bytes
        // =====================================================

        public ModelDiffuse_UBOData(Matrix4 modelMat, Color4 diffuseColor)
        {
            ModelMatrix = modelMat;
            DiffuseColor = diffuseColor;
        }

        public static ModelDiffuse_UBOData Identity
        {
            get
            {
                return new ModelDiffuse_UBOData(Matrix4.Identity, Color4.HotPink);
            }
        }
    }*/

    public struct ProjViewViewport_UBOData
    {
        public Matrix4 ProjectionMatrix;// 4*4*4 +                  64 bytes
        public Matrix4 ViewMatrix;      //       + 4*4*4 +          64 bytes
        public Vector2 ViewportSize;    //               + 4*2       8 bytes
        // --------------------------------------------------------------
        public const int SizeInBytes =     4*4*4 + 4*4*4 + 4*4; // 144 bytes (rounded up to an even % 16)
        // ==============================================================

        public ProjViewViewport_UBOData(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector2 viewportSize)
        {
            ProjectionMatrix = projectionMatrix;
            ViewMatrix = viewMatrix;
            ViewportSize = viewportSize;
        }

        public static ProjViewViewport_UBOData Identity
        {
            get
            {
                return new ProjViewViewport_UBOData(Matrix4.Identity, Matrix4.Identity, new Vector2(640, 480));
            }
        }
    }

#if DEBUG
    /// <summary>
    /// Provides a minimally thin profiling wrapper for all calls to <see cref="GL"/> functions.
    /// </summary>
    /// <remarks>Actually, no such thing occurs. Each call is immediately passed off to <see cref="GL"/>,
    /// but the use of this instances class allows visual studio to profile calls.</remarks>

    public sealed class GLDebug
    {
        private static bool s_IsInitialized = false;

        public GLDebug()
        {
            if (s_IsInitialized)
                return;
            Init();
        }

        private void Init()
        {
            TraceLog.Info($"{nameof(GLDebug)} Initializing GL logging shim...");
            GL.Enable(EnableCap.DebugOutput);
            s_IsInitialized = true;
            DumpLog("(unknown)", "(unknown)");
        }

        public string TryGetErrors()
        {
            StringBuilder sb = new StringBuilder();
            ErrorCode e = GL.GetError();

            while (e != ErrorCode.NoError)
            {
                sb.Append(e.ToString());
                sb.Append(", ");
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 3, 2);
                string ret = sb.ToString();
                TraceLog.Error($"[ERROR] GLDebug reporting the following error(s): {ret}.");
                return ret;
            }   
            else
                return null;
        }

        #region --- 1:1 GL wrappers ---

        public void AttachShader(int program, int shader)
        {
            using (new glock())
                GL.AttachShader(program, shader);
        }

        public void BindBuffer(BufferTarget target, int buffer)
        {
            using (new glock())
                GL.BindBuffer(target, buffer);
        }

        public void BindBufferBase(BufferRangeTarget target, int index, int buffer)
        {
            using (new glock())
                GL.BindBufferBase(target, index, buffer);
        }

        public void BufferSubData<T>(BufferTarget target, IntPtr offset, int size, ref T data) where T : struct
        {
            using (new glock())
                GL.BufferSubData(target, offset, size, ref data);
        }

        public void BindTexture(TextureTarget target, int texture)
        {
            using (new glock())
                GL.BindTexture(target, texture);
        }

        public void BindVertexArray(int array)
        {
            using (new glock())
                GL.BindVertexArray(array);
        }

        public void BlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor)
        {
            using (new glock())
                GL.BlendFunc(sfactor, dfactor);
        }

        public void BufferData(BufferTarget target, int size, IntPtr data, BufferUsageHint usage)
        {
            using (new glock())
                GL.BufferData(target, size, data, usage);
        }

        public void BufferData<T2>(BufferTarget target, int size, ref T2 data, BufferUsageHint usage) where T2 : struct
        {
            using (new glock())
                GL.BufferData(target, size, ref data, usage);
        }

        public void BufferData<T2>(BufferTarget target, int size, T2[] data, BufferUsageHint usage) where T2 : struct
        {
            using (new glock())
                GL.BufferData(target, size, data, usage);
        }

        public void Clear(ClearBufferMask mask)
        {
            using (new glock())
                GL.Clear(mask);
        }

        public void ClearColor(Color color)
        {
            using (new glock())
                GL.ClearColor(color);
        }

        public void CompileShader(int shader)
        {
            using (new glock())
                GL.CompileShader(shader);
        }

        public int CreateProgram()
        {
            using (new glock())
                return GL.CreateProgram();
        }

        public int CreateShader(ShaderType type)
        {
            using (new glock())
                return GL.CreateShader(type);
        }

        public void CreateTextures(TextureTarget target, int n, out int textures)
        {
            using (new glock())
                GL.CreateTextures(target, n, out textures);
        }

        public void DeleteBuffer(int buffers)
        {
            using (new glock())
                GL.DeleteBuffer(buffers);
        }

        public void DeleteProgram(int program)
        {
            using (new glock())
                GL.DeleteProgram(program);
        }

        public void DeleteShader(int shader)
        {
            using (new glock())
                GL.DeleteShader(shader);
        }

        public void DeleteTexture(int textures)
        {
            using (new glock())
                GL.DeleteTexture(textures);
        }

        public void DeleteVertexArray(int arrays)
        {
            using (new glock())
                GL.DeleteVertexArray(arrays);
        }

        public void DetachShader(int program, int shader)
        {
            using (new glock())
                GL.DetachShader(program, shader);
        }

        public void DrawArrays(PrimitiveType mode, int first, int count)
        {
            using (new glock())
                GL.DrawArrays(mode, first, count);
        }

        public void Enable(EnableCap cap)
        {
            using (new glock())
                GL.Enable(cap);
        }

        public void EnableVertexArrayAttrib(int vaobj, int index)
        {
            using (new glock())
                GL.EnableVertexArrayAttrib(vaobj, index);
        }

        public int GenBuffer()
        {
            using (new glock())
                return GL.GenBuffer();
        }

        public int GenVertexArray()
        {
            using (new glock())
                return GL.GenVertexArray();
        }

        public int GetAttribLocation(int program, string name)
        {
            using (new glock())
                return GL.GetAttribLocation(program, name);
        }

        public ErrorCode GetError()
        {
            using (new glock())
                return GL.GetError();
        }

        public string GetProgramInfoLog(int program)
        {
            using (new glock())
                return GL.GetProgramInfoLog(program);
        }

        public string GetShaderInfoLog(int shader)
        {
            using (new glock())
                return GL.GetShaderInfoLog(shader);
        }

        public int GetUniformBlockIndex(int program, string uniformBlockName)
        {
            using (new glock())
                return GL.GetUniformBlockIndex(program, uniformBlockName);
        }

        public int GetUniformLocation(int program, string name)
        {
            using (new glock())
                return GL.GetUniformLocation(program, name);
        }

        public void LineWidth(float width)
        {
            using (new glock())
                GL.LineWidth(width);
        }

        public void LinkProgram(int program)
        {
            using (new glock())
                GL.LinkProgram(program);
        }

        public void NamedBufferStorage<T>(int buffer, int size, T[] data, BufferStorageFlags flags) where T : struct
        {
            using (new glock())
                GL.NamedBufferStorage(buffer, size, data, flags);
        }

        public void PatchParameter(PatchParameterInt pname, int value)
        {
            using (new glock())
                GL.PatchParameter(pname, value);
        }

        public void PointSize(float size)
        {
            using (new glock())
                GL.PointSize(size);
        }

        public void PolygonMode(MaterialFace face, PolygonMode mode)
        {
            using (new glock())
                GL.PolygonMode(face, mode);
        }

        public void ProgramUniform4(int program, int location, Color4 color)
        {
            using (new glock())
                GL.ProgramUniform4(program, location, color);
        }

        public void ProgramUniformMatrix4(int program, int location, bool transpose, ref Matrix4 matrix)
        {
            using (new glock())
                GL.ProgramUniformMatrix4(program, location, transpose, ref matrix);
        }

        public void ShaderSource(int shader, string @string)
        {
            using (new glock())
                GL.ShaderSource(shader, @string);
        }

        public void TextureParameterI(int texture, All pname, ref int @params)
        {
            using (new glock())
                GL.TextureParameterI(texture, pname, ref @params);
        }

        public void TextureStorage2D(int texture, int levels, SizedInternalFormat internalformat, int width, int height)
        {
            using (new glock())
                GL.TextureStorage2D(texture, levels, internalformat, width, height);
        }

        public void TextureSubImage2D<T>(int texture, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, T[] pixels) where T : struct
        {
            using (new glock())
                GL.TextureSubImage2D(texture, level, xoffset, yoffset, width, height, format, type, pixels);
        }

        public void Uniform4(int location, Color4 color)
        {
            using (new glock())
                GL.Uniform4(location, color);
        }

        public void UniformBlockBinding(int program, int uniformBlockIndex, int uniformBlockBinding)
        {
            using (new glock())
                GL.UniformBlockBinding(program, uniformBlockIndex, uniformBlockBinding);
        }

        public void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix)
        {
            using (new glock())
                GL.UniformMatrix4(location, transpose, ref matrix);
        }

        public void UseProgram(int program)
        {
            using (new glock())
                GL.UseProgram(program);
        }

        public void VertexArrayAttribBinding(int vaobj, int attribindex, int bindingindex)
        {
            using (new glock())
                GL.VertexArrayAttribBinding(vaobj, attribindex, bindingindex);
        }

        public void VertexArrayAttribFormat(int vaobj, int attribindex, int size, VertexAttribType type, bool normalized, int relativeoffset)
        {
            using (new glock())
                GL.VertexArrayAttribFormat(vaobj, attribindex, size, type, normalized, relativeoffset);
        }

        public void VertexArrayVertexBuffer(int vaobj, int bindingindex, int buffer, IntPtr offset, int stride)
        {
            using (new glock())
                GL.VertexArrayVertexBuffer(vaobj, bindingindex, buffer, offset, stride);
        }

        public void Viewport(Rectangle rectangle)
        {
            using (new glock())
                GL.Viewport(rectangle);
        }

        #endregion // --- 1:1 GL wrappers ---

        #region --- private implementation ---

        private static void DumpLog(string parentDesc, string calledDesc)
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
                string output = $"{nameof(GLDebug)} {parentDesc}, {calledDesc}:  {id} - {sb.ToString()}";
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
                        TraceLog.Notice(lastLogMsg);
                    else if (severity == DebugSeverity.DebugSeverityMedium)
                        TraceLog.Info(lastLogMsg);
                    else
                        TraceLog.Debug(lastLogMsg);
                }
                else
                {
                    logRepeat++;
                }
            }
        }

        private class glock : IDisposable
        {
            public string MethodName { get; private set; }

            public StackFrame ParentFrame { get; private set; } = new StackFrame(2);

            public string ParentInfo { get; private set; }

            public glock([CallerMemberName] string methodName = null)
            {
                MethodName = methodName;
                ParentInfo = $"{ParentFrame.GetMethod().DeclaringType.Name}:{ParentFrame.GetMethod().Name}";
            }

            public void Dispose()
            {
                DumpLog(ParentInfo, MethodName);
                MethodName = null;
                ParentFrame = null;
                ParentInfo = null;
                GC.SuppressFinalize(this);
            }
        }

        #endregion // --- private implementation ---
    }
#endif
}
