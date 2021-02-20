﻿// GameScene.cs - Nick Monaco

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
using GameProject.Code.Scripts;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.UI;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Components.Entity;
using Microsoft.Xna.Framework.Media;

namespace GameProject.Code.Scenes {
    
    /// <summary>
    /// The scene where the main game occurs.
    /// </summary>
    public class GameScene : Scene {

        private GameObject _minimap;
        private Panel _fadeToBlack;

        private Panel _deathPanel;
        private SpriteRenderer _deathMsgRenderer;
        private SpriteRenderer _respawnPrompt;


        public override void Init() {
            base.Init();

            // This is essentially where there should be a list of default GameObjects in the scene.
            GameObjects.Add(new Prefab_MainCamera());
            Instantiate(new Prefab_Canvas());

            _fadeToBlack = Instantiate(new Prefab_Panel()).GetComponent<Panel>();
            _fadeToBlack.SetColor(Color.Black);

            _deathPanel = Instantiate(new Prefab_Panel()).GetComponent<Panel>();
            _deathPanel.SetColor(Color.Transparent);

            _deathMsgRenderer = Instantiate(new GameObject()).AddComponent<SpriteRenderer>();
            _deathMsgRenderer.transform.Parent = GameManager.MainCanvas.transform;
            _deathMsgRenderer.Sprite = Resources.Sprite_DeathMessage;
            _deathMsgRenderer.Color = Color.Transparent;
            _deathMsgRenderer.DrawLayer = DrawLayer.ID["TotalOverlay"];
            _deathMsgRenderer.OrderInLayer = 40;

            _respawnPrompt = Instantiate(new GameObject()).AddComponent<SpriteRenderer>();
            _respawnPrompt.transform.Parent = GameManager.MainCanvas.transform;
            _respawnPrompt.Sprite = Resources.Sprite_MM_Prompt;
            _respawnPrompt.Color = Color.Transparent;
            _respawnPrompt.DrawLayer = DrawLayer.ID["TotalOverlay"];
            _respawnPrompt.OrderInLayer = 39;

            Instantiate(new Prefab_Vignette());


            GameObject levelMapGO = Instantiate(new GameObject());
            MapManager map = levelMapGO.AddComponent<MapManager>();

            StartCoroutine(StartLevel());
        }

        private IEnumerator StartLevel() {
            while (GameManager.Map.Generated == false) {
                yield return StartCoroutine(GameManager.Map.GenerateLevel(LevelID.QuarantineLevel)); //replace with levelID variable later
            }

            _updating = true;

            // Start music
            MediaPlayer.Play(Resources.Music_QuarantineLevel);
            MediaPlayer.IsRepeating = true;

            // Create minimap
            _minimap = new Prefab_Minimap();
            Instantiate(_minimap);

            // Create player health bar
            Instantiate(new Prefab_PlayerHealthBar());
            yield return null;
            PlayerStats.SetHealth(6, 0); //set with character stats later

            // Spawn player
            //GameObjects.Add(new Prefab_Player());
            Instantiate(new Prefab_Player());

            // Create reticle
            //GameObjects.Add(new Prefab_Reticle());
            Instantiate(new Prefab_Reticle());


            //debug
            Instantiate(new Prefab_PickupGeneric(Pickup.Coin));


            yield return new WaitForSeconds(0.15f);
            yield return StartCoroutine(Panel.FadeFromBlack(_fadeToBlack, 3.5f));
        }


        public override void ResetScene() {
            StartCoroutine(ResetLevel());
        }

        private IEnumerator ResetLevel() {
            yield return new WaitForEndOfFrame();

            _updating = true; // This could probably be moved further down

            // Reset music
            MediaPlayer.Play(Resources.Music_QuarantineLevel);
            MediaPlayer.IsRepeating = true;

            GameObject.Destroy(_minimap);
            GameObject.Destroy(GameManager.Player.gameObject);

            _fadeToBlack.SetOpacity(1);

            GameManager.Map.Generated = false;
            while (GameManager.Map.Generated == false) {
                yield return StartCoroutine(GameManager.Map.GenerateLevel(LevelID.QuarantineLevel)); //replace with levelID variable later
            }

            // Create minimap
            _minimap = new Prefab_Minimap();
            Instantiate(_minimap);

            PlayerStats.SetHealth(6, 0); //set with character stats later

            // Spawn player
            Instantiate(new Prefab_Player());

            Camera.main.transform.Position = GameManager.Map.RoomGrid[GameManager.Map.GridPos_StartingRoom].transform.Position;

            GameManager.Map.LoadRoom(GameManager.Map.GridPos_StartingRoom);
            //GameManager.Map.CurrentGridPos = GameManager.Map.GridPos_StartingRoom;


            yield return StartCoroutine(Panel.FadeFromBlack(_fadeToBlack, 3.5f));
        }



        public void Die() {
            StartCoroutine(Die_C());
        }

        private IEnumerator Die_C() {
            // stop gametime
            _updating = false;

            //play player death animation

            // Stop music
            MediaPlayer.Stop();

            //play death sound
            Resources.Sound_Death.Play(0.2f, 0, 0);

            // fade in gray panel
            _deathPanel.SetColor(Color.Black);
            yield return StartCoroutine(Panel.FadeIntoBlack(_deathPanel, 4.5f));


            // once faded in, fade in text saying "you died"
            _deathMsgRenderer.transform.LocalPosition = Vector3.Zero;

            float dur_max = 2;
            float dur = dur_max;
            while(dur > 0) {
                yield return null;
                dur -= Time.deltaTime;

                _deathMsgRenderer.Color = Color.Lerp(Color.White, Color.Transparent, dur / dur_max);
            }

            yield return new WaitForEndOfFrame();

            _deathMsgRenderer.Color = Color.White;

            _respawnPrompt.transform.LocalPosition = new Vector3(0, -35f, 0);

            dur_max = 3f;
            dur = dur_max;
            Vector3 origLocal = _deathMsgRenderer.transform.LocalPosition;
            Vector3 newLocal = origLocal + new Vector3(0, 50, 0);
            while (dur > 0) {
                yield return null;
                dur -= Time.deltaTime;

                _deathMsgRenderer.transform.LocalPosition = Vector3.SmoothStep(newLocal, origLocal, dur/dur_max);
            }

            yield return new WaitForEndOfFrame();
            _deathMsgRenderer.transform.LocalPosition = newLocal;
            


            dur_max = 2f;
            dur = dur_max;
            while (dur > 0) {
                yield return null;
                dur -= Time.deltaTime;

                _respawnPrompt.Color = Color.Lerp(Color.White, Color.Transparent, dur / dur_max);
            }

            _respawnPrompt.Color = Color.White;


            yield return new WaitForSeconds(0.7f);

            // add level restart to shoot like in menuscene
            Input.OnShoot_Down += Respawn;
        }


        private void Respawn() {
            //later, make this fade out instead
            _respawnPrompt.Color = Color.Transparent;
            _deathMsgRenderer.Color = Color.Transparent;
            _deathPanel.SetOpacity(0);

            _fadeToBlack.SetOpacity(1);


            ResetScene();
            Input.OnShoot_Down -= Respawn;
        }

    }
}