// RectCollider2D.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core.Components {
    
    /// <summary>
    /// A 2-dimensional collider that is always rectangular in shape
    /// </summary>
    public class RectCollider2D : Collider2D {

        private PolygonBounds PolyBounds => Bounds as PolygonBounds;


        public RectCollider2D(GameObject attached) : base(attached) {
            Bounds = new PolygonBounds(new Vector2[] { new Vector2(-1, 1),       // Top Left
                                                new Vector2(1, 1),        // Top Right
                                                new Vector2(1, -1),       // Bottom Right
                                                new Vector2(-1, -1) },    // Bottom Left 
                                                false);   

            Bounds.Center = PolyBounds.GetRectCenter();
            Bounds.OrigCenter = Bounds.Center;
            Bounds.ParentCollider = this;
            Bounds.BoundsType = BoundsType.Rectangle;

            //Size = new Vector2(2, 2); // ??? is this right?
        }

        public RectCollider2D(GameObject attached, float Width, float Height, float xOffset, float yOffset) : base(attached) {
            Bounds = new PolygonBounds(new Vector2[] { new Vector2(-Width/2 + xOffset, Height/2 + yOffset),       // Top Left
                                                new Vector2(Width/2 + xOffset, Height/2 + yOffset),        // Top Right
                                                new Vector2(Width/2 + xOffset, -Height/2 + yOffset),       // Bottom Right
                                                new Vector2(-Width/2 + xOffset, -Height/2 + yOffset) },    // Bottom Left 
                                                false);    

            Bounds.Center = PolyBounds.GetRectCenter();
            Bounds.OrigCenter = Bounds.Center;
            Bounds.ParentCollider = this;
            Bounds.BoundsType = BoundsType.Rectangle;

            Size = new Vector2(Width, Height);
        }

        public RectCollider2D(GameObject attached, SpriteRenderer sr) : this(sr.gameObject, 
                                                                             sr.Sprite.Width * sr.SpriteScale.X, 
                                                                             sr.Sprite.Height * sr.SpriteScale.Y,
                                                                             sr.transform.LocalPosition.X + sr.SpriteOffset.X,
                                                                             sr.transform.LocalPosition.Y + sr.SpriteOffset.Y) { }

        public RectCollider2D(GameObject attached, float Width, float Height) : this(attached, Width, Height, 0, 0) { }
        public RectCollider2D(GameObject attached, Vector2 size, Vector2 offset) : this(attached, size.X, size.Y, offset.X, offset.Y){ }
        public RectCollider2D(GameObject attached, Vector2 size) : this(attached, size.X, size.Y, 0, 0) { }




        // Debug
        public override void Draw(SpriteBatch sb) {
            if (!Debug.ShowColliders) return;
            for (int i = 0; i < PolyBounds._points.Length - 1; i++) {
                DrawLine(sb, PolyBounds._points[i], PolyBounds._points[i + 1]);
            }
        }


        public override void Update() {
            Scripts.Components.Room rr = transform.GetComponent<Scripts.Components.Room>();

            if (rr != null && rr.GridPos == new Point(-3,-2)) {
                if (rr.ObstacleTilemap.ColliderMap[0, 0] != this) return;
                Vector2 colliderAtzz = rr.ObstacleTilemap.ColliderMap[0, 0].Bounds.Center;
                Debug.Log($"0,0 collider at {rr.GridPos} is {colliderAtzz}");
            }
        }
    }
}
