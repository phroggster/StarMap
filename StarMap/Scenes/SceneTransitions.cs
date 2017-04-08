﻿/*
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
        public bool IsBusy { get; private set; } = false;
        public event EventHandler<IScene> LoadAsyncCompleted;
        private IScene m_bgLoadingScene;

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
            container.Add(newScene);

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

        /// <summary>
        /// Loads a new <see cref="IScene"/> in the background, sending a <see cref="LoadAsyncCompleted"/> event when complete.
        /// </summary>
        /// <param name="control">A <see cref="GLControl"/> to provide the loading context.</param>
        /// <param name="newScene">The newly constructed <see cref="IScene"/> to be loaded.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="control"/> or <paramref name="newScene"/> are null.</exception>
        /// <exception cref="InvalidOperationException">If another scene is already being loaded.</exception>
        public void LoadAsync(phrogGLControl control, IScene newScene)
        {
            if (newScene == null)
                throw new ArgumentNullException(nameof(newScene));
            else if (newScene.IsLoaded)
            {
                LoadAsyncCompleted(this, newScene);
                return;
            }   
            else if (control == null)
                throw new ArgumentNullException(nameof(control));
            else if (IsBusy || m_bgLoadingScene != null)
                throw new InvalidOperationException("Scene loading is already in progress!");

            IsBusy = true;
            m_bgLoadingScene = newScene;
            Thread t = new Thread(asyncThreadLoadScene);
            t.SetApartmentState(ApartmentState.STA);
            t.Name = "SceneTransitions loader for " + newScene.Name;
            t.Start(control);
        }

        // TODO: There's really no way of cancelling this...
        [STAThread]
        private void asyncThreadLoadScene(object objControl)
        {
            phrogGLControl control = objControl as phrogGLControl;

            using (GameWindow gw = new GameWindow(control.Width, control.Height, control.GraphicsMode, Thread.CurrentThread.Name,
                GameWindowFlags.Default, DisplayDevice.Default, control.GLMajorVersion, control.GLMinorVersion, control.ContextFlags, control.Context))
            {
                gw.MakeCurrent();

                TraceLog.Debug($"{nameof(SceneTransitions)}: Asynchronously loading {m_bgLoadingScene.Name}.");

                lock (m_bgLoadingScene)
                {
                    m_bgLoadingScene.Load(control);
                }

                TraceLog.Debug($"{nameof(SceneTransitions)}: Async load of {m_bgLoadingScene.Name} complete.");

                Form f;
                if (LoadAsyncCompleted != null && control != null && !control.IsDisposed && ((f = control.FindForm()) != null) && !f.IsDisposed)
                {
                    foreach (EventHandler<IScene> t in LoadAsyncCompleted.GetInvocationList())
                    {
                        ISynchronizeInvoke sin = t.Target as ISynchronizeInvoke;
                        if (sin != null && sin.InvokeRequired)
                            sin.Invoke(t, new object[] { this, m_bgLoadingScene });
                        else
                            t.Invoke(this, m_bgLoadingScene);
                    }
                }
                else
                    m_bgLoadingScene.Dispose();

                f = null;
            }
            m_bgLoadingScene = null;
            IsBusy = false;
        }
    }
}
