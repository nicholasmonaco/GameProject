// Collider2D.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Components {
    
    /// <summary>
    /// The class governing the generic 2-dimensional collider.
    /// Colliders inheriting from this type are (currently) able to be any polygonal shape.
    /// </summary>
    public class Collider2D : Component {

        public bool IsTrigger = false;
        public Rigidbody2D AttachedRigidbody = null;
        public Bounds Bounds;

        public Dictionary<Collider2D, bool> Entered;


        public Collider2D(GameObject attached) : base(attached) {
            GameManager.CurrentScene.Collider2Ds.Add(this);
        }

        public override void Destroy() {
            base.Destroy();
            GameManager.CurrentScene.Collider2Ds.Remove(this);
            AttachedRigidbody.Subcolliders.Remove(this);
        }

        public override void PreAwake() {
            base.PreAwake();

            Entered = new Dictionary<Collider2D, bool>(GameManager.CurrentScene.Collider2Ds.Count);

            FindAttachedRigidbody();

            Bounds.ApplyWorldMatrix(transform.WorldMatrix);
            transform.WorldMatrixUpdateAction += () => { Bounds.ApplyWorldMatrix(transform.WorldMatrix); };
        }

        private void FindAttachedRigidbody() {
            Transform t = transform;
            while (t != null) {
                foreach (Component c in gameObject._components) {
                    if (c is Rigidbody2D rb) {
                        AttachedRigidbody = rb;
                        AttachedRigidbody.Subcolliders.Add(this);
                        return;
                    }
                }
                t = t.Parent;
            }
        }


        public float Distance(Collider2D other) {
            return 5;
            // min distance between this collider and other
        }

        public bool IsTouching(Collider2D other) {
            return Bounds.IsOverlapping(other.Bounds);
        }

        //public void DoCollision() {
        //    foreach(Collider2D c in GameManager.CurrentScene.Collider2Ds) {
        //        Bounds.IsOverlapping(c.Bounds);
        //    }
        //}



        public Action<Collider2D> OnCollisionEnter2D_Direct = (other) => {};
        public Action<Collider2D> OnCollisionStay2D_Direct = (other) => { };
        public Action<Collider2D> OnCollisionExit2D_Direct = (other) => { };
        public Action<Collider2D> OnTriggerEnter2D_Direct = (other) => { };
        public Action<Collider2D> OnTriggerStay2D_Direct = (other) => { };
        public Action<Collider2D> OnTriggerExit2D_Direct = (other) => { };
    }
}
