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
using StarMap.Cameras;
using StarMap.Renderables;
using StarMap.Shaders;
using System;
using System.Diagnostics;

namespace StarMap.Objects
{
    public abstract class AObject : IObject
    {
        public virtual ARenderable Model { get; set; }
        public virtual string Name { get; set; }
        
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
        public virtual Vector4 Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
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

        private Vector4 _position;
        private Vector4 _rotation;
        private Vector3 _scale;

        private Matrix4 translationMatrix;
        private Matrix4 rotationMatrix;
        private Matrix4 scaleMatrix;
        private bool isDirty = true;

        public AObject(ARenderable model, Vector4 position, Vector4 rotation, Vector3 scale)
        {
            Model = model;
            _position = position;
            _rotation = rotation;
            _scale = scale;
        }

        public virtual void Render()
        {
            Model.Bind();
            GL.UniformMatrix4(Model.Shader.UniformModel, false, ref ModelMatrix);
            Model.Render();
        }

        public virtual void Update(double delta)
        {
            // adjust properties when animated, rotating, scaling, or otherwise moving
            if (isDirty)
            {
                translationMatrix = Matrix4.CreateTranslation(_position.Xyz);
                rotationMatrix = Matrix4.CreateRotationX(_rotation.X) * Matrix4.CreateRotationY(_rotation.Y) * Matrix4.CreateRotationZ(_rotation.Z);
                scaleMatrix = Matrix4.CreateScale(_scale);
                ModelMatrix = rotationMatrix * scaleMatrix * translationMatrix;
                isDirty = false;
            }
        }
    }
}
