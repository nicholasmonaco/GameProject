using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    public struct RaycastHit2D {

        public Vector2 Origin;
        public Vector2 Point;

        public Vector2 RaycastDirection;
        public Vector2 NormalDirection;

        public float Distance;
        public float Fraction;

        public Collider2D HitCollider;
        public Rigidbody2D HitRigidbody => HitCollider.AttachedRigidbody;
        public Transform HitTransform => HitCollider.transform;
        

    }
}
