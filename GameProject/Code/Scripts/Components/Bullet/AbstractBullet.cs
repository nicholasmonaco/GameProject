using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;

namespace GameProject.Code.Scripts.Components.Bullet {
    public abstract class AbstractBullet : Component {

        protected delegate void CollisionReport(Collider2D other, ref bool met);
        protected CollisionReport ColliderAction;

        public Rigidbody2D BulletRB;
        protected Collider2D BulletCollider;
        protected SpriteRenderer BulletRenderer;

        public float CurLifeRemaining => _lifeTimer_Max - _lifeTimer;
        private float _lifeTimer_Max;
        protected float _lifeTimer;
        public float Damage = 0;
        protected float _speed = 0;
        protected int _curPiercingRemain = 0;

        public Action<AbstractBullet> _extraUpdateAction = (bullet) => { };
        public Action<AbstractBullet> _extraDeathAction = (bullet) => { };


        public AbstractBullet(GameObject attached) : base(attached) { }


        public void SetComponents(Rigidbody2D bulletRB, Collider2D bulletCollider, SpriteRenderer spriteRend) {
            BulletRB = bulletRB;
            BulletCollider = bulletCollider;
            BulletRenderer = spriteRend;
        }

        public void InitBullet(bool good, Vector2 dir, float speed, float damage, float lifetime) {
            BulletRB.Velocity = dir * speed;
            _lifeTimer_Max = lifetime;
            _lifeTimer = _lifeTimer_Max;
            Damage = damage;
            _speed = speed;

            ColliderAction = DefaultCollisionLogic;

            if (good) {
                _curPiercingRemain = PlayerStats.PiercingCount;
                gameObject.Layer = LayerID.Bullet_Good;
                ColliderAction += NaturalCollisionLogic_Good;
            } else {
                _curPiercingRemain = 0;
                gameObject.Layer = LayerID.Bullet_Evil;
                ColliderAction += NaturalCollisionLogic_Evil;
            }
        }


        public void SetScale(float scale) {
            //BulletRenderer.SpriteScale = new Vector2(scale, scale);
            transform.Scale = new Vector3(scale);
        }

        public void SetScale(float x, float y) {
            BulletRenderer.SpriteScale = new Vector2(x, y);
        }

        public void SetScale(Vector2 scale) {
            BulletRenderer.SpriteScale = scale;
        }

        public void SetSprite(Texture2D sprite) {
            BulletRenderer.Sprite = sprite;
        }

        public void SetSprite(Texture2D sprite, float rotation) {
            SetSprite(sprite);
            BulletRenderer.transform.Rotation = rotation;
        }

        public void SetColor(Color newColor) {
            BulletRenderer.Color = newColor;
        }

        public void TurnEvil() {
            gameObject.Layer = LayerID.Bullet_Evil;
        }



        public override void FixedUpdate() {
            if (_lifeTimer > 0) {
                _extraUpdateAction(this);

                _lifeTimer -= Time.fixedDeltaTime;

                //Debug.Log($"pos: ({transform.Position.X}, {transform.Position.Y})");

                if (_lifeTimer <= 0) {
                    StartCoroutine(DieCoroutine());
                }
            }
        }

        protected IEnumerator DieCoroutine() {
            BulletRB.Velocity = Vector2.Zero;
            Destroy(BulletCollider);

            float dieDur_Total = 0.25f; // Length of death animation
            float dieDur = dieDur_Total;

            Vector2 origScale = transform.Scale.ToVector2();
            
            while (dieDur > 0) {
                yield return new WaitForFixedUpdate();
                dieDur -= Time.fixedDeltaTime;
                transform.Scale = Vector2.Lerp(Vector2.Zero, origScale, dieDur / dieDur_Total).ToVector3();
            }

            yield return new WaitForEndOfFrame();
            transform.Scale = Vector3.Zero;

            _extraDeathAction(this);

            Destroy(this.gameObject);
        }


        public void Die() {
            if (_lifeTimer > 0) {
                _lifeTimer = -1;
                StartCoroutine(DieCoroutine());
            }
        }

        protected void DefaultCollisionLogic(Collider2D collision, ref bool met) {
            if (collision.gameObject.Layer == LayerID.EdgeWall) {
                Die();
                met = true;
            } else if (collision.gameObject.Layer == LayerID.Door) {
                Die();
                met = true;
            } else if (collision.gameObject.Layer == LayerID.Obstacle) {
                //check if tileid at collider position is physical
                if (Room.ObstacleSolid(GameManager.Map.CurrentRoom.GetObstacleAtPos(collision.Bounds.Center))) {
                    Die();
                    met = true;
                }
            }
        }

        protected void NaturalCollisionLogic_Good(Collider2D other, ref bool met) {
            if (met) return;

            if (other.gameObject.Layer == LayerID.Enemy || other.gameObject.Layer == LayerID.Enemy_Flying) {
                AbstractEnemy enemy = other.AttachedRigidbody.GetComponent<AbstractEnemy>();
                enemy.Health -= Damage;
                //enemy.ApplyKnockback(BulletRB.velocity.normalized * _knockbackForce / Game.Manager.PlayerStats.ShotCount);

                if (_curPiercingRemain == 0) {
                    Die();
                    met = true;
                }
            }
        }

        protected void NaturalCollisionLogic_Evil(Collider2D other, ref bool met) {
            if (met) return;

            if (other.gameObject.Layer == LayerID.Player) {
                GameManager.Player.HurtPlayer();

                if (_curPiercingRemain == 0) {
                    Die();
                    met = true;
                }
            }
        }




        public override void OnTriggerEnter2D(Collider2D collision) {
            bool met = false;
            ColliderAction(collision, ref met);
        }

        public override void OnCollisionEnter2D(Collider2D other) {
            if (other.gameObject.Layer == LayerID.Obstacle) {
                Die();
            }
        }



        // Static methods


    }
}
