using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.Entity;

namespace GameProject.Code.Prefabs {
    public class Prefab_PickupGeneric : GameObject {
        public Prefab_PickupGeneric(Pickup pickupType) : base() {
            AbstractPickup pickupComp;

            SpriteRenderer rend = AddComponent<SpriteRenderer>();
            rend.DrawLayer = DrawLayer.ID["Pickups"];
            rend.OrderInLayer = 10;

            Rigidbody2D pickupRB = AddComponent<Rigidbody2D>();
            pickupRB.Drag = 5;

            CircleCollider2D coll = AddComponent<CircleCollider2D>(3.5f);

            //DEBUG
            transform.LocalPosition += new Microsoft.Xna.Framework.Vector3(60, 0, 0);


            switch (pickupType) {
                default:
                case Pickup.Coin:
                case Pickup.Coin_5:
                case Pickup.Coin_Double:
                    pickupComp = AddComponent<Pickup_Coin>();
                    break;
                case Pickup.Heart_Half:
                case Pickup.Heart_Whole:
                case Pickup.Heart_Double:
                    pickupComp = AddComponent<Pickup_Heart>();
                    break;
                case Pickup.Key:
                case Pickup.Key_Double:
                    pickupComp = AddComponent<Pickup_Key>();
                    break;

                // Implementations to add
                //case Pickup.BonusHeart:
                //    //pickupComp = AddComponent<Pickup_BonusHeart>();
                //    break;
                //case Pickup.PowerCell:
                //    //pickupComp = AddComponent<Pickup_PowerCell>();
                //    break;
                //case Pickup.Chest_Free:
                //case Pickup.Chest_Locked:
                //    //pickupComp = AddComponent<Pickup_Chest>();
                //    break;
            }

            pickupComp.InitPickup(pickupType, rend);
        }

        public Prefab_PickupGeneric() : this(Pickup.Coin) { }
    }
}
