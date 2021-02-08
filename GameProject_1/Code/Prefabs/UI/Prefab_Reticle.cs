using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_Reticle : GameObject {
        public Prefab_Reticle() : base() {
            Name = "Reticle";

            //transform.Parent = GameManager.MainCamera.transform;

            SpriteRenderer sr = AddComponent<SpriteRenderer>();
            sr.Sprite = Resources.Sprite_UI_Reticles[0];
            sr.DrawLayer = DrawLayer.ID["HUD"];
            sr.OrderInLayer = 10;
            sr.SpriteScale = new Vector2(0.8f, 0.8f);

            AddComponent<Reticle>();
        }
    }
}
