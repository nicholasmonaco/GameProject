using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Prefabs.Enemies {
    public class Prefab_CaveChaserBuckshot : Prefab_CaveChaser {

        protected override void SetSpecificData() {
            Name = "Buckshot Cave Chaser";
            //GetComponent<SpriteRenderer>().Color = Color.DarkSeaGreen;
            GetComponent<SpriteRenderer>().Sprite = Resources.Sprites_EnemyAnimations[EntityID.CaveChaser_Buckshot][EnemyAnimationAction.Idle][0];
            AddComponent<Enemy_CaveChaserBuckshot>(EntityID.CaveChaser_Buckshot);
        }

    }
}
