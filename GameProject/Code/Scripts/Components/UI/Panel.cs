using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.UI {
    public class Panel : Component {

        private SpriteRenderer _panelRenderer;
        public float _origA { private get; set; } = 0;


        public Panel(GameObject attached) : base(attached) {
            _panelRenderer = gameObject.AddComponent<SpriteRenderer>();
            _panelRenderer.Sprite = Resources.Sprite_Pixel;
            _panelRenderer.SpriteScale = GameManager.Resolution.ToVector2();
            _panelRenderer.DrawLayer = DrawLayer.ID["TotalOverlay"];
            _panelRenderer.OrderInLayer = 20;
        }


        public void SetColor(Color newColor) {
            _panelRenderer.Color = newColor;
            _origA = (float)_panelRenderer.Color.A / 255;
        }

        public void SetOpacity(float alpha) {
            _panelRenderer.Color = new Color(_panelRenderer.Color, alpha); //use the byte implementation later if you can, it's faster but idk how it works
        }

        public bool IsVisible => _panelRenderer.Color.A > 0;



        public static IEnumerator FadeIntoBlack(Panel panel, float duration) {
            float timer = duration;
            //float maxOpacity = panel._origA;

            panel.SetOpacity(0);

            while (timer > 0) {
                yield return null;
                timer -= Time.deltaTime;
                panel.SetOpacity(MathHelper.SmoothStep(1, 0, timer / duration));
            }

            yield return new WaitForEndOfFrame();

            panel.SetOpacity(1);
        }

        public static IEnumerator FadeFromBlack(Panel panel, float duration) {
            float timer = duration;
            //float maxOpacity = panel._origA;

            panel.SetOpacity(1);

            while (timer > 0) {
                yield return null;
                timer -= Time.deltaTime;
                panel.SetOpacity(MathHelper.SmoothStep(0, 1, timer / duration));
            }

            yield return new WaitForEndOfFrame();

            panel.SetOpacity(0);
        }
    }
}