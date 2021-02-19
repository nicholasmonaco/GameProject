using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_Vignette : GameObject {

        public Prefab_Vignette() : base() {
            Name = "Vignette";

            transform.Parent = GameManager.MainCanvas.transform;
            UpdatePosition();

            GameManager.MainCanvas.ExtentsUpdate += UpdatePosition;

            SpriteRenderer sr = AddComponent<SpriteRenderer>();
            sr.Sprite = Resources.Sprite_Vignette;
            sr.DrawLayer = DrawLayer.ID["AboveAll"];
            sr.OrderInLayer = 10;
            sr.Color = new Color(1, 1, 1, 0.75f);

            transform.Position = Vector3.Zero;
            transform.Scale *= 0.3f;
        }

        private void UpdatePosition() {
            transform.LocalPosition = Vector3.Zero;
        }
    }
}
