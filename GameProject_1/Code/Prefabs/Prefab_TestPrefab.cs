using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_TestPrefab : GameObject {
        public Prefab_TestPrefab() : base() {
            Name = "Test Prefab";
            
            SpriteRenderer sr = _components.AddReturn(new SpriteRenderer(this)) as SpriteRenderer;
            sr.Sprite = Resources.Sprite_TestSprite;
        }
    }
}
