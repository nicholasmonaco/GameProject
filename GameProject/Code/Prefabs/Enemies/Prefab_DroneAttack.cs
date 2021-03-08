using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Prefabs.Enemies {
    public class Prefab_DroneAttack : Prefab_DroneBugged {

        protected override void SetSpecificData() {
            Name = "Attack Drone";
            AddComponent<Enemy_DroneAttack>(EntityID.Drone_Attack);
        }

    }
}
