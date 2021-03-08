using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Components {
    public class TextRenderer : Component {
        private string _text = "";
        public string Text {
            get => _text;
            set {
                _text = value;
                _textDrawPos = new Vector2(_font.MeasureString(value).X / 2f, _font.MeasureString(value).Y / 2f);
            }
        }

        private Vector2 _textDrawPos;
        private SpriteFont _font;
        private SpriteFont _secondaryFont;
        public Color Color = Color.White;
        public Color SecondaryColor = Color.Black;
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

        private Action<SpriteBatch> _drawAction = (sb) => { };


        public void SetFont(GameFont font) {
            switch (font) {
                default:
                case GameFont.Base:
                    _font = Resources.Font_Base_Outer;
                    _secondaryFont = Resources.Font_Base_Inner;
                    _drawAction = (sb) => {
                        DrawMethod(sb, _font, Color);
                        DrawMethod(sb, _secondaryFont, SecondaryColor);
                    };
                    break;
            }
        }


        public TextRenderer(GameObject attached) : base(attached) { }

        public TextRenderer(GameObject attached, GameFont font, string text) : base(attached) {
            SetFont(font);
            Text = text;
        }


        public override void Draw(SpriteBatch sb) {
            _drawAction(sb);

        }

        private void DrawMethod(SpriteBatch sb, SpriteFont font, Color color) {
            sb.DrawString(font,
                          Text,
                          transform.Position.ToVector2() + new Vector2(SpriteSize.X/2f, 0),
                          color,
                          transform.Rotation_Rads,
                          _textDrawPos,
                          transform.Scale.ToVector2().FlipY() * SpriteScale,
                          SpriteEffects.None,
                          _realDrawOrder);
        }

        public Point SpriteSize => (_font.MeasureString(Text) * transform.Scale.ToVector2()).ToPoint();
    }

    public enum GameFont {
        Base = 1,
        Styled = 1
    }
}