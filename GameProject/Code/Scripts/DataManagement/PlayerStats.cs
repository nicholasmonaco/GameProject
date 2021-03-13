using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Scripts {
    public static class PlayerStats {

        public static readonly int _MaxHearts = 16;
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
            MaxRedHearts += Math.Clamp(MaxRedHearts + containersToChange*2, 0, _MaxHearts);
            
            if (fillNew) {
                RedHearts.Add(HeartType.Red);
                RedHearts.Add(HeartType.Red);
            }

            UpdateHealth();
        }

        public static void ChangeRedHealth(int halfHeartChange) {
            int realChange = Math.Clamp(halfHeartChange, -RedHearts.Count, MaxRedHearts);
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
            int realChange = Math.Clamp(halfHeartChange, -BonusHearts.Count, MaxRedHearts - RedHearts.Count);
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
    }


    public enum HeartType {
        Red,
        Bonus,
        Invisible
    }
}
