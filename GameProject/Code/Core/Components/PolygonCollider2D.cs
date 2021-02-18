using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core.Components {
    public class PolygonCollider2D : Collider2D {

        private PolygonBounds PolyBounds => Bounds as PolygonBounds;
        

        public PolygonCollider2D(GameObject attached) : base(attached) {
            Bounds = new PolygonBounds(new Vector2[] { new Vector2(-1, 1),       // Top Left
                                                new Vector2(1, 1),        // Top Right
                                                new Vector2(1, -1),       // Bottom Right
                                                new Vector2(-1, -1)},     // Top Left
                                                false);    

            // This constructor will construct a 1x1 rect collider.

            Bounds.Center = PolyBounds.GetRectCenter(); 
            Bounds.OrigCenter = Bounds.Center;
            Bounds.ParentCollider = this;
            Bounds.BoundsType = BoundsType.Polygon;
        }

        public PolygonCollider2D(GameObject attached, Vector2[] bounds, bool preClosed) : base(attached) {
            Bounds = new PolygonBounds(new Vector2[1], true);
            PolyBounds.ResetBounds(bounds, preClosed);

            Bounds.Center = PolyBounds.GetPolygonCenter();
            Bounds.OrigCenter = Bounds.Center;
            Bounds.ParentCollider = this;
            Bounds.BoundsType = BoundsType.Polygon;
        }


        // Debug
        public override void Draw(SpriteBatch sb) {
            if (!Debug.ShowColliders) return;
            for (int i = 0; i < PolyBounds._points.Length - 1; i++) {
                DrawLine(sb, PolyBounds._points[i], PolyBounds._points[i + 1]);
            }
        }

    }
}
