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
        public delegate float RetFloat();


        public List<Collider2D> Subcolliders;
        public Collider2D MainCollider => Subcolliders[0];

        public RigidbodyType Type = RigidbodyType.Dynamic;

        public RetFloat TimeMultplier = ScaledTime;

        private Vector2 _rawVelocity = Vector2.Zero;
        public Vector2 Velocity {
            get => _rawVelocity * TimeMultplier();
            set {
                _rawVelocity = value;
            }
        }
        
        public float Mass = 0;
        public float Drag = 0;

        private Vector2 _position;

        private Action _emptyAction = () => { };
        private Action _triggerActions = () => { };
        private Action _collisionActions = () => { };

        // Physics update variables; essentially, locals that need to be used outside of FixedUpdate

        // End physics update variables



        public Rigidbody2D(GameObject attached) : base(attached) {
            Subcolliders = new List<Collider2D>(1);
            Velocity = Vector2.Zero;
            ResetPosition();
        }

        public void ResetPosition() {
            //if(gameObject.Name == "Bullet") Debug.Log($"pos8 ({gameObject.Name}): ({_position.X}, {_position.Y})");
            //if (gameObject.Name == "Bullet") Debug.Log($"pos8a ({gameObject.Name}): ({transform.Position.X}, {transform.Position.Y})");
            _position = transform.Position.ToVector2();
            //if (gameObject.Name == "Bullet") Debug.Log($"pos9 ({gameObject.Name}): ({_position.X}, {_position.Y})");
        }


        public static float UnscaledTime() { return 1; }
        public static float ScaledTime() { return Time.TimeScale; }
        public static float EntityTime() { return Time.EntityTimeScale; }


        public void _PhysicsUpdate() { //the rotation is being messed up by either this or someting in scene. fix it
            // Check for collision
            bool willCollide = false;
            bool nonTriggerCollision = false;
            Vector2 pushbackVec = Vector2.Zero;

            foreach (Collider2D localCollider in Subcolliders) {
                if (localCollider.Enabled == false) continue;
                foreach (Collider2D collider in GameManager.CurrentScene.Collider2Ds) {
                    if (collider.Enabled == false) continue;
                    if (Subcolliders.Contains(collider)) continue;

                    bool entered = false;
                    if (!localCollider.Entered.ContainsKey(collider)) localCollider.Entered.Add(collider, false);
                    else { entered = localCollider.Entered[collider]; }

                    Vector2 otherVelocity = collider.AttachedRigidbody == null ? Vector2.Zero : collider.AttachedRigidbody.Velocity;

                    // Use a switch depending on what type of shape match up it is
                    CollisionResult2D result;

                    AbstractBounds localBounds = localCollider.Bounds;
                    AbstractBounds otherBounds = collider.Bounds;
                    BoundsType localBoundType = localBounds.BoundsType;
                    BoundsType otherBoundType = otherBounds.BoundsType;

                    switch (localBoundType) {
                        case BoundsType.Rectangle:
                            switch (otherBoundType) {
                                case BoundsType.Rectangle:
                                case BoundsType.Polygon:
                                    //rectangle polygon/rectangle
                                    result = PolygonBounds.DetectPolygonCollision(localBounds as PolygonBounds, otherBounds as PolygonBounds, Velocity, otherVelocity);
                                    break;
                                case BoundsType.Circle:
                                    //rectangle circle
                                    result = AbstractBounds.DetectCircleRectangleCollision(otherBounds as CircleBounds, localBounds as PolygonBounds, otherVelocity, Velocity);
                                    break;
                                default:
                                    result = new CollisionResult2D();
                                    result.Intersecting = false;
                                    result.WillIntersect = false;
                                    result.MinimumTranslationVector = Vector2.Zero;
                                    break;
                            }
                            break;
                        case BoundsType.Polygon:
                            switch (otherBoundType) {
                                case BoundsType.Rectangle:
                                case BoundsType.Polygon:
                                    //polygon polygon
                                    result = PolygonBounds.DetectPolygonCollision(localBounds as PolygonBounds, otherBounds as PolygonBounds, otherVelocity, Velocity);
                                    break;
                                case BoundsType.Circle:
                                    //polygon circle
                                    result = AbstractBounds.DetectDualTypeCollision(otherBounds as CircleBounds, localBounds as PolygonBounds, otherVelocity, Velocity);
                                    break;
                                default:
                                    result = new CollisionResult2D();
                                    result.Intersecting = false;
                                    result.WillIntersect = false;
                                    result.MinimumTranslationVector = Vector2.Zero;
                                    break;
                            }
                            break;
                        case BoundsType.Circle:
                            switch (otherBoundType) {
                                case BoundsType.Rectangle:
                                    //circle rectangle
                                    result = AbstractBounds.DetectCircleRectangleCollision(localBounds as CircleBounds, otherBounds as PolygonBounds, Velocity, otherVelocity);
                                    break;
                                case BoundsType.Polygon:
                                    //circle polygon
                                    result = AbstractBounds.DetectDualTypeCollision(localBounds as CircleBounds, otherBounds as PolygonBounds, Velocity, otherVelocity);
                                    break;
                                case BoundsType.Circle:
                                    //circle circle
                                    result = CircleBounds.DetectCircleCollision(localBounds as CircleBounds, otherBounds as CircleBounds, Velocity, otherVelocity);
                                    break;
                                default:
                                    result = new CollisionResult2D();
                                    result.Intersecting = false;
                                    result.WillIntersect = false;
                                    result.MinimumTranslationVector = Vector2.Zero;
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


            // Application Logic
            Vector2 origVelNoPushback_N = Velocity.Norm();

            if (willCollide && nonTriggerCollision) {
                _position += Velocity * Time.fixedDeltaTime + pushbackVec;

                // This causes a ton of issues if the colliders are inside each other, and I don't think its needed anyway
                //if(Type == RigidbodyType.Dynamic) Velocity += pushbackVec / Time.fixedDeltaTime; 
            } else {
                _position += Velocity * Time.fixedDeltaTime;
            }

            //if (Drag != 0) _position -= Velocity * Drag * Time.fixedDeltaTime;
            //if (Drag != 0) _position -= ((Velocity * Drag).Length() >= Velocity.Length() ? Velocity : (Velocity * Drag)) * Time.fixedDeltaTime;

            //if (Velocity != Vector2.Zero) Velocity += -origVelNoPushback_N * Drag;

            if (Drag != 0 && Velocity != Vector2.Zero /*&& !float.IsNaN(origVelNoPushback_N.X)*/) {
                Vector2 checker = Velocity + (-origVelNoPushback_N * Drag);
                //if (checker.Norm() == -Velocity.Norm()) Velocity = Vector2.Zero; // I don't think this is needed
                /*else*/ 
                Velocity = checker;
            }

            //Debug.Log($"{gameObject.Name} | Velocity: {Velocity}, checker: {checker}, origVelNoPushback: {origVelNoPushback_N}, pushbackVec: {pushbackVec}");

            transform.Position = _position.ToVector3();
        }



        public bool CollisionVelFlipped = false;
        public Vector2 pushBack = Vector2.Zero;

        public void ApplyCollision(bool willCollide, bool nonTriggerCollision, Vector2 pushbackVec) {
            if (willCollide && nonTriggerCollision) {
                CollisionVelFlipped = true;
                pushBack += pushbackVec;
            }

            // Now that this is here, we can actually extend this to apply all at once after all the computations have been done.
            // If we want to do that, just move this into a delegate and keep adding it every time, then apply it all at once in a loop
        }

        public void UpdateValues() {
            Vector2 origVelNoPushback_N = Velocity.Norm();

            if (CollisionVelFlipped) {
                _position += Velocity * Time.fixedDeltaTime + pushBack;
                CollisionVelFlipped = false;
            } else {
                _position += Velocity * Time.fixedDeltaTime;
            }

            pushBack = Vector2.Zero;

            if (Drag != 0 && Velocity != Vector2.Zero) {
                Velocity = Velocity + (-origVelNoPushback_N * Drag);
            }

            //Debug.Log($"{gameObject.Name} | Velocity: {Velocity}, origVelNoPushback_N: {origVelNoPushback_N}, pushbackVec: {pushbackVec}");

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

    public enum RigidbodyType {
        Dynamic,        // Interacts with the world automatically
        Kinematic,      // Only is interactable through scripts
        Static          // Doesn't move
    }
}
