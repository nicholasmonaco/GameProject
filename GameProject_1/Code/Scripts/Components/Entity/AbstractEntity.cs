using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public abstract class AbstractEntity : Component {
        public AbstractEntity(GameObject attached) : base(attached) { }



    }
}