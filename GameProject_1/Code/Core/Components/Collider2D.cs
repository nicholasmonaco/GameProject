using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Components {
    public class Collider2D : Component {

        public bool IsTrigger = false;
        public Rigidbody2D AttachedRigidbody = null;
        public Bounds Bounds;


        public Collider2D(GameObject attached) : base(attached) {
            GameManager.CurrentScene.Collider2Ds.Add(this);
        }

        public override void PreAwake() {
            base.PreAwake();

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
