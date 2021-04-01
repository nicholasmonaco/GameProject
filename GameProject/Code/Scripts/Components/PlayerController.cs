using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.Animation;
using GameProject.Code.Prefabs;
using GameProject.Code.Scripts;
using GameProject.Code.Scripts.Components.Bullet;
using GameProject.Code.Scripts.Components.Entity.Arms;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Components {
    public class PlayerController : Component {

        // Constants
        private const int _vertFrames = 7;
        // End constants


        // Components
        public AnimationController PlayerAnimator { get; private set; }

        private Rigidbody2D _playerRB;
        public Collider2D PlayerCollider { get; private set; }
        private SpriteRenderer _playerSprite;

        public List<ArmController> Arms;
        // End components

        // Public values
        public bool CanMove = true;
        public bool FreezeMovement = false;

        public bool ArmsOut = false;
        // End public values

        // Private values
        private float _maxSpeed = 105;
        private float _acceleration = 800;

        private Vector2 _moveVec = Vector2.Zero;
        private float _shootDelayTimer = 0;
        private bool _shooting = false;

        private bool _iFraming = false;
        private bool _togglingArms = false;

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

            PlayerCollider = _playerRB.MainCollider;

            _playerSprite = GetComponent<SpriteRenderer>();

            PlayerAnimator = GetComponent<AnimationController>();

            // Input
            Input.OnShoot_Down += OnShootDown;
            Input.OnShoot_Released += OnShootUp;
            Input.OnSpace_Down += OnSpaceDown;
            // End input

            GameManager.Player = this;

            ToggleArms(false);
        }



        public override void Update() {
            _moveVec = Input.MovementDirection;

            FixDirAnim();

            
            if (!ArmsOut) {
                Shoot();
            }
        }


        public void SetArmState(ArmState state) {
            foreach(ArmController arm in Arms) {
                arm.CurState = state;
            }
        }


        public void ToggleArms(bool active) {
            ArmsOut = active;

            if (active) {
                foreach (ArmController arm in Arms) {
                    arm.CurState = ArmState.Idle;
                    arm.ArmRenderer.Color = Color.White;   
                }
            } else {
                foreach (ArmController arm in Arms) {
                    arm.CurState = ArmState.Locked;
                    arm.ArmRenderer.Color = Color.Transparent;
                    arm.ResetRotation();
                }
            }
        }

        public IEnumerator FadeToggleArms(bool active) {
            float fadeTime = 0.3f;
            float timer = fadeTime;
            _togglingArms = true;

            while (timer > 0) {
                foreach (ArmController arm in Arms) {
                    if(active) arm.ArmRenderer.Color = Color.Lerp(Color.White, Color.Transparent, timer / fadeTime);
                    else arm.ArmRenderer.Color = Color.Lerp(Color.Transparent, Color.White, timer / fadeTime);
                }

                yield return null;
                timer -= Time.deltaTime;
            }

            ToggleArms(active);

            yield return new WaitForEndOfFrame();

            _togglingArms = false;
        }




        private void FixDirAnim() {
            if(_moveVec.Y > 0 && _curDir != Direction.Up) {
                _curDir = Direction.Up;
                ChangePlayerAnimationState(PlayerAnimationState.Idle_Up);
            } else if(_moveVec.Y < 0 && _curDir != Direction.Down) {
                _curDir = Direction.Down;
                ChangePlayerAnimationState(PlayerAnimationState.Idle_Down);
            } 
        }

        private void HardFixDirAim() {
            if (_moveVec.Y > 0) {
                _curDir = Direction.Up;
                ForceChangePlayerAnimationState(PlayerAnimationState.Idle_Up);
            } else if (_moveVec.Y <= 0) {
                _curDir = Direction.Down;
                ForceChangePlayerAnimationState(PlayerAnimationState.Idle_Down);
            }
        }


        public PlayerAnimationState CurrentAnimationState { get; private set; }

        public void ChangePlayerAnimationState(PlayerAnimationState state) {
            bool success = false;

            switch (state) {
                case PlayerAnimationState.Idle_Up:
                case PlayerAnimationState.Idle_Down:
                case PlayerAnimationState.Walk_Up:
                case PlayerAnimationState.Walk_Down:
                    if (CurrentAnimationState == PlayerAnimationState.Idle_Down ||
                        CurrentAnimationState == PlayerAnimationState.Idle_Up ||
                        CurrentAnimationState == PlayerAnimationState.Walk_Down ||
                        CurrentAnimationState == PlayerAnimationState.Walk_Up)
                        success = true;
                    break;
                
            }

            if (success) {
                CurrentAnimationState = state;
                PlayerAnimator.ChangeAnimationState((int)state);
            } 
        }

        public void ForceChangePlayerAnimationState(PlayerAnimationState state) {
            PlayerAnimator.ChangeAnimationState((int)state);
        }


        //private IEnumerator MoveAnim() {
        //    while (true) {
        //        _animTimer -= Time.deltaTime;

        //        if(_animTimer <= 0 && !FreezeMovement) {
        //            if (_curDir == Direction.None) {
        //                _playerSprite.Sprite = Resources.Sprites_PlayerMove[Direction.Down][0];
        //            } else {
        //                _playerSprite.Sprite = Resources.Sprites_PlayerMove[_curDir][_animFrame];
        //            }

        //            _animTimer = 0.25f;
        //            _animFrame++;
        //            if (_animFrame >= _vertFrames) _animFrame = 0;
        //        }

        //        yield return null;
        //    }
        //}



        public override void FixedUpdate() {
            if (FreezeMovement) {
                _playerRB.Velocity = Vector2.Zero;
            } else {
                if (!CanMove) return;
                _playerRB.Velocity += _moveVec * _acceleration * Time.fixedDeltaTime;

                float vLength = _playerRB.Velocity.Length();

                if (vLength > _maxSpeed) {
                    _playerRB.Velocity += -_playerRB.Velocity * (_playerRB.Velocity.Length() - _maxSpeed) * Time.fixedDeltaTime;
                }


                if(vLength <= 4f && _moveVec == Vector2.Zero) {
                    _playerRB.Velocity = Vector2.Zero;
                }
            }
        }


        private void Shoot() {
            if(_shootDelayTimer <= 0) {
                if (_shooting && !FreezeMovement && CanMove) {
                    ShootLogic();
                    _shootDelayTimer = 1 / PlayerStats.ShotRate;
                }
            } else {
                _shootDelayTimer -= Time.deltaTime;
            }
        }

        //debug testing
        bool gray = false;
        private void ToggleGray() {
            gray = !gray;
            Effect shader = gray ? Resources.Effect_Grayscale : Resources.Effect_Base;
            Scene.ChangeShader(BatchID.Room, shader);
            Scene.ChangeShader(BatchID.Entities, shader);
            Scene.ChangeShader(BatchID.BehindEntities, shader);
            Scene.ChangeShader(BatchID.AbovePlayer, shader);
            Scene.ChangeShader(BatchID.Player, shader);
        }


        private void ShootLogic() {
            Vector2 aimDir = Vector2.Normalize(Input.MouseWorldPosition - transform.Position.ToVector2());
            
            Bullet_Standard bullet = Instantiate<Prefab_Bullet>().GetComponent<Bullet_Standard>();
            bullet.transform.Parent = GameManager.BulletHolder;
            bullet.transform.Position = transform.Position; //can customize this later
            bullet.InitBullet(true, aimDir, PlayerStats.ShotSpeed, PlayerStats.Damage, PlayerStats.Range);
            bullet.SetScale(PlayerStats.ShotSize);
            bullet.SetColor(PlayerStats.ShotColor);

            //DEBUG
            ToggleGray();
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

            ForceChangePlayerAnimationState(PlayerAnimationState.Damage);

            while (durTimer >= 0 && !_dead) {
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

            HardFixDirAim();

            _iFraming = false;
        }


        public IEnumerator PickupItem(float holdDuration) {
            ForceChangePlayerAnimationState(PlayerAnimationState.ItemPickup);

            yield return new WaitForSeconds(holdDuration);

            HardFixDirAim();
        }



        public static void DamagePlayer(Collider2D other) {
            if(other.gameObject.Layer == LayerID.Player) {
                GameManager.Player.HurtPlayer();
            }
        }


        //public override void Draw(SpriteBatch sb) {
        //    base.Draw(sb);
        //    sb.DrawString(Resources.Font_Debug, $"playerpos: {transform.Position}", Input.MouseWorldPosition, Color.Red, 0, Vector2.Zero, -0.15f, SpriteEffects.FlipHorizontally, 1);
        //}


        private void OnShootDown() {
            _shooting = true;

            if(ArmsOut && !GameManager.Paused) SetArmState(ArmState.Rushing);
        }

        private void OnShootUp() {
            _shooting = false;

            if (ArmsOut) SetArmState(ArmState.Idle);
        }

        private void OnSpaceDown() {
            if(!_togglingArms) StartCoroutine(FadeToggleArms(!ArmsOut));
        }




        public override void OnDestroy() {
            Input.OnShoot_Down -= OnShootDown;
            Input.OnShoot_Released -= OnShootUp;
            Input.OnSpace_Down -= OnSpaceDown;

            base.OnDestroy();
        }
    }


    public enum PlayerAnimationState {
        Idle_Down = 0,
        Idle_Up = 1,
        Walk_Down = 2,
        Walk_Up = 3,
        Damage = 4,
        ItemPickup = 5,
        Die = 6
    }
}
