using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Components.Entity;

namespace GameProject.Code.Scripts.Items.ItemTypes {
    public interface Item_OnEnemyKill {
        public void OnEnemyKill(AbstractEnemy enemy);
    }
}
