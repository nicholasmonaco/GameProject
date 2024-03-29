﻿using System;
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

        protected bool _applyWorldMatrix = true;

        //debug
        public Vector2 bottomLeft;
        public Vector2 topRight;
        //


        public Collider2D ParentCollider;

        public BoundsType BoundsType { get; set; }



        public abstract void ApplyWorldMatrix(Transform worldTransform);
        public virtual void ResolveCorners() { }

        public abstract void Destroy();



        public static CollisionResult2D DetectCircleRectangleCollision(CircleBounds cir, PolygonBounds rect, Vector2 velocityCir, Vector2 velocityRect) {
            CollisionResult2D result = new CollisionResult2D();
            result.Intersecting = false;
            result.WillIntersect = false;


            // Step 1: Check if currently intersecting
            //this is what we need to change - it has to find these automatically, not by index, as matrix transformations change these
            Vector2 bottomLeftRectPoint = rect.bottomLeft;
            Vector2 topRightRectPoint = rect.topRight;


            Vector2 nearest = Vector2.Clamp(cir.Center, bottomLeftRectPoint, topRightRectPoint);
            Vector2 difference = nearest - cir.Center;

            result.Intersecting = cir.Radius * cir.Radius >= difference.LengthSquared();


            // Step 2: Check if will intersect
            bottomLeftRectPoint += velocityRect * Time.fixedDeltaTime;
            topRightRectPoint += velocityRect * Time.fixedDeltaTime;
            Vector2 newCirCenter = cir.Center + velocityCir * Time.fixedDeltaTime;

            nearest = Vector2.Clamp(newCirCenter, bottomLeftRectPoint, topRightRectPoint);
            difference = nearest - newCirCenter;

            if (cir.Radius * cir.Radius >= difference.LengthSquared()) {
                result.WillIntersect = true;

                // Step 3: Set minTranslationVector
                result.MinimumTranslationVector = (Vector2.Normalize(difference) * cir.Radius) - difference;

                // This happens if the difference is (0,0), which is technically undefined behavior.
                if (float.IsNaN(result.MinimumTranslationVector.X)) { 
                    result.MinimumTranslationVector = Vector2.Zero;
                } else {
                    result.MinimumTranslationVector *= -1;
                }
            } else {
                result.MinimumTranslationVector = Vector2.Zero;
            }


            return result;
        }



        /// <summary>
        /// Detects the collision between a polygon and a circle.
        /// Original logic and code layout from 
        /// http://www.jeffreythompson.org/collision-detection/poly-circle.php
        /// </summary>
        /// <param name="cir">The CircleBounds involved in the collision.</param>
        /// <param name="poly">The PolygonBounds involved in the position.</param>
        /// <returns>A CollisionResult2D containing data about the collision.</returns>
        public static CollisionResult2D DetectDualTypeCollision(CircleBounds cir, PolygonBounds poly, Vector2 velocityCir, Vector2 velocityPoly) {
            CollisionResult2D result = new CollisionResult2D();
            result.Intersecting = false;
            result.WillIntersect = false;

            Vector2 minPushback = Vector2.Zero;

            //Vector2 velocity = velocityCir - velocityPoly; // Relative velocity


            int next;
            for (int current = 0; current < poly._points.Length; current++) {
                // Get next point in list
                next = current + 1;
                if (next == poly._points.Length) next = 0;

                // Get the points important to our current position
                Vector2 currentPoint = poly._points[current];
                Vector2 nextPoint = poly._points[next];


                // Step 1: Check if the colliders are currently intersecting

                if (!result.Intersecting) {
                    // Check for collision between the circle and the line formed from the two points
                    bool colliding = DetectLineCircleCollision(currentPoint, nextPoint, cir.Center, cir.Radius, out _);
                    if (colliding) {
                        result.Intersecting = true;
                    }
                }


                // Step 2: Find if the colliders will intersect
                currentPoint += velocityPoly * Time.fixedDeltaTime;
                nextPoint += velocityPoly * Time.fixedDeltaTime;

                Vector2 repulseVec;
                bool willCollide = DetectLineCircleCollision(currentPoint, nextPoint, cir.Center + velocityCir * Time.fixedDeltaTime, cir.Radius, out repulseVec);
                if (!result.WillIntersect && willCollide) result.WillIntersect = true;

                //if (!result.Intersecting && !willCollide) continue; // or should it be break? check later

                // Step 3: Add the minimum translation vector to pushback on the collider so that it will move back
                minPushback += repulseVec;
            }

            result.MinimumTranslationVector = minPushback;

            return result;
        }

        /// <summary>
        /// Detects if a circle is intersecting a line.
        /// Original logic and code from 
        /// http://www.jeffreythompson.org/collision-detection/poly-circle.php
        /// </summary>
        /// <param name="currentPoint">One endpoint of the line</param>
        /// <param name="nextPoint">The other endpoint of the line</param>
        /// <param name="circCenter">The center of the circle</param>
        /// <param name="circRadius">The radius of the circle</param>
        /// <returns>If the circle is intersecting with the line</returns>
        private static bool DetectLineCircleCollision(Vector2 currentPoint, Vector2 nextPoint, Vector2 circCenter, float circRadius, out Vector2 repulseVec) {
            // First, check that the ends of the lines aren't in the circle
            if(DetectPointCircleCollision(currentPoint, circCenter, circRadius) || 
               DetectPointCircleCollision(nextPoint, circCenter, circRadius)) {
                repulseVec = Vector2.Zero;
                return true;
            }

            // Get the length of the line
            float length = Vector2.Distance(currentPoint, nextPoint);

            // Get the dot product of the line and the circle
            float dot = (((circCenter.X - currentPoint.X) * (nextPoint.X - currentPoint.X)) + ((circCenter.Y - currentPoint.Y) * (nextPoint.Y-currentPoint.Y))) / MathF.Pow(length, 2);

            // Find closest point on the line to the circle
            Vector2 closest = new Vector2(currentPoint.X + (dot * (nextPoint.X - currentPoint.X)),
                                          currentPoint.Y + (dot * (nextPoint.Y - currentPoint.Y)));

            // Check that the point is actually on the line
            float distSum = Vector2.Distance(closest, currentPoint) + Vector2.Distance(closest, nextPoint);
            if (!(distSum >= length - 0.05f && distSum <= length + 0.05f)) { //this value doesnt seem to change much, i wouldn't worry about it
                repulseVec = Vector2.Zero;
                return false; 
            }

            // Get distance from center of circle to closest point
            float dist = Vector2.Distance(closest, circCenter);

            // Get the repulsion vector we need to adjust the position by to make sure we dont collide
            Vector2 closestCirclePoint = circCenter + Vector2.Normalize(closest - circCenter) * circRadius;
            repulseVec = (closestCirclePoint - closest) * Time.fixedDeltaTime;
            
            // Return if the circle is on the line
            return dist <= circRadius;
        }

        /// <summary>
        /// Detects if a point is within a circle.
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <param name="circCenter">The center of the circle</param>
        /// <param name="circRadius">The radius of the circle</param>
        /// <returns>If the point is within the circle</returns>
        public static bool DetectPointCircleCollision(Vector2 point, Vector2 circCenter, float circRadius) {
            float dist = Vector2.Distance(point, circCenter);
            return dist <= circRadius;
        }


    }
}
public struct CollisionResult2D {
    public bool WillIntersect;                  // Will they intersect next?
    public bool Intersecting;                   // Currently intersecting?
    public Vector2 MinimumTranslationVector;    // The motion to apply to collider A in order to push the polygons apart.
}

public enum BoundsType {
    Rectangle,
    Polygon,
    Circle
}