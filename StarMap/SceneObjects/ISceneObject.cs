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
using StarMap.Cameras;
using StarMap.Models;

namespace StarMap.SceneObjects
{
    /// <summary>
    /// A <see cref="SceneObject"/> interface. An <see cref="ISceneObject"/> represents a <see cref="Models.Model"/> that is placed in
    /// world space (at <see cref="Position"/>), oriented (to <see cref="Orientation"/>), and scaled (by <see cref="Scale"/>).
    /// <para>The <see cref="ISceneObject"/> is then refreshed via <see cref="Update(double, ICamera)"/>, and drawn via <see cref="Render"/>.</para>
    /// </summary>
    public interface ISceneObject : IIsDisposed
    {
        Model Model { get; }
        string Name { get; set; }
        Quaternion Orientation { get; }
        Vector3 Position { get; }
        Vector3 Scale { get; }

        void Render();
        void Update(double delta);
    }
}
