// Renderer.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core.Components {
    
    /// <summary>
    /// Governs all renderable components.
    /// No other type of component should be drawing anything if it isn't a subclass of Renderer.
    /// </summary>
    public class Renderer : Component, IGameDrawable {
        public Renderer(GameObject attached) : base(attached) {
            Material = new Material(this);
        }

        public Material Material { get; set; }

        public int DrawLayer {
            get { return _drawLayer; }
            set {
                _drawLayer = value;
                _realDrawOrder = (_drawLayer * 10000 + _orderInLayer) / 500000f;
            }
        }

        public int OrderInLayer {
            get { return _orderInLayer; }
            set {
                _orderInLayer = value;
                _realDrawOrder = (_drawLayer * 10000 + _orderInLayer) / 500000f;
            }
        }

        protected int _drawLayer = 0;
        protected int _orderInLayer = 0;
        protected float _realDrawOrder = 0;


        public bool IsInCamera(Camera camera) {
            return false;
        }





        public static void DrawLine2D(SpriteBatch sb, Vector2 start, Vector2 end, Color color) {
            Vector2 edge = end - start;
            float angle = (float)MathF.Atan2(edge.Y, edge.X);

            sb.Draw(Resources.Sprite_Pixel,
                    start,
                    null,
                    color,
                    angle,
                    Vector2.Zero,
                    new Vector2(edge.Length(), 0.5f).FlipY(),
                    SpriteEffects.None,
                    0.9f);
        }
    }
}
