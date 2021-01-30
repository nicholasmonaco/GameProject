using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core.Components {
    public class Rigidbody2D : Component {

        public List<Collider2D> Subcolliders;
        public Collider2D MainCollider => Subcolliders[0];
        
        public Vector2 Velocity = Vector2.Zero;
        public float Drag = 0;

        private Vector2 _position;

        private Action _emptyAction = () => { };
        private Action _triggerActions = () => { };
        private Action _collisionActions = () => { };

        // Physics update variables; essentially, locals that need to be used outside of FixedUpdate

        // End physics update variables



        public Rigidbody2D(GameObject attached) : base(attached) {
            Subcolliders = new List<Collider2D>(1);
            ResetPosition();
        }

        public void ResetPosition() {
            _position = transform.Position.ToVector2();
        }


        public void _PhysicsUpdate() {
            // Check for collision
            bool willCollide = false;
            bool nonTriggerCollision = false;
            Vector2 pushbackVec = Vector2.Zero;

            foreach (Collider2D localCollider in Subcolliders) {
                foreach (Collider2D collider in GameManager.CurrentScene.Collider2Ds) {
                    if (collider == localCollider) continue;

                    bool entered = false;
                    if (!localCollider.Entered.ContainsKey(collider)) localCollider.Entered.Add(collider, false);
                    else { entered = localCollider.Entered[collider]; }

                    Vector2 otherVelocity = collider.AttachedRigidbody == null ? Vector2.Zero : collider.AttachedRigidbody.Velocity;
                    CollisionResult2D result = Bounds.DetectCollision(localCollider.Bounds, collider.Bounds, Velocity, otherVelocity);

                    if (result.WillIntersect) {
                        pushbackVec += result.MinimumTranslationVector;

                        if (!willCollide) willCollide = true;
                        if (!localCollider.IsTrigger && !nonTriggerCollision) nonTriggerCollision = true;
                    }

                    //Debug.Log($"will intersect: {result.WillIntersect} | intersecting: {result.Intersecting}");

                    //okay hear me out
                    //instead of doing this this way, let's store a bool for each other collider in each collider that represents if it's been entered
                    //if true, it's colliding
                    //if false, it isn't
                    //as long as it's true, we are staying, but until its false then we cant exit

                    if (!entered && result.WillIntersect) {
                        if (localCollider.IsTrigger) {
                            _triggerActions += () => {
                                OnTriggerEnter2D_Direct(collider);
                                localCollider.OnTriggerEnter2D_Direct(collider);
                                collider.OnTriggerEnter2D_Direct(localCollider);
                                //Debug.Log("Trigger enter");
                            };
                        } else {
                            _collisionActions += () => {
                                OnCollisionEnter2D_Direct(collider);
                                localCollider.OnCollisionEnter2D_Direct(collider);
                                collider.OnCollisionEnter2D_Direct(localCollider);
                                //Debug.Log("Collision enter");
                            };
                        }
                    } else if (entered && !result.WillIntersect) {
                        if (localCollider.IsTrigger) {
                            _triggerActions += () => {
                                OnTriggerExit2D_Direct(collider);
                                localCollider.OnTriggerExit2D_Direct(collider);
                                collider.OnTriggerExit2D_Direct(localCollider);
                                //Debug.Log("Trigger exit");
                            };
                        } else {
                            _collisionActions += () => {
                                OnCollisionExit2D_Direct(collider);
                                MainCollider.OnCollisionExit2D_Direct(collider);
                                collider.OnCollisionExit2D_Direct(MainCollider);
                                //Debug.Log("Collision exit");
                            };
                        }
                    } else if (entered) {
                        if (localCollider.IsTrigger) {
                            _triggerActions += () => {
                                OnTriggerStay2D_Direct(collider);
                                localCollider.OnTriggerStay2D_Direct(collider);
                                collider.OnTriggerStay2D_Direct(localCollider);
                                //Debug.Log("Trigger stay");
                            };
                        } else {
                            _collisionActions += () => {
                                OnCollisionStay2D_Direct(collider);
                                localCollider.OnCollisionStay2D_Direct(collider);
                                collider.OnCollisionStay2D_Direct(localCollider);
                                //Debug.Log("Collision stay");
                            };
                        }
                    }

                    localCollider.Entered[collider] = result.WillIntersect;
                }

                
            }


            if (willCollide && nonTriggerCollision) {
                _position += (Velocity * Time.fixedDeltaTime) + pushbackVec * 0.4f;
            } else {
                _position += Velocity * Time.fixedDeltaTime;
            }

            if (Drag != 0) _position -= Velocity * Drag * Time.fixedDeltaTime;
            transform.Position = _position.ToVector3();
        }



        public void CallTriggerEvents() {
            _triggerActions();
            _triggerActions = _emptyAction;
        }

        public void CallCollisionEvents() {
            _collisionActions();
            _collisionActions = _emptyAction;
        }





        public Action<Collider2D> OnCollisionEnter2D_Direct = (other) => { };
        public Action<Collider2D> OnCollisionStay2D_Direct = (other) => { };
        public Action<Collider2D> OnCollisionExit2D_Direct = (other) => { };
        public Action<Collider2D> OnTriggerEnter2D_Direct = (other) => { };
        public Action<Collider2D> OnTriggerStay2D_Direct = (other) => { };
        public Action<Collider2D> OnTriggerExit2D_Direct = (other) => { };

        public void RefixActionsToSubcolliders() {
            foreach(Collider2D collider in Subcolliders) {
                collider.OnCollisionEnter2D_Direct += OnCollisionEnter2D_Direct;
                collider.OnCollisionStay2D_Direct += OnCollisionStay2D_Direct;
                collider.OnCollisionExit2D_Direct += OnCollisionExit2D_Direct;
                collider.OnTriggerEnter2D_Direct += OnTriggerEnter2D_Direct;
                collider.OnTriggerStay2D_Direct += OnTriggerStay2D_Direct;
                collider.OnTriggerExit2D_Direct += OnTriggerExit2D_Direct;
            }
        }
    }
}
