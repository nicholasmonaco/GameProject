// Bounds.cs - Nick Monaco
// Intersection logic (currently unused) from:
// https://rbrundritt.wordpress.com/2008/10/20/approximate-points-of-intersection-of-two-line-segments/
// Polygon collidion detection logic from:
// https://www.codeproject.com/Articles/15573/2D-Polygon-Collision-Detection

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Data structure representing the boundaries of a polygonal shape.
    /// </summary>
    public class Bounds {
        private Vector2[] _origPoints;
        private Vector2[] _points;

        private Vector2[] _edges;

        public delegate Vector2 Vector2Return();
        public Vector2 OrigCenter = Vector2.Zero;
        public Vector2 Center = Vector2.Zero;

        public Bounds() { }

        public Bounds(Vector2[] shapePoints) {
            _origPoints = new Vector2[shapePoints.Length];
            _points = new Vector2[shapePoints.Length];
            Array.Copy(shapePoints, _points, shapePoints.Length);
            Array.Copy(shapePoints, _origPoints, shapePoints.Length);

            _edges = new Vector2[shapePoints.Length - 1]; // As the shape will connect, the last point and first point are the same
            ComputeEdges();
        }


        private void ComputeEdges() {
            //direction that points from dir a to b
            //vec dir = b.position - a.position

            for(int i = 0; i < _edges.Length; i++) {
                _edges[i] = _points[i + 1] - _points[i];
            }
        }

        public bool IsOverlapping(Bounds other) {
            if(_points.Length > 2 && other._points.Length > 2) {
                for(int i = 0; i < _points.Length-1; i++) {
                    for (int k = 0; k < other._points.Length-1; k++) {
                        if(GetIntersectionPoint(_points[i], _points[i+1], other._points[k], other._points[k+1], out _)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool GetIntersectionPoint(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersectionPoint) {
            //latitude is y
            float a1 = p2.Y - p1.Y;
            float b1 = p1.X - p2.X;
            float c1 = a1 * p1.X + b1 * p1.Y;

            float a2 = q2.Y - q1.Y;
            float b2 = q1.X - q2.X;
            float c2 = a2 * q1.X + b2 * q1.Y;

            float determinate = a1 * b2 - a2 * b2;

            intersectionPoint = Vector2.Zero;

            if(determinate != 0) {
                intersectionPoint = new Vector2((b2 * c1 - b1 * c2), (a1 * c2 - a2 * c1)) / determinate;

                if(InBoundingBox(p1, p2, intersectionPoint) && InBoundingBox(q1, q2, intersectionPoint)) {
                    return true;
                } else {
                    return false;
                }
            } else {
                // Lines are parallel, so no collision
                return false;
            }
        }

        private bool InBoundingBox(Vector2 r1, Vector2 r2, Vector2 r3) {
            bool betweenY;
            bool betweenX;

            if(r1.Y < r2.Y) {
                betweenY = (r1.Y <= r3.Y && r2.Y >= r3.Y);
            } else {
                betweenY = (r1.Y >= r3.Y && r2.Y <= r3.Y);
            }

            if(r1.X < r2.X) {
                betweenX = (r1.X <= r3.X && r2.X >= r3.X);
            } else {
                betweenX = (r1.X >= r3.X && r2.X <= r3.X);
            }

            return (betweenX && betweenY);
        }


        public void ApplyWorldMatrix(Matrix worldMatrix) {
            for(int i = 0; i < _points.Length; i++) {
                _points[i] = Vector3.Transform(_origPoints[i].ToVector3(), worldMatrix).ToVector2();
            }

            Center = Vector3.Transform(OrigCenter.ToVector3(), worldMatrix).ToVector2();
            ComputeEdges();
        }


        // New Collision Algorithm - Seperating Axis Theorem

        public static CollisionResult2D DetectCollision(Bounds colliderA, Bounds colliderB, Vector2 velocityA, Vector2 velocityB) {
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

        private static void ProjectCollider(Vector2 axis, Bounds bounds, ref float min, ref float max) {
            float dot = Vector2.Dot(axis, bounds._points[0]);
            min = dot;
            max = dot;

            for(int i = 0; i < bounds._points.Length; i++) {
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
    }
}

public struct CollisionResult2D {
    public bool WillIntersect; // Will they intersect next?
    public bool Intersecting; // Currently intersecting?
    public Vector2 MinimumTranslationVector; //The motion to apply to collider A in order to push the polygons apart.
}
