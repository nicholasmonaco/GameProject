// GameScene.cs - Nick Monaco

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.UI;
using GameProject.Code.Prefabs;
using Microsoft.Xna.Framework;
using GameProject.Code.Scripts;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.UI;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Prefabs.UI;
using Microsoft.Xna.Framework.Audio;

namespace GameProject.Code.Scenes {
    
    /// <summary>
    /// The scene where the main game occurs.
    /// </summary>
    public class GameScene : Scene {

        private GameObject _minimap;
        private Panel _fadeToBlack;

        private Panel _deathPanel;
        private Panel _pausePanel;
        private SpriteRenderer _deathMsgRenderer;
        private SpriteRenderer _winMsgRenderer; //debug
        private SpriteRenderer _respawnPrompt;

        public static bool Paused { get; private set; } = false;


        public override void Init() {
            base.Init();

            Paused = false;

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
            _deathMsgRenderer.DrawLayer = DrawLayer.ID[DrawLayers.TotalOverlay];
            _deathMsgRenderer.OrderInLayer = 40;
            _deathMsgRenderer.Material.BatchID = BatchID.HUD;

            _winMsgRenderer = Instantiate(new GameObject()).AddComponent<SpriteRenderer>();
            _winMsgRenderer.transform.Parent = GameManager.MainCanvas.transform;
            _winMsgRenderer.Sprite = Resources.Sprite_WinMessage;
            _winMsgRenderer.Color = Color.Transparent;
            _winMsgRenderer.DrawLayer = DrawLayer.ID[DrawLayers.TotalOverlay];
            _winMsgRenderer.OrderInLayer = 40;
            _winMsgRenderer.Material.BatchID = BatchID.HUD;

            _respawnPrompt = Instantiate(new GameObject()).AddComponent<SpriteRenderer>();
            _respawnPrompt.transform.Parent = GameManager.MainCanvas.transform;
            _respawnPrompt.Sprite = Resources.Sprite_MM_Prompt;
            _respawnPrompt.Color = Color.Transparent;
            _respawnPrompt.DrawLayer = DrawLayer.ID[DrawLayers.TotalOverlay];
            _respawnPrompt.OrderInLayer = 39;
            _respawnPrompt.Material.BatchID = BatchID.HUD;

            GameObject controlGuide = Instantiate(new GameObject());
            controlGuide.transform.Position = Vector3.Zero;
            SpriteRenderer guideRend = controlGuide.AddComponent<SpriteRenderer>();
            guideRend.Sprite = Resources.Sprite_ControlGuide;
            guideRend.DrawLayer = DrawLayer.ID[DrawLayers.WorldStructs];
            guideRend.OrderInLayer = 500;
            guideRend.Material.BatchID = BatchID.Room;

            GameManager.BulletHolder = Instantiate(new GameObject()).transform;



            #region Creating pause menu
            _pausePanel = Instantiate(new Prefab_Panel()).GetComponent<Panel>();
            _pausePanel.SetColor(new Color(0, 0, 0, 0.5f));

            Color selected = new Color(17, 125, 240);

            float startPoint = 105;
            float diff = 30;

            GameManager.UILayoutMembers.Clear();

            List<(string, Action)> buttons = new List<(string, Action)>() {
                ("Resume", TogglePause),
                ("Restart", () => { ResetScene(); TogglePause(); }),
                ("Exit", () => { TogglePause(); StartCoroutine(BackToMainMenu()); }),
            };


            foreach ((string, Action) buttonData in buttons) {
                GameObject textButton = Instantiate(new Prefab_SelectableText(buttonData.Item1, Color.White, selected));
                textButton.transform.Parent = _pausePanel.transform;
                textButton.transform.Position = new Vector3(0, startPoint, 0);
                textButton.GetComponent<SelectableText>().OnActivate = buttonData.Item2;

                textButton.transform.Scale *= 0.2f; //debug

                startPoint -= diff;
            }

            GameManager.CurrentUIIndex = 0;

            _pausePanel.gameObject.Enabled = false;

            Input.OnSpace_Down += ActivateAction;
            #endregion


            Input.OnEscape_Down += TogglePause;




            Instantiate(new Prefab_Vignette());

            InventoryTracker inventory = GameManager.MainCanvas.gameObject.AddComponent<InventoryTracker>();


            #region Creating Main UI

            for (int i = 0; i < 3; i++) {
                TextRenderer counter = Instantiate(new GameObject()).AddComponent<TextRenderer>(GameFont.Base, "000");
                counter.transform.Parent = GameManager.MainCanvas.transform;
                counter.transform.LocalScale *= 0.75f;
                counter.transform.LocalPosition = new Vector3(0, 0, 0);
                counter.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
                counter.transform.LocalPosition = new Vector3(-GameManager.MainCanvas.Extents.X + 20,
                                                              GameManager.MainCanvas.Extents.Y - 45 - i * 15,
                                                              0);

                counter.Justification = Justify.Left;
                counter.Material.BatchID = BatchID.HUD;

                int dist = i;
                GameManager.MainCanvas.ExtentsUpdate += () => {
                    counter.transform.LocalPosition = new Vector3(-GameManager.MainCanvas.Extents.X + 20,
                                                                  GameManager.MainCanvas.Extents.Y - 45 - dist * 15,
                                                                  0);
                };

                SpriteRenderer iconRend = counter.gameObject.AddComponent<SpriteRenderer>();
                iconRend.SpriteOffset = new Vector2(-7, 0.5f);
                iconRend.SpriteScale = new Vector2(4 / 3f);
                iconRend.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
                iconRend.Material.BatchID = BatchID.HUD;

                switch (i) {
                    case 0:
                        inventory.MoneyRenderer = counter;
                        iconRend.Sprite = Resources.Sprite_UI_Money;
                        break;
                    case 1:
                        inventory.KeyRenderer = counter;
                        iconRend.Sprite = Resources.Sprite_UI_Keys;
                        break;
                    case 2:
                        inventory.BombRenderer = counter;
                        iconRend.Sprite = Resources.Sprite_UI_Bombs;
                        break;
                }
                
            }


            PlayerStats.Money = 0;
            PlayerStats.Keys = 0;
            PlayerStats.Bombs = 0;

            #endregion


            #region Creating Popup UI

            Instantiate(new Prefab_ItemPickupUI());

            #endregion


            GameObject levelMapGO = Instantiate(new GameObject());
            MapManager map = levelMapGO.AddComponent<MapManager>();

            StartCoroutine(StartLevel());
        }

        private IEnumerator StartLevel() {
            while (GameManager.Map.Generated == false) {
                yield return StartCoroutine(GameManager.Map.GenerateLevel(LevelID.QuarantineLevel)); //replace with levelID variable later
            }

            _pausePanel.gameObject.Enabled = false;

            _updating = true;

            // Reset inventory
            PlayerStats.ResetInventory();

            // Start music 
            GameManager.SetFloorSong(LevelID.QuarantineLevel); //use logic for this later

            // Create minimap
            _minimap = new Prefab_Minimap();
            Instantiate(_minimap);

            // Create player health bar
            Instantiate(new Prefab_PlayerHealthBar());
            yield return null;
            PlayerStats.SetHealth(6, 0); //set with character stats later

            // Spawn player
            Instantiate(new Prefab_Player());

            // Create reticle
            Instantiate(new Prefab_Reticle());

            // Start the game
            yield return new WaitForSeconds(0.15f);
            yield return StartCoroutine(Panel.FadeFromBlack(_fadeToBlack, 3.5f));
        }


        private IEnumerator BackToMainMenu() {
            yield return StartCoroutine(Panel.FadeIntoBlack(_fadeToBlack, 3f));

            GameManager.StopFloorSong();
            GameManager.DeactivateRoomSong();

            GameManager.UILayoutMembers.Clear();
            GameManager.CurrentUIIndex = 0;
            GameManager.OnSelectIndexChange = (value) => { };

            yield return new WaitForSeconds(0.35f);

            // Switch to Menu Scene
            GameManager.SwitchScene(0);
        }

        public override void ResetScene() {
            StartCoroutine(ResetLevel());
        }

        private IEnumerator ResetLevel() {
            yield return new WaitForEndOfFrame();

            GameManager.LevelResetting = true;

            ResetShaders();

            _updating = true; // This could probably be moved further down

            _pausePanel.gameObject.Enabled = false;

            // Reset inventory
            PlayerStats.ResetInventory();

            // Reset music
            GameManager.SetFloorSong(LevelID.QuarantineLevel); //use logic for this later
            GameManager.DeactivateRoomSong();
            

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
            PlayerStats.Money = 0;
            PlayerStats.Keys = 0;
            PlayerStats.Bombs = 0;

            // Spawn player
            Instantiate(new Prefab_Player());

            Camera.main.transform.Position = GameManager.Map.RoomGrid[GameManager.Map.GridPos_StartingRoom].transform.Position;

            GameManager.Map.LoadRoom(GameManager.Map.GridPos_StartingRoom);
            //GameManager.Map.CurrentGridPos = GameManager.Map.GridPos_StartingRoom;


            yield return StartCoroutine(Panel.FadeFromBlack(_fadeToBlack, 3.5f));

            GameManager.LevelResetting = false;
        }


        public override void UnloadContent() {
            base.UnloadContent();

            Input.OnSpace_Down -= ActivateAction;
            Input.OnEscape_Down -= TogglePause;
        }



        public void Die() {
            StartCoroutine(Die_C());
        }

        private IEnumerator Die_C() {
            // stop gametime
            _updating = false;

            //play player death animation

            // Stop music
            GameManager.StopFloorSong();
            GameManager.DeactivateRoomSong();

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
            Input.OnSpace_Down += Respawn;
        }


        public void Win() {
            StartCoroutine(Win_C());
        }

        private IEnumerator Win_C() {
            // stop gametime
            _updating = false;

            //play player death animation

            // Stop music
            GameManager.StopFloorSong();
            GameManager.DeactivateRoomSong();

            //play death sound
            //Resources.Sound_Death.Play(0.2f, 0, 0);

            // fade in gray panel
            _deathPanel.SetColor(Color.Black);
            yield return StartCoroutine(Panel.FadeIntoBlack(_deathPanel, 4.5f));


            // once faded in, fade in text saying "you died"
            _winMsgRenderer.transform.LocalPosition = Vector3.Zero;

            float dur_max = 2;
            float dur = dur_max;
            while (dur > 0) {
                yield return null;
                dur -= Time.deltaTime;

                _winMsgRenderer.Color = Color.Lerp(Color.White, Color.Transparent, dur / dur_max);
            }

            yield return new WaitForEndOfFrame();

            _winMsgRenderer.Color = Color.White;

            _respawnPrompt.transform.LocalPosition = new Vector3(0, -35f, 0);

            dur_max = 3f;
            dur = dur_max;
            Vector3 origLocal = _winMsgRenderer.transform.LocalPosition;
            Vector3 newLocal = origLocal + new Vector3(0, 50, 0);
            while (dur > 0) {
                yield return null;
                dur -= Time.deltaTime;

                _winMsgRenderer.transform.LocalPosition = Vector3.SmoothStep(newLocal, origLocal, dur / dur_max);
            }

            yield return new WaitForEndOfFrame();
            _winMsgRenderer.transform.LocalPosition = newLocal;



            dur_max = 2f;
            dur = dur_max;
            while (dur > 0) {
                yield return null;
                dur -= Time.deltaTime;

                _respawnPrompt.Color = Color.Lerp(Color.White, Color.Transparent, dur / dur_max);
            }

            _respawnPrompt.Color = Color.White;


            yield return new WaitForSeconds(0.7f);

            Input.OnSpace_Down += Respawn;
        }


        private void Respawn() {
            //later, make this fade out instead
            _respawnPrompt.Color = Color.Transparent;
            _deathMsgRenderer.Color = Color.Transparent;
            _winMsgRenderer.Color = Color.Transparent;
            _deathPanel.SetOpacity(0);

            _fadeToBlack.SetOpacity(1);


            ResetScene();
            Input.OnSpace_Down -= Respawn;
        }

        private void TogglePause() {
            Paused = !Paused;
            Time.TimeScale = Paused ? 0 : 1;

            GameManager.Player.FreezeMovement = Paused;

            _pausePanel.gameObject.Enabled = Paused;


            GameManager.UILayoutMembers.Clear();
            GameManager.OnSelectIndexChange = (index) => { };

            if (Paused) {
                foreach (UI_LayoutItem item in _pausePanel.gameObject.GetAllComponents<UI_LayoutItem>()) {
                    item.ForceAddToUIList();
                }
            } else {
                foreach (UI_LayoutItem item in _pausePanel.gameObject.GetAllComponents<UI_LayoutItem>()) {
                    item.ForceRemoveFromUIList();
                }
            }

            GameManager.CurrentUIIndex = 0;
        }

        private void SetPaused(bool paused) {
            Paused = !paused;
            TogglePause();
        }

    }
}
