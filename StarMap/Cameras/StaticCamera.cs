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
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */
using OpenTK;

namespace StarMap.Cameras
{
    /// <summary>
    /// A pretty boring static camera that can only be moved through code.
    /// </summary>
    public class StaticCamera : Camera
    {
        /// <summary>
        /// This camera is not movable, at least by the user.
        /// </summary>
        public override bool IsUserControlled { get { return false; } }

        /// <summary>
        /// The name of this <see cref="Camera"/>.
        /// </summary>
        public override string Name { get; set; } = nameof(StaticCamera);

        /// <summary>
        /// Constructs a new, boring, <see cref="StaticCamera"/>.
        /// </summary>
        /// <param name="position">The location of the camera, in world-coordinates.</param>
        /// <param name="orientation">The orientation of the camera.</param>
        public StaticCamera(Vector3 position, Quaternion orientation) : base(position, orientation) { }
    }
}
