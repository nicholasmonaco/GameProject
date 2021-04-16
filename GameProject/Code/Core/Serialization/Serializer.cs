using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameProject.Code.Core {
    public static class Serializer {


        public static GameObject Serialize(string filepath) {
            GameObject gameObject = null;

            FileStream file = File.OpenRead(filepath);

            JsonDocument data = JsonDocument.Parse(file);

            #region Interpret the Data

            //data.RootElement.
            //foreach(JsonElement )

            #endregion

            data.Dispose();
            file.Close();

            return gameObject;
        }

        //public static GameObject[] SerializeAll(string filepath) {

        //}

    }
}
