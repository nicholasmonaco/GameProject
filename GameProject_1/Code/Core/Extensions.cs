using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    public static class Extensions {

        public static void Set(this Vector2 value, float x, float y) {
            value.X = x;
            value.Y = y;
        }

        public static void Set(this Vector3 value, float x, float y, float z) {
            value.X = x;
            value.Y = y;
            value.Z = z;
        }

        public static Point ToPoint(this Vector2 value) {
            return new Point((int)value.X, (int)value.Y);
        }

        public static Point ToPoint2D(this Vector3 value) {
            return new Point((int)value.X, (int)value.Y);
        }

        public static Point Div(this Point value, float div) {
            return new Point((int)(value.X / div), (int)(value.Y / div));
        }

        public static Point Mult(this Point value, float mult) {
            return new Point((int)(value.X * mult), (int)(value.Y * mult));
        }

        public static T Last<T>(this List<T> list) {
            return list[list.Count - 1];
        }

        public static T AddReturn<T>(this List<T> list, T element) {
            list.Add(element);
            return element;
        }

    }
}
