using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_PlayerHealthBar : GameObject {
        public Prefab_PlayerHealthBar() : base() {
            Name = "Player Health Bar";

            transform.Parent = GameManager.MainCanvas.transform;
            //transform.LocalPosition = new Vector3(-190, 138, 0);
            UpdatePosition();

            GameManager.MainCanvas.ExtentsUpdate += UpdatePosition;

            AddComponent<HealthBarController>().InitHealthBar();
        }

        private void UpdatePosition() {
            transform.LocalPosition = new Vector3(-GameManager.MainCanvas.Extents.X + 50,
                                                  GameManager.MainCanvas.Extents.Y - 14,
                                                  0);
        }
    }
}
