using System;
using System.Collections.Generic;
using MonoGame.Framework.Content.Pipeline.Builder;
using MonoGame.Framework.Content;
using MonoGame.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace CustomContentPipeline {

    [ContentImporter(".rni", DefaultProcessor = "RoomDataProcessor", DisplayName = "Room Data Importer")]
    public class RoomDataImporter : ContentImporter<RawRoomData> {
        public override RawRoomData Import(string filename, ContentImporterContext context) {
            context.Logger.LogMessage("Importing room data file: {0}", filename);

            using (var streamReader = new StreamReader(filename)) {
                //load in the data from the rni file - the opposite of what's in the room data packer tool
                RawRoomData data = new RawRoomData();
                data.Lines = new string[6];

                int i = 0;
                while (i < 6) {
                    data.Lines[i] = streamReader.ReadLine();
                    i++;
                }

                return data;
            }
        }
    }
}
