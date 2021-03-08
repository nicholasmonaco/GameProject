// CircleCollider2D.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core.Components {
    public class CircleCollider2D : Collider2D {

        private CircleBounds CircBounds => Bounds as CircleBounds;

        public bool WorldMatrixChanged = false;
        private Vector2[] _debugPoints;



        public CircleCollider2D(GameObject attached) : base(attached) {
            Bounds = new CircleBounds(Vector2.Zero, Vector2.Zero, 1);

            Bounds.ParentCollider = this;
            WorldMatrixChanged = true;
        }

        public CircleCollider2D(GameObject attached, Vector2 center, Vector2 offset, float radius) : base(attached) {
            Bounds = new CircleBounds(center, offset, radius);

            Bounds.ParentCollider = this;
            Size = new Vector2(radius, radius) * 2;
            WorldMatrixChanged = true;
        }

        public CircleCollider2D(GameObject attached, Vector2 center, float radius) : this(attached, center, Vector2.Zero, radius) { }
        public CircleCollider2D(GameObject attached, float radius) : this(attached, Vector2.Zero, Vector2.Zero, radius) { }

        public CircleCollider2D(GameObject attached, SpriteRenderer sr) : this(attached,
                                                                               Vector2.Zero,
                                                                               Vector2.Zero,
                                                                               sr.Sprite.Width/2f * sr.SpriteScale.X * sr.transform.Scale.X) { }



        public override void Draw(SpriteBatch sb) {
            if (!Debug.ShowColliders) return;

            if (WorldMatrixChanged) {
                // Approximate as n-gon
                float radius = CircBounds.Radius;
                Vector2 pos = CircBounds.Center;

                int pointCount = 20;
                float div = MathF.PI * 2 / pointCount;
                _debugPoints = new Vector2[pointCount];
                for (int i = 0; i < pointCount; i++) {
                    float divI = div * i;
                    _debugPoints[i] = new Vector2(MathF.Cos(divI), MathF.Sin(divI)) * radius + pos;
                }
                _debugPoints[pointCount - 1] = _debugPoints[0];
                WorldMatrixChanged = false;
            }

            if (_debugPoints == null) return;
            for (int i = 0; i < _debugPoints.Length - 1; i++) {
                DrawLine(sb, _debugPoints[i], _debugPoints[i + 1]);
            }
            //DrawLine(sb, _debugPoints[^1], _debugPoints[0]);
        }
    }
}
