using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core.Components {
    public class Renderer : Component {
        public Renderer(GameObject attached) : base(attached) {
            
        }


        //public override void Draw(SpriteBatch sb) { //look into how to override this correctly
        //    //draw it with the MIDPOINT of the sprite being drawn at the transform's position

        //}



        public bool IsInCamera(Camera camera) {
            return false;
        }
    }
}
