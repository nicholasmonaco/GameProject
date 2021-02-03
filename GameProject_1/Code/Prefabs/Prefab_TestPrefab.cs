// Prefab_TestPrefab.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Prefabs {
    
    /// <summary>
    /// A test prefab.
    /// </summary>
    public class Prefab_TestPrefab : GameObject {
        public Prefab_TestPrefab() : base() {
            Name = "Test Prefab";
            
            SpriteRenderer sr = _components.AddReturn(new SpriteRenderer(this)) as SpriteRenderer;
            sr.Sprite = Resources.Sprite_TestSquare;
            sr.DrawLayer = 10;
            sr.SpriteScale *= 0.5f;

            Rigidbody2D rb = _components.AddReturn(new Rigidbody2D(this)) as Rigidbody2D;


            //_components.Add(new RectCollider2D(sr));
            _components.Add(new RectCollider2D(this, new Vector2(28, 28)));

            AddComponent<KeyboardController>();

        }
    }
}
