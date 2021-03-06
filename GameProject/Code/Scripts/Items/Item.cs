﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Components.Bullet;
using GameProject.Code.Scripts.Components.Entity;

namespace GameProject.Code.Scripts.Items {
    public abstract class Item {

        public ItemID ID { get; private set; }

        public Item(ItemID id) {
            ID = id;
        }



        public void OnItemPickup() {
            //add item to inventory


            OnPickup();
        }

        public void OnItemLoss() {
            //remove item from inventory

            OnLose();
        }


        protected abstract void OnPickup();
        protected abstract void OnLose();


        //public virtual void OnBulletSpawn(AbstractBullet bullet) { }
        //public virtual void OnBulletFixedUpdate(AbstractBullet bullet) { }
        //public virtual void OnBulletDeath(AbstractBullet bullet) { }
        //public virtual void OnRespawn() { }
        //public virtual void OnEnemyDamage(AbstractEnemy enemy) { }
        //public virtual void OnEnemyContact(AbstractEnemy enemy) { }
        //public virtual void OnEnemyKill(AbstractEnemy enemy) { }
        //public virtual void OnHealthChange() { }
        //public virtual void OnPickupContact(Pickup pickupType) { }
        //public virtual void OnActiveItemUse() { }
        //public virtual void OnPlayerDamage() { }
        //public virtual void OnRoomEnter(Room room) { }
        //public virtual void OnRoomClear(Room room) { }
        //public virtual void OnRoomExit(Room room) { }
        //public virtual void OnLevelChange(LevelID newLevel) { }

    }
}
