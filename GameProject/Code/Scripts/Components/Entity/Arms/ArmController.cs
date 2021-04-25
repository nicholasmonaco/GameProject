using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity.Arms {
    public class ArmController : Component {

        private static readonly float MaxDriftDistance = 3.5f;
        private static readonly float DriftDuration = 5;

        private static Action EmptyAction = () => { };


        public ArmController(GameObject attached) : base(attached) {
            _driftTimer = DriftDuration;

            CurState = ArmState.Locked;
        }


        private ArmState _curState;
        public ArmState CurState {
            get => _curState;
            set {
                //if (_curState == ArmState.Locked && value != ArmState.Idle) return; //i dont think need this
                _curState = value;

                transform.LocalPosition = LastOrigLocalPos;

                if (CurrentAction != null) CurrentAction.Finished = true;

                switch (value) {
                    case ArmState.Locked:
                        // Nothing.
                        CurrentAction = null;
                        break;
                    case ArmState.Idle:
                        CurrentAction = StartCoroutine(Idle());
                        break;
                    case ArmState.Rushing:
                        CurrentAction = StartCoroutine(Rush());
                        break;
                }
            }
        }


        private Coroutine CurrentAction;

        public SpriteRenderer ArmRenderer;
        public ParticleSystem ArmParticles;
        public Vector3 LastOrigLocalPos;


        public Transform TargetTransform = null;
        public float TargetDist => Vector3.Distance(TargetTransform.Position, transform.Parent.Position);
        public Vector3 EmptyDirection {
            get {
                //if the distance of the mouse from lastoriglocalpos is greatter than range, use that
                //else, use the parent thing
                Vector3 mousePos = Input.MouseWorldPosition.ToVector3();
                Vector3 worldLastLocal = transform.TransformPoint(LastOrigLocalPos);

                float dist = Vector3.Distance(mousePos, worldLastLocal);

                if (dist >= RushDistance) {
                    _curRushDistance = RushDistance;
                    return (Input.MouseWorldPosition.ToVector3() - transform.TransformPoint(LastOrigLocalPos)).Norm();
                } else {
                    //return (Input.MouseWorldPosition.ToVector3() - transform.Parent.Position).Norm();
                    _curRushDistance = dist;
                    return (Input.MouseWorldPosition.ToVector3() - transform.TransformPoint(LastOrigLocalPos)).Norm();
                }

            }
        } 

        public float RushDelay = 0;
        public float RushDamage => PlayerStats.Damage;
        public float RushDistance => PlayerStats.Range * 40;
        private float _curRushDistance;
        public float RushSpeed => 1 / (PlayerStats.ShotRate * 2);

        private bool _hit = false;




        public override void PreAwake() {
            base.PreAwake();

            transform.Position -= new Vector3(0, MaxDriftDistance, 0);

            ArmParticles.Main.StartSize = new ValueCurve_Vector3(transform.Scale);
        }

        public override void FixedUpdate() {
            if(CurState == ArmState.Idle && transform.Rotation2D != 0) {
                transform.Rotation2D = MathHelper.Lerp(transform.Rotation2D, 0, Time.entityFixedDeltaTime * 5);
            }
        }



        private float _driftTimer = 0;

        public IEnumerator Idle() {
            LastOrigLocalPos = transform.LocalPosition;
            //_driftTimer = DriftDuration;
            bool driftUp = true;

            ArmParticles.IsEmitting = false;
            ArmParticles.Stop();

            while (CurState == ArmState.Idle) {
                float currentDrift;
                if (driftUp) {
                    currentDrift = MathHelper.SmoothStep(-MaxDriftDistance, MaxDriftDistance, _driftTimer / DriftDuration);
                } else {
                    currentDrift = MathHelper.SmoothStep(MaxDriftDistance, -MaxDriftDistance, _driftTimer / DriftDuration);
                }

                transform.LocalPosition = LastOrigLocalPos + new Vector3(0, currentDrift, 0);

                _driftTimer += Time.entityDeltaTime;
                if (_driftTimer >= DriftDuration) {
                    _driftTimer = 0;
                    driftUp = !driftUp;
                }

                yield return null;
            }
        }


        private IEnumerator Rush() {
            yield return null;
            yield return null;

            LastOrigLocalPos = transform.LocalPosition;
            Vector3 farthestVec;
            Vector3 middlePos;

            float speedFrac = RushSpeed / 5f;
            float toTime = speedFrac * 4;

            float timer;

            if (RushDelay > 0) yield return new WaitForSeconds(RushSpeed / 2);

            ArmParticles.IsEmitting = true;
            ArmParticles.Play();


            while (CurState == ArmState.Rushing) {
                timer = toTime;
                _hit = false;

                ArmParticles.Main.StartRotation = new ValueCurve_Vector3(new Vector3(0, 0, transform.Rotation_Rads2D));

                //punch towards
                while (timer > 0) {
                    if (TargetTransform != null && TargetDist <= RushDistance) {
                        transform.LocalPosition = Vector3.Lerp(TargetTransform.Position, LastOrigLocalPos, timer / toTime);
                    } else {
                        Vector3 dir = EmptyDirection;
                        farthestVec = LastOrigLocalPos + dir * _curRushDistance;
                        transform.LocalPosition = Vector3.Lerp(farthestVec, LastOrigLocalPos, timer / toTime);

                        int sign = MathF.Sign(transform.Scale.X);
                        transform.Rotation2D = MathF.Atan2(-dir.Y, dir.X * -sign) * MathEx.Rad2Deg * sign + 45 * -sign;
                    }

                    yield return new WaitForFixedUpdate();
                    if (Destroyed) yield break;
                    timer -= Time.entityFixedDeltaTime;
                }

                timer = speedFrac;
                middlePos = transform.LocalPosition;

                //reel back
                while(timer > 0) {
                    transform.LocalPosition = Vector3.Lerp(LastOrigLocalPos, middlePos, timer / speedFrac);

                    yield return new WaitForFixedUpdate();
                    if (Destroyed) yield break;
                    timer -= Time.entityFixedDeltaTime;
                }

                transform.LocalPosition = LastOrigLocalPos;
            }
        }




        public void ResetRotation() {
            transform.Rotation2D = 0;
        }


        public override void OnTriggerEnter2D(Collider2D other) {
            HitLogic(other);
        }

        public override void OnTriggerStay2D(Collider2D other) {
            HitLogic(other);
        }


        private void HitLogic(Collider2D other) {
            //to fix the not hitting stuff sometimes thing, we would need these to have a real velocity
            //otherwise, we need to make it be ontriggerstay amd make sure it only hits once then cant hit anymore until it's position is reset
            //second one is easier but not better probably

            if (CurState == ArmState.Rushing || CurState == ArmState.ChargedPunch) {
                bool met = false;

                if (met || _hit || GameManager.Paused) return; // Check to see if already hit a wall or already hit an entity

                if (other.gameObject.Layer == LayerID.Enemy || other.gameObject.Layer == LayerID.Enemy_Flying) {
                    AbstractEnemy enemy = other.AttachedRigidbody.GetComponent<AbstractEnemy>();
                    enemy.Health -= RushDamage;
                    //enemy.ApplyKnockback(BulletRB.velocity.normalized * _knockbackForce / Game.Manager.PlayerStats.ShotCount);

                    Resources.Sound_Punch_Impact.Play(0.5f);

                    _hit = true;

                    //if (_curPiercingRemain == 0) {
                    //met = true;
                    //}

                }
            }
        }



        public override void OnDestroy() {
            if (CurrentAction != null) CurrentAction.Finished = true;

            base.OnDestroy();
        }

    }

    public enum ArmState {
        Locked,
        Transition,
        Idle,
        Rushing,
        Charging,
        ChargedPunch    
    }
}