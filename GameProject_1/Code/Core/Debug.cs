// Debug.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Contains methods used for debugging.
    /// </summary>
    public static class Debug {

        public static readonly bool ShowColliders = true;


        public static void Log(string output) {
            System.Diagnostics.Debug.WriteLine(output);
        }

    }
}
