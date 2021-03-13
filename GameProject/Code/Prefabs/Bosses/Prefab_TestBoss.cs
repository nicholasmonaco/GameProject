using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Prefabs.Enemies {
    public class Prefab_TestBoss : Prefab_CaveChaserOmega {

        protected override void SetSpecificData() {
            base.SetSpecificData();

            Name = "Bossy Cave Chaser";
            GetComponent<SpriteRenderer>().Color = Color.Maroon;
            //Enemy_CaveChaserOmega enemy = AddComponent<Enemy_CaveChaserOmega>(EntityID.CaveChaser_Omega);
            Enemy_CaveChaserOmega enemy = GetComponent<Enemy_CaveChaserOmega>();
            transform.Scale *= 3;
            enemy.SetHealth(10f);
            enemy.SetSpeed(2);
        }

    }
}
