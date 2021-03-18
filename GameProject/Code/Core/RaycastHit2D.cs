using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    public class RaycastHit2D {

        public Vector2 Origin { get; private set; }
        public Vector2 Direction { get; private set; }
        public float Distance { get; private set; }
        public int LayerMask { get; private set; }


        public RaycastHit2D(Vector2 origin, Vector2 direction, float distance = float.PositiveInfinity, int layerMask = (int)LayerID.Max) {

        }
    }
}
