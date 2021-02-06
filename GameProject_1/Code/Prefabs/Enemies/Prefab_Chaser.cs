using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;


namespace GameProject.Code.Prefabs {
    public class Prefab_Chaser : GameObject {

        public Prefab_Chaser() : base() {
            Name = "Chaser";
            Layer = (int)LayerID.Enemy;

            // Adding components
            RectCollider2D collider = AddComponent<RectCollider2D>(26, 26); //Change this to be a circle collider later maybe?

            AddComponent<Rigidbody2D>();

            SpriteRenderer sr = AddComponent<SpriteRenderer>(Resources.Sprite_Pixel);
            sr.SpriteScale = collider.Size;
            sr.Color = Color.Red;
            sr.DrawLayer = DrawLayer.ID["Enemies"];
            sr.OrderInLayer = 15;

            transform.Position = new Vector3(80, 0, 0);

            AddComponent<Enemy_Chaser>();
        }

    }
}
