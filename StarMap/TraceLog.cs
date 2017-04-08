using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarMap
{
    public static class TraceLog
    {
        private const string DebugTag  = " [DEBUG]";
        private const string InfoTag   = "  [INFO]";
        private const string NoticeTag = "[NOTICE]";
        private const string WarnTag   = "  [WARN]";
        private const string ErrorTag  = " [ERROR]";
        private const string FatalTag  = "[***FATAL***]";

        public static void Fatal(string format, params object[] args)
        {
            WriteLine(FatalTag, format, args);
            if (Debugger.IsAttached)
                Debugger.Break();
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

        public static void WriteIdenfierTags()
        {
            Trace.WriteLine($"{InfoTag} Logging will utilize the following tags (most to least severe): {FatalTag.ToString().TrimStart()}, {ErrorTag.ToString().TrimStart()}, {WarnTag.ToString().TrimStart()}, {NoticeTag.ToString().TrimStart()}, {InfoTag.ToString().TrimStart()}, and {DebugTag.ToString().TrimStart()}.");
        }

        private static void WriteLine(string tag, string format, params object[] args)
        {
            if (args != null && args.Length > 0)
                Trace.WriteLine($"{tag} {string.Format(format, args)}");
            else
                Trace.WriteLine($"{tag} {format}");
        }
    }
}
