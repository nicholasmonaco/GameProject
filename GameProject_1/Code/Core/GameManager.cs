using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core {
    public static class GameManager {

        private static MainGame _mainGame;
        private static int _curSceneID;

        public static Camera MainCamera;


        public static bool MainGameAssigned => _mainGame != null;

        public static Scene CurrentScene => _mainGame.SceneList[_curSceneID];

        public static Viewport Viewport => _mainGame.GraphicsDevice.Viewport;
        public static Point Resolution { get { return _mainGame.GraphicsDevice.Viewport.Bounds.Size; } }

        // Camera stuff
        public static Matrix ProjectionMatrix => MainCamera.ProjectionMatrix;
        public static Matrix ViewMatrix => MainCamera.ViewMatrix;

        public static Vector3 ViewOffset => new Vector3(Resolution.X / 2, Resolution.Y / 2, 0);




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
