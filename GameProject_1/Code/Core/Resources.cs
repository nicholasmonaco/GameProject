// Resources.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using GameProject.Code.Scripts.Components.Entity;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Loads/unloads all resources (assets) into the game.
    /// </summary>
    public static class Resources {
        public static Texture2D Sprite_TestSprite;
        public static Texture2D Sprite_TestArrowSprite;
        public static Texture2D Sprite_TestSquare;
        public static Texture2D Sprite_Pixel;

        public static Texture2D[] Sprite_RoomCorner_1;
        public static Texture2D[] Sprite_RoomCorner_2;

        public static Texture2D Sprite_Door_Inside;
        public static Texture2D Sprite_Door_Normal_Base;

        public static Texture2D Sprite_Bullet_Standard;

        public static Texture2D[] Sprite_UI_Reticles;

        public static Dictionary<Pickup, Texture2D> Sprite_Pickups;

        public static SpriteFont Font_Debug;







        public static void LoadContent(ContentManager content) {
            LoadTextures(content);
        }

        private static void LoadTextures(ContentManager content) {
            Sprite_TestSprite = content.Load<Texture2D>("Textures/Misc/Ball");
            Sprite_TestArrowSprite = content.Load<Texture2D>("Textures/Misc/Arrow");
            Sprite_TestSquare = content.Load<Texture2D>("Textures/Misc/Square_01");
            Sprite_Pixel = content.Load<Texture2D>("Textures/Misc/Pixel");

            Sprite_RoomCorner_1 = new Texture2D[2]; // Change later when there are more
            Sprite_RoomCorner_1[0] = content.Load<Texture2D>("Textures/Level/Wall_1-01");
            Sprite_RoomCorner_1[1] = content.Load<Texture2D>("Textures/Level/Wall_1-02");

            Sprite_RoomCorner_2 = new Texture2D[2];
            Sprite_RoomCorner_2[0] = content.Load<Texture2D>("Textures/Level/Wall_2-01");
            Sprite_RoomCorner_2[1] = content.Load<Texture2D>("Textures/Level/Wall_2-02");

            Sprite_Door_Inside = content.Load<Texture2D>("Textures/Level/Door/Door_Inside");
            Sprite_Door_Normal_Base = content.Load<Texture2D>("Textures/Level/Door/Door_Regular_Base");

            Sprite_Bullet_Standard = content.Load<Texture2D>("Textures/Bullet/PhotonShot");

            Sprite_UI_Reticles = new Texture2D[1];
            Sprite_UI_Reticles[0] = content.Load<Texture2D>("Textures/UI/Reticle_0");

            Sprite_Pickups = new Dictionary<Pickup, Texture2D>();



            Font_Debug = content.Load<SpriteFont>("Fonts/arial");


            Debug.Log("Textures loaded.");
        }
    }
}
