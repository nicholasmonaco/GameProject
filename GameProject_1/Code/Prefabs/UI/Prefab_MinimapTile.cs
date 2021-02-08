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
            baseIcon.DrawLayer = DrawLayer.ID["HUD"];
            baseIcon.OrderInLayer = 55;

            SpriteRenderer overlayIcon = AddComponent<SpriteRenderer>();
            overlayIcon.DrawLayer = DrawLayer.ID["HUD"];
            overlayIcon.OrderInLayer = 56;

            MinimapTile tile = AddComponent<MinimapTile>();
            tile.SetSpriteRenderers(baseIcon, overlayIcon);
        }

    }
}
