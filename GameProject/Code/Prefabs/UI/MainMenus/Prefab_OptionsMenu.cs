using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.UI;
using GameProject.Code.Scenes;

namespace GameProject.Code.Prefabs.UI.MainMenus {
    public class Prefab_OptionsMenu : GameObject {
        public Prefab_OptionsMenu() : base() {
            Color selected = new Color(17, 125, 240);
            Color deselected = new Color(108, 112, 117);

            MenuScene menu = (GameManager.CurrentScene as MenuScene);

            float init = 120;
            float diff = 40;


            GameObject creditText = Instantiate<GameObject>(Vector3.Zero, transform);
            TextRenderer textRend = creditText.AddComponent<TextRenderer>();
            textRend.SetFont(GameFont.Debug); //make styled later
            textRend.Text = "Options";
            textRend.Color = new Color(108, 112, 117);
            textRend.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
            textRend.OrderInLayer = 42;

            creditText.transform.LocalPosition = new Vector3(0, init, 0);

            creditText.transform.Scale *= 0.3f; //debug

            init -= diff + 25;




            List<(string, Action)> buttons = new List<(string, Action)>() {
                ("Fullscreen", Input.OnFullscreenToggle),
                //("Keyboard Controls", () => { menu.SwitchMenu(MenuState.Options, false); }),
            };

            List<(string, Action<float>)> sliders = new List<(string, Action<float>)>(3) {
                ("Master Volume", (value) => { GameManager.MasterVolume = value; }),
                ("Music Volume", (value) => { GameManager.MusicVolume = value; }),
                ("SFX Volume", (value) => { GameManager.SoundVolume = value; })
            };



            foreach ((string, Action) buttonData in buttons) {
                GameObject textButton = Instantiate(new Prefab_SelectableText(buttonData.Item1, deselected, selected));
                textButton.transform.Parent = transform;
                textButton.transform.LocalPosition = new Vector3(0, init, 0);
                textButton.GetComponent<SelectableText>().OnActivate = buttonData.Item2;

                textButton.transform.Scale *= 0.3f; //debug

                init -= diff;
            }


            const float sliderPos = 80;

            foreach ((string, Action<float>) sliderData in sliders) {
                GameObject sliderText = Instantiate<GameObject>(Vector3.Zero, transform);
                TextRenderer labelRend = sliderText.AddComponent<TextRenderer>();
                labelRend.SetFont(GameFont.Debug); //make styled later
                //labelRend.Justification = Justify.Right;
                labelRend.Text = sliderData.Item1;
                labelRend.Color = deselected;
                labelRend.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
                labelRend.OrderInLayer = 42;
                
                sliderText.transform.LocalPosition = new Vector3(-sliderPos, init, 0);
                sliderText.transform.Scale *= 0.2f; //debug

                ValueSlider slider = Instantiate<Prefab_Slider>(Vector3.Zero, transform).GetComponent<ValueSlider>();
                slider.SetOnValueChangedAction(sliderData.Item2);
                slider.transform.LocalPosition = new Vector3(sliderPos, init, 0);
                slider.ExtraSelectAction += () => { labelRend.Color = selected; };
                slider.ExtraDeselectAction += () => { labelRend.Color = deselected; };

                init -= diff;
            }
        }
    }
}
