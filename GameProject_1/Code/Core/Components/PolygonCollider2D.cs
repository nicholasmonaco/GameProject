using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core.Components {
    public class PolygonCollider2D : Collider2D {

        public PolygonCollider2D(GameObject attached) : base(attached) {
            Bounds = new Bounds(new Vector2[] { new Vector2(-1, 1),       // Top Left
                                                new Vector2(1, 1),        // Top Right
                                                new Vector2(1, -1),       // Bottom Right
                                                new Vector2(-1, -1),      // Bottom Left 
                                                new Vector2(-1, 1) });    // Top Left

            // This constructor will construct a 1x1 rect collider.

            Bounds.Center = Bounds.GetRectCenter(); 
            Bounds.OrigCenter = Bounds.Center;
            Bounds.ParentCollider = this;
        }

        public PolygonCollider2D(GameObject attached, Vector2[] bounds) : base(attached) {
            Bounds = new Bounds(new Vector2[1]);
            Bounds.ResetBounds(bounds);

            Bounds.Center = Bounds.GetPolygonCenter();
            Bounds.OrigCenter = Bounds.Center;
            Bounds.ParentCollider = this;
        }

    }
}
