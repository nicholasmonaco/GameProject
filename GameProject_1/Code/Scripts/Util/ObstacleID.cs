using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Util {
    public enum ObstacleID {
        None = 0,

        // Hole is a space that bullets and flying things can go over, but not normal things
        // ID from tilemap: 1
        Hole = 1,

        // Rocks are holes that bullets cannot go through
        // ID from tilemap: 2
        Rock0 = 11,
        Rock1 = 12,
        Rock2 = 13,
        Rock3 = 14,
        Rock4 = 15,
        Rock5 = 16,

        // Destructables are rocks that can be destroyed by bullets
        Destructable0 = 21,     // ID from tilemap: ?
        Destructable1 = 22,     // ID from tilemap: ?
        Destructable2 = 23,     // ID from tilemap: ?
        Destructable3 = 24,     // ID from tilemap: ?
        Destructable4 = 25,     // ID from tilemap: ?
        Destructable5 = 26,     // ID from tilemap: ?

        // Walls are indestructable rocks
        // ID from tilemap: 3
        Wall = 31,     

        // Damage obstacles block no movement, but deal damage to the player if they can't fly
        // ID from tilemap: 50
        Damage0 = 41,
        Damage1 = 42,
        Damage2 = 43,
    }
}
