using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;
using GameProject.Code.Scenes;

namespace GameProject.Code.Core {
    public static class Physics2D {


        public static Collider2D[] Raycast(Ray2D ray, float distance, int layerMask) {
            List<Collider2D> hits = new List<Collider2D>();

            GameScene game = GameManager.CurrentScene as GameScene;
            foreach(Collider2D collider in game.Collider2Ds) {
                if (collider.Enabled == false) continue;

                bool hitRay = false;
                Vector2 collisionPoint = Vector2.Zero;
                switch (collider) {
                    case RectCollider2D _:
                    case PolygonCollider2D _:
                        hitRay = RayCollidesPoly(ray, distance, collider, out collisionPoint);
                        break;

                    case CircleCollider2D circleCollider:
                        hitRay = RayCollidesCircle(ray, distance, circleCollider, out collisionPoint);
                        break;
                }

                if (hitRay) {
                    hits.Add(collider);
                }
            }

            return hits.ToArray();
        }


        private static bool RayCollidesPoly(Ray2D ray, float maxDistance, Collider2D collider, out Vector2 intersection) {
            Vector2 rayEnd = ray.GetPoint(maxDistance);
            Vector2 rayLine = rayEnd - ray.Origin;

            Vector2[] points = (collider.Bounds as PolygonBounds)._points;
            intersection = Vector2.Zero;

            for (int i = 0; i < points.Length - 1; i++) {
                Vector2 edge = points[i + 1] - points[i];
                float dot = rayLine.X * edge.Y - rayLine.Y * edge.X;

                // If the lines are parallel:
                if (dot == 0) {
                    return false;
                }

                Vector2 startComp = ray.Origin - points[i];
                float dirComp = (startComp.X * edge.Y - startComp.Y * edge.X) / dot;
                if (dirComp < 0 || dirComp > 1) {
                    return false;
                }

                dirComp = (startComp.X * rayLine.Y - startComp.Y * rayLine.X) / dot;
                if (dirComp < 0 || dirComp > 1) {
                    return false;
                }

                intersection = ray.Origin + startComp * rayLine;

                return true;
            }

            return false;
        }

        private static bool RayCollidesCircle(Ray2D ray, float maxDistance, CircleCollider2D collider, out Vector2 intersection) {
            Vector2 rayOrigin = ray.Origin;
            Vector2 rayEnd = ray.GetPoint(maxDistance);

            intersection = Vector2.Zero;

            Vector2 circCenter = collider.Bounds.Center;
            float circleRadius = (collider.Bounds as CircleBounds).Radius;

            // First, check that the ends of the lines aren't in the circle
            if(AbstractBounds.DetectPointCircleCollision(rayOrigin, circCenter, circleRadius)) {
                return true;
            }else if(AbstractBounds.DetectPointCircleCollision(rayEnd, circCenter, circleRadius)) {
                intersection = rayEnd;
                return true;
            }

            // Get the length of the line
            float length = Vector2.Distance(rayOrigin, rayEnd);

            // Get the dot product of the line and the circle
            float dot = (((circCenter.X - rayOrigin.X) * (rayEnd.X - rayOrigin.X)) + 
                         ((circCenter.Y - rayOrigin.Y) * (rayEnd.Y - rayOrigin.Y))) / (length * length);

            // Find closest point on the line to the circle
            Vector2 closest = new Vector2(rayOrigin.X + (dot * (rayEnd.X - rayOrigin.X)),
                                          rayOrigin.Y + (dot * (rayEnd.Y - rayOrigin.Y)));

            // Check that the point is actually on the line
            float distSum = Vector2.Distance(closest, rayOrigin) + Vector2.Distance(closest, rayEnd);
            if (!(distSum >= length - 0.05f && distSum <= length + 0.05f)) { //this value doesnt seem to change much, i wouldn't worry about it
                return false;
            }

            // Get distance from center of circle to closest point
            float dist = Vector2.Distance(closest, circCenter);

            // Return if the circle is on the line
            return dist <= circleRadius;
        }

    }
}
