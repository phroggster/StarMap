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

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using StarMap.Cameras;
using StarMap.Models;
using System;
using System.Diagnostics;

#if DEBUG
using gldebug = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.SceneObjects
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class SceneObject : ISceneObject
    {
        public SceneObject(Model model, Vector3 position, Quaternion rotation, Vector3 scale, string name = null)
        {
            if (!string.IsNullOrEmpty(name))
                Name = name;
            Model = model;
            _position = position;
            _orientation = rotation;
            _scale = scale;
        }

        #region --- ISceneObject interface ---

        public bool IsDisposed { get; private set; }
        public virtual Model Model { get; set; }
        public virtual string Name { get; set; } = nameof(SceneObject);

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
                    _isModelMatDirty = true;
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
                    _isModelMatDirty = true;
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
                    _isModelMatDirty = true;
                }
            }
        }

        // dumb uniforms
        public Color4 DiffuseColor { get; set; } = Color4.HotPink;
        private Matrix4 _ModelMat = Matrix4.Identity;
        public Matrix4 ModelMat { get { return _ModelMat; } }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Render()
        {
            Model.Bind();
            gld.UniformMatrix4(Model.Shader.UniformModelMatIndex, false, ref _ModelMat);
            if (Model.Shader.UniformDiffuseColorIndex != -1)
                gld.Uniform4(Model.Shader.UniformDiffuseColorIndex, DiffuseColor);
            Model.Render();
        }

        public void Update(double delta)
        {
            OnUpdate(delta);

            if (_isModelMatDirty)
            {
                UpdateModelMat();
            }
        }

        #endregion // --- ISceneObject interface ---

        #region --- protected implementation ---
#if DEBUG
        protected gldebug gld = new gldebug();
#endif
        protected void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
            }
        }

        protected virtual void OnUpdate(double delta) { }

        #endregion // --- protected implementation ---

        #region --- private implementation ---

        private bool _isModelMatDirty = true;

        private Quaternion _orientation;
        private Vector3 _position;
        private Vector3 _scale;
        private int gl_ModelDiffuseID = -1;

        private string DebuggerDisplay
        {
            get
            {
                return string.Format("{0}: POS {1}, ROT {2}, Scale {3}", Name, _position.ToString(), _orientation.ToString(), _scale.ToString());
            }
        }

        private void UpdateModelMat()
        {
            _ModelMat = Matrix4.CreateFromQuaternion(_orientation) * Matrix4.CreateScale(_scale) * Matrix4.CreateTranslation(_position);
            _isModelMatDirty = false;
        }

        ~SceneObject()
        {
#if DEBUG
            TraceLog.Warn($"Leaked scene object {GetType().Name}; Did you forget to call Dispose()?");
#endif
            Dispose(false);
        }

        #endregion // --- private implementation ---
    }
}
