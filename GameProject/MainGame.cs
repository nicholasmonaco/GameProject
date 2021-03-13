// MainGame.cs - Nick Monaco

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scenes;

namespace GameProject {
    public class MainGame : Game {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //public static readonly int TargetFPS = 60; // This is already true due to the default Monogame settings

        private float _fixedUpdateMeasurer = 0;
        private const int _initialSceneID = 0; //0 is menu, 1 is game

        public Scene[] SceneList;



        public MainGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            //_graphics.PreferredBackBufferWidth = 480 * 2;
            //_graphics.PreferredBackBufferHeight = 320 * 2;
            _graphics.PreferredBackBufferWidth = (int)Camera.ConstantResolution.X * 2;
            _graphics.PreferredBackBufferHeight = (int)Camera.ConstantResolution.Y * 2;
            _graphics.ApplyChanges();


            Debug.Start();
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            GameManager.SetMainGame(this);
            GameManager.InitInternalValues();
            GameManager.SetLayerRules();

            int seed = new System.Random().Next();
            GameManager.WorldRandom = new System.Random(seed); //Move this later to work with seed
            GameManager.DeltaRandom = new System.Random();
            Debug.Log($"WorldRandom seed: {seed}");

            Window.Title = "Quake Break";

            Input.Start();
            Input.OnFullscreenToggle = () => {
                bool fullscreen = !_graphics.IsFullScreen;
                _graphics.IsFullScreen = fullscreen;
                int width = fullscreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : (int)Camera.ConstantResolution.X * 2;
                int height = fullscreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height : (int)Camera.ConstantResolution.Y * 2;
                _graphics.PreferredBackBufferWidth = width; //These two actually change the resolution of the window
                _graphics.PreferredBackBufferHeight = height;
                //GraphicsDevice.Viewport = new Viewport(0, 0, width - width % 480, height - height % 320);
                _graphics.ApplyChanges();
                GameManager.MainCamera.ResetResolution();
            };
            // End user initialization

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Resources.LoadContent(Content);
            // End user content loading

            AfterLoad();
        }

        private void AfterLoad() {
            SceneList = new Scene[2]; // Main Menu scene and Game scene
            SceneList[0] = new MenuScene();
            SceneList[1] = new GameScene();

            GameManager.SwitchScene(_initialSceneID);
        }

        protected override void Update(GameTime gameTime) {
            Time.time = (float)gameTime.TotalGameTime.Seconds;
            Time.SetDeltaTime((float)gameTime.ElapsedGameTime.TotalSeconds);

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            // Physics logic
            _fixedUpdateMeasurer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while(_fixedUpdateMeasurer > Time.fixedDeltaTime) {
                GameManager.CurrentScene.FixedUpdate();
                GameManager.CurrentScene.PhysicsUpdate();
                _fixedUpdateMeasurer -= Time.unscaledFixedDeltaTime;
            }
            // End Physics logic

            // Input logic
            Input.Update();
            // End input logic

            // TODO: Add your update logic here
            GameManager.CurrentScene.Update();
            GameManager.CurrentScene.LateUpdate();
            // End user update logic

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {            
            GraphicsDevice.Clear(Color.Black);

            // I think these transformations could somehow be implemented to when the variable viewmatrix is set, but as of now it doesn't work.
            Matrix viewMatrix = GameManager.MainCamera.ViewMatrix * 
                                Matrix.CreateScale(1, -1, 1) *
                                Matrix.CreateTranslation(GameManager.Resolution.X / 2, GameManager.Resolution.Y / 2, 0);

            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone, transformMatrix: viewMatrix);
            // TODO: Add your drawing code here
            GameManager.CurrentScene.Draw(_spriteBatch);
            // End user drawing
            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
