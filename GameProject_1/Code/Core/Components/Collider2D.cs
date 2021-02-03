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
        public AbstractBounds Bounds;

        public Vector2 Size { get; protected set; } = Vector2.Zero;

        public Dictionary<Collider2D, bool> Entered;


        public Collider2D(GameObject attached) : base(attached) {
            GameManager.CurrentScene.Collider2Ds.Add(this);
        }

        public override void OnDestroy() {
            GameManager.CurrentScene.Collider2Ds.Remove(this);
            AttachedRigidbody.Subcolliders.Remove(this);
        }

        public override void PreAwake() {
            base.PreAwake();

            Entered = new Dictionary<Collider2D, bool>(GameManager.CurrentScene.Collider2Ds.Count);

            FindAttachedRigidbody();

            Bounds.ApplyWorldMatrix(transform);
            transform.WorldMatrixUpdateAction += () => { Bounds.ApplyWorldMatrix(transform); };
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
            throw new NotImplementedException();
            // min distance between this collider and other
        }



        


        protected static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end) {
            Vector2 edge = end - start;
            float angle = (float)MathF.Atan2(edge.Y, edge.X);

            sb.Draw(Resources.Sprite_Pixel,
                    start,
                    null,
                    Color.Lime,
                    angle,
                    Vector2.Zero,
                    new Vector2(edge.Length(), 0.5f).FlipY(),
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
