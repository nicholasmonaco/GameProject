using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class MinimapTile : Component {
        public MinimapTile(GameObject attached) : base(attached) { }

        public Point RepPoint { get; private set; }

        private SpriteRenderer _baseRend;
        private SpriteRenderer _iconRend;

        public void SetSpriteRenderers(SpriteRenderer baseRenderer, SpriteRenderer iconRenderer) {
            _baseRend = baseRenderer;
            _iconRend = iconRenderer;
        }


        public void InitMinimapTile(MinimapIcon overlayTile, Point respectiveGridPoint) {
            RepPoint = respectiveGridPoint;

            _baseRend.Sprite = Resources.Sprite_MinimapIcons[MinimapIcon.Unexplored];

            if(!Resources.Sprite_MinimapIcons.ContainsKey(overlayTile)) {
                _iconRend.Destroy();
                _iconRend = null;
            } else {
                _iconRend.Sprite = Resources.Sprite_MinimapIcons[overlayTile];
                _iconRend.Color = Color.Transparent;
            }

            _baseRend.Color = Color.Transparent;
        }

        public void SetExplored() {
            _baseRend.Sprite = Resources.Sprite_MinimapIcons[MinimapIcon.Explored];
        }

        public void SetSeen() {
            _baseRend.Color = Color.White;
            if(_iconRend != null) _iconRend.Color = Color.White;
        }

    }
}