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

namespace StarMap.Renderables
{
    public class TexturedRenderable : ARenderable
    {
        private readonly int _texture;

        public TexturedRenderable(TexturedVertex[] vertices, Shader shader, Bitmap texture)
            : base(shader, vertices.Length)
        {
            GL.NamedBufferStorage(Buffer, TexturedVertex.Size * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);

            // attrib 0: location (4 floats, no offset)
            GL.VertexArrayAttribBinding(VertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 0);
            GL.VertexArrayAttribFormat(VertexArray, 0, 4, VertexAttribType.Float, false, 0);

            // attrib 1: texture coordinate (2 floats, 16 byte offset
            GL.VertexArrayAttribBinding(VertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 1);
            GL.VertexArrayAttribFormat(VertexArray, 1, 2, VertexAttribType.Float, false, 16);

            // link the array and the buffer
            GL.VertexArrayVertexBuffer(VertexArray, 0, Buffer, IntPtr.Zero, TexturedVertex.Size);

            // Texture data
            _texture = LoadTexture(texture);
            SetFiltering(All.Linear);
        }

        public override void Bind()
        {
            base.Bind();
            GL.BindTexture(TextureTarget.ProxyTexture2D, _texture);
        }

        public void SetFiltering(All filter)
        {
            var textureMinFilter = (int)filter;
            GL.TextureParameterI(_texture, All.TextureMinFilter, ref textureMinFilter);
            var textureMagFilter = (int)filter;
            GL.TextureParameterI(_texture, All.TextureMagFilter, ref textureMagFilter);
        }

        /// <summary>
        /// Load an RGBA bitmap.
        /// </summary>
        private static int LoadTexture(Bitmap bmp)
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

            GL.CreateTextures(TextureTarget.Texture2D, 1, out ret);
            GL.TextureStorage2D(ret, 1, SizedInternalFormat.Rgba32f, width, height);
            GL.BindTexture(TextureTarget.Texture2D, ret);
            GL.TextureSubImage2D(ret, 0, 0, 0, width, height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.Float, imgData);

            return ret;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                GL.DeleteTexture(_texture);
            base.Dispose(disposing);
        }
    }
}
