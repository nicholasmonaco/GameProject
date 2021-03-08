using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Prefabs;
using GameProject.Code.Scripts;
using GameProject.Code.Scripts.Components.Bullet;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Components {
    public class PlayerController : Component {

        // Constants
        private const int _vertFrames = 7;
        // End constants


        // Components
        private Rigidbody2D _playerRB;
        private SpriteRenderer _playerSprite;
        // End components

        // Public values
        public bool CanMove = true;
        public bool FreezeMovement = false;
        // End public values

        // Private values
        private float _maxSpeed = 105;
        private float _acceleration = 800;
        private float _shotRate = 2;

        private Vector2 _moveVec = Vector2.Zero;
        private float _shootDelayTimer = 0;
        private bool _shooting = false;

        private bool _iFraming = false;

        private Direction _curDir = Direction.Down;
        private int _animFrame = 0;
        private float _animTimer = 0;

        private bool _dead = false;
        // End private values

        public PlayerController(GameObject attached) : base(attached) {
            _dead = false;

            PlayerStats.DeathAction = () => {
                _dead = true;
                GameManager.Die();
                PlayerStats.DeathAction = () => { };
            };
        }


        public override void PreAwake() {
            base.PreAwake();

            _playerRB = GetComponent<Rigidbody2D>();
            _playerRB.Drag = 5f;

            _playerSprite = GetComponent<SpriteRenderer>();

            // Input
            Input.OnShoot_Down += OnShootDown;
            Input.OnShoot_Released += OnShootUp;
            // End input

            GameManager.Player = this;

            StartCoroutine(MoveAnim());
        }



        public override void Update() {
            _moveVec = Input.MovementDirection;

            FixDirAnim();

            Shoot();
        }

        private void FixDirAnim() {
            bool refix = false;

            if(_moveVec.Y > 0 && _curDir != Direction.Up) {
                _curDir = Direction.Up;
                refix = true;
            } else if(_moveVec.Y < 0 && _curDir != Direction.Down) {
                _curDir = Direction.Down;
                refix = true;
            } else if(_moveVec.Y == 0 && _curDir != Direction.None) {
                _curDir = Direction.None;
                refix = true;
            }


            if (refix) {
                _animFrame = 0;
                _animTimer = 0;
            }

        }

        private IEnumerator MoveAnim() {
            while (true) {
                _animTimer -= Time.deltaTime;

                if(_animTimer <= 0) {
                    if (_curDir == Direction.None) {
                        _playerSprite.Sprite = Resources.Sprites_PlayerMove[Direction.Down][0];
                    } else {
                        _playerSprite.Sprite = Resources.Sprites_PlayerMove[_curDir][_animFrame];
                    }

                    _animTimer = 0.25f;
                    _animFrame++;
                    if (_animFrame >= _vertFrames) _animFrame = 0;
                }

                yield return null;
            }
        }



        public override void FixedUpdate() {
            if (FreezeMovement) {
                _playerRB.Velocity = Vector2.Zero;
            } else {
                if (!CanMove) return;
                _playerRB.Velocity += _moveVec * _acceleration * Time.fixedDeltaTime;

                if (_playerRB.Velocity.Length() > _maxSpeed) {
                    _playerRB.Velocity += -_playerRB.Velocity * (_playerRB.Velocity.Length() - _maxSpeed) * Time.fixedDeltaTime;
                }
            }
        }


        private void Shoot() {
            if(_shootDelayTimer <= 0) {
                if (_shooting) {
                    ShootLogic();
                    _shootDelayTimer = 1 / _shotRate;
                }
            } else {
                _shootDelayTimer -= Time.deltaTime;
            }
        }

        private void ShootLogic() {
            Vector2 aimDir = Vector2.Normalize(Input.MouseWorldPosition - transform.Position.ToVector2());
            
            Bullet_Standard bullet = Instantiate<Prefab_Bullet>().GetComponent<Bullet_Standard>();
            bullet.transform.Position = transform.Position; //can customize this later
            bullet.InitBullet(true, aimDir, PlayerStats.ShotSpeed, PlayerStats.Damage, PlayerStats.Range);
        }


        public void HurtPlayer() {
            if (!_iFraming) {
                PlayerStats.TakeDamage();
                
                Resources.Sound_PlayerHurt.Play(0.2f, 0, 0);
                _iFraming = true;
                StartCoroutine(IFrames());
            }
        }

        private IEnumerator IFrames() {
            float durTimer = 1.2f; //can be variable later
            const float flashDur = 0.1f;
            float flashTimer = 0;
            bool invis = false;
            
            while(durTimer >= 0 && !_dead) {
                if(flashTimer <= 0) {
                    invis = !invis;
                    flashTimer = flashDur;
                    _playerSprite.Color = invis ? Color.Transparent : Color.White;
                }

                yield return new WaitForFixedUpdate();
                durTimer -= Time.fixedDeltaTime;
                flashTimer -= Time.fixedDeltaTime;
            }

            _playerSprite.Color = Color.White;

            _iFraming = false;
        }


        public static void DamagePlayer(Collider2D other) {
            if(other.gameObject.Layer == (int)LayerID.Player) {
                GameManager.Player.HurtPlayer();
            }
        }


        //public override void Draw(SpriteBatch sb) {
        //    base.Draw(sb);
        //    sb.DrawString(Resources.Font_Debug, $"mousepos: {Input.MouseWorldPosition}", transform.Position.ToVector2(), Color.Red, 0, Vector2.Zero, -0.15f, SpriteEffects.FlipHorizontally, 1);
        //}


        private void OnShootDown() {
            _shooting = true;
        }

        private void OnShootUp() {
            _shooting = false;
        }

    }
}
