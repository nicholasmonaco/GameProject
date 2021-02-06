// Debug.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Contains methods used for debugging.
    /// </summary>
    public static class Debug {

        public static readonly bool ShowColliders = false;


        public static void Log(string output, [CallerLineNumber] int lineNum = 0, [CallerMemberName] string caller = null, [CallerFilePath] string fp = null) {
            string[] splits = fp.Split('\\', '.');
            System.Diagnostics.Debug.WriteLine($"{splits[splits.Length-2]}.{caller}() (Line {lineNum}) | {output}");
        }

    }
}
