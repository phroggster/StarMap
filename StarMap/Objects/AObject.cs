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
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using StarMap.Renderables;

namespace StarMap.Objects
{
    public abstract class AObject : IObject
    {
        public virtual ARenderable Model { get; set; }
        public virtual string Name { get; set; }

        public virtual Quaternion Orientation
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
                    isDirty = true;
                }
            }
        }
        public virtual Vector4 Position
        {
            get {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    isDirty = true;
                }
            }
        }
        public virtual Vector3 Scale
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
                    isDirty = true;
                }
            }
        }

        protected Matrix4 ModelMatrix;

        private Quaternion _orientation;
        private Vector4 _position;
        private Vector3 _scale;

        private bool isDirty = true;

        public AObject(ARenderable model, Vector4 position, Quaternion rotation, Vector3 scale, string name = "")
        {
            Model = model;
            Name = name;
            _position = position;
            _orientation = rotation;
            _scale = scale;
        }

        public virtual void Render()
        {
            Model.Bind();
            GL.UniformMatrix4(Model.Shader.UniformModel, false, ref ModelMatrix);
            Model.Render();
        }

        public void Update(double delta)
        {
            OnUpdate(delta);
        }

        protected virtual void OnUpdate(double delta)
        {
            // adjust properties when animated, rotating, scaling, or otherwise moving
            if (isDirty)
            {
                ModelMatrix = Matrix4.CreateFromQuaternion(Orientation) * Matrix4.CreateScale(_scale) * Matrix4.CreateTranslation(_position.Xyz);
                isDirty = false;
            }
        }
    }
}
