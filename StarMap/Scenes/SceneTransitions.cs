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
        public bool IsBusy = false;
        public event EventHandler<AScene> LoadAsyncCompleted;

        /// <summary>
        /// Given a newly constructed scene, load it in the calling thread, optionally set the projection matrix,
        /// assign it to the <c>ref currentScene</c> parameter, <c>Dispose()</c> of the old scene, and return.
        /// </summary>
        /// <param name="control">The <see cref="GLControl"/> being used.</param>
        /// <param name="currentScene">The <see cref="AScene"/> currently in use.</param>
        /// <param name="newScene">The new <see cref="AScene"/> to be transitioned to.</param>
        /// <param name="isMatrixSet">If <c>false</c>, this function will reset the projection matrix. If <c>true</c>,
        /// this function will assume that the caller has or will reset the projection matrix (or constructed
        /// <c>newScene</c> with the size parameters).</param>
        public static void Immediate(GLControl control, ref AScene currentScene, AScene newScene, bool isMatrixSet = false)
        {
            Debug.WriteLine($"[INFO] SceneTransitions.Immediate to scene {newScene.Name}.");

            if (!(newScene is HelloWorldScene || newScene is LoadingScene))
                Debug.Assert(Program.Shaders.IsLoaded);

            if (!isMatrixSet)
                newScene.ResetProjectionMatrix(control.Width, control.Height);

            if (!newScene.IsLoaded)
                newScene.Load();

            AScene old = currentScene;
            currentScene = newScene;
            old?.Dispose();
            control.Invalidate();
        }

        private AScene NewScene;
        public void LoadAsync(GLControl control, AScene newScene)
        {
            if (IsBusy || NewScene != null)
                throw new InvalidOperationException("Scene load already in progress!");

            IsBusy = true;
            NewScene = newScene;
            Thread t = new Thread(asyncThreadLoadScene);
            t.SetApartmentState(ApartmentState.STA);
            t.Name = "SceneTransitions loader for " + newScene.Name;
            t.Start(control);
        }

        // TODO: There's no way of cancelling this...
        private void asyncThreadLoadScene(object objControl)
        {
            GLControl control = objControl as GLControl;
            if (control == null)
                return;

            GameWindow gw = new GameWindow(control.Width, control.Height, control.GraphicsMode, Thread.CurrentThread.Name,
                GameWindowFlags.Default, DisplayDevice.Default, control.GLMajorVersion, control.GLMinorVersion, control.ContextFlags, control.Context);
            gw.MakeCurrent();

            lock (NewScene)
            {
                NewScene.Load();
            }

            Form f;
            if (LoadAsyncCompleted != null && control != null && !control.IsDisposed && ((f = control.FindForm()) != null) && !f.IsDisposed)
            {
                foreach(EventHandler<AScene> t in LoadAsyncCompleted.GetInvocationList())
                {
                    ISynchronizeInvoke sin = t.Target as ISynchronizeInvoke;
                    if (sin != null && sin.InvokeRequired)
                        sin.Invoke(t, new object[] { this, NewScene });
                    else
                        t.Invoke(this, NewScene);
                }
            }
            else
                NewScene.Dispose();

            // Can't dispose NewScene if gw is already disposed...
            gw.Dispose();
            NewScene = null;
            IsBusy = false;
        }
    }
}
