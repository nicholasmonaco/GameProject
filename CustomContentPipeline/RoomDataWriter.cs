using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace CustomContentPipeline {

    [ContentTypeWriter]
    public class RoomDataWriter : ContentTypeWriter<RoomFileData> {

        protected override void Write(ContentWriter output, RoomFileData value) {
            // Output each value from the RoomFileData
            output.Write(value.RoomID);
            output.Write(value.LevelID);
            output.Write(value.RoomType);

            output.Write(value.Door_Up);
            output.Write(value.Door_Down);
            output.Write(value.Door_Left);
            output.Write(value.Door_Right);

            // Array Data
            for(int i = 0; i < 91; i++) {
                output.Write(value.ObstacleData[i]);
                output.Write(value.EntityData[i]);
            }
        }


        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return typeof(RoomData).AssemblyQualifiedName;
        }


        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return "GameProject.Code.Pipeline.RoomDataReader, GameProject";
        }

    }
}
