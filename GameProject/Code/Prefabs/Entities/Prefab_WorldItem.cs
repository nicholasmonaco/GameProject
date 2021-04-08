using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_WorldItem : GameObject {
        public Prefab_WorldItem(ItemID id) {
            Name = $"Item Pedastal ({id.ToString()})";
            Layer = LayerID.Item;

            SpriteRenderer pedastalRend = AddComponent<SpriteRenderer>(Resources.Sprite_ItemPedastal);
            pedastalRend.DrawLayer = DrawLayer.ID[DrawLayers.Entities];
            pedastalRend.OrderInLayer = 100;

            Texture2D sprite = Resources.Sprites_Items.ContainsKey(id) ? Resources.Sprites_Items[id] : Resources.Sprite_Item_Unknown;
            SpriteRenderer itemRend = AddComponent<SpriteRenderer>(sprite);
            itemRend.DrawLayer = DrawLayer.ID[DrawLayers.Entities];
            itemRend.OrderInLayer = 101;
            itemRend.Material.BatchID = BatchID.AbovePlayer;

            WorldItem item = AddComponent<WorldItem>();
            item.Init(id, itemRend, pedastalRend);

            Collider2D coll = AddComponent<CircleCollider2D>(10);
            //coll.IsTrigger = true;



            //Particles
            ParticleSystem testParticles = AddComponent<ParticleSystem>();
            testParticles.Sprite = Resources.Sprite_ParticleEx;
            testParticles.Material.BatchID = BatchID.AbovePlayer;
            testParticles.DrawLayer = DrawLayer.ID[DrawLayers.Entities];
            testParticles.OrderInLayer = 99;

            testParticles.Shape.ShapeType = Core.Particles.ShapeType.Circle;
            testParticles.Shape.Scale = new Vector3(1, 1.5f, 1);
            testParticles.Shape.RawRadius = 10;

            testParticles.EmissionModule.RateOverTime = 2.5f / 3f;
            testParticles.Main.SimulationSpace = Core.Particles.ParticleSimulationSpace.World;
            testParticles.Main.StartLifetime = new ValueCurve_Float(3.5f, 6f, InterpolationBehaviour.Lerp);
            //testParticles.Main.StartSize = new ValueCurve_Vector3(new Vector3(0.005f, 0.01f, 1));
            float speed = 4f;
            testParticles.Main.StartSpeed = new ValueCurve_Vector3(new Vector3(-speed, -speed, 0), new Vector3(speed, speed, 0), InterpolationBehaviour.ComponentIndependent);
            //testParticles.Main.StartSpeed = new ValueCurve_Vector3(Vector3.Zero);

            //testParticles.Main.StartColor = new ValueCurve_Color(PlayerStats.ShotColor); //should be set upon bullet init
            testParticles.ColorOverLifetimeModule.Enabled = true;
            testParticles.ColorOverLifetimeModule.Gradient = new List<(float, Color)>() {
                (0, Color.Transparent),
                (0.15f, Color.Gold),
                (0.85f, Color.Gold),
                (1, Color.Transparent)
            };

            testParticles.SizeOverLifetimeModule.Enabled = true;
            testParticles.SizeOverLifetimeModule.Gradient = new List<(float, Vector3)>() {
                (0, new Vector3(0.8f, 0.8f, 1)),
                (0.85f, new Vector3(1f, 1f, 1)),
                (1, new Vector3(0, 0, 0)),
            };

            testParticles.Main.Prewarm = true;
            //
        }
    }
}
