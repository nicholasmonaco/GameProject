using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.UI {
    public class InventoryTracker : Component {
        public InventoryTracker(GameObject attached) : base(attached) {
            GameManager.Inventory = this;
        }


        public TextRenderer MoneyRenderer { private get; set; }
        public TextRenderer KeyRenderer { private get; set; }
        public TextRenderer BombRenderer { private get; set; }



        public void UpdateMoneyText() {
            MoneyRenderer.Text = PlayerStats.Money.ToString();
        }

        public void UpdateKeyText() {
            KeyRenderer.Text = PlayerStats.Keys.ToString();
        }

        public void UpdateBombText() {
            BombRenderer.Text = PlayerStats.Bombs.ToString();
        }
    }
}