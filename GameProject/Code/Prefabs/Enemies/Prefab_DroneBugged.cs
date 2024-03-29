﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Prefabs.Enemies {
    public class Prefab_DroneBugged : GameObject {

        public Prefab_DroneBugged() : base() {
            Layer = LayerID.Enemy;

            // Adding components
            CircleCollider2D collider = AddComponent<CircleCollider2D>(4.5f); //Change this to be a circle collider later maybe?

            rigidbody2D = AddComponent<Rigidbody2D>();
            rigidbody2D.Type = RigidbodyType.Kinematic;

            SpriteRenderer sr = AddComponent<SpriteRenderer>(Resources.Sprite_Pixel);
            sr.DrawLayer = DrawLayer.ID[DrawLayers.Enemies];
            sr.OrderInLayer = 15;

            SetSpecificData();
        }

        protected virtual void SetSpecificData() {
            Name = "Bugged Drone";
            AddComponent<Enemy_DroneBugged>(EntityID.Drone_Bugged);
        }

    }
}
