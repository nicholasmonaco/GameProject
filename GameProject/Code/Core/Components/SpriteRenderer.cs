// SpriteRenderer.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;

namespace GameProject.Code.Core.Components {
    
    /// <summary>
    /// Component allowing a sprite to be drawn into the scene.
    /// </summary>
    public class SpriteRenderer : Renderer {

        public Texture2D Sprite;
        public Color Color = Color.White;
        public Vector2 SpriteScale = Vector2.One;
        public Vector2 SpriteOffset = Vector2.Zero;

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

        private int _drawLayer = 0;
        private int _orderInLayer = 0;
        private float _realDrawOrder = 0;


        public SpriteRenderer(GameObject attached) : base(attached) { }
        
        public SpriteRenderer(GameObject attached, Texture2D sprite) : base(attached) {
            Sprite = sprite;
        }


        public override void Draw(SpriteBatch sb) {
            // We'll probably change how this works with the quad rendering thing
            // At that point, honestly just make a new class for this that does that and for Transforms that uses quaternions
            
            // While this doesnt fully implement quaternions, it is technically possible, it would just take a toooon of math. Just do it later with quads.
            sb.Draw(Sprite, 
                    transform.Position.ToVector2() + SpriteOffset, 
                    null, 
                    Color, 
                    transform.Rotation_Rads, 
                    new Vector2(Sprite.Width/2f, Sprite.Height/2f), 
                    transform.Scale.ToVector2().FlipY() * SpriteScale, 
                    SpriteEffects.None, 
                    _realDrawOrder);
        }

        public Point SpriteSize => (new Vector2(Sprite.Width, Sprite.Height) * transform.Scale.ToVector2()).ToPoint();

    }
}
