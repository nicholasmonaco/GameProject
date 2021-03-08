// DrawLayer.cs - Nick Monaco

using System.Collections.Generic;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Holds the draw layer names-int dictionary
    /// </summary>
    public static class DrawLayer {

        // This should only go up to 49

        public static Dictionary<DrawLayers, int> ID = new Dictionary<DrawLayers, int>() {
            { DrawLayers.Default, 0 },
            { DrawLayers.Background, 1 },
            { DrawLayers.WorldStructs, 2 },
            { DrawLayers.Pickups, 3 },
            { DrawLayers.Enemies, 4 },
            { DrawLayers.Boss, 5 },
            { DrawLayers.Entities, 6 },
            { DrawLayers.Player, 7 },
            { DrawLayers.Projectiles, 8 },
            { DrawLayers.OverlayFX, 9 },
            { DrawLayers.AboveAll, 10 },
            { DrawLayers.HUD, 11 },
            { DrawLayers.OverHUD, 12 },
            { DrawLayers.TotalOverlay, 13 }
        };

    }

    public enum DrawLayers {
        Default,
        Background,
        WorldStructs,
        Pickups,
        Enemies,
        Boss,
        Entities,
        Player,
        Projectiles,
        OverlayFX,
        AboveAll,
        HUD,
        OverHUD,
        TotalOverlay
    }
}
