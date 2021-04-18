using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Items.Passive {
    public class Item_FocusLens : Item {

        public Item_FocusLens() : base(ItemID.FocusLens) {
            Name = "Focus Lens";
            FlavorText = "More Megapixels per Pixel!";
        }


        protected override void OnPickup() {
            PlayerStats.Range += 1;
        }

        protected override void OnLose() {
            PlayerStats.Range -= 1;
        }
    }
}
