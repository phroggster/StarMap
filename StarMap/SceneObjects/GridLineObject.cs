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
using OpenTK.Graphics.OpenGL4;
using StarMap.Cameras;
using StarMap.Models;
using StarMap.Shaders;
using System;
using System.Drawing;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif
#endregion // --- using ... ---

namespace StarMap.SceneObjects
{
    /// <summary>
    /// <see cref="GridLinesObject"/> is a little more hands-on than most <see cref="SceneObject"/>s.
    /// </summary>
    public class GridLinesObject : SceneObject
    {
        private GridLineData m_GridLineModelData = GridLineData.Identity;
        private bool _colourChanged = false;
        private int gl_UBO_GridLineDataBuffer = -1;

        public GridLinesObject(GridLinesModel model) : base(model, Vector3.Zero, Quaternion.Identity, Vector3.One)
        {
            Name = nameof(GridLinesObject);

            Config.Instance.GridLineColourChanged += Config_GridLineColourChanged;
            Config.Instance.FineGridLineColourChanged += Config_FineGridLineColourChanged;

            Shader s = model.Shader;
            gl_UBO_GridLineDataBuffer = gld.GenBuffer();
            gld.BindBuffer(BufferTarget.UniformBuffer, gl_UBO_GridLineDataBuffer);
            gld.BufferData(BufferTarget.UniformBuffer, GridLineData.SizeInBytes, ref m_GridLineModelData, BufferUsageHint.StaticDraw);
            gld.BindBuffer(BufferTarget.UniformBuffer, 0);

            int bindPost = BindingPosts.GenBindingPost();
            gld.BindBufferBase(BufferRangeTarget.UniformBuffer, bindPost, gl_UBO_GridLineDataBuffer);
            gld.UniformBlockBinding(s.ProgramID, s.GridLineData, bindPost);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (gl_UBO_GridLineDataBuffer >= 0)
                    gld.DeleteBuffer(gl_UBO_GridLineDataBuffer);
                if (disposing)
                {
                    Config.Instance.GridLineColourChanged -= Config_GridLineColourChanged;
                    Config.Instance.FineGridLineColourChanged -= Config_FineGridLineColourChanged;
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnUpdate(double delta)
        {
            base.OnUpdate(delta);

            if (_colourChanged)
            {
                _colourChanged = false;
                gld.BindBuffer(BufferTarget.UniformBuffer, gl_UBO_GridLineDataBuffer);
                gld.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, GridLineData.SizeInBytes, ref m_GridLineModelData);
                gld.BindBuffer(BufferTarget.UniformBuffer, 0);
            }
        }

        private void Config_GridLineColourChanged(object sender, ColorEventArgs e)
        {
            m_GridLineModelData.CoarseColor = e.Colour;
            _colourChanged = true;
        }

        private void Config_FineGridLineColourChanged(object sender, ColorEventArgs e)
        {
            m_GridLineModelData.FineColor = e.Colour;
            _colourChanged = true;
        }
    }
}
