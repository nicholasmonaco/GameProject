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
    public class Prefab_TitleMenu : GameObject {
        public Prefab_TitleMenu() : base() {


            GameObject ground = Instantiate(new GameObject());
            ground.Name = "Ground Image";
            SpriteRenderer groundRend = ground.AddComponent<SpriteRenderer>();
            groundRend.Sprite = Resources.Sprite_MM_Ground;
            //groundRend.transform.Scale *= 0.26f;
            Vector3 scl = groundRend.transform.Scale * 0.3f;
            scl.Y *= 2;
            scl.X *= 0.85f;
            groundRend.transform.Scale = scl;
            groundRend.transform.LocalPosition += new Vector3(0, 60, 0);
            groundRend.DrawLayer = DrawLayer.ID[DrawLayers.Background];
            groundRend.OrderInLayer = 11;
            //groundRend.Color = Color.Transparent; //DEBUG


            GameObject title = Instantiate(new GameObject());
            title.Name = "Title";
            SpriteRenderer titleRend = title.AddComponent<SpriteRenderer>();
            titleRend.Sprite = Resources.Sprite_MM_Title;
            titleRend.transform.Scale *= 1.75f;
            //title.transform.Parent = GameManager.MainCanvas.transform;
            titleRend.transform.LocalPosition += new Vector3(0, 71, 0);
            titleRend.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
            titleRend.OrderInLayer = 5;

            UI_Bounce bounce = title.AddComponent<UI_Bounce>();
            bounce.InitBounce(0.7f, new Vector3(0, -3, 0));


            GameObject prompt = Instantiate(new GameObject());
            prompt.Name = "Start Prompt";
            SpriteRenderer promptRend = prompt.AddComponent<SpriteRenderer>();
            promptRend.Sprite = Resources.Sprite_MM_Prompt;
            promptRend.Color = new Color(181, 181, 181);
            //prompt.transform.Parent = GameManager.MainCanvas.transform;
            promptRend.transform.LocalPosition += new Vector3(-5, -55, 0);
            promptRend.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
            promptRend.OrderInLayer = 4;

            UI_Wobble wobble = prompt.AddComponent<UI_Wobble>();
            wobble.Init(2.5f, 2);



            GameObject activator = Instantiate(new Prefab_SelectableText("", Color.Transparent, Color.Transparent));
            activator.transform.Parent = transform;
            activator.transform.LocalPosition = Vector3.Zero;
            activator.GetComponent<SelectableText>().OnActivate = () => { (GameManager.CurrentScene as MenuScene).SwitchMenu(MenuState.Main, false); };
        }
    }
}
