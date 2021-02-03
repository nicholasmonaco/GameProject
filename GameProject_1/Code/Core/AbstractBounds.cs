using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    public abstract class AbstractBounds {

        public delegate Vector2 Vector2Return();
        public Vector2 OrigCenter = Vector2.Zero;
        public Vector2 Center = Vector2.Zero;

        public Collider2D ParentCollider;



        public abstract void ApplyWorldMatrix(Transform worldTransform);



        /// <summary>
        /// Detects the collision between a polygon and a circle.
        /// Original logic and code layout from 
        /// https://stackoverflow.com/questions/43485700/xna-monogame-detecting-collision-between-circle-and-rectangle-not-working
        /// </summary>
        /// <param name="cir">The CircleBounds involved in the collision.</param>
        /// <param name="poly">The PolygonBounds involved in the position.</param>
        /// <returns>A CollisionResult2D containing data about the collision.</returns>
        //private static CollisionResult2D DetectDualTypeCollision(CircleBounds cir, PolygonBounds poly) {

        //    // Get the rectangle half width and height
        //    float rW = (rect.Width) / 2;
        //    float rH = (rect.Height) / 2;

        //    // Get the positive distance. This exploits the symmetry so that we now are
        //    // just solving for one corner of the rectangle (memory tell me it fabs for 
        //    // floats but I could be wrong and its abs)
        //    float distX = Math.Abs(cir.Center.X - (rect.Left + rW));
        //    float distY = Math.Abs(cir.Center.Y - (rect.Top + rH));

        //    if (distX >= cir.Radius + rW || distY >= cir.Radius + rH) {
        //        // Outside see diagram circle E
        //        return false;
        //    }
        //    if (distX < rW || distY < rH) {
        //        // Inside see diagram circles A and B
        //        return true; // touching
        //    }

        //    // Now only circles C and D left to test
        //    // get the distance to the corner
        //    distX -= rW;
        //    distY -= rH;

        //    // Find distance to corner and compare to circle radius 
        //    // (squared and the sqrt root is not needed
        //    if (distX * distX + distY * distY < cir.Radius * cir.Radius) {
        //        // Touching see diagram circle C
        //        return true;
        //    }
        //    return false;
        //}
    }
}
public struct CollisionResult2D {
    public bool WillIntersect;                  // Will they intersect next?
    public bool Intersecting;                   // Currently intersecting?
    public Vector2 MinimumTranslationVector;    // The motion to apply to collider A in order to push the polygons apart.
}