// Collider2D.cs - Nick Monaco

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

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



        public override void Draw(SpriteBatch sb) {
            for (int i = 0; i < Bounds._points.Length-1; i++) {
                DrawLine(sb, transform, Bounds._points[i], Bounds._points[i + 1]);
            }
        }


        private static void DrawLine(SpriteBatch sb, Transform t, Vector2 start, Vector2 end) {
            Vector2 edge = end - start;
            float angle = (float)MathF.Atan2(edge.Y, edge.X);

            sb.Draw(Resources.Sprite_Pixel,
                    start,
                    null,
                    Color.Lime,
                    angle,
                    Vector2.Zero,
                    new Vector2(edge.Length(), 1).FlipY(),
                    SpriteEffects.None,
                    0.9f);
        }



        public Action<Collider2D> OnCollisionEnter2D_Direct = (other) => {};
        public Action<Collider2D> OnCollisionStay2D_Direct = (other) => { };
        public Action<Collider2D> OnCollisionExit2D_Direct = (other) => { };
        public Action<Collider2D> OnTriggerEnter2D_Direct = (other) => { };
        public Action<Collider2D> OnTriggerStay2D_Direct = (other) => { };
        public Action<Collider2D> OnTriggerExit2D_Direct = (other) => { };
    }
}
