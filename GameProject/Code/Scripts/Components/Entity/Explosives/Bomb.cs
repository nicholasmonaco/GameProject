using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Core.Animation;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Bomb : AbstractEntity {
        public static readonly float StandardBombLifetime = 3;
        public static readonly float StandardExplosionForce = 5;
        
        
        public Bomb(GameObject attached, float explosionForce) : base(attached, EntityID.Bomb_Normal) {
            _lifeTimer = StandardBombLifetime;
            _explosionForce = explosionForce;

            _bombRenderer = GetComponent<SpriteRenderer>();

            _explosionParticles = GetComponent<ParticleSystem>();
            //_bombAnimator = GetComponent<AnimationController>();
        }


        public CircleCollider2D PhysicalCollider;
        public Collider2D ExplosionCollider;

        private ParticleSystem _explosionParticles;
        private AnimationController _bombAnimator;
        private SpriteRenderer _bombRenderer;

        private float _explosionForce;
        private float _lifeTimer;


        // when timer is done, explode

        public override void FixedUpdate() {
            _lifeTimer -= Time.fixedDeltaTime;

            if (!_dead) {
                _bombRenderer.Color = Color.Lerp(Color.White, Color.Red, (_lifeTimer % 0.35f) / 0.35f);

                if (_lifeTimer <= 0) {
                    _dead = true;
                    StartCoroutine(Explode());
                }
            }
        }

        public override void OnTriggerStay2D(Collider2D other) {
            if(other.Layer == LayerID.Obstacle) {
                Point gridPos = GameManager.Map.CurrentRoom.GetGridPos(other.Bounds.Center);
                ObstacleID type = GameManager.Map.CurrentRoom.GetObstacleAtGridPos(gridPos);
                if((int)type >= 11 && (int)type <= 30) {
                    GameManager.Map.CurrentRoom.ChangeTile(gridPos, ObstacleID.None);
                }

                return;
            }

            AbstractEntity entity = other.GetComponent<AbstractEntity>();
            if(entity != null) {
                entity.OnExploded(transform.Position.ToVector2(), _explosionForce);
            }
        }


        //when player leaves bomb collider, allow real physics interaction between them



        protected IEnumerator Explode() {
            //play exploding animation
            //_bombAnimator.ChangeAnimationState(1); //1 is exploding, 0 is flashing

            // wait for exploding animation to be over
            //while (_bombAnimator.CurrentAnimation.Loops < 1) yield return null;
            float timer = 0.1f;
            Vector3 origScale = transform.LocalScale;
            while(timer > 0) {
                transform.LocalScale = Vector3.Lerp(new Vector3(1, 0, 1), origScale, timer / 0.1f);
                yield return null;
                timer -= Time.deltaTime;
            }

            //yield return new WaitForEndOfFrame(); //commetned out due to non-official animation usage
            transform.LocalScale = Vector3.One; //refixing the scale

            //remove sprite and physics collider
            RemoveComponent<SpriteRenderer>();
            //Destroy(PhysicalCollider);
            gameObject.Layer = LayerID.Damage;

            PhysicalCollider.Enabled = true;
            PhysicalCollider.IsTrigger = true;
            (PhysicalCollider.Bounds as CircleBounds).ResetRadius(35);

            //activate particles
            _explosionParticles.Play();

            //play sfx
            Resources.Sound_Explosion.Play(0.25f);

            //enable circlecollider
            //ExplosionCollider.Enabled = true;

            //wait till end of frame twice
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            // delete circlecollider
            //Destroy(ExplosionCollider);
            PhysicalCollider.Enabled = false;

            // wait for particles to be done
            while (_explosionParticles.ParticleCount > 0) yield return null;

            // delete self
            Destroy(gameObject);
        }
    }
}