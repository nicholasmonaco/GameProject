using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Items.Passive {
    public class Item_RationBar : Item {

        public Item_RationBar() : base(ItemID.RationBar) {
            Name = "Ration Bar";
            FlavorText = "Military Grade";
        }


        protected override void OnLose() {
            PlayerStats.ChangeMaxRedHealth(-1, true);
        }

        protected override void OnPickup() {
            PlayerStats.ChangeMaxRedHealth(1, true);
        }
    }
}
