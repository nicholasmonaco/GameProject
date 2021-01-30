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
    public class GameScene : Scene {

        //public GameScene() : base() {
            
        //}

        public override void Init() {
            base.Init();

            // This is essentially where there should be a list of default GameObjects in the scene.
            GameObjects.Add(new Prefab_MainCamera());

            GameObject test = GameObjects.AddReturn(new Prefab_TestPrefab());
            test.transform.Position = new Vector3(0, 25, 0);
            test.GetComponent<Rigidbody2D>().Velocity = new Vector2(20, 0);
            test.GetComponent<SpriteRenderer>().Tint = Color.Yellow;

            GameObject test2 = GameObjects.AddReturn(new Prefab_TestPrefab());
            test2.transform.Position = new Vector3(150, 25, 0);
            test2.GetComponent<Rigidbody2D>().Velocity *= -1;
            //test2.transform.Scale *= 1.5f;
            //test2.RemoveComponent<Rigidbody2D>();
            test2.AddComponent<KeyboardController>();
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
