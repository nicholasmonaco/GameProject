using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core {
    public static class GameManager {

        private static MainGame _mainGame;
        private static int _curSceneID;


        public static Scene CurrentScene => _mainGame.SceneList[_curSceneID];





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
