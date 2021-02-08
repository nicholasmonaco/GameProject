using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_Minimap : GameObject {

        public Prefab_Minimap() : base() {
            Name = "Minimap";

            transform.Parent = GameManager.MainCanvas.transform;
            transform.LocalPosition = new Vector3(165, 100, 0);
            transform.LocalScale *= new Vector3(0.85f, 0.85f, 1);
            

            MinimapController mc = AddComponent<MinimapController>();
            mc.InitMinimap();
        }

    }
}
