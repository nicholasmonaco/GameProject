using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    public class CircleBounds : AbstractBounds {

        private float _origRadius = 1;
        public float Radius = 1;

        public Vector2 RadiusScale = Vector2.One;


        
        public CircleBounds(Vector2 center, Vector2 offset, float radius) {
            ResetBounds(center + offset, radius);
        }

        public CircleBounds(Vector2 center, float radius) : this(center, Vector2.Zero, radius) { }


        public void ResetBounds(Vector2 newCenter, float newRadius) {
            OrigCenter = newCenter;
            Center = newCenter;
            _origRadius = newRadius;
            Radius = newRadius;
        }

        public override void ApplyWorldMatrix(Transform worldTransform) {
            Center = Vector3.Transform(OrigCenter.ToVector3(), worldTransform.WorldMatrix).ToVector2();
            RadiusScale = worldTransform.Scale.ToVector2();

            if (Debug.ShowColliders) {
                if (ParentCollider != null)
                    (ParentCollider as CircleCollider2D).WorldMatrixChanged = true;
            }
        }

        public override void Destroy() {
            ParentCollider = null;

            _origRadius = 0;
            Radius = 0;
            RadiusScale = Vector2.Zero;
            // Note: none of this should be needed, but it may as well be implemented as a failsafe
        }


        public static CollisionResult2D DetectCircleCollision(CircleBounds colliderA, CircleBounds colliderB, Vector2 velocityA, Vector2 velocityB) {
            Vector2 velocity = (velocityA - velocityB) * Time.fixedDeltaTime; // Relative velocity
            CollisionResult2D result = new CollisionResult2D();
            result.Intersecting = true;
            result.WillIntersect = true;

            
            // Step 1: Check if currently intersecting
            result.Intersecting = (colliderB.Center - colliderA.Center).Length() < (colliderB.Radius + colliderA.Radius);

            // Step 2: Check if will intersect
            Vector2 cA_v = colliderA.Center + velocityA * Time.fixedDeltaTime;
            Vector2 cB_v = colliderB.Center + velocityB * Time.fixedDeltaTime;
            result.WillIntersect = (cB_v - cA_v).Length() < (colliderB.Radius + colliderA.Radius);

            //Debug.Log($"Colliding: {result.Intersecting} | WillCollide: {result.WillIntersect}");
            //Debug.Log($"CenterDist: {(colliderB.Center - colliderA.Center).Length()} | Radius difference: {colliderB.Radius + colliderA.Radius}");
            //Debug.Log($"Pos: ({colliderA.Center.X}, {colliderA.Center.Y})");

            // Step 3: Set to mintranslationVector

            //difference of radii - actual measured distance
            Vector2 radiusDist = Vector2.Normalize(cB_v - cA_v) * (colliderB.Radius + colliderA.Radius);
            Vector2 actualDist = (cB_v - cA_v);
            Vector2 pushback = radiusDist - actualDist;
            result.MinimumTranslationVector = -pushback; //maybe not negative, but i think its right

            return result;
        }
    }
}
