// GameScene.cs - Nick Monaco

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Prefabs;
using Microsoft.Xna.Framework;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Components.Entity;

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
            Instantiate(new Prefab_Canvas());

            GameObject levelMapGO = GameObjects.AddReturn(new GameObject());
            MapManager map = levelMapGO.AddComponent<MapManager>();

            StartCoroutine(StartLevel());
        }

        private IEnumerator StartLevel() {
            yield return StartCoroutine(GameManager.Map.GenerateLevel(LevelID.QuarantineLevel)); //replace with levelID variable later

            // Create minimap
            Instantiate(new Prefab_Minimap());

            // Spawn player
            //GameObjects.Add(new Prefab_Player());
            Instantiate(new Prefab_Player());

            // Create reticle
            //GameObjects.Add(new Prefab_Reticle());
            Instantiate(new Prefab_Reticle());
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
