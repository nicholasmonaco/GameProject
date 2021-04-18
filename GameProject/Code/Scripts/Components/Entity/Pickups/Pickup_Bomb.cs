using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Pickup_Bomb : AbstractPickup {
        public Pickup_Bomb(GameObject attached) : base(attached) { }


        private int _bombAmount;

        public override void InitPickup(Pickup type, SpriteRenderer pickupRenderer) {
            base.InitPickup(type, pickupRenderer);

            DeathAction = () => { StartCoroutine(YScaleFadeOut()); };

            switch (type) {
                default:
                case Pickup.Bomb:
                    _bombAmount = 1;
                    break;
                case Pickup.Bomb_Double:
                    _bombAmount = 2;
                    break;
            }
        }


        protected override bool CanPickup() {
            return true;
        }

        protected override void OnPickup() {
            PlayerStats.Bombs += _bombAmount;
        }
    }
}