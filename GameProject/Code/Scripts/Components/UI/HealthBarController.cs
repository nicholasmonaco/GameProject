using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class HealthBarController : Component {
        public HealthBarController(GameObject attached) : base(attached) { }

        private static Vector2 HeartSize = new Vector2(17, 17);

        private List<UI_Heart> _hearts = null;


        public void InitHealthBar() {
            _hearts = new List<UI_Heart>(16);
            for (int y = 0; y < 2; y++) {
                for (int x = 0; x < 8; x++) {
                    GameObject heart = Instantiate<GameObject>(transform.Position, transform);
                    heart.Name = "UI Heart";

                    heart.transform.LocalPosition = (HeartSize * new Vector2(x, -y)).ToVector3();

                    SpriteRenderer rend = heart.AddComponent<SpriteRenderer>();
                    rend.DrawLayer = DrawLayer.ID[DrawLayers.HUD];
                    rend.OrderInLayer = 60;

                    UI_Heart data = heart.AddComponent<UI_Heart>();
                    data.SetHeartRenderer(rend);

                    _hearts.Add(data);
                }
            }

            PlayerStats.HeartUpdateAction = HealthUpdateAction;
        }


        private void HealthUpdateAction(int index, HeartContainer containerType) {
            _hearts[index].ContainerType = containerType;
        }
        
    }
}