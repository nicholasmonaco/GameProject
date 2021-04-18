using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Items.Passive {
    public class Item_Cake : Item {

        public Item_Cake() : base(ItemID.Cake) {
            Name = "Cake";
            FlavorText = "Tell me the Truth";
        }


        protected override void OnPickup() {
            PlayerStats.ChangeBonusHealth(4);
        }

        protected override void OnLose() {
            //PlayerStats.ChangeMaxRedHealth(-1, true);
        }
    }
}
