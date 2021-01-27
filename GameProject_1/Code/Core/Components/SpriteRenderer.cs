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
            // We'll probably switch this with the quad rendering thing
            Point SpriteSize = new Point(Sprite.Width, Sprite.Height);
            Rectangle drawRect = GameManager.MainCamera.GetScreenPosRect(new Rectangle(transform.Position.ToPoint2D() + SpriteSize.Div(2), 
                                                                                       SpriteSize), 
                                                                         transform.Scale);

            sb.Draw(Sprite, drawRect, Color.White);
            Debug.Log($"pos: ({drawRect.X}, {drawRect.Y}) | size: ({drawRect.Width}, {drawRect.Height})");
        }

    }
}
