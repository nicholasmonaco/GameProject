using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Components.Bullet;

namespace GameProject.Code.Scripts.Items.ItemTypes {
    public interface Item_OnBulletFixedUpdate {
        public void OnBulletFixedUpdate(AbstractBullet bullet);
    }
}
