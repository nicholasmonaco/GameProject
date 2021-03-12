using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Items.ItemTypes {
    public interface Item_OnLevelChange {
        public void OnLevelChange(LevelID newLevel);
    }
}
