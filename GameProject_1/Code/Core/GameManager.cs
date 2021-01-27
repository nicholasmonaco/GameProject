using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    public static class GameManager {

        private static MainGame _mainGame;
        private static int _curSceneID;

        public static Camera MainCamera;



        public static Scene CurrentScene => _mainGame.SceneList[_curSceneID];

        public static Point Resolution { get { return _mainGame.GraphicsDevice.Viewport.Bounds.Size; } }




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
