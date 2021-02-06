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

            GameObject levelMapGO = GameObjects.AddReturn(new GameObject());
            MapManager map = levelMapGO.AddComponent<MapManager>();
            map.GenerateLevel(LevelID.QuarantineLevel);


            GameObjects.Add(new Prefab_Player());

            //GameObjects.Add(new Prefab_Chaser());

            GameObjects.Add(new Prefab_Reticle());


            GameObject sk = GameObjects.AddReturn(new GameObject());
            TextRenderer tr = sk.AddComponent<TextRenderer>(Resources.Font_Debug, "Score: 999");
            sk.transform.Position = new Vector3(0, 140, 0);
            sk.transform.Scale = new Vector3(0.2f, 0.2f, 1);
            tr.DrawLayer = DrawLayer.ID["HUD"];
            tr.OrderInLayer = 90;
            tr.Color = Color.Red;

            GameObject wk = GameObjects.AddReturn(new GameObject());
            TextRenderer tr2 = wk.AddComponent<TextRenderer>(Resources.Font_Debug, "Wave 1");
            wk.transform.Position = new Vector3(0, 120, 0);
            wk.transform.Scale = new Vector3(0.14f, 0.14f, 1);
            tr2.DrawLayer = DrawLayer.ID["HUD"];
            tr2.OrderInLayer = 89;
            tr2.Color = Color.Red;

            ScoreKeeper score = sk.AddComponent<ScoreKeeper>();
            score.WaveText = tr;
            score.ScoreText = tr2;

            score.NextWave();
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
