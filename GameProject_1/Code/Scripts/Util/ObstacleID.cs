using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Util {
    public enum ObstacleID {
        None = 0,

        // Hole is a space that bullets and flying things can go over, but not normal things
        Hole = 1,

        // Rocks are holes that bullets cannot go through
        Rock0 = 11,
        Rock1 = 12,
        Rock2 = 13,
        Rock3 = 14,
        Rock4 = 15,
        Rock6 = 16,

        // Destructables are rocks that can be destroyed by bullets
        Destructable0 = 21,
        Destructable1 = 22,
        Destructable2 = 23,
        Destructable3 = 24,
        Destructable4 = 25,
        Destructable5 = 26,

        // Walls are indestructable rocks
        Wall0 = 31,
        Wall1 = 32,
        Wall2 = 33,

        // Damage obstacles block no movement, but deal damage to the player if they can't fly
        Damage0 = 41,
        Damage1 = 42,
        Damage2 = 43,
    }
}
