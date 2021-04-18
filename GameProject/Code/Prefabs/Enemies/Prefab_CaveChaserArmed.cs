using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Scripts.Util;


namespace GameProject.Code.Prefabs.Enemies {
    public class Prefab_CaveChaserArmed : Prefab_CaveChaser {

        protected override void SetSpecificData() {
            Name = "Armed Cave Chaser";
            //GetComponent<SpriteRenderer>().Color = Color.Purple;
            GetComponent<SpriteRenderer>().Sprite = Resources.Sprites_EnemyAnimations[EntityID.CaveChaser_Armed][EnemyAnimationAction.Idle][0];
            AddComponent<Enemy_CaveChaserArmed>(EntityID.CaveChaser_Armed);
        }

    }
}
