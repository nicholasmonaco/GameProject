// Resources.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

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

        public static Texture2D Sprite_Door_Inside;
        public static Texture2D Sprite_Door_Normal_Base;







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

            Sprite_Door_Inside = content.Load<Texture2D>("Textures/Level/Door/Door_Inside");
            Sprite_Door_Normal_Base = content.Load<Texture2D>("Textures/Level/Door/Door_Regular_Base");



            Debug.Log("Textures loaded.");
        }
    }
}
