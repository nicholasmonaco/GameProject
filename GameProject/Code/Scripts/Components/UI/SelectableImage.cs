using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.UI {
    public class SelectableImage : UI_LayoutItem {
        public SelectableImage(GameObject attached, Texture2D altImage, SpriteRenderer spriteRenderer) : base(attached) {
            _spriteRenderer = spriteRenderer;

            _origImg = spriteRenderer.Sprite;
            _altImg = altImage;

            _origColor = spriteRenderer.Color;
            _altColor = _origColor;
        }


        public SelectableImage(GameObject attached, Color altColor, SpriteRenderer spriteRenderer) : base(attached) {
            _spriteRenderer = spriteRenderer;

            _origImg = spriteRenderer.Sprite;
            _altImg = _origImg;

            _origColor = spriteRenderer.Color;
            _altColor = altColor;
        }


        private SpriteRenderer _spriteRenderer;
        private Texture2D _origImg;
        private Texture2D _altImg;
        private Color _origColor;
        private Color _altColor;



        public override void OnSelected() {
            _spriteRenderer.Sprite = _altImg;
            _spriteRenderer.Color = _altColor;
        }

        public override void OnDeselected() {
            _spriteRenderer.Sprite = _origImg;
            _spriteRenderer.Color = _origColor;
        }
    }
}