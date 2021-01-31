// GameScene.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Prefabs;
using Microsoft.Xna.Framework;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Scenes {
    
    /// <summary>
    /// The scene where the main game occurs.
    /// </summary>
    public class GameScene : Scene {

        //public GameScene() : base() {
            
        //}

        public override void Init() {
            base.Init();

            // This is essentially where there should be a list of default GameObjects in the scene.
            GameObjects.Add(new Prefab_MainCamera());


            for(int x = -1; x < 2; x += 2) {
                for (int y = -1; y < 2; y += 2) {
                    GameObject wallCorner = GameObjects.AddReturn(new GameObject());
                    wallCorner.transform.Position = new Vector3(x * 117, y * 78, 0);
                    wallCorner.transform.Scale = new Vector3(-x, y, 0);
                    wallCorner.AddComponent<SpriteRenderer>();
                    SpriteRenderer wallCornerRend = wallCorner.GetComponent<SpriteRenderer>();
                    wallCornerRend.Sprite = Resources.Sprite_Room_WallCorner_01;
                    wallCornerRend.DrawLayer = DrawLayer.ID["Background"];
                    wallCornerRend.OrderInLayer = 21;
                }
            }

            //GameObject floor = GameObjects.AddReturn(new GameObject());
            //floor.transform.Scale = new Vector3(480, 320, 0);
            //floor.AddComponent<SpriteRenderer>();
            //SpriteRenderer sr = floor.GetComponent<SpriteRenderer>();
            //sr.DrawLayer = DrawLayer.ID["Background"];
            //sr.OrderInLayer = 20; // Just in case
            //sr.Sprite = Resources.Sprite_Pixel;
            //sr.Tint = new Color(28, 35, 64);

        }



        //public override void Awake() {
        //    
        //}

        //public override void Start() {
        //    
        //}

        //public override void LoadContent(ContentManager content) {

        //}

        //public override void UnloadContent() {

        //}

        //public override void Update() {

        //}

        //public override void FixedUpdate() {

        //}

        //public override void LateUpdate() {

        //}

        //public override void Draw(SpriteBatch sb) {

        //}
    }
}
