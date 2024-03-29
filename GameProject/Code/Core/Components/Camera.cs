﻿// Camera.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core.Components {

    /// <summary>
    /// The class governing the Camera component.
    /// Currently, only one camera should be active in the scene at any given time, but this
    /// could be worked around with some clever scripting.
    /// </summary>
    public class Camera : Component {
        public static Camera main => GameManager.MainCamera;


        private const float ConstantSize = 1f;
        public float Size;
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix ViewMatrix => transform.ViewMatrix;

        // At this resolution, the camera has a size of ConstantSize
        //public static readonly Vector2 ConstantResolution = new Vector2(480, 320); // Default Monogame resolution: 800x480 px
        public static readonly Vector2 ConstantResolution = new Vector2(465, 305);

        public Vector2 CurrentResolution { 
            set {
                float scaleFactor = value.Y / ConstantResolution.Y;
                Size = ConstantSize * scaleFactor;
            } 
        }


        public float NearZ = 0;
        public float FarZ = -100;



        public Camera(GameObject attached) : base(attached) {
            GameManager.MainCamera = this; // For now, there should only be one camera ever, so this is fine

            ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, GameManager.Viewport.Width, GameManager.Viewport.Height, 0, NearZ, FarZ);

            //Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GameManager.Viewport.Width / GameManager.Viewport.Height, 0, 1000);

            // Now that the projection matrix exists, we need to modify it so that 0,0 is at the center of the screen and +y is up, not down.
            //ProjectionMatrix = Matrix.CreateTranslation(GameManager.Resolution.X, GameManager.Resolution.Y, 0) * ProjectionMatrix;

            CurrentResolution = GameManager.Resolution.ToVector2();

            transform.ViewChangeAction = transform.ViewChangeAction_Camera;
            transform.ViewChangeAction();
        }

        public void ResetResolution() {
            CurrentResolution = GameManager.Resolution.ToVector2();
            transform.ViewChangeAction();
            transform.UpdateChildren();
        }



        public Matrix FinalTransformationMatrix => ProjectionMatrix * ViewMatrix;
        //also probably want something to make worldspace not be clientspace (in between projection and view matricies)


    }
}
