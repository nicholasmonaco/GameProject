using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public abstract class AbstractEnemy : AbstractEntity {
        public AbstractEnemy(GameObject attached) : base(attached) { }


        private bool _dead = false;
        private float _health = 1;
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


    }
}