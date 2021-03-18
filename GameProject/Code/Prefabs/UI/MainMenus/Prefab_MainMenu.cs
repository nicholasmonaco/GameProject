using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.UI;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.UI;
using GameProject.Code.Scenes;

namespace GameProject.Code.Prefabs.UI.MainMenus {
    public class Prefab_MainMenu : GameObject {
        public Prefab_MainMenu() : base() {
            Name = "Main Menu";

            Color selected = new Color(17, 125, 240);
            Color deselected = new Color(108, 112, 117);

            MenuScene menu = (GameManager.CurrentScene as MenuScene);
            float startPoint = 105;
            float diff = 40;

            List<(string, Action)> buttons = new List<(string, Action)>() {
                ("Play", menu.StartGame),
                //("Continue", () => { }),
                ("Options", () => { menu.SwitchMenu(MenuState.Options, false); }),
                ("Credits", () => { menu.SwitchMenu(MenuState.Credits, false); }),
                //("Challenges", () => { }),
                ("Exit", GameManager.ExitGame)
            };


            foreach((string, Action) buttonData in buttons) {
                GameObject textButton = Instantiate(new Prefab_SelectableText(buttonData.Item1, deselected, selected));
                textButton.transform.Parent = transform;
                
                textButton.GetComponent<SelectableText>().OnActivate = buttonData.Item2;

                textButton.transform.LocalPosition = new Vector3(0, startPoint, 0);

                textButton.transform.Scale *= 0.3f; //debug

                startPoint -= diff;
            }

        }
    }
}
