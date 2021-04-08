using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components.Entity.Arms;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Prefabs {
    public class Prefab_Arm : GameObject {
        public Prefab_Arm() : base() {
            Name = "Arm";

            Layer = LayerID.Bullet_Good;

            SpriteRenderer armSprite = AddComponent<SpriteRenderer>();
            armSprite.Sprite = Resources.Sprite_Arm_Outer;
            armSprite.DrawLayer = DrawLayer.ID[DrawLayers.Player];
            armSprite.OrderInLayer = 31;

            CircleCollider2D coll = AddComponent<CircleCollider2D>(4.5f);
            coll.Bounds.OrigCenter -= new Vector2(4, 4);
            coll.Bounds.Center -= new Vector2(4, 4);
            coll.IsTrigger = true;
            

            


            //arm particles
            ParticleSystem testParticles = AddComponent<ParticleSystem>();
            testParticles.Sprite = armSprite.Sprite;
            testParticles.Material.BatchID = armSprite.Material.BatchID;
            testParticles.DrawLayer = DrawLayer.ID[DrawLayers.Player];
            testParticles.OrderInLayer = 30;

            testParticles.Shape.RawRadius = 10;

            testParticles.EmissionModule.RateOverTime = 10;
            testParticles.Main.MaxParticles = 100;
            testParticles.Main.SimulationSpace = Core.Particles.ParticleSimulationSpace.World;
            testParticles.Main.StartLifetime = new ValueCurve_Float(0.15f, 0.25f, InterpolationBehaviour.Lerp);
            //testParticles.Main.StartSize = new ValueCurve_Vector3(new Vector3(1f, 1f, 1));
            testParticles.Main.StartSpeed = new ValueCurve_Vector3(Vector3.Zero);

            testParticles.Main.StartColor = new ValueCurve_Color(new Color(255, 255, 255, 170));

            testParticles.ColorOverLifetimeModule.Enabled = true;
            testParticles.ColorOverLifetimeModule.Gradient = new List<(float, Color)>() {
                (0, Color.Transparent),
                (0.15f, Color.White),
                (0.85f, Color.White),
                (1, Color.Transparent)
            };

            testParticles.Main.PlayOnAwake = false; 


            // Arm controller
            ArmController arm = AddComponent<ArmController>();
            arm.ArmRenderer = armSprite;
            arm.ArmParticles = testParticles;
        }
    }
}
