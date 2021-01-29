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



        public Rigidbody2D(GameObject attached) : base(attached) {
            Subcolliders = new List<Collider2D>(1);
            ResetPosition();
        }

        public void ResetPosition() {
            _position = transform.Position.ToVector2();
        }


        public override void FixedUpdate() {
            // Check for collision
            bool willCollide = false;
            bool nonTriggerCollision = false;
            Vector2 pushbackVec = Vector2.Zero;

            foreach(Collider2D localCollider in Subcolliders) {
                foreach (Collider2D collider in GameManager.CurrentScene.Collider2Ds) {
                    Vector2 otherVelocity = collider.AttachedRigidbody == null ? Vector2.Zero : collider.AttachedRigidbody.Velocity;
                    CollisionResult2D result = Bounds.DetectCollision(localCollider.Bounds, collider.Bounds, Velocity, otherVelocity);

                    if (result.WillIntersect) {
                        pushbackVec += result.MinimumTranslationVector;

                        if (!willCollide) willCollide = true;
                        if (!localCollider.IsTrigger && !nonTriggerCollision) nonTriggerCollision = true;
                    }


                    if (result.WillIntersect && !result.Intersecting) {
                        if (localCollider.IsTrigger) {
                            OnTriggerEnter2D_Direct(collider);
                            localCollider.OnTriggerEnter2D_Direct(collider);
                            collider.OnTriggerEnter2D_Direct(localCollider);
                        } else {
                            OnCollisionEnter2D_Direct(collider);
                            localCollider.OnCollisionEnter2D_Direct(collider);
                            collider.OnCollisionEnter2D_Direct(localCollider);
                        }
                    } else if (result.WillIntersect && result.Intersecting) {
                        if (localCollider.IsTrigger) {
                            OnTriggerStay2D_Direct(collider);
                            localCollider.OnTriggerStay2D_Direct(collider);
                            collider.OnTriggerStay2D_Direct(localCollider);
                        } else {
                            OnCollisionStay2D_Direct(collider);
                            localCollider.OnCollisionStay2D_Direct(collider);
                            collider.OnCollisionStay2D_Direct(localCollider);
                        }
                    } else if (!result.WillIntersect && result.Intersecting) {
                        if (localCollider.IsTrigger) {
                            OnTriggerExit2D_Direct(collider);
                            localCollider.OnTriggerExit2D_Direct(collider);
                            collider.OnTriggerExit2D_Direct(localCollider);
                        } else {
                            OnCollisionExit2D_Direct(collider);
                            MainCollider.OnCollisionExit2D_Direct(collider);
                            collider.OnCollisionExit2D_Direct(MainCollider);
                        }
                    }
                }

                
            }


            if (willCollide && nonTriggerCollision) {
                _position += (Velocity + pushbackVec) * Time.fixedDeltaTime;
            } else {
                _position += Velocity * Time.fixedDeltaTime;
            }

            if (Drag != 0) _position -= Velocity * Drag * Time.fixedDeltaTime;
            transform.Position = _position.ToVector3();
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
