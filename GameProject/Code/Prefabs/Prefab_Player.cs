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

            AnimationController animController = AddComponent<AnimationController>(7);


            #region Create Animation States

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

            ArmController rightArm = Instantiate<Prefab_Arm>(Vector3.Zero, transform).GetComponent<ArmController>();
            rightArm.gameObject.Name = "Right Player Arm";
            rightArm.transform.LocalPosition = new Vector3(armDist, 0, 0);
            rightArm.LastOrigLocalPos = rightArm.transform.LocalPosition;

            player.Arms = new List<ArmController>(2);
            player.Arms.Add(leftArm);
            player.Arms.Add(rightArm);

            // End adding components
        }

    }
}
