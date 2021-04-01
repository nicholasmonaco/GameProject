using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Components {
    [AnimatableComponent]
    public class TextRenderer : Renderer {
        private string _text = "";

        [AnimatableValue]
        public string Text {
            get => _text;
            set {
                _text = value;
                if (value != null) { 
                    _textDrawPos = new Vector2(_font.MeasureString(value).X / 2f, _font.MeasureString(value).Y / 2f);
                    Justification = _justification;
                }
            }
        }

        private Vector2 _textDrawPos;
        private SpriteFont _font;
        private SpriteFont _secondaryFont;
        [AnimatableValue] public Color Color = Color.White;
        [AnimatableValue] public Color SecondaryColor = Color.Black;
        [AnimatableValue] public Vector2 SpriteScale = Vector2.One;
        [AnimatableValue] public Vector2 SpriteOffset = Vector2.Zero;

        private Justify _justification = Justify.Center;

        [AnimatableValue]
        public Justify Justification {
            get => _justification;
            set { 
                _justification = value;
                switch (value) {
                    default:
                    case Justify.Center:
                        _justificationVector = Vector2.Zero;
                        break;
                    case Justify.Left:
                        _justificationVector = new Vector2(SpriteSize.X / 2f, 0);
                        break;
                    case Justify.Right:
                        _justificationVector = new Vector2(-SpriteSize.X / 2f, 0);
                        break;
                }
            }
        }

        private static readonly float MinDrawIncrement = 1 / 500000f;

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
                        DrawMethod_Over(sb, _font, Color);
                        DrawMethod(sb, _secondaryFont, SecondaryColor);
                    };
                    break;
                case GameFont.Debug:
                    _font = Resources.Font_Debug;
                    _drawAction = (sb) => { DrawMethod(sb, _font, Color); };
                    break;
            }

            Justification = _justification;
        }


        public TextRenderer(GameObject attached) : base(attached) { }

        public TextRenderer(GameObject attached, GameFont font, string text) : base(attached) {
            SetFont(font);
            Text = text;

            transform.WorldMatrixUpdateAction += () => { Justification = _justification; };
        }


        public override void Draw(SpriteBatch sb) {
            _drawAction(sb);

        }

        private void DrawMethod(SpriteBatch sb, SpriteFont font, Color color) {
            sb.DrawString(font,
                          Text,
                          transform.Position.ToVector2() + SpriteOffset + _justificationVector,
                          color,
                          transform.Rotation_Rads,
                          _textDrawPos,
                          transform.Scale.ToVector2().FlipY() * SpriteScale,
                          SpriteEffects.None,
                          _realDrawOrder);
        }

        private void DrawMethod_Over(SpriteBatch sb, SpriteFont font, Color color) {
            sb.DrawString(font,
                          Text,
                          transform.Position.ToVector2() + SpriteOffset + _justificationVector,
                          color,
                          transform.Rotation_Rads,
                          _textDrawPos,
                          transform.Scale.ToVector2().FlipY() * SpriteScale,
                          SpriteEffects.None,
                          _realDrawOrder + MinDrawIncrement);
        }

        public Vector2 SpriteSize => _font.MeasureString(Text) * transform.Scale.ToVector2();

        private Vector2 _justificationVector = Vector2.Zero;
    }

    public enum GameFont {
        Base = 0,
        Styled = 1,
        Debug = 2
    }

    public enum Justify {
        Left,
        Center,
        Right
    }
}