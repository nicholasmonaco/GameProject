using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class Reticle : Component {
        public Reticle(GameObject attached) : base(attached) { }

        public override void Update() {
            transform.Position = Input.MouseWorldPosition.ToVector3();
        }
    }
}