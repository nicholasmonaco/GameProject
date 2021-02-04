using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_Bullet : GameObject {

        public Prefab_Bullet() : base() {
            Name = "Bullet";

            // Add components
            Rigidbody2D rb = AddComponent<Rigidbody2D>();

            Collider2D collider = AddComponent<CircleCollider2D>(1); //Radius of 1
            collider.IsTrigger = true;
            // End adding components
        }

    }
}
