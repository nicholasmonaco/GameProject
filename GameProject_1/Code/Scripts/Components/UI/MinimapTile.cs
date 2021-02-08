using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class MinimapTile : Component {
        public MinimapTile(GameObject attached) : base(attached) { }

        private SpriteRenderer _baseRend;
        private SpriteRenderer _iconRend;

        public void SetSpriteRenderers(SpriteRenderer baseRenderer, SpriteRenderer iconRenderer) {
            _baseRend = baseRenderer;
            _iconRend = iconRenderer;
        }


        public void InitMinimapTile(MinimapIcon overlayTile) {
            _baseRend.Sprite = Resources.Sprite_MinimapIcons[MinimapIcon.Unexplored];

            if(overlayTile == MinimapIcon.Normal) {
                _iconRend.Destroy();
            } else {
                _iconRend.Sprite = Resources.Sprite_MinimapIcons[MinimapIcon.Unexplored];
            }
        }

    }
}