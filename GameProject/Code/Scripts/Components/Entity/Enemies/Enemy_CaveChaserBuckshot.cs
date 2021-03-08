using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Enemy_CaveChaserBuckshot : Enemy_CaveChaser {
        public Enemy_CaveChaserBuckshot(GameObject attached, EntityID id) : base(attached, id) { }

        private const float _buckshotAngle = 20;


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
                    Buckshot();
                    _shotTimer = _shotTimer_Max;
                }
            }
        }


        private void Buckshot() {
            int bulletCount = GameManager.DeltaRandom.Next(4, 7);

            Vector2 playerDir = _dirToPlayer;

            for(int i = 0; i < bulletCount; i++) {
                float angle = GameManager.DeltaRandom.NextValue(-_buckshotAngle, _buckshotAngle) * MathEx.Deg2Rad;
                ShootInDirection(playerDir.RotateDirection(angle));
            }
        }
    }
}