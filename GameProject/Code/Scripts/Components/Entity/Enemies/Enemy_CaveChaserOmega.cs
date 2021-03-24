using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Enemy_CaveChaserOmega : Enemy_CaveChaser {
        public Enemy_CaveChaserOmega(GameObject attached, EntityID id) : base(attached, id) { }


        public override void PreAwake() {
            base.PreAwake();

            _health *= 1.75f;
            _shotRate /= 1.5f;
            _shotSize *= 1.4f;

            _shotTimer_Max = 2;
            _shotTimer = _shotTimer_Max + GameManager.DeltaRandom.NextValue(-_shotTimer_Max / 2, _shotTimer_Max / 2);

            if(_flag)
                Do();
        }


        //TEMP
        bool _flag = false;
        public void flag() {
            _flag = true;
        }

        void Do() {
            _health *= 17;
            transform.Scale *= 3;
            _shotSize *= 1.5f;
            _shotSpeed *= 2;
            _shotRate *= 1.25f;
        }
        //



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
