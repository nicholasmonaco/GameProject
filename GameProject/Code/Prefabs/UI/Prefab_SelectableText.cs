using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.UI;

namespace GameProject.Code.Prefabs.UI {
    public class Prefab_SelectableText : GameObject {
        public Prefab_SelectableText(string text, Color deselectColor, Color selectColor) : base() {
            TextRenderer textRend = AddComponent<TextRenderer>();
            textRend.SetFont(GameFont.Debug); //make styled later
            textRend.Text = text;
            textRend.Color = deselectColor;
            textRend.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
            textRend.OrderInLayer = 42;

            AddComponent<SelectableText>(selectColor, textRend);
        }
    }
}
