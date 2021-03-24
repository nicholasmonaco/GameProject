using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core.UI;

namespace GameProject.Code.Scripts.Components.UI {
    public class ItemPickupUI : UIComponent {
        public ItemPickupUI(GameObject attached) : base(attached) {
            GameManager.ItemPickupUI = this;
        }


        public TextRenderer NameRenderer;
        public TextRenderer FlavorTextRenderer;

        public float OrigY = 0;


        public Vector3 OffscreenPos_Right => new Vector3(GameManager.Resolution.X * 1.5f, OrigY, 0);
        public Vector3 OffscreenPos_Left => new Vector3(-GameManager.Resolution.X * 1.5f, OrigY, 0);
        public Vector3 CenteredPos => new Vector3(0, OrigY, 0);
    }
}