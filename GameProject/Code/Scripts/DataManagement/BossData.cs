using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts {
    public struct BossData {
        public BossID ID;
        public string Name;
        public string FlavorText;

        public float MaxHealth;
    }


    public enum BossID {
        Debug = -1,

        // Quarantine Level
        Feedy,                  // Monster of the Pit
        SCX_07,                 // Security Paladin
        ddddd,                  // 

        // Cold Storage
        Frankfurt,              // Mystery Meat
        Iceberger,              // Freezerburn Champion
        Chunk,                  // Gluttony Incarnate


        // Dark Caves
        Melacobra,              // Night Snake
        Burrow,                 // Cavern Digger
        LED,                    // Portable Sun

        // Infested Hive
        Waspra,                 // Queen of the Hive
        Patrick,                // Unforgiving Parasite
        awdawdawda,             //


        // Deep Irradiation Zone
        Yaramir,                // Toxic Corpse
        ZetaBloom,              // Fungus Horde
        Coreflux,               // Corium Amalgam

        // Mana Exposure
        LeatherClaw,            // Interdimensional Terror
        


        // Magma Chambers
        Cyclock,                // One-Eyed Guardian
        Puhmp,                  // Pressure Control Unit
        Pythron,                // Magma Swimmer

        // Geothermal Research Center
        ProfessorHall,          // Mad Scientist


    }
}
