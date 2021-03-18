using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    public class RectangleF {
        private static readonly RectangleF _empty;
        private static readonly RectangleF _unit;
        public static RectangleF Empty => _empty;
        public static RectangleF Unit => _unit;

        static RectangleF() {
            _empty = new RectangleF();
            _unit = new RectangleF(new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, -0.5f));
        }


        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        public float Left => Min.X;
        public float Right => Max.X;
        public float Bottom => Min.Y;
        public float Top => Max.Y;


        public float Width => Max.X - Min.X;
        public float Height => Max.Y - Min.Y;

        public Vector2 Center => (Max + Min) / 2f;


        private RectangleF() {
            Min = Vector2.Zero;
            Max = Vector2.Zero;
        }

        public RectangleF(Vector2 min, Vector2 max) {
            Min = min;
            Max = max;
        }

        public RectangleF(float x, float y, float width, float height) {
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;
            
            Min = new Vector2(x - halfWidth, y - halfHeight);
            Max = new Vector2(x + halfWidth, y + halfHeight);
        }

        public RectangleF(Vector2 center, float width, float height) {
            Vector2 half = new Vector2(width / 2f, height / 2f);

            Min = center - half;
            Max = center + half;
        }


        public bool Contains(Vector2 point) {
            return point.X >= Min.X &&
                   point.X <= Max.X &&
                   point.Y >= Min.Y &&
                   point.Y <= Max.Y;
        }
    }
}
