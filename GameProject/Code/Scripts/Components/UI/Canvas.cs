using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class Canvas : Component {
        public Canvas(GameObject attached) : base(attached) {
            GameManager.MainCanvas = this;

            transform.WorldMatrixUpdateAction += UpdateExtents;

            UpdateExtents();
        }


        public Action ExtentsUpdate = () => {};
        public Vector2 Extents { get; private set; } = Vector2.Zero;

        private void UpdateExtents() {
            Extents = new Vector2(transform.LocalPosition.X + GameManager.ViewOffset.X*2,
                                  transform.LocalPosition.Y + GameManager.ViewOffset.Y*2);
            
            transform.UpdateChildren();
            ExtentsUpdate();
        }
        
    }
}