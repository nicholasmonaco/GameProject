using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Prefabs {
    public class Prefab_MainCamera : GameObject {

        public Prefab_MainCamera() : base() {
            // Create the structure of the prefab
            Name = "Main Camera";

            //transform.Rotation = Quaternion.CreateFromYawPitchRoll(Vector3.Forward.X, Vector3.Forward.Y, Vector3.Forward.Z);
            _components.Add(new Camera(this));
        }
    }
}
