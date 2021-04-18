using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Scripts.Components;
using Microsoft.Xna.Framework;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Items;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Scripts.Components.Bullet;
using GameProject.Code.Scripts.Items.ItemTypes;

namespace GameProject.Code.Scripts {
    public static class PlayerStats {

        public static readonly int _MaxHearts = 16; //measured in whole hearts
        public static readonly int _MaxHalfHearts = 32;
        public static Action DeathAction = () => { };


        // Health
        public static Action<int, HeartContainer> HeartUpdateAction = (heartID, containerType) => { };

        private static int MaxRedHearts = 6; //Measured in half hearts
        private static List<HeartType> RedHearts = new List<HeartType>(32);
        private static List<HeartType> BonusHearts = new List<HeartType>(32);


        /// <summary>
        /// Used to hard reset health to full heart values.
        /// </summary>
        /// <param name="redHealth">The amount of red half-hearts</param>
        /// <param name="bonusHealth">The amount of bonus half-hearts</param>
        public static void SetHealth(int redHealth, int bonusHealth) {
            int index = 0;
            (HeartType, HeartType) curContainer = (HeartType.Invisible, HeartType.Invisible);
            MaxRedHearts = redHealth;

            RedHearts.Clear();
            BonusHearts.Clear();

            for(int i = 0; i < MaxRedHearts; i++) {
                RedHearts.Add(HeartType.Red);
                if (i % 2 == 0) curContainer.Item1 = HeartType.Red;
                else if (i % 2 == 1) {
                    curContainer.Item2 = HeartType.Red;
                    HeartUpdateAction(index/2, SolveContainerType(curContainer));
                }
                index++;
            }

            for (int i = 0; i < bonusHealth; i++) {
                BonusHearts.Add(HeartType.Bonus);
                if (i % 2 == 0) curContainer.Item1 = HeartType.Bonus;
                else if (i % 2 == 1) {
                    curContainer.Item2 = HeartType.Bonus;
                    HeartUpdateAction(index/2, SolveContainerType(curContainer));
                }
                index++;
            }


            int remainder = _MaxHalfHearts - index;
            for (int i = 0; i < remainder; i += 2) {
                // Set the rest of the UI hearts invisible
                HeartUpdateAction(index/2, HeartContainer.Invisible);
                index += 2;
            }


            // While this isn't nessecary here, it is worth doing to make sure stuff isn't messed up.
            UpdateHealth();
        }

        /// <summary>
        /// Used to update the health bar upon changing any health value.
        /// </summary>
        private static void UpdateHealth() {
            int index = 0;
            (HeartType, HeartType) curContainer = (HeartType.Invisible, HeartType.Invisible);

            for (int i = 0; i < MaxRedHearts; i++) {
                HeartType curHalfHeart = index >= RedHearts.Count ? HeartType.Invisible : HeartType.Red;

                if (i % 2 == 0) curContainer.Item1 = curHalfHeart;
                else if (i % 2 == 1) {
                    curContainer.Item2 = curHalfHeart;
                    HeartUpdateAction(index/2, SolveContainerType(curContainer));
                }
                index++;
            }


            curContainer = (HeartType.Invisible, HeartType.Invisible);

            for (int i = 0; i < BonusHearts.Count; i++) {
                HeartType curHalfHeart = i >= BonusHearts.Count ? HeartType.Invisible : BonusHearts[i];

                if (i % 2 == 0) curContainer.Item1 = curHalfHeart;
                else if (i % 2 == 1) {
                    curContainer.Item2 = curHalfHeart;
                    HeartUpdateAction(index/2, SolveContainerType(curContainer));
                }
                index++;
            }

            if(index % 2 == 1) {
                //finish the last one
                curContainer.Item2 = HeartType.Invisible;
                HeartUpdateAction(index/2, SolveContainerType(curContainer));
                index++;
            }

            int remainder = _MaxHalfHearts - index;
            for (int i = 0; i < remainder; i+=2) {
                //set hearts invisible
                HeartUpdateAction(index/2, HeartContainer.Invisible);
                index += 2;
            }


            if(RedHearts.Count == 0 && BonusHearts.Count == 0) {
                // Die
                DeathAction();
            }
        }

        /// <summary>
        /// Solves what type of UI heart container should be used based on each half-heart
        /// </summary>
        /// <param name="types">A tuple storing the types of each half of the heart</param>
        /// <returns>What type of UI heart container should be used to represent the data in the tuple</returns>
        public static HeartContainer SolveContainerType((HeartType, HeartType) types) {
            // red and red
            // red and none
            // none and none
            // blue and blue
            // blue and none
            switch (types) {
                case (HeartType.Red, HeartType.Red):
                    return HeartContainer.Red_Full;
                case (HeartType.Red, HeartType.Invisible):
                    return HeartContainer.Red_Half;
                default:
                case (HeartType.Invisible, HeartType.Invisible):
                    return HeartContainer.Empty;
                case (HeartType.Bonus, HeartType.Bonus):
                    return HeartContainer.Bonus_Full;
                case (HeartType.Bonus, HeartType.Invisible):
                    return HeartContainer.Bonus_Half;
            }
        }


        public static void ChangeMaxRedHealth(int containersToChange, bool fillNew = false) {
            // Add
            MaxRedHearts = Math.Clamp(MaxRedHearts + containersToChange * 2, 0, _MaxHearts);
            
            if (fillNew) {
                RedHearts.Add(HeartType.Red);
                RedHearts.Add(HeartType.Red);
            }

            UpdateHealth();
        }

        public static void ChangeRedHealth(int halfHeartChange) {
            int realChange = Math.Clamp(halfHeartChange, -RedHearts.Count, MaxRedHearts - RedHearts.Count);
            realChange = Math.Abs(realChange);

            if(halfHeartChange > 0) {
                // Add health
                for(int i = 0; i < realChange; i++) {
                    RedHearts.Add(HeartType.Red);
                }
            } else {
                // Take damage
                for (int i = 0; i < realChange; i++) {
                    RedHearts.RemoveAt(RedHearts.Count - 1);
                }
            }

            UpdateHealth();
        }

        public static void ChangeBonusHealth(int halfHeartChange, HeartType addType = HeartType.Bonus) {
            if (halfHeartChange == 0) return;

            int realChange = Math.Clamp(halfHeartChange, -BonusHearts.Count, _MaxHalfHearts - RedHearts.Count);
            realChange = Math.Abs(realChange);

            if (halfHeartChange > 0) {
                // Add health
                for (int i = 0; i < realChange; i++) {
                    BonusHearts.Add(addType);
                }
            } else {
                // Take damage
                for (int i = 0; i < realChange; i++) {
                    BonusHearts.RemoveAt(BonusHearts.Count - 1);
                }
            }

            UpdateHealth();
        }


        public static void TakeDamage(int damage = 1) {
            bool doFullBonusDamage = damage >= BonusHearts.Count;
            bool doEvenMore = damage > BonusHearts.Count;
            int preBonusCount = BonusHearts.Count;

            if (doFullBonusDamage) {
                //do bonus damage
                ChangeBonusHealth(-preBonusCount);

                if (doEvenMore) {
                    //do red damage
                    ChangeRedHealth(-(damage - preBonusCount));
                }
            } else {
                ChangeBonusHealth(-damage);
            }

            UpdateHealth();
        }

        public static void TakeDamage_FocusRed(int damage = 1) {
            bool doFullBonusDamage = damage >= RedHearts.Count - 1;
            bool doEvenMore = damage > RedHearts.Count - 1;
            int preRedCount = RedHearts.Count;

            if (doFullBonusDamage) {
                //do bonus damage
                ChangeRedHealth(-(preRedCount - 1));

                if (doEvenMore) {
                    //do red damage
                    ChangeBonusHealth(-(damage - preRedCount - 1));
                }
            } else {
                ChangeRedHealth(-damage);
            }
            
            UpdateHealth();
        }


        public static bool FullRedHealth => RedHearts.Count == MaxRedHearts;
        public static bool FullHealth => RedHearts.Count + BonusHearts.Count == _MaxHalfHearts;



        // Regular stats
        public static float Speed = 1;
        public static float Range = 1.2f;
        public static float ShotRate = 2;
        public static float ShotSpeed = 90;
        public static float ShotSize = 1;
        public static float Damage = 3;
        public static float Knockback = 0.5f;
        public static float Luck = 0;

        public static float Ex_Benefit = 0;
        public static float Ex_Curse = 0;

        


        // Special abilities
        public static Color ShotColor = new Color(255, 252, 230); // Super light yellow

        public static int HomingStr = 0;
        public static int PiercingCount = 0;


        // Other values
        private static int _money = 0;
        private static int _keys = 0;
        private static int _bombs = 0;

        public static int Money { 
            get => _money;
            set {
                _money = Math.Clamp(value, 0, 999);
                GameManager.Inventory?.UpdateMoneyText();
            }
        }
        public static int Keys {
            get => _keys;
            set {
                _keys = Math.Clamp(value, 0, 999);
                GameManager.Inventory?.UpdateKeyText();
            }
        }
        public static int Bombs {
            get => _bombs;
            set {
                _bombs = Math.Clamp(value, 0, 999);
                GameManager.Inventory?.UpdateBombText();
            }
        }

        public static void ResetStats() {
            Money = 0;
            Keys = 0;
            Bombs = 0;

            Speed = 1;
            Range = 1.2f;
            ShotRate = 2;
            ShotSpeed = 90;
            ShotSize = 1;
            Damage = 3;
            Knockback = 0.5f;
            Luck = 0;

            Ex_Benefit = 0;
            Ex_Curse = 0;

            ShotColor = new Color(255, 252, 230); // Super light yellow

            HomingStr = 0;
            PiercingCount = 0;
        }


        // Inventory
        public static ItemID ActiveItem = ItemID.None;
        public static Dictionary<ItemID, List<Item>> Inventory;

        #region Item Events

        public static Action OnActiveItemUse;
        public static Action<AbstractBullet> OnBulletDeath;
        public static Action<AbstractBullet> OnBulletFixedUpdate;
        public static Action<AbstractBullet> OnBulletSpawn;
        public static Action<AbstractEnemy> OnEnemyContact;
        public static Action<AbstractEnemy> OnEnemyDamage;
        public static Action<AbstractEnemy> OnEnemyKill;
        public static Action OnHealthChange;
        public static Action<LevelID> OnLevelChange;
        public static Action<Pickup> OnPickupContact;
        public static Action OnPlayerDamage;
        public static Action OnRespawn;
        public static Action<Room> OnRoomClear;
        public static Action<Room> OnRoomEnter;
        public static Action<Room> OnRoomExit;

        #endregion


        public static void ResetInventory() {
            Inventory = new Dictionary<ItemID, List<Item>>();
            ActiveItem = ItemID.None;

            OnActiveItemUse = () => { };
            OnBulletDeath = (value) => { };
            OnBulletFixedUpdate = (value) => { };
            OnBulletSpawn = (value) => { };
            OnEnemyContact = (value) => { };
            OnEnemyDamage = (value) => { };
            OnEnemyKill = (value) => { };
            OnHealthChange = () => { };
            OnLevelChange = (value) => { };
            OnPickupContact = (value) => { };
            OnPlayerDamage = () => { };
            OnRespawn = () => { };
            OnRoomClear = (value) => { };
            OnRoomEnter = (value) => { };
            OnRoomExit = (value) => { };
        }


        public static void AddItem(ItemID id) {
            Item item = (Item)Activator.CreateInstance(Item.GetItem(id));

            
            if(item != null) {
                if (Inventory.ContainsKey(id)) {
                    item._index = Inventory[id].Count;
                    Inventory[id].Add(item);
                } else {
                    item._index = 0;
                    Inventory.Add(id, new List<Item>(1) { item });
                }


                item.OnItemPickup();

                //go through each interface and if it is, add it to the thing
                #region Checking Item Interfaces

                if (item is Item_OnActiveItemUse i_01) {
                    OnActiveItemUse += i_01.OnActiveItemUse;
                }

                if(item is Item_OnBulletDeath i_02) {
                    OnBulletDeath += i_02.OnBulletDeath;
                }

                if (item is Item_OnBulletFixedUpdate i_03) {
                    OnBulletFixedUpdate += i_03.OnBulletFixedUpdate;
                }

                if(item is Item_OnBulletSpawn i_04) {
                    OnBulletSpawn += i_04.OnBulletSpawn;
                }

                if (item is Item_OnEnemyContact i_05) {
                    OnEnemyContact += i_05.OnEnemyContact;
                }

                if(item is Item_OnEnemyDamage i_06) {
                    OnEnemyDamage += i_06.OnEnemyDamage;
                }

                if(item is Item_OnEnemyKill i_07) {
                    OnEnemyKill += i_07.OnEnemyKill;
                }

                if(item is Item_OnHealthChange i_08) {
                    OnHealthChange += i_08.OnHealthChange;
                }

                if(item is Item_OnLevelChange i_09) {
                    OnLevelChange += i_09.OnLevelChange;
                }

                if(item is Item_OnPickupContact i_10) {
                    OnPickupContact += i_10.OnPickupContact;
                }

                if(item is Item_OnPlayerDamage i_11) {
                    OnPlayerDamage += i_11.OnPlayerDamage;
                }

                if(item is Item_OnRespawn i_12) {
                    OnRespawn += i_12.OnRespawn;
                }

                if(item is Item_OnRoomClear i_13) {
                    OnRoomClear += i_13.OnRoomClear;
                }

                if(item is Item_OnRoomEnter i_14) {
                    OnRoomEnter += i_14.OnRoomEnter;
                }

                if(item is Item_OnRoomExit i_15) {
                    OnRoomExit += i_15.OnRoomExit;
                }

                #endregion
            }
        }

        public static void RemoveItem(ItemID id) {
            if (!Inventory.ContainsKey(id)) return;


            Item item = Inventory[id][^1];

            item.OnItemLoss();


            //go through each interface and if it is, add it to the thing
            #region Checking Item Interfaces

            if (item is Item_OnActiveItemUse i_01) {
                OnActiveItemUse += i_01.OnActiveItemUse;
            }

            if (item is Item_OnBulletDeath i_02) {
                OnBulletDeath += i_02.OnBulletDeath;
            }

            if (item is Item_OnBulletFixedUpdate i_03) {
                OnBulletFixedUpdate += i_03.OnBulletFixedUpdate;
            }

            if (item is Item_OnBulletSpawn i_04) {
                OnBulletSpawn += i_04.OnBulletSpawn;
            }

            if (item is Item_OnEnemyContact i_05) {
                OnEnemyContact += i_05.OnEnemyContact;
            }

            if (item is Item_OnEnemyDamage i_06) {
                OnEnemyDamage += i_06.OnEnemyDamage;
            }

            if (item is Item_OnEnemyKill i_07) {
                OnEnemyKill += i_07.OnEnemyKill;
            }

            if (item is Item_OnHealthChange i_08) {
                OnHealthChange += i_08.OnHealthChange;
            }

            if (item is Item_OnLevelChange i_09) {
                OnLevelChange += i_09.OnLevelChange;
            }

            if (item is Item_OnPickupContact i_10) {
                OnPickupContact += i_10.OnPickupContact;
            }

            if (item is Item_OnPlayerDamage i_11) {
                OnPlayerDamage += i_11.OnPlayerDamage;
            }

            if (item is Item_OnRespawn i_12) {
                OnRespawn += i_12.OnRespawn;
            }

            if (item is Item_OnRoomClear i_13) {
                OnRoomClear += i_13.OnRoomClear;
            }

            if (item is Item_OnRoomEnter i_14) {
                OnRoomEnter += i_14.OnRoomEnter;
            }

            if (item is Item_OnRoomExit i_15) {
                OnRoomExit += i_15.OnRoomExit;
            }

            #endregion


            if(Inventory[id].Count == 1) {
                Inventory.Remove(id);
            } else {
                Inventory[id].RemoveAt(item._index);
            }
        }


    }


    public enum HeartType {
        Red,
        Bonus,
        Invisible
    }
}
