using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Items.Passive {
    public class Item_VitaminH : Item {

        public Item_VitaminH() : base(ItemID.VitaminH) {
            Name = "Vitamin H";
            FlavorText = "Full of Nutrients!";
        }


        protected override void OnLose() {
            PlayerStats.ChangeMaxRedHealth(-1, true);
        }

        protected override void OnPickup() {
            PlayerStats.ChangeMaxRedHealth(1, true);
        }
    }
}
