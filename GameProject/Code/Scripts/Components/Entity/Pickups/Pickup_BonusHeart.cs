using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Pickup_BonusHeart : AbstractPickup {
        public Pickup_BonusHeart(GameObject attached) : base(attached) { }


        private int _halfHearts;

        public override void InitPickup(Pickup type, SpriteRenderer pickupRenderer) {
            base.InitPickup(type, pickupRenderer);

            switch (type) {
                default:
                case Pickup.BonusHeart:
                    _halfHearts = 2;
                    break;
            }
        }



        protected override bool CanPickup() {
            return !PlayerStats.FullHealth;
        }

        protected override void OnPickup() {
            PlayerStats.ChangeBonusHealth(_halfHearts);
        }
    }
}