using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.UI;
using GameProject.Code.Scripts.Components.UI;

namespace GameProject.Code.Prefabs.UI {
    public class Prefab_Slider : GameObject {
        public Prefab_Slider() : base() {
            Name = "Slider";

            GameObject handle = Instantiate<GameObject>(transform.Position, transform);
            handle.Name = "Slider Handle";
            handle.transform.Parent = transform;
            SpriteRenderer handleRenderer = handle.AddComponent<SpriteRenderer>();
            handleRenderer.Sprite = Resources.Sprite_Pixel;
            handleRenderer.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
            handleRenderer.OrderInLayer = 41;

            SpriteRenderer backgroundRenderer = AddComponent<SpriteRenderer>();
            backgroundRenderer.Sprite = Resources.Sprite_Pixel;
            backgroundRenderer.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
            backgroundRenderer.OrderInLayer = 40;

            //debug
            handleRenderer.SpriteScale = new Vector2(18, 18);
            backgroundRenderer.SpriteScale = new Vector2(120, 5);
            //


            ValueSlider slider = AddComponent<ValueSlider>(0.3f);
            slider.ForceSetHandle(handle.transform);
            slider.SetOnValueChangedAction((value) => { GameManager.MasterVolume = value; });
            slider.BackgroundRenderer = backgroundRenderer;
            slider.HandleRenderer = handleRenderer;
        }
    }
}
