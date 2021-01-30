using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;

namespace GameProject.Code.Core.Components {
    public class SpriteRenderer : Renderer {

        public Texture2D Sprite;
        public Color Tint = Color.White;

        public SpriteRenderer(GameObject attached) : base(attached) {

        }


        public override void Draw(SpriteBatch sb) {
            // We'll probably change how this works with the quad rendering thing
            // At that point, honestly just make a new class for this that does that and for Transforms that uses quaternions

            // While this doesnt fully implement quaternions, it is technically possible, it would just take a toooon of math. Just do it later with quads.
            sb.Draw(Sprite, transform.Position.ToVector2(), null, Tint, transform.Rotation, new Vector2(Sprite.Width/2f, Sprite.Height/2f), transform.Scale.ToVector2().FlipY(), SpriteEffects.None, 0);
        }

        public override void Update() {
            //transform.Position += new Vector3(0, 0.05f, 0);
            //transform.Scale += new Vector3(0, 0.005f, 0);
            //transform.Rotation += 0.01f;
        }

    }
}
