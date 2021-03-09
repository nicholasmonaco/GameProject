using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core {
    [Flags]
    public enum LayerID : uint{
        None = 0,               //None

        Default = 1,            //0
        IgnoreRaycast = 2,      //1

        Unnamed_2 = 4,          //2
        Unnamed_3 = 8,          //3
        Unnamed_4 = 16,          //4
        Unnamed_5 = 32,          //5
        Unnamed_6 = 64,          //6
        Unnamed_7 = 128,          //7

        Player = 256,             //8     // The player
        Enemy = 512,              //9     // Enemies
        Pickup = 1024,            //10    // Pickups
        Item = 2048,              //11    // Collectable items
        Wall = 4096,              //12    // Wall that isn't the edge wall
        EdgeWall = 8192,          //13    // The walls that make up the edges of the room
        Door = 16384,              //14    // Doors
        Bullet_Good = 32768,       //15    // Bullets from the player & familiars
        Bullet_Evil = 65536,       //16    // Bullets from enemies
        Familiar = 131072,          //17    // Familiars that follow the player and do something
        Obstacle = 262144,          //18    // Obstacles like rocks, invinicible walls, etc
        Hole = 524288,              //19    // Holes in the floor
        ShopItem = 1048576,          //20    // Items that can be obtained by trading
        Special = 2097152,           //21    // Misc stuff, like entrances to the next floor, secret area entrances, special event handles
        Enemy_Flying = 4194304,        //22     // Enemies that are flying

        Unnamed_23 = 8388608,        //23
        Unnamed_24 = 16777216,        //24
        Unnamed_25 = 33554432,        //25
        Unnamed_26 = 67108864,        //26
        Unnamed_27 = 134217728,        //27
        Unnamed_28 = 268435456,        //28
        Unnamed_29 = 536870912,        //29
        Unnamed_30 = 1073741824,        //30
        Unnamed_31 = 2147483648,         //31

        Max = 4294967295

        // This should be redone with bitshifts, but this is right too.
    }
}
