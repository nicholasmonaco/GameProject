using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;
using GameProject.Code.Scenes;

namespace GameProject.Code.Core {
    public static class Physics2D {

        public static bool Raycast(Ray2D ray, float distance, int layerMask, out RaycastHit2D hitData) {
            RaycastHit2D[] hits;
            if(RaycastAll(ray, distance, layerMask, out hits)) {
                float minFrac = float.MaxValue;
                int id = 0;
                for(int i = 0; i < hits.Length; i++) {
                    if(hits[i].Fraction < minFrac) {
                        id = i;
                        minFrac = hits[i].Fraction;
                    }
                }

                hitData = hits[id];
                return true;
            } else {
                hitData = new RaycastHit2D();
                return false;
            }
        }

        public static bool Raycast_List(Ray2D ray, float distance, int layerMask, ICollection<Collider2D> hitlist, out RaycastHit2D hitData) {
            RaycastHit2D[] hits;
            if (RaycastAll_List(ray, distance, layerMask, hitlist, out hits)) {
                float minFrac = float.MaxValue;
                int id = 0;
                for (int i = 0; i < hits.Length; i++) {
                    if (hits[i].Fraction < minFrac) {
                        id = i;
                        minFrac = hits[i].Fraction;
                    }
                }

                hitData = hits[id];
                return true;
            } else {
                hitData = new RaycastHit2D();
                return false;
            }
        }



        public static bool LayerInMask(LayerID layer, int mask) {
            //if (layer == LayerID.None) return true; // This can be either way, I don't think it'll ever be used.

            return (mask & ((int)layer)) != 0;
        }

        public static int GetMask(params LayerID[] parameters) {
            int mask = 0;
            foreach(LayerID layer in parameters) {
                mask |= (int)layer;
            }
            return mask;
        }


        public static bool RaycastAll(Ray2D ray, float distance, int layerMask, out RaycastHit2D[] hits) {
            //List<RaycastHit2D> rayhits = new List<RaycastHit2D>();

            GameScene game = GameManager.CurrentScene as GameScene;
            return RaycastAll_List(ray, distance, layerMask, game.Collider2Ds, out hits);

            //foreach(Collider2D collider in game.Collider2Ds) {
            //    if (collider.Enabled == false || !LayerInMask(collider.Layer, layerMask)) continue;

            //    bool hitRay = false;
            //    Vector2 collisionPoint = Vector2.Zero;
            //    switch (collider) {
            //        case RectCollider2D _:
            //        case PolygonCollider2D _:
            //            hitRay = RayCollidesPoly(ray, distance, collider, out collisionPoint);
            //            break;

            //        case CircleCollider2D circleCollider:
            //            hitRay = RayCollidesCircle(ray, distance, circleCollider, out collisionPoint);
            //            break;
            //    }

            //    if (hitRay) {
            //        RaycastHit2D hit;
            //        hit.Origin = ray.Origin;
            //        hit.Point = collisionPoint;
            //        hit.RaycastDirection = ray.Direction;
            //        hit.NormalDirection = Vector2.Zero; //todo
            //        hit.Distance = Vector2.Distance(collisionPoint, ray.Origin);
            //        hit.Fraction = hit.Distance / distance;
            //        hit.HitCollider = collider;

            //        rayhits.Add(hit);
            //    }
            //}

            //hits = rayhits.ToArray();

            //return rayhits.Count > 0;
        }

        public static bool RaycastAll_List(Ray2D ray, float distance, int layerMask, ICollection<Collider2D> hitlist, out RaycastHit2D[] hits) {
            List<RaycastHit2D> rayhits = new List<RaycastHit2D>();

            foreach (Collider2D collider in hitlist) {
                if (collider.Enabled == false || !LayerInMask(collider.Layer, layerMask)) continue;

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
                    RaycastHit2D hit;
                    hit.Origin = ray.Origin;
                    hit.Point = collisionPoint;
                    hit.RaycastDirection = ray.Direction;
                    hit.NormalDirection = Vector2.Zero; //todo
                    hit.Distance = Vector2.Distance(collisionPoint, ray.Origin);
                    hit.Fraction = hit.Distance / distance;
                    hit.HitCollider = collider;

                    rayhits.Add(hit);
                }
            }

            hits = rayhits.ToArray();

            return rayhits.Count > 0;
        }


        private static bool RayCollidesPoly(Ray2D ray, float maxDistance, Collider2D collider, out Vector2 intersection) {
            //somethings wrong with this method - it isnt hitting right. also the circle one hits through spikes becasue theyre small circles
            Vector2 rayEnd = ray.GetPoint(maxDistance);
            Vector2 rayLine = rayEnd - ray.Origin;

            Vector2[] points = (collider.Bounds as PolygonBounds)._points;
            intersection = Vector2.Zero;

            for (int i = 0; i < points.Length - 1; i++) {
                Vector2 edge = points[i + 1] - points[i];
                float dot = rayLine.X * edge.Y - rayLine.Y * edge.X;

                // If the lines are parallel:
                if (dot == 0) {
                    //return false;
                    continue;
                }

                Vector2 startComp = points[i] - ray.Origin; //order switched
                float dirComp = (startComp.X * edge.Y - startComp.Y * edge.X) / dot;
                if (dirComp < 0 || dirComp > 1) {
                    //return false;
                    continue;
                }

                float dirComp2 = (startComp.X * rayLine.Y - startComp.Y * rayLine.X) / dot;
                if (dirComp2 < 0 || dirComp2 > 1) {
                    //return false;
                    continue;
                }

                intersection = ray.Origin + dirComp * rayLine;

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
