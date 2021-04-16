using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Util {
    public enum EntityID {
        None = 0,

        ShopKeep = 1,
        ItemPedastal = 2,
        ItemPedastal_Option = 3,

        Trade_Coin = 10,
        Trade_RedHeart = 11,
        Trade_BonusHeart = 12,
        Trade_Key = 13,
        Trade_Item = 14,
        Trade_Planet = 15,
        Trade_PlanetSpecific = 16,



        // Pickups
        Pickup_Random = 290,          // Any standard one
        Pickup_RandomChest = 291,     // Any unopended chest
        Pickup_RandomAllBasic = 292,  // Any standard one or chest
        Pickup_RandomPlanet = 293,    // Any planet consumable
        Pickup_RandomAll = 299,       // Anything except doubles and opened chests

        Pickup_Heart_Half = 201,
        Pickup_Heart_Whole = 202,
        Pickup_BonusHeart = 203,
        Pickup_Key = 204,
        Pickup_PowerCell = 205,
        Pickup_Coin = 206,

        Pickup_Coin_5 = 207,

        Pickup_Chest_Free = 208,
        Pickup_Chest_Free_Opened = 209,
        Pickup_Chest_Locked = 210,
        Pickup_Chest_Locked_Opened = 211,

        Pickup_Heart_Double = 212,
        Pickup_Key_Double = 213,
        Pickup_Coin_Double = 214,

        Pickup_Bomb = 215,
        Pickup_Bomb_Double = 216,
        // End pickups

        // Enemies
        CaveChaser = 301,               // Chases the player
        CaveChaser_Armed = 302,         // Chases the player and shoots bullets
        CaveChaser_Omega = 303,         // Chaser that is slower but has more health
        CaveChaser_Buckshot = 304,      // Chaser that stops periodically to shoot a spread of bullets at the player

        Drone_Bugged = 305,             // Hovers in place
        Drone_Attack = 306,             // Bugged drone that flies towards the player
        
        Turret_Guard = 307,             // Stays still and shoots at the player
        Turret_Multi = 308,             // Shoots bursts of 3 bullets at the player
        // End enemies
    }
}
