// DrawLayer.cs - Nick Monaco

using System.Collections.Generic;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Holds the draw layer names-int dictionary
    /// </summary>
    public static class DrawLayer {

        // This should only go up to 49

        public static Dictionary<string, int> ID = new Dictionary<string, int>() {
            { "Default", 0 },
            { "Background", 1 },
            { "WorldStructs", 2 },
            { "Pickups", 3 },
            { "Enemies", 4 },
            { "Boss", 5 },
            { "Entities", 6 },
            { "Player", 7 },
            { "Projectiles", 8 },
            { "OverlayFX", 9 },
            { "AboveAll", 10 },
            { "HUD", 11 },
            { "OverHUD", 12 },
            { "TotalOverlay", 13 }
        };

    }
}
