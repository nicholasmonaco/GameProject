using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Components.Bullet;

namespace GameProject.Code.Scripts.Items.ItemTypes {
    public interface Item_OnBulletDeath {
        public void OnBulletDeath(AbstractBullet bullet);
    }
}
