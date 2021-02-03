using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    public class CircleBounds : AbstractBounds {

        private float _origRadius = 1;
        private float _radius = 1;

        private Vector2 _radiusScale = Vector2.One;


        
        public CircleBounds(Vector2 center, Vector2 offset, float radius) {
            ResetBounds(center + offset, radius);
        }

        public CircleBounds(Vector2 center, float radius) : this(center, Vector2.Zero, radius) { }


        public void ResetBounds(Vector2 newCenter, float newRadius) {
            OrigCenter = newCenter;
            Center = newCenter;
            _origRadius = newRadius;
            _radius = newRadius;
        }

        public override void ApplyWorldMatrix(Transform worldTransform) {

            Center = Vector3.Transform(OrigCenter.ToVector3(), worldTransform.WorldMatrix).ToVector2();
            _radiusScale = worldTransform.Scale.ToVector2();

            if(GameManager.Debug) (ParentCollider as CircleCollider2D).WorldMatrixChanged = true;
        }


        public void DetectCircleCollision(CircleBounds colliderA, CircleBounds colliderB, Vector2 velocityA, Vector2 velocityB) {
            Vector2 velocity = (velocityA - velocityB) * Time.fixedDeltaTime; // Relative velocity
            CollisionResult2D result = new CollisionResult2D();
            result.Intersecting = true;
            result.WillIntersect = true;

            //TODO
        }
    }
}
