using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core.PathFinding {
    public class PathNode : IEquatable<PathNode> {
        public Point Position {
            get => new Point(X, Y);
            set {
                X = value.X;
                Y = value.Y;
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public PathNode Parent { get; set; }



        public void SetDistance(int targetX, int targetY) {
            Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
        }

        public void SetDistance(Point p) {
            Distance = Math.Abs(p.X - X) + Math.Abs(p.Y - Y);
        }


        public PathNode() { }

        public PathNode(int x, int y) {
            X = x;
            Y = y;
        }

        public PathNode(Point position) : this(position.X, position.Y) { }

        public PathNode(Vector2 position) : this((int)position.X, (int)position.Y) { }


        public bool SamePosition(PathNode other) {
            if (other == null) return false;
            return X == other.X && Y == other.Y;
        }


        public override int GetHashCode() {
            return X.GetHashCode() * Y.GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj.GetHashCode() == this.GetHashCode();
        }

        public bool Equals([AllowNull] PathNode other) {
            return SamePosition(other);
        }


        //public static bool operator == (PathNode node1, PathNode node2) {
        //    if (node1 == null || node2 == null) return Object.Equals(node1, node2);

        //    return node1.Equals(node2);
        //}

        //public static bool operator != (PathNode node1, PathNode node2) {
        //    if (node1 == null || node2 == null) return !Object.Equals(node1, node2);

        //    return !node1.Equals(node2);
        //}
    }
}
