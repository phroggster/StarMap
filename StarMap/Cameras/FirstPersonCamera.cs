﻿#region --- Apache v2.0 license ---
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
using StarMap.Scenes;
#endregion // --- using ... ---

namespace StarMap.Cameras
{
    public class FirstPersonCamera : Camera
    {
        /// <summary>
        /// Constructs a new <see cref="FirstPersonCamera"/>.
        /// </summary>
        /// <param name="position">The location of the camera, in world-coordinates.</param>
        /// <param name="orientation">The orientation of the camera.</param>
        public FirstPersonCamera(Vector3 position, Quaternion orientation, IScene scene) : base(position, orientation, scene)
        {
            IsUserControlled = true;
            Name = nameof(FirstPersonCamera);
        }
    }
}
