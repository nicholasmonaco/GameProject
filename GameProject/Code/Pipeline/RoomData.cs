using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Pipeline {
    public struct RoomData {
        public int[,] ObstacleData;
        public int[,] EntityData;

        public bool Door_Up;
        public bool Door_Down;
        public bool Door_Left;
        public bool Door_Right;

        public int LevelID;

        public int RoomType;

        public int RoomID;
    }
}
