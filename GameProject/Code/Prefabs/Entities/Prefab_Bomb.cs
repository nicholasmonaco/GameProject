using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Prefabs {
    public class Prefab_Bomb : GameObject {
        public Prefab_Bomb() : base() {
            Name = "Bomb";

            Layer = LayerID.Pickup;

            SpriteRenderer spriteRend = AddComponent<SpriteRenderer>();
            spriteRend.Sprite = Resources.Sprite_Pickups[Pickup.Bomb];
            spriteRend.DrawLayer = DrawLayer.ID[DrawLayers.Projectiles];
            spriteRend.OrderInLayer = 20;
            spriteRend.SpriteScale *= 0.7f;//thsi is bad, jsut resprite it

            Rigidbody2D rigidbody = AddComponent<Rigidbody2D>();
            rigidbody.Drag = 2.5f;

            CircleCollider2D coll = AddComponent<CircleCollider2D>(4.8f);
            coll.Bounds.OrigCenter += new Vector2(0, -2.5f);
            coll.Bounds.Center += new Vector2(0, -2.5f);
            //coll.IsTrigger = true;

            //CircleCollider2D expCollider = AddComponent<CircleCollider2D>(20);
            //expCollider.IsTrigger = true;
            //expCollider.Enabled = false;

            //Explosion particles
            ParticleSystem particles = AddComponent<ParticleSystem>();
            particles.Sprite = Resources.Sprite_BombExplosion;
            particles.Material.BatchID = spriteRend.Material.BatchID;
            particles.DrawLayer = DrawLayer.ID[DrawLayers.Projectiles];
            particles.OrderInLayer = 21;

            particles.Main.Looping = false;

            particles.Shape.ShapeType = Core.Particles.ShapeType.Circle;
            particles.Shape.RawRadius = 30;

            int explosionParticleCount = 400;

            particles.EmissionModule.RateOverTime = 0;
            particles.Main.MaxParticles = explosionParticleCount;
            particles.Main.SimulationSpace = Core.Particles.ParticleSimulationSpace.World;
            particles.Main.StartLifetime = new ValueCurve_Float(0.1f, 0.15f, InterpolationBehaviour.Lerp);
            particles.Main.StartSize = new ValueCurve_Vector3(new Vector3(0.5f, 0.5f, 1), new Vector3(1.25f, 1.25f, 1));
            particles.Main.StartSpeed = new ValueCurve_Vector3(Vector3.Zero);

            particles.EmissionModule.AddBurst((0, explosionParticleCount));

            particles.Main.StartColor = new ValueCurve_Color(Color.DarkGray);

            //particles.ColorOverLifetimeModule.Enabled = true;
            //particles.ColorOverLifetimeModule.Gradient = new List<(float, Color)>() {
            //    (0, Color.White),
            //    (0.85f, Color.White),
            //    (1, Color.Transparent)
            //};

            particles.TextureSheetAnimationModule.Enabled = true;
            particles.TextureSheetAnimationModule.Tiles = new Point(7, 1);
            particles.TextureSheetAnimationModule.SetFramesEvenly_X();

            particles.Main.PlayOnAwake = false;



            Bomb bomb = AddComponent<Bomb>(Bomb.StandardExplosionForce);
            coll.Enabled = false;
            bomb.PhysicalCollider = coll;
            //bomb.ExplosionCollider = expCollider;
        }
    }
}
