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
    [AnimatableComponent]
    public class SpriteRenderer : Renderer {

        [AnimatableValue] public Texture2D Sprite {
            get => Material.Texture as Texture2D;
            set { if(Material != null) Material.Texture = value; }
        }

        [AnimatableValue] public Color Color {
            get => Material.Color;
            set { Material.Color = value; }
        }

        [AnimatableValue] public Vector2 SpriteScale = Vector2.One;
        [AnimatableValue] public Vector2 SpriteOffset = Vector2.Zero;

        


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
                    transform.Rotation_Rads2D, 
                    new Vector2(Sprite.Width/2f, Sprite.Height/2f), 
                    transform.Scale.ToVector2().FlipY() * SpriteScale, 
                    SpriteEffects.None, 
                    _realDrawOrder);
        }

        public Point SpriteSize => (new Vector2(Sprite.Width, Sprite.Height) * transform.Scale.ToVector2()).ToPoint();

    }
}
