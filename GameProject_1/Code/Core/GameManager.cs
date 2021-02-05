// GameManager.cs - Nick Monaco

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Holds values and methods commonly needed across the game.
    /// </summary>
    public static class GameManager {

        // Overarching game stuff
        private static MainGame _mainGame;
        private static int _curSceneID;
        public static Scene CurrentScene => _mainGame.SceneList[_curSceneID];

        // Camera stuff
        public static Viewport Viewport => _mainGame.GraphicsDevice.Viewport;
        public static Point Resolution { get { return _mainGame.Window.ClientBounds.Size; } }
        public static Matrix ProjectionMatrix => MainCamera.ProjectionMatrix;
        public static Matrix ViewMatrix => MainCamera.ViewMatrix;
        public static Vector3 ViewOffset => new Vector3(Resolution.X / 2, Resolution.Y / 2, 0) / (MainCamera.Size * 2);

        // Game world stuff
        public static Camera MainCamera;
        public static Random WorldRandom;
        public static PlayerController Player;
        public static Transform PlayerTransform => Player.transform;
        public static MapManager Map;
        




        public static void SetMainGame(MainGame game) {
            _mainGame = game;
        }

        public static void SwitchScene(int newSceneID) {
            if (_curSceneID == newSceneID) return;

            if (CurrentScene != null) {
                Scene.UnloadScene(CurrentScene);
            }

            _curSceneID = newSceneID;

            Scene.LoadScene(CurrentScene, _mainGame.Content);
        }
    }
}
