using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core.Components {
    public class Camera : Component {
        public float Size = 5;
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix ViewMatrix => transform.ViewMatrix;


        public float NearZ = 0;
        public float FarZ = -100;



        public Camera(GameObject attached) : base(attached) {
            GameManager.MainCamera = this; // For now, there should only be one camera ever, so this is fine

            //ProjectionMatrix = Matrix.CreateOrthographic(GameManager.Viewport.Width, GameManager.Viewport.Height, NearZ, FarZ);
            ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, GameManager.Viewport.Width, GameManager.Viewport.Height, 0, NearZ, FarZ);

            // Now that the projection matrix exists, we need to modify it so that 0,0 is at the center of the screen and +y is up, not down.
            //ProjectionMatrix = Matrix.CreateTranslation(GameManager.Resolution.X, GameManager.Resolution.Y, 0) * ProjectionMatrix;

            transform.ViewChangeAction = transform.ViewChangeAction_Camera;
        }




        public Matrix FinalTransformationMatrix => ProjectionMatrix * ViewMatrix;
        //we're going to need another step between => and ProjectionMatrix to flip the y
        //also probably something to make worldspace not be clientspace (in between projection and view matricies)

    }
}
