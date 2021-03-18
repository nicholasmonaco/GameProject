using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.UI;

namespace GameProject.Code.Prefabs.UI.MainMenus {
    public class Prefab_CreditsMenu : GameObject {
        public Prefab_CreditsMenu() : base() {
            Name = "Credits Menu";

            float init = 15;
            float diff = 30;
            List<string> list = new List<string>(2) { "Programming, Art, Music, and SFX", "by Nick Monaco" };

            for(int i = 0; i < list.Count; i++) {
                GameObject creditText = Instantiate<GameObject>(Vector3.Zero, transform);
                creditText.Name = $"Label ({list[i]})";
                TextRenderer textRend = creditText.AddComponent<TextRenderer>();
                textRend.SetFont(GameFont.Debug); //make styled later
                textRend.Text = list[i];
                textRend.Color = new Color(108, 112, 117);
                textRend.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
                textRend.OrderInLayer = 42;

                creditText.transform.LocalPosition = new Vector3(0, init, 0);

                creditText.transform.Scale *= 0.2f; //debug

                init -= diff;
            }
        }
    }
}
