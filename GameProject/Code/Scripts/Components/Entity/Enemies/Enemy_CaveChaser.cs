using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Enemy_CaveChaser : AbstractEnemy {
        public Enemy_CaveChaser(GameObject attached, EntityID id) : base(attached, id) { }

        protected float _shotTimer_Max;
        protected float _shotTimer;


        
        public override void PreAwake() {
            base.PreAwake();

            _health = 24;
            _speed = 50;
            _shotSpeed = 150;

            _shotTimer_Max = 2;
            _shotTimer = _shotTimer_Max;
        }

        public override void Start() {
            base.Start();

            Vibrate(1);
            PlaySoundUntilDeath(Resources.Sound_CaveChaser, 3f, 5f);
        }



        public override void FixedUpdate_Enemy() {
            //ChasePlayer();
            TrackChasePlayer();
        }


        


        protected override IEnumerator DeathAnimation() {
            _enemyRB.Velocity = Vector2.Zero;
            //Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<Rigidbody2D>());

            float dieDur_Total = 0.25f; // Length of death animation
            float dieDur = dieDur_Total;

            Vector2 origScale = transform.Scale.ToVector2();

            Vector2 end = new Vector2(0.0001f, 0.0001f);

            while (dieDur > 0) {
                yield return new WaitForFixedUpdate();
                dieDur -= Time.fixedDeltaTime;
                transform.Scale = Vector2.Lerp(end, origScale, dieDur / dieDur_Total).ToVector3();
            }

            yield return new WaitForEndOfFrame();
            transform.Scale = end.ToVector3();

            //_extraDeathAction();

            //Destroy(gameObject);
        }
    }
}