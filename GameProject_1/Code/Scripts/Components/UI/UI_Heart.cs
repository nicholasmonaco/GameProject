using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class UI_Heart : Component {
        public UI_Heart(GameObject attached) : base(attached) { }


        private SpriteRenderer _heartRenderer;
        
        private HeartContainer _containerType;
        public HeartContainer ContainerType {
            get { return _containerType; }
            set {
                _containerType = value;
                _heartRenderer.Sprite = Resources.Sprites_HeartContainers[value];
            }
        }


        public void SetHeartRenderer(SpriteRenderer heartRenderer) {
            _heartRenderer = heartRenderer;
        }


    }

    public enum HeartContainer {
        Empty,

        Red_Full,
        Red_Half,

        Bonus_Full,
        Bonus_Half,

        Invisible
    }
}