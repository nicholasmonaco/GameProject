using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Pickup_Key : AbstractPickup {
        public Pickup_Key(GameObject attached) : base(attached) { }


        private int _keyCount;

        public override void InitPickup(Pickup type, SpriteRenderer pickupRenderer) {
            base.InitPickup(type, pickupRenderer);

            switch (type) {
                default:
                case Pickup.Key:
                    _keyCount = 1;
                    break;
                case Pickup.Key_Double:
                    _keyCount = 2;
                    break;
            }
        }



        protected override bool CanPickup() {
            return true;
        }

        protected override void OnPickup() {
            PlayerStats.Keys += _keyCount;
        }
    }
}