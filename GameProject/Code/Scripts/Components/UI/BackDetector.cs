using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.UI {
    public class BackDetector : Component {
        public BackDetector(GameObject attached) : base(attached) {
            _menu = (GameManager.CurrentScene as MenuScene);

            Input.OnEscape_Released += BackToPreviousMenu;
        }

        private MenuScene _menu;


        private void BackToPreviousMenu() {
            if (_menu._menuStack.Count == 1) {
                GameManager.ExitGame();
            } else {
                _menu._menuStack.Pop();
                _menu.SwitchMenu(_menu._menuStack.Peek(), true);
            }
        }


        public override void OnDestroy() {
            base.OnDestroy();

            Input.OnEscape_Released -= BackToPreviousMenu;
        }

    }
}