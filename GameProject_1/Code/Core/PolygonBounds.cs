// Bounds.cs - Nick Monaco
// Polygon-polygon collision detection logic from:
// https://www.codeproject.com/Articles/15573/2D-Polygon-Collision-Detection

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Data structure representing the boundaries of a polygonal shape.
    /// </summary>
    public class PolygonBounds : AbstractBounds {
        private Vector2[] _origPoints;
        public Vector2[] _points { get; private set; }

        private Vector2[] _edges;

        
        public PolygonBounds() { }

        public PolygonBounds(Vector2[] shapePoints, bool preClosed) {
            ResetBounds(shapePoints, preClosed);
        }

        public void ResetBounds(Vector2[] newPoints, bool preClosed) {
            int additive = preClosed ? 0 : 1;
            _origPoints = new Vector2[newPoints.Length + additive];
            _points = new Vector2[newPoints.Length + additive];

            Array.Copy(newPoints, _points, newPoints.Length);
            Array.Copy(newPoints, _origPoints, newPoints.Length);

            if (!preClosed) {
                _points[^1] = _points[0]; // This is apparantly able to use this indexing operator thing, very cool
                _origPoints[^1] = _origPoints[0];
            }

            _edges = new Vector2[_origPoints.Length - 1]; // As the shape will connect, the last point and first point are the same
            ComputeEdges();
        }


        private void ComputeEdges() {
            //direction that points from dir a to b
            //vec dir = b.position - a.position

            for(int i = 0; i < _edges.Length; i++) {
                _edges[i] = Vector2.Normalize(_points[i + 1] - _points[i]);
            }
        }



        public override void ApplyWorldMatrix(Transform worldTransform) {
            for(int i = 0; i < _points.Length; i++) {
                _points[i] = Vector3.Transform(_origPoints[i].ToVector3(), worldTransform.WorldMatrix).ToVector2();
            }

            Center = Vector3.Transform(OrigCenter.ToVector3(), worldTransform.WorldMatrix).ToVector2();
            ComputeEdges();
        }


        // New Collision Algorithm - Seperating Axis Theorem

        public static CollisionResult2D DetectPolygonCollision(PolygonBounds colliderA, PolygonBounds colliderB, Vector2 velocityA, Vector2 velocityB) {
            Vector2 velocity = (velocityA - velocityB) * Time.fixedDeltaTime; // Relative velocity
            CollisionResult2D result = new CollisionResult2D();
            result.Intersecting = true;
            result.WillIntersect = true;

            int edgeCountA = colliderA._edges.Length;
            int edgeCountB = colliderB._edges.Length;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = Vector2.Zero;
            Vector2 edge;

            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++) {
                if(edgeIndex < edgeCountA) {
                    edge = colliderA._edges[edgeIndex];
                } else {
                    edge = colliderB._edges[edgeIndex - edgeCountA];
                }

                // Step 1: Find if the colliders are currently intersecting
                Vector2 axis = new Vector2(-edge.Y, edge.X); // Perpendicular
                axis.Normalize();

                float minA = 0;
                float minB = 0;
                float maxA = 0;
                float maxB = 0;
                ProjectCollider(axis, colliderA, ref minA, ref maxA);
                ProjectCollider(axis, colliderB, ref minB, ref maxB);

                if(IntervalDistance(minA, maxA, minB, maxB) > 0) {
                    result.Intersecting = false;
                }


                // Step 2: Find if the colliders will intersect
                float velocityProjection = Vector2.Dot(axis, velocity);

                if(velocityProjection < 0) {
                    minA += velocityProjection;
                } else {
                    maxA += velocityProjection;
                }

                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.WillIntersect = false;

                if (!result.Intersecting && !result.WillIntersect) break; // Exit the loop if they won't collide

                intervalDistance = MathF.Abs(intervalDistance);
                if(intervalDistance < minIntervalDistance) {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = colliderA.Center - colliderB.Center;
                    if(Vector2.Dot(d, translationAxis) < 0) {
                        translationAxis = -translationAxis;
                    }
                }
            }

            if (result.WillIntersect) {
                result.MinimumTranslationVector = translationAxis * minIntervalDistance;
            }

            return result;
        }

        private static void ProjectCollider(Vector2 axis, PolygonBounds bounds, ref float min, ref float max) {
            float dot = Vector2.Dot(axis, bounds._points[0]);
            min = dot;
            max = dot;

            for(int i = 0; i < bounds._points.Length-1; i++) {                
                dot = Vector2.Dot(bounds._points[i], axis);
                if(dot < min) {
                    min = dot;
                } else {
                    if(dot > max) {
                        max = dot;
                    }
                }
            }
        }

        private static float IntervalDistance(float minA, float maxA, float minB, float maxB) {
            if(minA < minB) {
                return minB - maxA;
            } else {
                return minA - maxB;
            }
        }


        public Vector2 GetRectCenter() {
            return new Vector2(_points[0].X + (_points[1].X-_points[0].X) / 2, _points[2].Y + (_points[1].Y - _points[2].Y) / 2);
        }


        /// <summary>
        /// Finds the centroid of an n-sided, closed, convex, polygon.
        /// Logic and code sample from
        /// https://bell0bytes.eu/centroid-convex/
        /// </summary>
        /// <returns>The center of the polygon represented by the bound's points.</returns>
        public Vector2 GetPolygonCenter() {
            Vector2 centroid = Vector2.Zero;
            float determinant = 0;
            float tempDeterminant;
            int j;
            int pointCount = _points.Length;

            for(int i = 0; i < pointCount; i++) {
                if(i + 1 == pointCount) {
                    j = 0;
                } else {
                    j = i + 1;
                }

                tempDeterminant = _points[i].X * _points[j].Y - _points[j].X * _points[i].Y;
                determinant += tempDeterminant;

                centroid.X += (_points[i].X + _points[j].X) * tempDeterminant;
                centroid.Y += (_points[i].Y + _points[j].Y) * tempDeterminant;
            }

            centroid /= 3 * determinant;

            return centroid;
        }



        
    }
}


