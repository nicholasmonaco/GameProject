using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.PathFinding;
using GameProject.Code.Scripts.Components.Bullet;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Prefabs;
using GameProject.Code.Prefabs.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public abstract class AbstractEnemy : AbstractEntity {
        public AbstractEnemy(GameObject attached, EntityID id) : base(attached, id) { }

        // Enemy variables
        protected float _health = 1;
        
        public float Health {
            get { return _health; }
            set {
                if (_health - value <= _health) {
                    if (_redFlashTimer > 0) _redFlashTimer = 0.2f;
                    else { StartCoroutine(RedFlash(0.2f)); }
                }

                _health = value;
                if (_health <= 0) Die();
            }
        }

        protected EnemyAnimationAction _animState;
        protected int _frameID = 0;

        protected Dictionary<EnemyAnimationAction, (Texture2D[], int[], float)> _animFrames;
        protected Dictionary<EnemyAnimationAction, (int[], float)> _frameIDs = null;

        protected Color _origColor;
        protected float _redFlashTimer = 0;

        // End enemy variables


        // Enemy getters
        protected Vector2 _playerPos => GameManager.PlayerTransform.Position.ToVector2();
        protected Vector2 _dirToPlayer => (_playerPos - transform.transform.Position.ToVector2()).Norm();
        // End enemy getters

        // Enemy components
        protected Rigidbody2D _enemyRB;
        protected SpriteRenderer _enemyRenderer;
        protected PathFinder _enemyPathFinder;
        // End enemy components

        // Enemy stats
        protected float _seeDistance = 300;

        protected float _speed = 50;

        protected float _shotSpeed = 90;
        protected float _shotRate = 2;
        protected float _shotLifetime = 10;
        protected float _shotSize = 0.8f;
        protected Color _shotColor = Color.Red;
        // End enemy stats

        

        public override void PreAwake() {
            base.PreAwake();

            _animFrames = new Dictionary<EnemyAnimationAction, (Texture2D[], int[], float)>();

            _enemyRB = GetComponent<Rigidbody2D>();
            _enemyRB.Velocity = Vector2.Zero;
            _enemyRB.TimeMultplier = Rigidbody2D.EntityTime;

            _enemyRenderer = GetComponent<SpriteRenderer>();
            _enemyRenderer.Material.BatchID = BatchID.Entities;

            _enemyPathFinder = GetComponent<PathFinder>();

            _origColor = _enemyRenderer.Color;

            SetAnimationFrames();
            StartCoroutine(Animate());
        }

        protected virtual void SetAnimationFrames() {
            if (_frameIDs == null || !Resources.Sprites_EnemyAnimations.ContainsKey(ID)) return;

            foreach (EnemyAnimationAction anim in Enum.GetValues(typeof(EnemyAnimationAction))) {
                if (!Resources.Sprites_EnemyAnimations[ID].ContainsKey(anim)) continue;

                int count = Resources.Sprites_EnemyAnimations[ID][anim].Count;

                Texture2D[] frames = new Texture2D[count];
                for (int i = 0; i < count; i++) {
                    frames[i] = Resources.Sprites_EnemyAnimations[ID][anim][i];
                }

                _animFrames[anim] = (frames, _frameIDs[anim].Item1, _frameIDs[anim].Item2);
            }
        }


        protected IEnumerator Animate() {
            if (_frameIDs == null) yield break;

            float frameDur = _animFrames[_animState].Item3;
            float timer = frameDur + GameManager.DeltaRandom.NextValue(-frameDur, 0);
            _frameID = GameManager.DeltaRandom.Next(0, _animFrames[_animState].Item2.Length);
            EnemyAnimationAction lastAnim = _animState;

            while (true) {
                _enemyRenderer.Sprite = _animFrames[_animState].Item1[_animFrames[_animState].Item2[_frameID]];
                
                while(timer > 0) {
                    yield return null;
                    timer -= Time.deltaTime;

                    if (_dead || Destroyed) yield break;
                }

                _frameID = _frameID + 1 >= _animFrames[_animState].Item2.Length ? 0 : _frameID + 1;
                timer = frameDur;

                if (_animState != lastAnim) _frameID = 0;
            }
        }



        public override sealed void FixedUpdate() {
            _fixedUpdateAction();
        }

        private Action _fixedUpdateAction = () => { };
        public virtual void FixedUpdate_Enemy() { }



        public override void OnEnable() {
            StartCoroutine(HoldOnEntrance());
        }

        private IEnumerator HoldOnEntrance() {
            yield return new WaitForSeconds(0.5f);
            _fixedUpdateAction = FixedUpdate_Enemy;
        }

        private IEnumerator RedFlash(float duration) {
            _redFlashTimer = duration;

            _enemyRenderer.Color = Color.Red;

            while(_redFlashTimer > 0) {
                yield return null;
                _redFlashTimer -= Time.entityDeltaTime;
            }

            _enemyRenderer.Color = _origColor;
        }



        public override void OnDisable() {
            _fixedUpdateAction = () => { };
        }


        public override void OnDestroy() {
            base.OnDestroy();

            _dead = true;
        }

        public void Die() {
            if (!_dead) StartCoroutine(Die_Coroutine());
        }

        private IEnumerator Die_Coroutine() {
            //set dead
            _dead = true;
            _fixedUpdateAction = () => { };

            //remove collider and rigidbody
            GetComponent<Collider2D>().Destroy();
            GetComponent<Rigidbody2D>().Destroy();

            //play death animation
            yield return StartCoroutine(DeathAnimation());

            OnDeathFlag();

            //when death animation is over, destroy
            Destroy(this.gameObject);
        }

        protected abstract IEnumerator DeathAnimation();


        // Sound stuff

        protected void PlaySound(SoundEffect sound, float minDelay) {
            StartCoroutine(PlaySound_C(Resources.Sound_CaveChaser, 0.5f));
        }

        protected void PlaySoundUntilDeath(SoundEffect sound, float minRandRange, float maxRandRange) {
            StartCoroutine(PlaySoundUntilDeath_C(sound, minRandRange, maxRandRange));
        }


        private IEnumerator PlaySoundUntilDeath_C(SoundEffect sound, float minRandRange, float maxRandRange) {
            yield return new WaitForSeconds(GameManager.DeltaRandom.NextValue(minRandRange, maxRandRange));

            while (!_dead) {
                if (Enabled) {
                    float waitTime = GameManager.DeltaRandom.NextValue(minRandRange, maxRandRange);
                    sound.Play(1);
                    yield return new WaitForSeconds(waitTime);
                } else {
                    yield return null;
                }
            }
        }

        private IEnumerator PlaySound_C(SoundEffect sound, float minDelay) {
            sound.Play(1);
            if(minDelay != 0) yield return new WaitForSeconds((float)sound.Duration.TotalSeconds + minDelay);
        }



        #region Enemy AI Methods

        protected void SetIdleVelocity() {
            _enemyRB.Velocity = new Vector2(GameManager.DeltaRandom.NextValue(-5, 5),
                                            GameManager.DeltaRandom.NextValue(-5, 5)).Norm() * 3.5f;
        }

        protected void Idle() {
            float rng = GameManager.DeltaRandom.NextValue(0, 20);
            if(rng < 0.5f) {
                SetIdleVelocity();
            }
        }

        private int _reachMask = Physics2D.GetMask(LayerID.Hole, LayerID.Item, LayerID.Obstacle, LayerID.Wall, LayerID.Player);

        protected bool CanSeePlayer() {
            if (GameManager.Map.CurrentRoom.ObstacleColliders == null) return true;

            Ray2D ray = new Ray2D(transform.Position.ToVector2(), _playerPos - transform.Position.ToVector2());

            Collider2D targetCollider = GameManager.Player.PlayerCollider;
            int maxIndex = GameManager.Map.CurrentRoom.ObstacleColliders.Count - 1;
            Collider2D[] hitlist = new Collider2D[maxIndex + 2];
            GameManager.Map.CurrentRoom.ObstacleColliders.CopyTo(hitlist, 0);
            hitlist[maxIndex + 1] = targetCollider;

            RaycastHit2D hit;
            if(Physics2D.Raycast_List(ray, 1000, _reachMask, hitlist, out hit)) {
                return hit.HitTransform.gameObject.Layer == LayerID.Player;
            }
            return false;
        }


        protected void ChasePlayer() {
            if (GameManager.Player == null) return;

            if (Vector2.Distance(_playerPos, transform.Position.ToVector2()) < _seeDistance) {
                _enemyRB.Velocity = Vector2.Normalize(_playerPos - transform.Position.ToVector2()) * _speed;
            } else {
                _enemyRB.Velocity = Vector2.Zero;
            }
        }

        protected void TrackChasePlayer() {
            if (GameManager.Player == null) return;

            // If the player can be seen, just move towards it
            //if (CanSeePlayer()) {
            //    _enemyRB.Velocity = Vector2.Normalize(_playerPos - transform.Position.ToVector2()) * _speed;

            //    //DBEUG
            //    trackerposss = _enemyRB.Velocity.Norm();
            //    trackercolorrr = Color.Pink;

            //    return;
            //}

            //THE OFSSET IS OFF BY HALF A TILE!!!!!!!! but maybe only for this - not the tiles themselves

            // Otherwise, use pathfinding
            Vector2 dir;
            if (_enemyPathFinder.FindPath(_playerPos, out dir)) {
                if (dir == Vector2.Zero) {
                    _enemyRB.Velocity = Vector2.Normalize(_playerPos - transform.Position.ToVector2()) * _speed;
                    return;
                }

                _enemyRB.Velocity = dir * _speed;

                //Debug.Log($"Path found: Direction Vector {dir}");
                //DBEUG
                trackerposss = dir;
                trackercolorrr = Color.LimeGreen;

                return;
            }

            //DEBUG
            trackerposss = Vector2.Zero;
            trackercolorrr = Color.Transparent;

            _enemyRB.Velocity = Vector2.Zero;
        }



        //DEBUG
        private Vector2 trackerposss = Vector2.Zero;
        private Color trackercolorrr = Color.LimeGreen;
        public override void Draw(SpriteBatch sb) {
            base.Draw(sb);

            float angle = MathF.Atan2(trackerposss.Y, trackerposss.X);
            sb.Draw(Resources.Sprite_Pixel, transform.Position.ToVector2() + trackerposss, null, trackercolorrr, angle, Vector2.Zero, new Vector2(32, 1), SpriteEffects.None, 1);
        }


        protected void ShootAtPlayer() {
            ShootInDirection(_dirToPlayer);
        }

        protected void ShootInDirection(Vector2 direction) {
            if (Time.EntityTimeScale == 0) return;

            AbstractBullet bullet = Instantiate<Prefab_Bullet>().GetComponent<AbstractBullet>();
            bullet.transform.Position = transform.Position;
            bullet.transform.LocalScale = new Vector3(_shotSize);

            bullet.InitBullet(false, direction, _shotSpeed, 1, _shotLifetime);
            bullet.SetColor(_shotColor);
        }



        #endregion


        public override void OnCollisionStay2D(Collider2D other) {
            base.OnCollisionStay2D(other);

            if(other.gameObject.Layer == LayerID.Player) {
                GameManager.Player.HurtPlayer();
            }
        }


        public static GameObject GetEnemyFromID(EntityID id) {
            switch (id) {
                default:
                case EntityID.Drone_Bugged:
                    return new Prefab_DroneBugged();
                case EntityID.Drone_Attack:
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

    public enum EnemyAnimationAction {
        Idle = 0,
        Walk = 1,
        Attack = 2,
        Special01 = 3,
        Special02 = 4,
        Die = 5
    }
}