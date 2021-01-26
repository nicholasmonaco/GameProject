﻿// MainGame.cs - Nick Monaco

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Code.Core;
using GameProject.Code.Scenes;

namespace GameProject {
    public class MainGame : Game {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //public static readonly int TargetFPS = 60; // This is already true due to the default Monogame settings

        private float _fixedUpdateMeasurer = 0;
        private const int _initialSceneID = 0;

        public Scene[] SceneList;



        public MainGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            SceneList = new Scene[2]; // Main Menu scene and Game scene
            SceneList[0] = new MenuScene();
            SceneList[1] = new GameScene();

            GameManager.SetMainGame(this);

            GameManager.SwitchScene(_initialSceneID);

            Input.Start();
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here

            // End user initialization

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // End user content loading
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Physics logic
            _fixedUpdateMeasurer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while(_fixedUpdateMeasurer > Time.fixedDeltaTime) {
                GameManager.CurrentScene.FixedUpdate();
                GameManager.CurrentScene.PhysicsUpdate();
                _fixedUpdateMeasurer -= Time.fixedDeltaTime;
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // TODO: Add your drawing code here
            GameManager.CurrentScene.Draw(_spriteBatch);
            // End user drawing
            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
