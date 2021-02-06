using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Scripts.Components.Bullet {
    public abstract class AbstractBullet : Component {

        public Rigidbody2D BulletRB;
        protected Collider2D BulletCollider;
        protected SpriteRenderer BulletRenderer;

        private float _lifeTimer_Max;
        protected float _lifeTimer;
        protected float _damage = 0;
        protected float _speed = 0;

        public Action<float> _extraUpdateAction = (curLifeDuration) => { };
        public Action _extraDeathAction = () => { };


        public AbstractBullet(GameObject attached) : base(attached) { }


        public void SetComponents(Rigidbody2D bulletRB, Collider2D bulletCollider, SpriteRenderer spriteRend) {
            BulletRB = bulletRB;
            BulletCollider = bulletCollider;
            BulletRenderer = spriteRend;
        }

        public void InitBullet(Vector2 dir, float speed, float damage, float lifetime) {
            BulletRB.Velocity = dir * speed;
            _lifeTimer_Max = lifetime;
            _lifeTimer = _lifeTimer_Max;
            _damage = damage;
            _speed = speed;

            
        }


        public void SetScale(float scale) {
            BulletRenderer.SpriteScale = new Vector2(scale, scale);
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



        public override void FixedUpdate() {
            if (_lifeTimer > 0) {
                _extraUpdateAction(_lifeTimer_Max - _lifeTimer);

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

            _extraDeathAction();

            Destroy(this.gameObject);
        }


        public void Die() {
            if (_lifeTimer > 0) {
                _lifeTimer = -1;
                StartCoroutine(DieCoroutine());
            }
        }

        protected bool DefaultCollisionLogic(Collider2D collision) {
            if (collision.gameObject.Layer == (int)LayerID.EdgeWall) {
                Die();
                return true;
            }
            return false;
        }
    }
}
