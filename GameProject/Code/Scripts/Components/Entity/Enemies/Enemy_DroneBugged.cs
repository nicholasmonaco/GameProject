using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Enemy_DroneBugged : AbstractEnemy {
        public Enemy_DroneBugged(GameObject attached, EntityID id) : base(attached, id) {
            _frameIDs = new Dictionary<EnemyAnimationAction, (int[], float)>(1) {
                { EnemyAnimationAction.Idle, (new int[7] { 0, 1, 2, 3, 2, 1, 0 }, 0.2f)}
            };
        }



        public override void PreAwake() {
            base.PreAwake();

            _health = 12;
        }

        public override void Start() {
            base.Start();

            PlaySoundUntilDeath(Resources.Sound_CaveChaser, 1.5f, 3.5f);
            SetIdleVelocity();
        }

        

        public override void FixedUpdate_Enemy() {
            Idle();
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

            Destroy(gameObject);
        }



        
    }
}