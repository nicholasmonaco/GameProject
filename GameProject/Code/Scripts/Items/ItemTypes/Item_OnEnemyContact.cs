using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Components.Entity;

namespace GameProject.Code.Scripts.Items.ItemTypes {
    public interface Item_OnEnemyContact {
        public void OnEnemyContact(AbstractEnemy enemy);
    }
}
