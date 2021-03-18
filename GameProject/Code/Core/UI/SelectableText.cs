using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.UI {
    public class SelectableText : UI_LayoutItem {
        public SelectableText(GameObject attached, Color altColor, TextRenderer textRenderer) : base(attached) {
            _textRenderer = textRenderer;

            _origColor = textRenderer.Color;
            _altColor = altColor;
        }


        private TextRenderer _textRenderer;
        private Color _origColor;
        private Color _altColor;

        

        public override void OnSelected() {
            _textRenderer.Color = _altColor;
        }

        public override void OnDeselected() {
            _textRenderer.Color = _origColor;
        }
    }
}