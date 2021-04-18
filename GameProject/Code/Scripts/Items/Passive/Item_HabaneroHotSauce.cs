using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Items.Passive {
    public class Item_HabaneroHotSauce : Item {

        public Item_HabaneroHotSauce() : base(ItemID.HabaneroHotSauce) {
            Name = "Habanero Hot Sauce";
            FlavorText = "Feel the pain!";
        }


        protected override void OnPickup() {
            PlayerStats.ShotRate += 2;
        }

        protected override void OnLose() {
            PlayerStats.ShotRate -= 2;
        }
    }
}
