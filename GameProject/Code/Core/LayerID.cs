using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core {
    [Flags]
    public enum LayerID : int{
        None = 0,               //None

        Default = 1,                 //0
        IgnoreRaycast = 1 << 1,      //1

        Unnamed_2 = 1 << 2,          //2
        Unnamed_3 = 1 << 3,          //3
        Unnamed_4 = 1 << 4,          //4
        Unnamed_5 = 1 << 5,          //5
        Unnamed_6 = 1 << 6,          //6
        Unnamed_7 = 1 << 7,          //7

        Player = 1 << 8,             //8     // The player
        Enemy = 1 << 9,              //9     // Enemies
        Pickup = 1 << 10,            //10    // Pickups
        Item = 1 << 11,              //11    // Collectable items
        Wall = 1 << 12,              //12    // Wall that isn't the edge wall
        EdgeWall = 1 << 13,          //13    // The walls that make up the edges of the room
        Door = 1 << 14,              //14    // Doors
        Bullet_Good = 1 << 15,       //15    // Bullets from the player & familiars
        Bullet_Evil = 1 << 16,       //16    // Bullets from enemies
        Familiar = 1 << 17,          //17    // Familiars that follow the player and do something
        Obstacle = 1 << 18,          //18    // Obstacles like rocks, invinicible walls, etc
        Hole = 1 << 19,              //19    // Holes in the floor
        ShopItem = 1 << 20,          //20    // Items that can be obtained by trading
        Special = 1 << 21,           //21    // Misc stuff, like entrances to the next floor, secret area entrances, special event handles
        Enemy_Flying = 1 << 22,      //22    // Enemies that are flying
        Damage = 1 << 23,            //23    // Damage all living things

        Unnamed_24 = 1 << 24,        //24
        Unnamed_25 = 1 << 25,        //25
        Unnamed_26 = 1 << 26,        //26
        Unnamed_27 = 1 << 27,        //27
        Unnamed_28 = 1 << 28,        //28
        Unnamed_29 = 1 << 29,        //29
        Unnamed_30 = 1 << 30,        //30
        Unnamed_31 = int.MinValue,   //31

        Max = -1
    }
}
