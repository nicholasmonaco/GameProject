using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Prefabs.Enemies;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public abstract class AbstractEnemy : AbstractEntity {
        public AbstractEnemy(GameObject attached) : base(attached) { }


        private bool _dead = false;
        protected float _health = 1;
        public float Health {
            get { return _health; }
            set {
                _health = value;
                if (_health <= 0) Die();
            }
        }



        public void Die() {
            if (!_dead) StartCoroutine(Die_Coroutine());
        }

        private IEnumerator Die_Coroutine() {
            //set dead
            _dead = true;

            //remove collider and rigidbody
            GetComponent<Collider2D>().Destroy();
            GetComponent<Rigidbody2D>().Destroy();

            //play death animation
            yield return StartCoroutine(DeathAnimation());

            //when death animation is over, destroy
            Destroy(this.gameObject);
        }

        protected abstract IEnumerator DeathAnimation();




        public static GameObject GetEnemyFromID(EntityID id) {
            switch (id) {
                default:
                case EntityID.BuggedDrone:
                    return new Prefab_DroneBugged();
                case EntityID.AttackDrone:
                    return new Prefab_DroneAttack();
                case EntityID.CaveChaser:
                    return new Prefab_CaveChaser();
                case EntityID.CaveChaser_Armed:
                    return new Prefab_CaveChaserArmed();
                case EntityID.CaveChaser_Omega:
                    return new Prefab_CaveChaserOmega();
                case EntityID.CaveChaser_Buckshot:
                    return new Prefab_CaveChaserBuckshot();
                case EntityID.Turret_Guard:
                    return new Prefab_TurretGuard();
                case EntityID.Turret_Multi:
                    return new Prefab_TurretMulti();
            }
        }
    }
}