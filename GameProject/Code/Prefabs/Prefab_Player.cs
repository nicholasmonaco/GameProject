using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_Player : GameObject {

        public Prefab_Player() : base() {
            Name = "Player";
            Layer = (int)LayerID.Player;

            // Adding components
            //RectCollider2D collider = AddComponent<RectCollider2D>(26, 26); //Change this to be a circle collider later maybe?
            CircleCollider2D collider = AddComponent<CircleCollider2D>(9);

            AddComponent<Rigidbody2D>();

            SpriteRenderer sr = AddComponent<SpriteRenderer>(Resources.Sprite_Pixel);
            //sr.SpriteScale = collider.Size;
            //sr.Color = Color.Blue;
            sr.DrawLayer = DrawLayer.ID[DrawLayers.Player];
            sr.OrderInLayer = 20;

            AddComponent<PlayerController>();
            // End adding components
        }

    }
}
