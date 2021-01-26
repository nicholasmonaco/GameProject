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

        public static Point ToPoint(this Vector2 value) {
            return new Point((int)value.X, (int)value.Y);
        }

    }
}
