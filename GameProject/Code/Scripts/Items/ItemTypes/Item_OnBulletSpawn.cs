using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Components.Bullet;

namespace GameProject.Code.Scripts.Items.ItemTypes {
    public interface Item_OnBulletSpawn {
        public void OnBulletSpawn(AbstractBullet bullet);
    }
}
