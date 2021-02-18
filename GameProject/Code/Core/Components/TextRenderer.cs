using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Components {
    public class TextRenderer : Component {
        public string Text;
        public SpriteFont Font;
        public Color Color = Color.White;
        public Vector2 SpriteScale = Vector2.One;

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


        public TextRenderer(GameObject attached) : base(attached) { }

        public TextRenderer(GameObject attached, SpriteFont font, string text) : base(attached) {
            Font = font;
            Text = text;
        }


        public override void Draw(SpriteBatch sb) {
            sb.DrawString(Font, 
                    Text,
                    transform.Position.ToVector2(),
                    Color,
                    transform.Rotation_Rads,
                    new Vector2(Font.MeasureString(Text).X / 2f, Font.MeasureString(Text).Y / 2f),
                    transform.Scale.ToVector2().FlipY() * SpriteScale,
                    SpriteEffects.None,
                    _realDrawOrder);

        }

        public Point SpriteSize => (Font.MeasureString(Text) * transform.Scale.ToVector2()).ToPoint();
    }
}