using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Bullet;

namespace GameProject.Code.Prefabs {
    public class Prefab_Bullet : GameObject {

        public Prefab_Bullet() : base() {
            Name = "Bullet";

            transform.Position = GameManager.PlayerTransform.Position;

            // Add components
            Rigidbody2D rb = AddComponent<Rigidbody2D>();

            SpriteRenderer sr = AddComponent<SpriteRenderer>();
            sr.Sprite = Resources.Sprite_Bullet_Standard;
            //sr.Color = Color.White; //base this off of items and stuff
            sr.DrawLayer = DrawLayer.ID[DrawLayers.Projectiles];
            sr.OrderInLayer = 100;
            sr.SpriteScale = new Vector2(0.65f, 0.65f);
            sr.Material.BatchID = BatchID.AbovePlayer;

            //Collider2D collider = AddComponent<CircleCollider2D>(this, sr.Sprite.Width * sr.SpriteScale.X);
            Collider2D collider = AddComponent<CircleCollider2D>(sr);
            collider.IsTrigger = true;

            Bullet_Standard bullet = AddComponent<Bullet_Standard>();
            bullet.SetComponents(rb, collider, sr);
            // End adding components
        }

    }
}
