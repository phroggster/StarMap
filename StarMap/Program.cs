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
using StarMap.Shaders;
using System;
using System.Reflection;
using System.Windows.Forms;
#endregion // --- using ... ---

namespace StarMap
{
    static class Program
    {
        /// <summary>
        /// The version of this application
        /// </summary>
        public static string AppVersion;

        /// <summary>
        /// Static <see cref="ShaderManager"/> for easy access from anywhere.
        /// </summary>
        public static ShaderManager Shaders;

        /// <summary>
        /// Static <see cref="MainForm"/> for easy access from anywhere.
        /// </summary>
        public static MainForm MainFrm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            TraceLog.Notice("Program starting up, version {0}", AppVersion);
            TraceLog.WriteIdenfierTags();

            using (OpenTK.Toolkit.Init(new OpenTK.ToolkitOptions { EnableHighResolution = false }))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (MainFrm = new MainForm())
                    Application.Run(MainFrm);
            }
            TraceLog.Notice("Program shutting down.");
        }
    }
}
