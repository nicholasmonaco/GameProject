using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core {
    public enum LayerID {
        Default = 0,
        IgnoreRaycast = 1,

        Player = 8,             // The player
        Enemy = 9,              // Enemies
        Pickup = 10,            // Pickups
        Item = 11,              // Collectable items
        Wall = 12,              // Wall that isn't the edge wall
        EdgeWall = 13,          // The walls that make up the edges of the room
        Door = 14,              // Doors
        Bullet_Good = 15,       // Bullets from the player & familiars
        Bullet_Evil = 16,       // Bullets from enemies
        Familiar = 17,          // Familiars that follow the player and do something
        Obstacle = 18,          // Obstacles like rocks, invinicible walls, etc
        Hole = 19,              // Holes in the floor
        ShopItem = 20,          // Items that can be obtained by trading
        Special = 21            // Misc stuff, like entrances to the next floor, secret area entrances, special event handles
    }
}
