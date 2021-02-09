﻿using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public class Pickup_Heart : AbstractPickup {
        public Pickup_Heart(GameObject attached) : base(attached) { }


        private int _healthRestore;

        public override void InitPickup(Pickup type) {
            base.InitPickup(type);

            switch (type) {
                default:
                case Pickup.Heart_Half:
                    _healthRestore = 1;
                    break;
                case Pickup.Heart_Whole:
                    _healthRestore = 2;
                    break;
                case Pickup.Heart_Double:
                    _healthRestore = 4;
                    break;
            }
        }



        protected override bool CanPickup() {
            return PlayerStats.CurHealth_Red <= PlayerStats.MaxHealth_Red;
        }

        protected override void OnPickup() {
            PlayerStats.CurHealth_Red += _healthRestore;
        }
    }
}