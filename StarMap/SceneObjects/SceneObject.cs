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
using StarMap.Cameras;
using StarMap.Models;
using StarMap.Shaders;
using System;
using System.Diagnostics;

#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif
#endregion // --- using ... ---

namespace StarMap.SceneObjects
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class SceneObject : ISceneObject, IIsDisposed
    {
        #region --- protected SceneObject(Model, Vector3, Quaternion, Vector3) ---

        protected SceneObject(Model model, Vector3 position, Quaternion orientation, Vector3 scale)
        {
            Model = model;
            _position = position;
            _orientation = orientation;
            _scale = scale;

            gl_UBO_ModelMatrix = gld.GenBuffer();
            gld.BindBuffer(BufferTarget.UniformBuffer, gl_UBO_ModelMatrix);
            gld.BufferData(BufferTarget.UniformBuffer, 4 * 4 * 4, IntPtr.Zero, BufferUsageHint.StaticDraw);
            gld.BindBuffer(BufferTarget.UniformBuffer, 0);

            int bindPost = BindingPosts.GenBindingPost();
            gld.BindBufferBase(BufferRangeTarget.UniformBuffer, bindPost, gl_UBO_ModelMatrix);
            gld.UniformBlockBinding(model.Shader.ProgramID, model.Shader.Model, bindPost);
        }

        #endregion // --- protected SceneObject(Model, Vector3, Quaternion, Vector3) ---

        #region --- public interfaces ---

        #region --- ISceneObject interface ---

        public Model Model { get; set; }
        public string Name { get; set; }

        public Quaternion Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    _IsModelMatrixDirty = true;
                }
            }
        }
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    _IsModelMatrixDirty = true;
                }
            }
        }
        public Vector3 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    _IsModelMatrixDirty = true;
                }
            }
        }

        public void Render()
        {
            OnRender();
        }

        public void Update(double delta)
        {
            OnUpdate(delta);
        }

        #endregion // --- ISceneObject interface ---

        #region --- IIsDisposed interface ---

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // --- IIsDisposed interface ---

        #endregion // --- public interfaces ---

        #region --- protected implementation ---

        protected virtual string DebuggerDisplay
        {
            get
            {
                return string.Format("{0}: POS {1}, ROT {2}, Scale {3}", Name, _position.ToString(), _orientation.ToString(), _scale.ToString());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                if (gl_UBO_ModelMatrix >= 0)
                    gld.DeleteBuffer(gl_UBO_ModelMatrix);

                if (!disposing)
                    TraceLog.Warn($"{GetType().Name} leaked! Did you forget to call `Dispose()`?");
            }
        }

        protected virtual void OnRender()
        {
            Model.Bind();
            Model.Render();
        }

        protected virtual void OnUpdate(double delta)
        {
            if (_IsModelMatrixDirty)
            {
                _IsModelMatrixDirty = false;
                _modelMatrix = Matrix4.CreateFromQuaternion(_orientation) * Matrix4.CreateScale(_scale) * Matrix4.CreateTranslation(_position);
                gld.BindBuffer(BufferTarget.UniformBuffer, gl_UBO_ModelMatrix);
                gld.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, 4*4*4, ref _modelMatrix);
                gld.BindBuffer(BufferTarget.UniformBuffer, 0);
            }
        }

        #endregion // --- protected implementation ---

        #region --- private implementation ---

        private bool _IsModelMatrixDirty = true;
        private Matrix4 _modelMatrix = Matrix4.Identity;

        private Quaternion _orientation;
        private Vector3 _position;
        private Vector3 _scale;

        private int gl_UBO_ModelMatrix = -1;

        ~SceneObject()
        {
            Dispose(false);
        }

        #endregion // --- private implementation ---
    }
}
