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
        private const float _speed = 65;

        private Rigidbody2D _enemyRB;



        public override void PreAwake() {
            base.PreAwake();

            _enemyRB = GetComponent<Rigidbody2D>();
            _enemyRB.Velocity = Vector2.Zero;
        }



        public override void FixedUpdate() {
            Vector2 playerPos = GameManager.PlayerTransform.Position.ToVector2();

            if(Vector2.Distance(playerPos, transform.Position.ToVector2()) < _seeDistance) {
                _enemyRB.Velocity = Vector2.Normalize(playerPos - transform.Position.ToVector2()) * _speed;
            } else {
                _enemyRB.Velocity = Vector2.Zero;
            }
        }



        protected override IEnumerator DeathAnimation() {
            throw new NotImplementedException();
        }
    }
}