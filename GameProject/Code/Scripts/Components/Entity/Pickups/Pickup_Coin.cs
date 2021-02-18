using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Pickup_Coin : AbstractPickup {
        public Pickup_Coin(GameObject attached) : base(attached) { }


        private int _moneyAmount;

        public override void InitPickup(Pickup type, SpriteRenderer pickupRenderer) {
            base.InitPickup(type, pickupRenderer);

            DeathAction = () => { StartCoroutine(YScaleFade()); };

            switch (type) {
                default:
                case Pickup.Coin:
                    _moneyAmount = 1;
                    break;
                case Pickup.Coin_5:
                    _moneyAmount = 5;
                    break;
                case Pickup.Coin_Double:
                    _moneyAmount = 2;
                    break;
            }
        }

        private void CoinDeathAction() {
            //disappear coin
            //spawn sparkles
            //play sfx
        }


        protected override bool CanPickup() {
            return true;
        }

        protected override void OnPickup() {
            PlayerStats.Money += _moneyAmount;
        }
    }
}