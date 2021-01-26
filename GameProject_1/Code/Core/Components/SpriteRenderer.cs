using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core.Components {
    public class SpriteRenderer : Renderer {

        public Texture2D Sprite;
        public Color Tint = Color.White;

        public SpriteRenderer(GameObject attached) : base(attached) {

        }


        public override void Draw(SpriteBatch sb) {
            // base.Draw(sb); // We'll probably switch to this with the quad rendering thing

            sb.Draw(Sprite, new Rectangle(transform.Position.ToPoint(), transform.Scale.ToPoint()), Color.White); // This will have to be changed to work with world position vs screen position
        }

    }
}
