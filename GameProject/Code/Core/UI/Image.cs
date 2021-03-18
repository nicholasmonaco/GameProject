using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.UI {
    public class Image : UIComponent {
        
        public Texture2D Texture;
        public Color Color = Color.White;


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



        public Image(GameObject attached) : base(attached) { }

        public Image(GameObject attached, Texture2D image) : base(attached) {
            Texture = image;
        }



        public override void Draw(SpriteBatch sb) {
            sb.Draw(Texture,
                    transform.Position.ToVector2(),
                    null,
                    Color,
                    transform.Rotation_Rads,
                    ImageSize.ToVector2() / 2f,
                    AppliedScale.FlipY(),
                    SpriteEffects.None,
                    _realDrawOrder);
        }


        public Point ImageSize => Texture.Bounds.Size;
        public Vector2 AppliedScale => new Vector2(rectTransform.Width / ImageSize.X, rectTransform.Height / ImageSize.Y) * rectTransform.Scale.ToVector2();

    }
}