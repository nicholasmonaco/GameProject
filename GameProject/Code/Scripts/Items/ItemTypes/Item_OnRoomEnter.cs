using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Scripts.Items.ItemTypes {
    public interface Item_OnRoomEnter {
        public void OnRoomEnter(Room room);
    }
}
