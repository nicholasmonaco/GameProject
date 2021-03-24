using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity.Arms;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Prefabs {
    public class Prefab_Arm : GameObject {
        public Prefab_Arm() : base() {
            Name = "Arm";

            Layer = LayerID.Bullet_Good;

            SpriteRenderer armSprite = AddComponent<SpriteRenderer>();
            armSprite.Sprite = Resources.Sprite_Arm_Outer;
            armSprite.DrawLayer = DrawLayer.ID[DrawLayers.Player];
            armSprite.OrderInLayer = 30;

            CircleCollider2D coll = AddComponent<CircleCollider2D>(4.5f);
            coll.Bounds.OrigCenter -= new Vector2(4, 4);
            coll.Bounds.Center -= new Vector2(4, 4);
            coll.IsTrigger = true;
            

            ArmController arm = AddComponent<ArmController>();
            arm.ArmRenderer = armSprite;
        }
    }
}
