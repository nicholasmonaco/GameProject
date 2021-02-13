using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;


namespace GameProject.Code.Prefabs.Enemies {
    public class Prefab_CaveChaser : GameObject {

        public Prefab_CaveChaser() : base() {
            Name = "Cave Chaser";
            Layer = (int)LayerID.Enemy;

            // Adding components
            RectCollider2D collider = AddComponent<RectCollider2D>(26, 26); //Change this to be a circle collider later maybe?

            rigidbody2D = AddComponent<Rigidbody2D>();

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
