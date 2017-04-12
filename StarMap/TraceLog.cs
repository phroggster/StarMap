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

#region --- using ... ---
using System.Diagnostics;
#endregion // --- using ... ---

namespace StarMap
{
    public static class TraceLog
    {
        #region --- public static TagName() methods ---

        public static void Fatal(string format, params object[] args)
        {
            WriteLine(FatalTag, format, args);
            if (Debugger.IsAttached)
                Debugger.Break();   // XXX
        }

        public static void Error(string format, params object[] args)
        {
            WriteLine(ErrorTag, format, args);
        }

        public static void Warn(string format, params object[] args)
        {
            WriteLine(WarnTag, format, args);
        }

        public static void Notice(string format, params object[] args)
        {
            WriteLine(NoticeTag, format, args);
        }
        
        public static void Info(string format, params object[] args)
        {
            WriteLine(InfoTag, format, args);
        }

        public static void Debug(string format, params object[] args)
        {
            WriteLine(DebugTag, format, args);
        }

        #endregion // --- public static TagName() methods ---

        public static void WriteIdenfierTags()
        {
            Trace.WriteLine($"{InfoTag} Logging will utilize the following tags (most to least severe): {FatalTag.ToString().TrimStart()}, {ErrorTag.ToString().TrimStart()}, {WarnTag.ToString().TrimStart()}, {NoticeTag.ToString().TrimStart()}, {InfoTag.ToString().TrimStart()}, and {DebugTag.ToString().TrimStart()}.");
        }

        #region --- private implementation ---

        private const string DebugTag = " [DEBUG]";
        private const string InfoTag = "  [INFO]";
        private const string NoticeTag = "[NOTICE]";
        private const string WarnTag = "  [WARN]";
        private const string ErrorTag = " [ERROR]";
        private const string FatalTag = "[***FATAL***]";

        private static string LastLogMsg = string.Empty;
        private static string LastLogTag = string.Empty;
        private static int LastLogRepeat = 0;

        private static void WriteLine(string tag, string format, params object[] args)
        {
            string msg;

            if (args != null && args.Length > 0)
                msg = $"{tag} {string.Format(format, args)}";
            else
                msg = $"{tag} {format}";

            if (string.Compare(msg, LastLogMsg) == 0)
            {
                LastLogRepeat++;
                return;
            }
            else if (LastLogRepeat > 0)
            {
                Trace.WriteLine($"{LastLogTag} Above message repeated {LastLogRepeat} {(LastLogRepeat > 1 ? "times" : "time")}.");
                LastLogRepeat = 0;
            }

            LastLogMsg = msg;
            LastLogTag = tag;
            Trace.WriteLine(msg);
        }

        #endregion // --- private implementation ---
    }
}
