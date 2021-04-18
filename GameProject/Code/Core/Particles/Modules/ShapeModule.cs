using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core.Particles {
    public class ShapeModule : IParticleModule {
        public bool Enabled { get; set; }
        public ParticleSystem AttachedSystem { get; set; }


        public ShapeType ShapeType;

        public Vector3 Position => AttachedSystem.transform.Position + AttachedSystem.RelativePosition;
        public Vector3 Offset;
        public Vector3 Scale;
        public Vector3 Rotation;

        public float RawRadius;
        public Vector3 Radius => RawRadius * Scale;

        public float Width;
        public float Height;

        public SpriteRenderer SpriteRenderer;
        public Texture2D Sprite => SpriteRenderer.Sprite;


        #region Initializers

        public void Initialize() {
            Initialize(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.Zero);
        }

        public void Initialize(Vector3 position, Vector3 scale, Vector3 rotation, ShapeType shapeType = ShapeType.Circle) {
            Initialize(position, Vector3.Zero, scale, rotation, shapeType);
        }

        public void Initialize(Vector3 position, Vector3 offset, Vector3 scale, Vector3 rotation, ShapeType shapeType = ShapeType.Circle) {
            Enabled = true;
            
            Offset = offset;
            Scale = scale;
            Rotation = rotation;
            ShapeType = shapeType;

            RawRadius = 5;
            Width = 10;
            Height = 10;
            SpriteRenderer = null;
        }

        #endregion


        public Vector3 GetPosition() {
            switch (ShapeType) {
                default:
                case ShapeType.Circle: return GetCirclePosition();
                case ShapeType.Rectangle: return GetRectanglePosition();
                case ShapeType.Edge: return GetEgdePosition();
                case ShapeType.Sprite: return GetSpritePosition();
            }
        }

        private Vector3 GetCirclePosition() {
            Vector2 offset = new Vector2(GameManager.DeltaRandom.NextValue(-1, 1),
                                         GameManager.DeltaRandom.NextValue(-1, 1)).Norm();

            offset *= Radius.ToVector2() * GameManager.DeltaRandom.NextValue(0, 1);

            return Position + Offset + offset.RotateDirectionNonUnit(Rotation.Z).ToVector3(); 
        }

        private Vector3 GetRectanglePosition() {
            float halfwidth = Width / 2f;
            float halfheight = Height / 2f;
            Vector2 offset = new Vector2(GameManager.DeltaRandom.NextValue(-halfwidth, halfwidth),
                                         GameManager.DeltaRandom.NextValue(-halfheight, halfheight));

            return Position + Offset + offset.RotateDirectionNonUnit(Rotation.Z).ToVector3();
        }

        private Vector3 GetEgdePosition() {
            float halfwidth = Width / 2f;
            float halfheight = Height / 2f;

            float t = GameManager.DeltaRandom.NextValue();

            Vector2 offset = Vector2.Lerp(new Vector2(-halfwidth, -halfheight), new Vector2(halfwidth, halfheight), t);

            return Position + Offset + offset.RotateDirectionNonUnit(Rotation.Z).ToVector3();
        }

        private Vector3 GetSpritePosition() {
            //todo
            return Vector3.Zero;
        }
    }


    public enum ShapeType {
        // This could be expanded later to include 3D shapes, but I'm not dealing with that right now

        Circle,
        Rectangle,
        Edge,
        Sprite
    }
}
