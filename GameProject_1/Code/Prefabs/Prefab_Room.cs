using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Prefabs {
    public class Prefab_Room : GameObject {
        public Prefab_Room() : base() {
            Name = "Room";

            Room roomData = AddComponent<Room>();


            // get corner textures
            // get optional floor overlay textures
            // load room layout
            // spawn doors/door fillers based on door data
            // spawn obstacles based on room data
            // set position in gridmap
            // if special room, set it in mapmanager
        
        }
    }
}