using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Bullet;
using GameProject.Code.Scripts;

namespace GameProject.Code.Prefabs {
    public class Prefab_Bullet : GameObject {

        public Prefab_Bullet() : base() {
            Name = "Bullet";

            transform.Position = GameManager.PlayerTransform.Position;

            // Add components
            Rigidbody2D rb = AddComponent<Rigidbody2D>();

            SpriteRenderer sr = AddComponent<SpriteRenderer>();
            sr.Sprite = Resources.Sprite_Bullet_Standard;
            //sr.Color = Color.White; //base this off of items and stuff
            sr.DrawLayer = DrawLayer.ID[DrawLayers.Projectiles];
            sr.OrderInLayer = 100;
            sr.SpriteScale = new Vector2(0.65f, 0.65f);
            sr.Material.BatchID = BatchID.AbovePlayer;

            //Collider2D collider = AddComponent<CircleCollider2D>(this, sr.Sprite.Width * sr.SpriteScale.X);
            Collider2D collider = AddComponent<CircleCollider2D>(sr);
            collider.IsTrigger = true;

            Bullet_Standard bullet = AddComponent<Bullet_Standard>();
            bullet.SetComponents(rb, collider, sr);


            //Particles
            //ParticleSystem testParticles = AddComponent<ParticleSystem>();
            //testParticles.Sprite = Resources.Sprite_ParticleDefault;
            //testParticles.Material.BatchID = BatchID.BehindPlayer;
            //testParticles.DrawLayer = DrawLayer.ID[DrawLayers.Projectiles];
            //testParticles.OrderInLayer = 1;

            //testParticles.EmissionModule.RateOverTime = 70;
            //testParticles.Main.SimulationSpace = Core.Particles.ParticleSimulationSpace.Local;
            //testParticles.Main.StartLifetime = new ValueCurve_Float(0.15f, 0.25f, InterpolationBehaviour.Lerp);
            //testParticles.Main.StartSize = new ValueCurve_Vector3(new Vector3(0.005f, 0.01f, 1));
            //float speed = 0.4f;
            ////testParticles.Main.StartSpeed = new ValueCurve_Vector3(new Vector3(-speed, -speed, 0), new Vector3(speed, speed, 0), InterpolationBehaviour.ComponentIndependent);
            //testParticles.Main.StartSpeed = new ValueCurve_Vector3(Vector3.Zero);

            ////testParticles.Main.StartColor = new ValueCurve_Color(PlayerStats.ShotColor); //should be set upon bullet init
            //testParticles.ColorOverLifetimeModule.Gradient = new List<(float, Color)>() {
            //    (0, Color.Transparent),
            //    (0.15f, PlayerStats.ShotColor),
            //    (0.5f, Color.Transparent),
            //    (0.85f, PlayerStats.ShotColor),
            //    (1, Color.Transparent)
            //};

            //testParticles.SizeOverLifetimeModule.Enabled = true;
            //testParticles.SizeOverLifetimeModule.Gradient = new List<(float, Vector3)>() {
            //    (0, new Vector3(0.01f, 0.01f, 1)),
            //    (0.85f, new Vector3(0.01f, 0.01f, 1)),
            //    (1, new Vector3(0, 0, 0)),
            //};
            //

            // End adding components
        }

    }
}
