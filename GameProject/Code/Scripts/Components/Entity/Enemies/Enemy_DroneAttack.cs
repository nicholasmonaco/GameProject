using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Enemy_DroneAttack : Enemy_DroneBugged {
        public Enemy_DroneAttack(GameObject attached, EntityID id) : base(attached, id) { }

        


        public override void PreAwake() {
            base.PreAwake();

            _speed = 30;
        }


        public override void FixedUpdate_Enemy() {
            ChasePlayer();
        }
    }
}