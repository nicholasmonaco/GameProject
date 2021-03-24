using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.UI;
using Microsoft.Xna.Framework;
using GameProject.Code.Scripts.Components.UI;

namespace GameProject.Code.Prefabs.UI {
    public class Prefab_ItemPickupUI : Prefab_Panel {
        public Prefab_ItemPickupUI() : base() {
            Panel panel = GetComponent<Panel>();
            panel.SetColor(Color.Black);
            panel.SetOpacity(1);

            RectTransform _rectTransform = panel.rectTransform;

            _rectTransform.Height *= 0.2f;
            _rectTransform.VerticalAlignment = VerticalStick.Bottom;
            //_rectTransform.LocalPosition += new Vector3(0, 1, 0);

            ItemPickupUI itemUI = AddComponent<ItemPickupUI>();
            //itemUI.CenteredPos = transform.Position;
            //itemUI.OffscreenPos_Left = transform.Position - new Vector3(_rectTransform.Width * 1.5f, 0, 0);
            //itemUI.OffscreenPos_Right = transform.Position + new Vector3(_rectTransform.Width * 1.5f, 0, 0);
            itemUI.OrigY = 40;

            _rectTransform.Width *= 2 / 3f;

            TextRenderer nameRenderer = AddComponent<TextRenderer>();
            nameRenderer.SetFont(GameFont.Debug);
            nameRenderer.DrawLayer = DrawLayer.ID[DrawLayers.OverHUD];
            nameRenderer.OrderInLayer = 10;
            nameRenderer.Text = "_item";
            nameRenderer.Color = Color.White;
            nameRenderer.Justification = Justify.Center;
            nameRenderer.SpriteScale *= 0.25f;
            nameRenderer.SpriteOffset += new Vector2(0, 12);
            itemUI.NameRenderer = nameRenderer;

            TextRenderer descriptionRenderer = AddComponent<TextRenderer>();
            descriptionRenderer.SetFont(GameFont.Debug);
            descriptionRenderer.DrawLayer = DrawLayer.ID[DrawLayers.OverHUD];
            descriptionRenderer.OrderInLayer = 9;
            descriptionRenderer.Text = "_itemdesc";
            descriptionRenderer.Color = new Color(224, 224, 224, 255);
            descriptionRenderer.Justification = Justify.Center;
            descriptionRenderer.SpriteScale *= 0.18f;
            descriptionRenderer.SpriteOffset += new Vector2(0, -17);
            itemUI.FlavorTextRenderer = descriptionRenderer;

            Image panelImg = GetComponent<Image>();
            panelImg.DrawLayer = DrawLayer.ID[DrawLayers.OverHUD];
            panelImg.OrderInLayer = 8;

            _rectTransform.Scale *= 0.6f;
            _rectTransform.LocalPosition = itemUI.OffscreenPos_Right;
        }
    }
}
