using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Components.Bullet;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Items.ItemTypes;
using GameProject.Code.Core;

namespace GameProject.Code.Scripts.Items.Passive {
    public class Item_FourLeafClover : Item, Item_OnBulletSpawn {

        public Item_FourLeafClover() : base(ItemID.FourLeafClover) {
            Name = "4 Leaf Clover";
            FlavorText = "I'm Feeling Lucky";
        }


        protected override void OnPickup() {
            PlayerStats.Luck += 1;
        }

        protected override void OnLose() {
            PlayerStats.Luck -= 1;
        }

        public void OnBulletSpawn(AbstractBullet bullet) {
            float mult = GameManager.DeltaRandom.NextValue(0.7f, 2f);
            bullet.transform.LocalScale *= mult;
            bullet.Damage *= mult;
        }

    }
}
