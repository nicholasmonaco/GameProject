using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core.Components {
    public class Camera : Component {
        public Point AspectRatio = new Point(16, 9);
        public float Size = 5;

        public Rectangle FOV { get { return new Rectangle(transform.Position.ToPoint(), AspectRatio.Mult(Size * 12)); } }



        public Camera(GameObject attached) : base(attached) {
            GameManager.MainCamera = this; // For now, there should only be one camera ever, so this is fine
        }


        public bool InFOV(Vector2 position) {
            return FOV.Contains(position.ToPoint());
        }

        public Rectangle GetScreenPosRect(Rectangle worldRect, Vector2 scale) {
            return new Rectangle(worldRect.Location - transform.Position.ToPoint(), 
                                 worldRect.Size * (scale / Size).ToPoint());
        }
    }
}
