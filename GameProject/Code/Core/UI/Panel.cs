using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.UI {
    [AnimatableComponent]
    public class Panel : UIComponent {

        public Image PanelRenderer { get; private set; }
        [AnimatableValue] public Color PanelColor {
            get => PanelRenderer.Color;
            set {
                SetColor(value);
            }
        }

        [AnimatableValue] public float Alpha {
            get => PanelRenderer.Color.ToVector4().W;
            set {
                SetOpacity(value);
            }
        }

        public float _origA { private get; set; } = 0;


        public Panel(GameObject attached) : base(attached) {
            PanelRenderer = gameObject.AddComponent<Image>();
            PanelRenderer.Texture = Resources.Sprite_Pixel;
            PanelRenderer.rectTransform.Size = GameManager.Resolution.ToVector2();
            PanelRenderer.DrawLayer = DrawLayer.ID[DrawLayers.TotalOverlay];
            PanelRenderer.OrderInLayer = 20;
        }


        public void SetColor(Color newColor) {
            PanelRenderer.Color = newColor;
            _origA = (float)PanelRenderer.Color.A / 255;
        }

        public void SetOpacity(float alpha) {
            PanelRenderer.Color = new Color(PanelRenderer.Color, alpha); //use the byte implementation later if you can, it's faster but idk how it works
        }

        public void AddToOrder(int addition) {
            PanelRenderer.OrderInLayer += addition;
        }

        public bool IsVisible => PanelRenderer.Color.A > 0;



        public static IEnumerator FadeIntoBlack(Panel panel, float duration) {
            float timer = duration;
            //float maxOpacity = panel._origA;

            panel.SetOpacity(0);

            while (timer > 0) {
                yield return null;
                timer -= Time.unscaledDeltaTime;
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
                timer -= Time.unscaledDeltaTime;
                panel.SetOpacity(MathHelper.SmoothStep(0, 1, timer / duration));
            }

            yield return new WaitForEndOfFrame();

            panel.SetOpacity(0);
        }
    }
}