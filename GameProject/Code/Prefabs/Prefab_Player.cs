using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.Entity.Arms;
using GameProject.Code.Core.Animation;

namespace GameProject.Code.Prefabs {
    public class Prefab_Player : GameObject {

        public Prefab_Player() : base() {
            Name = "Player";
            Layer = LayerID.Player;

            // Adding components
            //RectCollider2D collider = AddComponent<RectCollider2D>(26, 26); //Change this to be a circle collider later maybe?
            CircleCollider2D collider = AddComponent<CircleCollider2D>(9);

            AddComponent<Rigidbody2D>();

            SpriteRenderer bodyRenderer = AddComponent<SpriteRenderer>(Resources.Sprite_Pixel);
            bodyRenderer.DrawLayer = DrawLayer.ID[DrawLayers.Player];
            bodyRenderer.OrderInLayer = 20;
            bodyRenderer.Material.BatchID = BatchID.Player;

            //debug testing
            //bodyRenderer.Material.BatchID = BatchID.NonAuto;
            //bodyRenderer.Material.Shader = Resources.Effect_Outline;
            //Scene.ChangeShader(BatchID.Player, Resources.Effect_Outline);
            //


            AnimationController animController = AddComponent<AnimationController>(7);


            #region Create Animation States

            // Idle Animations

            List<AnimationData> animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){ 
                    (0,  Resources.Sprite_Player_IdleDown_0),
                    (30, Resources.Sprite_Player_IdleDown_1)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Idle_Down, new Animation(animController, true, animData));


            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_IdleUp_0),
                    (30, Resources.Sprite_Player_IdleUp_1)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Idle_Up, new Animation(animController, true, animData));


            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_IdleLeft_0),
                    (30, Resources.Sprite_Player_IdleLeft_1)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Idle_Left, new Animation(animController, true, animData));


            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_IdleRight_0),
                    (30, Resources.Sprite_Player_IdleRight_1)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Idle_Right, new Animation(animController, true, animData));


            // Walking Animations

            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_WalkDown_0),
                    (20, Resources.Sprite_Player_WalkDown_1),
                    (40, Resources.Sprite_Player_WalkDown_2)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Walk_Down, new Animation(animController, true, animData));


            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_WalkUp_0),
                    (20, Resources.Sprite_Player_WalkUp_1),
                    (40, Resources.Sprite_Player_WalkUp_2)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Walk_Up, new Animation(animController, true, animData));


            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_WalkLeft_0),
                    (20, Resources.Sprite_Player_WalkLeft_1),
                    (40, Resources.Sprite_Player_WalkLeft_2)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Walk_Left, new Animation(animController, true, animData));


            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_WalkRight_0),
                    (20, Resources.Sprite_Player_WalkRight_1),
                    (40, Resources.Sprite_Player_WalkRight_2)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Walk_Right, new Animation(animController, true, animData));


            // Other Animations

            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_ItemPickup)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.ItemPickup, new Animation(animController, false, animData));


            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_Damage)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Damage, new Animation(animController, false, animData));


            animData = new List<AnimationData>() {
                new AnimationData(bodyRenderer, new List<(int, Texture2D)>(){
                    (0,  Resources.Sprite_Player_ItemPickup)
                }),
            };
            animController.StateMachine.AddState((int)PlayerAnimationState.Die, new Animation(animController, false, animData));

            #endregion


            PlayerController player = AddComponent<PlayerController>();


            float armDist = 18;

            ArmController leftArm = Instantiate<Prefab_Arm>(Vector3.Zero, transform).GetComponent<ArmController>();
            leftArm.gameObject.Name = "Left Player Arm";
            leftArm.transform.LocalPosition = new Vector3(-armDist, 0, 0);
            leftArm.transform.Scale *= new Vector3(-1, 1, 1);
            leftArm.LastOrigLocalPos = leftArm.transform.LocalPosition;
            leftArm.RushDelay = 0.05f;
            leftArm.ArmRenderer.Material.BatchID = BatchID.Player;
            leftArm.ArmParticles.Material.BatchID = BatchID.Player;

            ArmController rightArm = Instantiate<Prefab_Arm>(Vector3.Zero, transform).GetComponent<ArmController>();
            rightArm.gameObject.Name = "Right Player Arm";
            rightArm.transform.LocalPosition = new Vector3(armDist, 0, 0);
            rightArm.LastOrigLocalPos = rightArm.transform.LocalPosition;
            rightArm.ArmRenderer.Material.BatchID = BatchID.Player;
            rightArm.ArmParticles.Material.BatchID = BatchID.Player;

            player.Arms = new List<ArmController>(2);
            player.Arms.Add(leftArm);
            player.Arms.Add(rightArm);


            //DEBUG
            ParticleSystem testParticles = AddComponent<ParticleSystem>();
            testParticles.Sprite = Resources.Sprite_Pixel;
            testParticles.Material.BatchID = BatchID.BehindPlayer;
            testParticles.DrawLayer = DrawLayer.ID[DrawLayers.Projectiles];
            testParticles.OrderInLayer = 1;

            testParticles.Shape.Offset = new Vector3(0, -5, 0);

            testParticles.EmissionModule.RateOverTime = 70;
            testParticles.Main.SimulationSpace = Core.Particles.ParticleSimulationSpace.World;
            testParticles.Main.StartLifetime = new ValueCurve_Float(0.25f * 2, 0.45f * 2, InterpolationBehaviour.Lerp);
            testParticles.Main.StartSize = new ValueCurve_Vector3(new Vector3(3f, 3f, 1));
            float speed = 0.4f;
            testParticles.Main.StartSpeed = new ValueCurve_Vector3(new Vector3(-speed, -speed, 0), new Vector3(speed, speed, 0), InterpolationBehaviour.ComponentIndependent);

            //testParticles.Main.StartColor = new ValueCurve_Color(new Color(0, 0, 0, 255), new Color(255, 255, 255, 255), InterpolationBehaviour.ComponentIndependent);

            testParticles.Main.StartColor = new ValueCurve_Color(Color.Gray);

            //testParticles.ColorOverLifetimeModule.Enabled = true;
            //testParticles.ColorOverLifetimeModule.Gradient = new List<(float, Color)>() {
            //    (0, Color.White),
            //    (0.9f, Color.White),
            //    (1, Color.Transparent)
            //};

            testParticles.SizeOverLifetimeModule.Enabled = true;
            testParticles.SizeOverLifetimeModule.Gradient = new List<(float, Vector3)>() {
                (0, new Vector3(3, 3, 1)),
                (1, new Vector3(0, 0, 0)),
            };

            player.RunParticles = testParticles;

            //DEBUG

            // End adding components
        }

    }
}
