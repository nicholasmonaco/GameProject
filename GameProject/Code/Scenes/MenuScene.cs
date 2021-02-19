// MenuScene.cs - Nick Monaco

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Prefabs;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.UI;

namespace GameProject.Code.Scenes {
    
    /// <summary>
    /// The scene of the main menu.
    /// </summary>
    public class MenuScene : Scene {

        private Panel _fadeToBlack;


        public override void Init() {
            base.Init();

            // This is essentially where there should be a list of default GameObjects in the scene.
            GameObjects.Add(new Prefab_MainCamera());
            GameObject canv = Instantiate(new Prefab_Canvas());
            canv.transform.LocalPosition = Vector3.Zero;

            //Instantiate(new Prefab_Reticle());



            GameObject background = Instantiate(new GameObject());
            background.Name = "Background Image";
            SpriteRenderer backRend = background.AddComponent<SpriteRenderer>();
            backRend.transform.Position = Vector3.Zero;
            backRend.transform.Scale *= 0.4f;
            backRend.Sprite = Resources.Sprite_MM_Background;

            GameObject ground = Instantiate(new GameObject());
            ground.Name = "Ground Image";
            SpriteRenderer groundRend = ground.AddComponent<SpriteRenderer>();
            groundRend.Sprite = Resources.Sprite_MM_Ground;
            groundRend.transform.Scale *= 0.26f;
            ground.transform.Parent = GameManager.MainCanvas.transform;
            groundRend.transform.LocalPosition += new Vector3(0, -30, 0);
            groundRend.Color = Color.Transparent; //DEBUG


            GameObject title = Instantiate(new GameObject());
            title.Name = "Title";
            SpriteRenderer titleRend = title.AddComponent<SpriteRenderer>();
            titleRend.Sprite = Resources.Sprite_MM_Title;
            titleRend.transform.Scale *= 1.75f;
            title.transform.Parent = GameManager.MainCanvas.transform;
            titleRend.transform.LocalPosition += new Vector3(0, 71, 0);

            UI_Bounce bounce = title.AddComponent<UI_Bounce>();
            bounce.InitBounce(0.7f, new Vector3(0, -3, 0));


            GameObject prompt = Instantiate(new GameObject());
            prompt.Name = "Start Prompt";
            SpriteRenderer promptRend = prompt.AddComponent<SpriteRenderer>();
            promptRend.Sprite = Resources.Sprite_MM_Prompt;
            promptRend.Color = new Color(181, 181, 181);
            prompt.transform.Parent = GameManager.MainCanvas.transform;
            promptRend.transform.LocalPosition += new Vector3(-5, -55, 0);

            UI_Wobble wobble = prompt.AddComponent<UI_Wobble>();
            wobble.Init(2.5f, 2);


            StartCoroutine(LoadMainMenu());
        }


        private IEnumerator LoadMainMenu() {
            _fadeToBlack = Instantiate(new Prefab_Panel()).GetComponent<Panel>();
            _fadeToBlack.SetColor(Color.Black);

            yield return new WaitForSeconds(0.4f);

            yield return StartCoroutine(Panel.FadeFromBlack(_fadeToBlack, 5.5f));

            Input.OnShoot_Down += StartGame;
        }

        private IEnumerator StartGame_C() {
            yield return StartCoroutine(Panel.FadeIntoBlack(_fadeToBlack, 3f));

            yield return new WaitForSeconds(0.35f);

            // Switch to Game Scene
            GameManager.SwitchScene(1); 
        }


        private void StartGame() {
            Input.OnShoot_Down -= StartGame;
            StartCoroutine(StartGame_C());
        }
        

    }
}
