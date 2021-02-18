using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace CustomContentPipeline {

    [ContentProcessor(DisplayName = "RoomData Processor")]
    public class RoomDataProcessor : ContentProcessor<RawRoomData, RoomFileData> {
        public override RoomFileData Process(RawRoomData input, ContentProcessorContext context) {
            try {
                context.Logger.LogMessage("Processing room data");

                RoomFileData output = new RoomFileData();

                // Obstacle Data
                string[] valsAsStrings = input.Lines[0].Replace("\n", "").Replace("\r", "").Split(' ');
                output.ObstacleData = new int[valsAsStrings.Length];
                for(int i = 0; i < valsAsStrings.Length; i++) {
                    output.ObstacleData[i] = Convert.ToInt32(valsAsStrings[i]);
                }

                // Entity Data
                valsAsStrings = input.Lines[1].Replace("\n", "").Replace("\r", "").Split(' ');
                output.EntityData = new int[valsAsStrings.Length];
                for (int i = 0; i < valsAsStrings.Length; i++) {
                    output.EntityData[i] = Convert.ToInt32(valsAsStrings[i]);
                }

                // Door Data
                string line = input.Lines[2].Replace("\n", "").Replace("\r", "");
                output.Door_Down = line[0] == '1';
                output.Door_Left = line[1] == '1';
                output.Door_Right = line[2] == '1';
                output.Door_Up = line[3] == '1';

                // LevelID Data
                line = input.Lines[3];
                output.LevelID = Convert.ToInt32(line[0]);

                // RoomType Data
                line = input.Lines[4];
                output.RoomType = Convert.ToInt32(line[0]);

                // RoomID Data
                line = input.Lines[5].Replace("\n", "").Replace("\r", "");
                output.RoomID = Convert.ToInt32(line);

                return output;

            } catch (Exception e) {
                context.Logger.LogMessage("Error: {0}", e);
                throw;
            }
        }
    }
}
