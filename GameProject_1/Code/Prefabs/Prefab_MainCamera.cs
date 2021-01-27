using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_MainCamera : GameObject {

        public Prefab_MainCamera() : base() {
            // Create the structure of the prefab
            Name = "Main Camera";

            _components.Add(new Camera(this));
        }
    }
}
