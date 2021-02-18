using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GameProject.Code.Pipeline {
    public class RoomDataReader : ContentTypeReader<RoomData> {
        protected override RoomData Read(ContentReader input, RoomData existingInstance) {
            RoomData data = new RoomData();
            data.RoomID = input.ReadInt32();
            data.LevelID = input.ReadInt32();
            data.RoomType = input.ReadInt32();

            data.Door_Up = input.ReadBoolean();
            data.Door_Down = input.ReadBoolean();
            data.Door_Left = input.ReadBoolean();
            data.Door_Right = input.ReadBoolean();

            data.ObstacleData = new int[13, 7];
            data.EntityData = new int[13, 7];

            for (int y = 0; y < 7; y++) {
                for(int x = 0; x < 13; x++) {
                    data.ObstacleData[x, y] = input.ReadInt32();
                    data.EntityData[x, y] = input.ReadInt32();
                }
            }

            return data;
        }
    }
}
