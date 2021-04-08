using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core.Particles {
    public class Particle {
        public int Seed;

        public ParticleType ParticleType = ParticleType.Sprite;

        public Vector3 Position;
        public Vector3 AxisOfRotation;
        public Color StartColor;
        public Color Color;

        public float StartLifetime;
        public float RemainingLifetime;
        public float TimeSinceStart => StartLifetime - RemainingLifetime;
        public float InverseLifetimeRatio => RemainingLifetime / StartLifetime;
        public float LifetimeRatio => TimeSinceStart / StartLifetime;
        public bool Alive => RemainingLifetime > 0;
        

        public Vector3 Rotation3D;
        public float Rotation {
            get => Rotation3D.Z;
            set { Rotation3D = new Vector3(0, 0, value); }
        }

        public Vector3 Scale3D;
        public Vector2 Scale {
            get => Scale3D.ToVector2();
            set { Scale3D = value.ToVector3(); }
        }

        public Vector3 Velocity3D;
        public Vector2 Velocity {
            get => Velocity3D.ToVector2();
            set { Velocity3D = value.ToVector3(); }
        }

        public Vector3 AngularVelocity3D;
        public float AngularVelocity {
            get => AngularVelocity3D.Z;
            set { AngularVelocity3D = new Vector3(0, 0, value); }
        }


        #region Initializers

        public void Initialize() {
            Seed = GameManager.DeltaRandom.Next(int.MinValue, int.MaxValue);

            Position = Vector3.Zero;
            AxisOfRotation = new Vector3(0, 0, 1);
            StartColor = Color.White;
            Color = StartColor;

            StartLifetime = 5;
            RemainingLifetime = StartLifetime;

            Rotation3D = Vector3.Zero;
            Scale3D = Vector3.One;
            Velocity3D = Vector3.Zero;
            AngularVelocity3D = Vector3.Zero;
        }

        public void Initialize(Vector3 position, Vector3 axisOfRotation, Color startColor,
                               Vector3 startRotation3D, Vector3 startSize3D, Vector3 velocity3D, Vector3 angularVelocity3D,
                               float startLifetime = 5, ParticleType particleType = ParticleType.Sprite, int seed = 0) {
            
            Seed = seed == 0 ? GameManager.DeltaRandom.Next(int.MinValue, int.MaxValue) : seed;

            Position = position;
            AxisOfRotation = axisOfRotation;
            StartColor = startColor;
            Color = startColor;

            StartLifetime = startLifetime;
            RemainingLifetime = startLifetime;

            Rotation3D = startRotation3D;
            Scale3D = startSize3D;
            Velocity3D = velocity3D;
            AngularVelocity3D = angularVelocity3D;

            ParticleType = particleType;
        }

        public void Initialize(Vector3 position, Color startColor,
                               float startRotation, Vector2 startSize, Vector2 velocity, float angularVelocity,
                               float startLifetime = 5, int seed = 0) {

            Seed = seed == 0 ? GameManager.DeltaRandom.Next(int.MinValue, int.MaxValue) : seed;

            Position = position;
            AxisOfRotation = new Vector3(0, 0, 1);
            StartColor = startColor;
            Color = startColor;

            StartLifetime = startLifetime;
            RemainingLifetime = startLifetime;

            Rotation = startRotation;
            Scale = startSize;
            Velocity = velocity;
            AngularVelocity = angularVelocity;

            ParticleType = ParticleType.Sprite;
        }

        public void Initialize(Vector3 position, Vector3 axisOfRotation, Color startColor,
                               float startRotation, Vector2 startSize, Vector2 velocity, float angularVelocity,
                               float startLifetime = 5, int seed = 0) {

            Seed = seed == 0 ? GameManager.DeltaRandom.Next(int.MinValue, int.MaxValue) : seed;

            Position = position;
            AxisOfRotation = axisOfRotation;
            StartColor = startColor;
            Color = startColor;

            StartLifetime = startLifetime;
            RemainingLifetime = startLifetime;

            Rotation = startRotation;
            Scale = startSize;
            Velocity = velocity;
            AngularVelocity = angularVelocity;

            ParticleType = ParticleType.Sprite;
        }


        public void Initialize(Vector3 position, Vector2 startSize, Vector2 velocity,
                               float startLifetime = 5, int seed = 0) {

            Seed = seed == 0 ? GameManager.DeltaRandom.Next(int.MinValue, int.MaxValue) : seed;

            Position = position;
            AxisOfRotation = new Vector3(0, 0, 1);
            StartColor = Color.White;
            Color = StartColor;

            StartLifetime = startLifetime;
            RemainingLifetime = startLifetime;

            Rotation = 0;
            Scale = startSize;
            Velocity = velocity;
            AngularVelocity = 0;

            ParticleType = ParticleType.Sprite;
        }

        #endregion


        #region Methods

        //public void Revive() {
        //    RemainingLifetime = StartLifetime;
        //}

        #endregion
    }

    public enum ParticleType {
        Sprite,
        Model
    }
}
