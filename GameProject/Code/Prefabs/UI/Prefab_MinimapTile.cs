using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_MinimapTile : GameObject{
        public Prefab_MinimapTile() : base() {
            Name = "MinimapTile";

            SpriteRenderer baseIcon = AddComponent<SpriteRenderer>();
            baseIcon.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
            baseIcon.OrderInLayer = 55;
            baseIcon.Material.BatchID = BatchID.HUD;

            SpriteRenderer overlayIcon = AddComponent<SpriteRenderer>();
            overlayIcon.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
            overlayIcon.OrderInLayer = 57;
            overlayIcon.Material.BatchID = BatchID.HUD;

            MinimapTile tile = AddComponent<MinimapTile>();
            tile.SetSpriteRenderers(baseIcon, overlayIcon);
        }

    }
}
