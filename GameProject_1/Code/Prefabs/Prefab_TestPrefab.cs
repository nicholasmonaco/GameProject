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

            Rigidbody2D rb = _components.AddReturn(new Rigidbody2D(this)) as Rigidbody2D;

            //RectCollider2D coll = _components.AddReturn(new RectCollider2D(this, sr.Sprite.Width * transform.Scale.X, sr.Sprite.Height * transform.Scale.Y, sr.Sprite.Width / 2f * transform.Scale.X, sr.Sprite.Height / 2f * transform.Scale.Y)) as RectCollider2D;

            _components.Add(new RectCollider2D(sr));

            AddComponent<KeyboardController>();

            //_components.Add(new RectCollider2D(this, 64, 64));
        }
    }
}
