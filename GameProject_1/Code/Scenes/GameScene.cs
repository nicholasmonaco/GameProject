using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Prefabs;

namespace GameProject.Code.Scenes {
    public class GameScene : Scene {

        public GameScene() : base() {
            // This is essentially where there should be a list of default GameObjects in the scene.
            GameObjects.Add(new Prefab_MainCamera());
            GameObjects.Add(new Prefab_TestPrefab());
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
