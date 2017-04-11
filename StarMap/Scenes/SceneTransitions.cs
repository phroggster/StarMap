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
using Phroggiesoft.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace StarMap.Scenes
{
    public class SceneTransitions
    {
        /// <summary>
        /// Given a newly constructed scene, load it in the calling thread, optionally set the projection matrix,
        /// assign it to the <c>ref currentScene</c> parameter, <c>Dispose()</c> of the old scene, and return.
        /// </summary>
        /// <param name="control">The <see cref="GLControl"/> being used.</param>
        /// <param name="currentScene">The <see cref="IScene"/> currently in use.</param>
        /// <param name="newScene">The new <see cref="IScene"/> to be transitioned to.</param>
        public void Immediate(IContainer container, phrogGLControl control, ref IScene currentScene, IScene newScene)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            bool newSceneNull = false;

            if (newScene == null)
            {
                newSceneNull = true;
                newScene = currentScene;
            }

            TraceLog.Info($"{nameof(SceneTransitions)}.{nameof(Immediate)} to scene {newScene.Name}.");

            if (!newScene.IsLoaded)
                newScene.Load(control);

            if (!newSceneNull)
                currentScene?.Stop();

            IScene old = currentScene;
            currentScene = newScene;

            if (!newSceneNull && old != null)
            {
                container.Remove(old);
                old.Dispose();
                old = null;
                GC.Collect();
            }

            newScene.Start();
            control.Invalidate();
        }
    }
}
