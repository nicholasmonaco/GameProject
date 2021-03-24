using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core.UI;

namespace GameProject.Code.Core.UI {
    [AnimatableComponent]
    public class RectTransform : Transform {

        private Vector3 _alignOffset = Vector3.Zero;

        private HorizontalStick _hStick = HorizontalStick.Center;
        private VerticalStick _vStick = VerticalStick.Center;

        [AnimatableValue]
        public HorizontalStick HorizontalAlignment {
            get => _hStick;
            set {
                _hStick = value;
                switch (value) {
                    case HorizontalStick.Center:
                        _alignOffset.X = 0;
                        break;
                    case HorizontalStick.Left:
                        _alignOffset.X = -GameManager.MainCanvas.Extents.X;
                        break;
                    case HorizontalStick.Right:
                        _alignOffset.X = GameManager.MainCanvas.Extents.X;
                        break;
                }

                LocalPosition = _localPosition;
            }
        }

        [AnimatableValue]
        public VerticalStick VerticalAlignment {
            get => _vStick;
            set {
                _vStick = value;
                switch (value) {
                    case VerticalStick.Center:
                        _alignOffset.Y = 0;
                        break;
                    case VerticalStick.Bottom:
                        _alignOffset.Y = -GameManager.MainCanvas.Extents.Y;
                        break;
                    case VerticalStick.Top:
                        _alignOffset.Y = GameManager.MainCanvas.Extents.Y;
                        break;
                }

                LocalPosition = _localPosition;
            }
        }


        private float _width = 100;
        private float _height = 100;

        [AnimatableValue]
        public float Width {
            get => _width;
            set {
                _width = value;
                _size.X = value;
                RecalculateRect();
            }
        }

        [AnimatableValue]
        public float Height {
            get => _height;
            set {
                _height = value;
                _size.Y = value;
                RecalculateRect();
            }
        }

        private Vector2 _size = Vector2.Zero;

        [AnimatableValue]
        public Vector2 Size {
            get => _size;
            set {
                _size = value;
                _width = value.X;
                _height = value.Y;
                RecalculateRect();
            }
        }

        public RectangleF Rect;



        [AnimatableValue]
        public override Vector3 LocalPosition {
            get { return _localPosition; }
            set {
                if (Parent == null) {
                    _worldPosition = _alignOffset + value;
                } else {
                    _localPosition = _alignOffset + value;
                    _worldPosition = ParentPos + (_localPosition * ParentScale);
                }

                ViewChangeAction();
                RecalculateWorldMatrix();
                gameObject.rigidbody2D?.ResetPosition();

                UpdateChildren();
            }
        }



        public RectTransform(GameObject attach) : base(attach) {
            transform = this;
            gameObject = attach;

            //_children = new List<Transform>();

            

            //LocalPosition = Vector3.Zero;
            //Rotation = 0;
            //LocalScale = Vector3.One;

            _width = 100;
            _height = 100;

            HorizontalAlignment = HorizontalStick.Center;
            VerticalAlignment = VerticalStick.Center;

            RecalculateWorldMatrix();
        }




        protected override void RecalculateWorldMatrix() {
            WorldMatrix = Matrix.CreateScale(_worldScale) *
                          Matrix.CreateRotationZ(_worldRotationRad) *
                          Matrix.CreateTranslation(_worldPosition);

            // Apply RectTransform Logic
            RecalculateRect();

            WorldMatrixUpdateAction();
        }

        private void RecalculateRect() {
            Rect = new RectangleF(transform.Position.ToVector2(), Width * transform.Scale.X, Height * transform.Scale.Y);
        }

        public void ApplyTexture(Texture2D texture) {
            _width = texture.Width;
            _height = texture.Height;
            RecalculateRect();
        }

        public void FindApplyTexture() {
            foreach(UIComponent c in gameObject._components) {
                if(c is Image image) {
                    ApplyTexture(image.Texture);
                    return;
                }
            }
        }



    }

    public enum HorizontalStick { Left, Center, Right }
    public enum VerticalStick { Top, Center, Bottom }
}
