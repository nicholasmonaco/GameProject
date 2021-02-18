using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Prefabs;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class ScoreKeeper : Component {
        public ScoreKeeper(GameObject attached) : base(attached) { }

        public override void PreAwake() {
            base.PreAwake();

            //GameManager.ScoreKeeper = this;
        }


        //public TextRenderer ScoreText;
        //public TextRenderer WaveText;


        //public void UpdateScore(int newScore) {
        //    GameManager.Score = newScore;
        //    ScoreText.Text = "Score: " + GameManager.Score;
        //}

        //public void ResetWave() {
        //    GameManager.Wave = 1;
        //    WaveText.Text = "Wave " + GameManager.Wave;
        //}

        //public void NextWave() {
        //    GameManager.Wave++;
        //    WaveText.Text = "Wave " + GameManager.Wave;
        //    StartCoroutine(NextWave_C());
        //}

        //private IEnumerator NextWave_C() {
        //    yield return new WaitForSeconds(1.2f);

        //    //spawn new enemies

        //    int enemies = GameManager.WorldRandom.Next(1, (int)MathF.Max(1, GameManager.Wave / 2.5f));
        //    GameManager.EnemiesLeft = enemies;

        //    for(int i = 0; i < enemies; i++) {
        //        Transform e = GameManager.CurrentScene.GameObjects.AddReturn(new Prefab_Chaser()).transform;
        //        e.gameObject.Awake();
        //        int xPos = GameManager.WorldRandom.Next(-1, 1) == 0 ? GameManager.WorldRandom.Next(60, 200) : GameManager.WorldRandom.Next(-200, -60);
        //        int yPos = GameManager.WorldRandom.Next(-1, 1) == 0 ? GameManager.WorldRandom.Next(60, 100) : GameManager.WorldRandom.Next(-100, -60);

        //        e.Position = new Vector3(xPos, yPos, 0);
        //    }
        //}
    }
}