using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Scripts.Util;


namespace GameProject.Code.Prefabs.Enemies {
    public class Prefab_CaveChaser : GameObject {

        public Prefab_CaveChaser() : base() {
            Layer = LayerID.Enemy;

            // Adding components
            CircleCollider2D collider = AddComponent<CircleCollider2D>(9); //Change this to be a circle collider later maybe?

            rigidbody2D = AddComponent<Rigidbody2D>();

            SpriteRenderer sr = AddComponent<SpriteRenderer>(Resources.Sprite_Pixel);
            sr.SpriteScale = collider.Size;
            sr.Color = Color.Orange;
            sr.DrawLayer = DrawLayer.ID[DrawLayers.Enemies];
            sr.OrderInLayer = 15;

            SetSpecificData();
        }

        protected virtual void SetSpecificData() {
            Name = "Cave Chaser";
            AddComponent<Enemy_CaveChaser>(EntityID.CaveChaser);
        }

    }
}
