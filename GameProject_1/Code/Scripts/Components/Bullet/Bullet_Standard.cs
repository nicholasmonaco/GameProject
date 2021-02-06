using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts;

namespace GameProject.Code.Scripts.Components.Bullet {
    public class Bullet_Standard : AbstractBullet {

        //private Action _fixedUpdateAction = () => { };
        //private Transform _trackedTransform;

        private int _curPiercingRemain = 0;


        public Bullet_Standard(GameObject attached) : base(attached) { }

        public override void Start() {
            _curPiercingRemain = PlayerStats.PiercingCount;
        }

      

        public override void OnTriggerEnter2D(Collider2D collision) {
            if (DefaultCollisionLogic(collision)) { return; }

            if (collision.gameObject.Layer == (int)LayerID.Enemy) { // Enemy layer
                //AbstractEnemy enemy = collision.attachedRigidbody.GetComponent<AbstractEnemy>();
                //enemy.Health -= _damage;
                //enemy.ApplyKnockback(BulletRB.velocity.normalized * _knockbackForce / Game.Manager.PlayerStats.ShotCount);

                if (_curPiercingRemain == 0)
                    Die();
            }
        }
    }
}
