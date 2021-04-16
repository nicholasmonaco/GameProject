using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    public class Ray2D {

        public Vector2 Origin { get; private set; }
        public Vector2 Direction { get; private set; }


        public Ray2D(Vector2 origin, Vector2 direction) {
            Origin = origin;
            Direction = direction.Norm();
        }

        public Vector2 GetPoint(float distance) {
            return Origin + (Direction * distance);
        }

    }
}
