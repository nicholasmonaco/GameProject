using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Items.Passive {
    public class Item_VitaminH : Item {

        public Item_VitaminH() : base(ItemID.VitaminH) { }


        protected override void OnLose() {
            Debug.Log("Lost a Vitamin H!");
        }

        protected override void OnPickup() {
            Debug.Log("Picked up Vitamin H!");
        }
    }
}
