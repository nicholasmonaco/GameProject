// Debug.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Contains methods used for debugging.
    /// </summary>
    public static class Debug {
        public static readonly bool ShowColliders = false;
        public static readonly bool Mute = false;
        public static bool DebugDraw = false;


        public static void Log(string output, [CallerLineNumber] int lineNum = 0, [CallerMemberName] string caller = null, [CallerFilePath] string fp = null) {
            string[] splits = fp.Split('\\', '.');
            System.Diagnostics.Debug.WriteLine($"{splits[splits.Length-2]}.{caller}() (Line {lineNum}) | {output}");
        }


        public static void Start() {
            if (Mute) {
                // Turn sound effects off
                SoundEffect.MasterVolume = 0;

                // Turn music volume off
                MediaPlayer.Volume = 0;
            }
        }

    }
}
