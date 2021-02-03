using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.DataManagement {
    public static class PlayerStats {

        public static Action DeathAction = () => { };

        // Health
        public static int MaxHealth = 3; // Current number of RED hearts - Measured in full hearts

        public static void TakeDamage(int damage) {
            int toChange = damage;
            int takeaway;

            if (CurHealth_Bonus > 0) {
                takeaway = toChange - (toChange - CurHealth_Bonus);
                CurHealth_Bonus -= takeaway;
                toChange -= takeaway;
            }
            if (CurHealth_Red > 0) {
                takeaway = toChange - (toChange - CurHealth_Red);
                CurHealth_Red -= takeaway;
                toChange -= takeaway;
            }

            if (toChange > 0) {
                DeathAction();
            }
        }

        public static int CurHealth_Red = 6; // Current number of filled RED hearts - Measured in half-hearts
        public static int CurHealth_Bonus = 0; // Current number of BONUS hearts - Measured in in half-hearts


        // Regular stats
        public static float Speed;
        public static float Range;
        public static float ShotRate;
        public static float ShotSpeed;
        public static float ShotSize;
        public static float Damage;
        public static float Knockback;
        public static float Luck;

        public static float Ex_Benefit = 0;
        public static float Ex_Curse = 0;


        // Special abilities
        public static int HomingStr = 0;
        public static int PiercingCount = 0;
    }
}
