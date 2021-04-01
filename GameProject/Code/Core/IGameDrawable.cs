using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core {
    public interface IGameDrawable {
        public Material Material { get; set; }

        public void Draw(SpriteBatch sb);
    }
}
