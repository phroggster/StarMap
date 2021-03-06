﻿#if GLDEBUG
using gld = StarMap.GLDebug;
#else
using gld = OpenTK.Graphics.OpenGL4.GL;
#endif

namespace StarMap.Scenes
{
    partial class Scene
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;

                if (disposing)
                    TraceLog.Debug($"Disposing of {Name}.");
                else
                    TraceLog.Warn($"{Name} leaked! Did you forget to call Dispose()?");

                if (gl_UBO_ProjViewViewPort_ID >= 0)
                    gld.DeleteBuffer(gl_UBO_ProjViewViewPort_ID);

                if (disposing)
                {
                    if (Contents != null)
                    {
                        foreach (var c in Contents)
                            c?.Dispose();
                        Contents.Clear();
                    }
                    ToggleKeys?.Clear();
                    components?.Dispose();
                }

                Camera = null;
                FPSUpdate = null;
                m_keyData = null;
                Parent = null;
                ToggleKeys = null;
                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
