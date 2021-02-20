using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Enemy_Chaser : AbstractEnemy {
        public Enemy_Chaser(GameObject attached) : base(attached) { }


        private const float _seeDistance = 150;
        private const float _speed = 50;

        private Rigidbody2D _enemyRB;



        public override void PreAwake() {
            base.PreAwake();

            _health = 15;

            _enemyRB = GetComponent<Rigidbody2D>();
            _enemyRB.Velocity = Vector2.Zero;
        }



        public override void FixedUpdate() {
            if (GameManager.Player == null) return;

            Vector2 playerPos = GameManager.PlayerTransform.Position.ToVector2();

            if(Vector2.Distance(playerPos, transform.Position.ToVector2()) < _seeDistance) {
                _enemyRB.Velocity = Vector2.Normalize(playerPos - transform.Position.ToVector2()) * _speed;
            } else {
                _enemyRB.Velocity = Vector2.Zero;
            }

            if(GameManager.DeltaRandom.NextValue(0, 20) < 0.1f) {
                Resources.Sound_CaveChaser.Play();
            }
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