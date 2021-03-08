using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Enemy_CaveChaserArmed : Enemy_CaveChaser {
        public Enemy_CaveChaserArmed(GameObject attached, EntityID id) : base(attached, id) { }

        
        public override void PreAwake() {
            base.PreAwake();

            _shotTimer_Max = 2;
            _shotTimer = _shotTimer_Max + GameManager.DeltaRandom.NextValue(-_shotTimer_Max / 2, _shotTimer_Max / 2);
        }



        public override void FixedUpdate_Enemy() {
            base.FixedUpdate_Enemy();

            if (CanSeePlayer()) {
                _shotTimer -= Time.deltaTime;
                if (_shotTimer <= 0) {
                    ShootAtPlayer();
                    _shotTimer = _shotTimer_Max;
                }
            }
        }
    }
}