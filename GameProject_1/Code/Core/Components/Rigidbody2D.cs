// Rigidbody2D.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core.Components {
    
    /// <summary>
    /// Allows all physics (other than collision) to occur within the game world.
    /// </summary>
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


        public void _PhysicsUpdate() { //the rotation is being messed up by either this or someting in scene. fix it
            // Check for collision
            bool willCollide = false;
            bool nonTriggerCollision = false;
            Vector2 pushbackVec = Vector2.Zero;

            foreach (Collider2D localCollider in Subcolliders) {
                foreach (Collider2D collider in GameManager.CurrentScene.Collider2Ds) {
                    //if (collider == localCollider) continue; // This doesn't account for multiple colliders. relpaced with below
                    if (Subcolliders.Contains(collider)) continue;

                    bool entered = false;
                    if (!localCollider.Entered.ContainsKey(collider)) localCollider.Entered.Add(collider, false);
                    else { entered = localCollider.Entered[collider]; }

                    Vector2 otherVelocity = collider.AttachedRigidbody == null ? Vector2.Zero : collider.AttachedRigidbody.Velocity;

                    // Use a switch depending on what type of shape match up it is
                    //CollisionResult2D result = PolygonBounds.DetectPolygonCollision(localCollider.Bounds, collider.Bounds, Velocity, otherVelocity);
                    CollisionResult2D result;

                    switch (localCollider) {
                        case PolygonCollider2D local_poly:
                            switch (collider) {
                                case PolygonCollider2D other_poly:
                                    //polygon polygon
                                    result = PolygonBounds.DetectPolygonCollision(local_poly.Bounds as PolygonBounds, other_poly.Bounds as PolygonBounds, Velocity, otherVelocity);
                                    break;
                                case CircleCollider2D other_circle:
                                    //polygon circle
                                    break;
                            }
                            break;
                        case CircleCollider2D local_circle:
                            switch (collider) {
                                case PolygonCollider2D other_poly:
                                    //circle polygon
                                    break;
                                case CircleCollider2D other_circle:
                                    //circle circle
                                    break;
                            }
                            break;
                        default:
                            result = new CollisionResult2D();
                            result.Intersecting = false;
                            result.WillIntersect = false;
                            result.MinimumTranslationVector = Vector2.Zero;
                            break;
                    }


                    if (result.WillIntersect) {
                        bool notTrigger = !(collider.IsTrigger || localCollider.IsTrigger);
                        if (notTrigger) pushbackVec += result.MinimumTranslationVector;

                        if (!willCollide) willCollide = true;
                        if (notTrigger && !nonTriggerCollision) nonTriggerCollision = true;
                    }

                    //Debug.Log($"will intersect: {result.WillIntersect} | intersecting: {result.Intersecting}");

                    //okay hear me out
                    //instead of doing this this way, let's store a bool for each other collider in each collider that represents if it's been entered
                    //if true, it's colliding
                    //if false, it isn't
                    //as long as it's true, we are staying, but until its false then we cant exit

                    if (!entered && result.WillIntersect) {
                        if (!nonTriggerCollision) {
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
                        if (!nonTriggerCollision) {
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
                        if (!nonTriggerCollision) {
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

            Vector2 origVelNoPushback_N = Vector2.Normalize(Velocity);

            if (willCollide && nonTriggerCollision) {
                _position += Velocity * Time.fixedDeltaTime + pushbackVec;
                Velocity += pushbackVec / Time.fixedDeltaTime;
            } else {
                _position += Velocity * Time.fixedDeltaTime;
            }

            //if (Drag != 0) _position -= Velocity * Drag * Time.fixedDeltaTime;
            //if (Drag != 0) _position -= ((Velocity * Drag).Length() >= Velocity.Length() ? Velocity : (Velocity * Drag)) * Time.fixedDeltaTime;

            //if (Velocity != Vector2.Zero) Velocity += -origVelNoPushback_N * Drag;

            if (Drag != 0 && Velocity != Vector2.Zero) {
                Vector2 checker = Velocity + (-origVelNoPushback_N * Drag);
                if (Vector2.Normalize(checker) == -Vector2.Normalize(Velocity)) Velocity = Vector2.Zero;
                else Velocity = checker;
            }

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
