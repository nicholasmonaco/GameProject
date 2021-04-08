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
using GameProject.Code.Core.UI;
using GameProject.Code.Prefabs;
using GameProject.Code.Prefabs.UI;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.UI;
using GameProject.Code.Prefabs.UI.MainMenus;

namespace GameProject.Code.Scenes {
    
    /// <summary>
    /// The scene of the main menu.
    /// </summary>
    public class MenuScene : Scene {

        private Panel _fadeToBlack;

        private bool _changingMenus = false;
        private float _menuTransitionTime = 1;

        private Dictionary<MenuState, GameObject> _menus;
        private MenuState _lastMenuState = MenuState.Title;
        private MenuState _curMenuState = MenuState.Title;
        private GameObject _curMenu => _menus[_curMenuState];
        private GameObject _lastMenu => _menus[_lastMenuState];
        public Stack<MenuState> _menuStack;


        public override void Init() {
            base.Init();

            // This is essentially where there should be a list of default GameObjects in the scene.
            GameObjects.Add(new Prefab_MainCamera());
            GameObject canv = Instantiate(new Prefab_Canvas());
            canv.transform.LocalPosition = Vector3.Zero;

            //Instantiate(new Prefab_Reticle());

            GameManager.UILayoutMembers.Clear();
            GameManager.OnSelectIndexChange = (value) => { };

            _lastMenuState = MenuState.Title;
            _curMenuState = MenuState.Title;

            _menus = new Dictionary<MenuState, GameObject>(6);
            _menuStack = new Stack<MenuState>();
            _menuStack.Push(MenuState.Title);

            GameObject backDetector = Instantiate(new GameObject());
            backDetector.Name = "Back Detector";
            backDetector.AddComponent<BackDetector>();

            GameObject titleMenu = Instantiate(new Prefab_TitleMenu());
            _menus.Add(MenuState.Title, titleMenu);

            GameObject mainMenu = Instantiate(new Prefab_MainMenu());
            mainMenu.transform.Position = new Vector3(0, -500, 0);
            _menus.Add(MenuState.Main, mainMenu);

            GameObject optionsMenu = Instantiate(new Prefab_OptionsMenu());
            optionsMenu.transform.Position = new Vector3(-800, -500, 0);
            _menus.Add(MenuState.Options, optionsMenu);

            GameObject creditsMenu = Instantiate(new Prefab_CreditsMenu());
            creditsMenu.transform.Position = new Vector3(-800, -1000, 0);
            _menus.Add(MenuState.Credits, creditsMenu);

            GameManager.CurrentUIIndex = 0;


            GameObject background = Instantiate(new GameObject()); //we need to figure out how we want to do this
            background.Name = "Background Image";
            SpriteRenderer backRend = background.AddComponent<SpriteRenderer>();
            backRend.transform.Position = Vector3.Zero;
            backRend.transform.Scale *= 0.4f;
            backRend.Sprite = Resources.Sprite_MM_Background;
            backRend.DrawLayer = DrawLayer.ID[DrawLayers.Background];
            backRend.OrderInLayer = 10;

            GameObject stars = Instantiate(new GameObject());

            ParticleSystem starSystem = stars.AddComponent<ParticleSystem>();
            starSystem.Material.BatchID = backRend.Material.BatchID;
            starSystem.DrawLayer = backRend.DrawLayer;
            starSystem.OrderInLayer = backRend.OrderInLayer + 1;
            starSystem.Sprite = Resources.Sprite_ParticleDense;

            starSystem.Main.StartRotation = new ValueCurve_Vector3(Vector3.Zero, new Vector3(0, 0, MathHelper.TwoPi), InterpolationBehaviour.Lerp);
            starSystem.Main.StartSpeed = new ValueCurve_Vector3(Vector3.Zero);
            starSystem.Main.StartLifetime = new ValueCurve_Float(11, 16);
            starSystem.Shape.ShapeType = Core.Particles.ShapeType.Rectangle;
            starSystem.Shape.Width = 465;
            starSystem.Shape.Height = 350;
            starSystem.Main.StartSize = new ValueCurve_Vector3(new Vector3(0.005f), new Vector3(0.01f));
            starSystem.Main.MaxParticles = 10000;
            starSystem.EmissionModule.RateOverTime = 275;

            starSystem.ColorOverLifetimeModule.Enabled = true;

            Color halfWhite = new Color(1f, 1f, 1f, 0f);
            starSystem.ColorOverLifetimeModule.Gradient = new List<(float, Color)>() {
                (0, Color.Transparent),
                (0.15f, halfWhite),
                (0.5f, Color.Transparent),
                (0.85f, halfWhite),
                (1, Color.Transparent)
            };

            starSystem.Main.Prewarm = true;



            GameObject backgroundGradient = Instantiate(new GameObject());
            backgroundGradient.Name = "Background Gradient";
            SpriteRenderer gradientRend = backgroundGradient.AddComponent<SpriteRenderer>();
            gradientRend.transform.Position = new Vector3(0, -700, 0);
            gradientRend.transform.Scale = new Vector3(3000, 0.5f, 1);
            gradientRend.Sprite = Resources.Sprite_MM_BackgroundGradient;
            gradientRend.DrawLayer = DrawLayer.ID[DrawLayers.Background];
            gradientRend.OrderInLayer = 11;


            // Options testing
            //GameObject slider = Instantiate(new Prefab_Slider());
            //slider.transform.Position = new Vector3(0, 0, 0);
            //


            StartCoroutine(LoadMainMenu());
        }


        private IEnumerator LoadMainMenu() {
            _fadeToBlack = Instantiate(new Prefab_Panel()).GetComponent<Panel>();
            _fadeToBlack.SetColor(Color.Black);

            yield return new WaitForSeconds(0.4f);

            yield return StartCoroutine(Panel.FadeFromBlack(_fadeToBlack, 5.5f));

            foreach (UI_LayoutItem item in _curMenu.GetAllComponents<UI_LayoutItem>()) {
                item.ForceAddToUIList();
            }

            Input.OnSpace_Down += ActivateAction;
        }


        public override void UnloadContent() {
            base.UnloadContent();

            Debug.Log("starting to unload menu");

            Input.OnSpace_Down -= ActivateAction;


            foreach (UI_LayoutItem item in _lastMenu.GetAllComponents<UI_LayoutItem>()) {
                item.ForceRemoveFromUIList();
            }

            foreach (UI_LayoutItem item in _curMenu.GetAllComponents<UI_LayoutItem>()) {
                item.ForceRemoveFromUIList();
            }

            GameManager.UILayoutMembers.Clear();
            GameManager.OnSelectIndexChange = (value) => { };
            GameManager.CurrentUIIndex = 0;

            _menus.Clear();
            _menus = null;

            _menuStack.Clear();
            _menuStack = null;

            _fadeToBlack = null;

            Debug.Log("Finished unloading menu");
        }


        private IEnumerator StartGame_C() {            
            yield return StartCoroutine(Panel.FadeIntoBlack(_fadeToBlack, 3f));

            yield return new WaitForSeconds(0.35f);

            // Switch to Game Scene
            GameManager.SwitchScene(1); 
        }


        public void StartGame() {
            StartCoroutine(StartGame_C());
        }


        


        public void SwitchMenu(MenuState newMenu, bool goingBack) {
            if (_changingMenus) return;
            _changingMenus = true;

            _lastMenuState = _curMenuState;
            _curMenuState = newMenu;
            
            if (!goingBack) {
                _menuStack.Push(newMenu);
                Resources.Sound_Menu_Back.Play(GameManager.RealSoundVolume);
            } else {
                Resources.Sound_Menu_Next.Play(GameManager.RealSoundVolume);
            }

            foreach(UI_LayoutItem item in _lastMenu.GetAllComponents<UI_LayoutItem>()) {
                item.ForceRemoveFromUIList();
            }

            GameManager.UILayoutMembers.Clear();
            GameManager.OnSelectIndexChange = (index) => { };            

            foreach (UI_LayoutItem item in _curMenu.GetAllComponents<UI_LayoutItem>()) {
                item.ForceAddToUIList();
            }

            GameManager.CurrentUIIndex = 0;

            StartCoroutine(SwitchMenus_C());
        }


        private IEnumerator SwitchMenus_C() {
            float timer = _menuTransitionTime;

            Vector3 origPos = Camera.main.transform.Position;
            Vector3 newPos = _menus[_curMenuState].transform.Position;
            _menus[_curMenuState].Enabled = true;

            while(timer > 0) {
                timer -= Time.unscaledDeltaTime;
                Camera.main.transform.Position = Vector3.SmoothStep(newPos, origPos, timer / _menuTransitionTime);
                yield return null;
            }

            Camera.main.transform.Position = newPos;
            yield return new WaitForEndOfFrame();

            _menus[_lastMenuState].Enabled = false;

            _changingMenus = false;
        }

    }

    public enum MenuState {
        Title,
        Main,
        CharSelect,
        Options,
        Credits,
        Challenges
    }
}
