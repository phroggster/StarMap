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
using StarMap.Shaders;
using System;
using System.Drawing;

#if DEBUG
using gldebug = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Models
{
    public class TexturedRenderable : Model
    {
        private readonly int _texture;

        public TexturedRenderable(TexturedVertex[] vertices, Shader shader, Bitmap texture)
            : base(shader, vertices.Length)
        {
            gld.NamedBufferStorage(m_gl_vboId, TexturedVertex.Size * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);

            int attrib = 0;
            // attrib 0: location
            gld.VertexArrayAttribBinding(m_gl_vaoId, attrib, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, attrib);
            gld.VertexArrayAttribFormat(m_gl_vaoId, attrib, TexturedVertex.PositionSize, VertexAttribType.Float, false, TexturedVertex.PositionOffsetBytes);

            attrib++;
            // attrib 1: texture coordinate
            gld.VertexArrayAttribBinding(m_gl_vaoId, attrib, 0);
            gld.EnableVertexArrayAttrib(m_gl_vaoId, attrib);
            gld.VertexArrayAttribFormat(m_gl_vaoId, attrib, TexturedVertex.TexCoordAttribSize, VertexAttribType.Float, false, TexturedVertex.TexCoordAttribOffsetBytes);

            // link the array and the buffer
            gld.VertexArrayVertexBuffer(m_gl_vaoId, 0, m_gl_vboId, IntPtr.Zero, TexturedVertex.Size);

            // Texture data
            _texture = LoadTexture(texture);
            SetFiltering(All.Linear);
        }

        public override void Bind()
        {
            base.Bind();
            gld.BindTexture(TextureTarget.ProxyTexture2D, _texture);
        }

        public void SetFiltering(All filter)
        {
            var textureMinFilter = (int)filter;
            gld.TextureParameterI(_texture, All.TextureMinFilter, ref textureMinFilter);
            var textureMagFilter = (int)filter;
            gld.TextureParameterI(_texture, All.TextureMagFilter, ref textureMagFilter);
        }

        /// <summary>
        /// Load an RGBA bitmap.
        /// </summary>
        private int LoadTexture(Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height, index = 0, ret;
            float[] imgData = new float[bmp.Width * bmp.Height * 4];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    imgData[index++] = pixel.R / 255f;
                    imgData[index++] = pixel.G / 255f;
                    imgData[index++] = pixel.B / 255f;
                    imgData[index++] = pixel.A / 255f;
                }
            }

            gld.CreateTextures(TextureTarget.Texture2D, 1, out ret);
            gld.TextureStorage2D(ret, 1, SizedInternalFormat.Rgba32f, width, height);
            gld.BindTexture(TextureTarget.Texture2D, ret);
            gld.TextureSubImage2D(ret, 0, 0, 0, width, height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.Float, imgData);

            return ret;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                gld.DeleteTexture(_texture);
                base.Dispose(disposing);
            }
        }
    }
}
