using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_Minimap : GameObject {

        public Prefab_Minimap() : base() {
            Name = "Minimap";

            transform.Parent = GameManager.MainCanvas.transform;
            //transform.LocalPosition = new Vector3(165, 100, 0);
            UpdatePosition();

            GameManager.MainCanvas.ExtentsUpdate += UpdatePosition;
            

            GameObject minimapHolder = Instantiate<GameObject>(transform.Position, transform);
            minimapHolder.transform.LocalPosition = Vector3.Zero;
            //minimapHolder.transform.LocalScale *= new Vector3(0.85f, 0.85f, 1);

            MinimapController mc = minimapHolder.AddComponent<MinimapController>();
            mc.InitMinimap();

            GameObject curRoomHighlighter = Instantiate<GameObject>(transform.Position, transform);
            curRoomHighlighter.transform.LocalPosition = Vector3.Zero;
            //curRoomHighlighter.transform.LocalScale *= new Vector3(0.85f, 0.85f, 1);
            SpriteRenderer s = curRoomHighlighter.AddComponent<SpriteRenderer>();
            s.Sprite = Resources.Sprite_MinimapIcons[MinimapIcon.Current];
            s.DrawLayer = DrawLayer.ID["HUD"];
            s.OrderInLayer = 56;

            GameObject background = Instantiate<GameObject>(transform.Position, transform);
            background.transform.LocalPosition = Vector3.Zero;
            //background.transform.LocalScale *= new Vector3(0.85f, 0.85f, 1);
            s = background.AddComponent<SpriteRenderer>();
            s.Sprite = Resources.Sprite_MinimapBackground;
            s.DrawLayer = DrawLayer.ID["HUD"];
            s.OrderInLayer = 50;

        }

        private void UpdatePosition() {
            transform.LocalPosition = new Vector3(GameManager.MainCanvas.Extents.X - 50,
                                                  GameManager.MainCanvas.Extents.Y - 50,
                                                  0);
        }

    }
}
