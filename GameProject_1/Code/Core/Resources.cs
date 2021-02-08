// Resources.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Loads/unloads all resources (assets) into the game.
    /// </summary>
    public static class Resources {
        public static Texture2D Sprite_TestSprite;
        public static Texture2D Sprite_TestArrowSprite;
        public static Texture2D Sprite_TestSquare;
        public static Texture2D Sprite_Pixel;

        public static Dictionary<RoomStyle, List<Texture2D>> Sprites_RoomCorners;

        public static Texture2D Sprite_Door_Inside;
        public static Texture2D Sprite_Door_Normal_Base;

        public static Texture2D Sprite_Bullet_Standard;

        public static Texture2D[] Sprite_UI_Reticles;

        public static Dictionary<Pickup, Texture2D> Sprite_Pickups;

        public static Dictionary<MinimapIcon, Texture2D> Sprite_MinimapIcons;

        public static SpriteFont Font_Debug;







        public static void LoadContent(ContentManager content) {
            LoadTextures(content);
        }

        private static void LoadTextures(ContentManager content) {
            Sprite_TestSprite = content.Load<Texture2D>("Textures/Misc/Ball");
            Sprite_TestArrowSprite = content.Load<Texture2D>("Textures/Misc/Arrow");
            Sprite_TestSquare = content.Load<Texture2D>("Textures/Misc/Square_01");
            Sprite_Pixel = content.Load<Texture2D>("Textures/Misc/Pixel");


            Sprites_RoomCorners = new Dictionary<RoomStyle, List<Texture2D>>(1);

            Sprites_RoomCorners.Add(RoomStyle.QuarantineLevel_01, new List<Texture2D>(2) {
                content.Load<Texture2D>("Textures/Level/Wall_2-01"),
                content.Load<Texture2D>("Textures/Level/Wall_2-02") });



            Sprite_Door_Inside = content.Load<Texture2D>("Textures/Level/Door/Door_Inside");
            Sprite_Door_Normal_Base = content.Load<Texture2D>("Textures/Level/Door/Door_Regular_Base");

            Sprite_Bullet_Standard = content.Load<Texture2D>("Textures/Bullet/PhotonShot");

            Sprite_UI_Reticles = new Texture2D[1];
            Sprite_UI_Reticles[0] = content.Load<Texture2D>("Textures/UI/Reticle_0");

            Sprite_Pickups = new Dictionary<Pickup, Texture2D>(6);//change when all pickups are sprited
            Sprite_Pickups.Add(Pickup.Heart_Half, content.Load<Texture2D>("Textures/Pickup/Heart_Half"));
            Sprite_Pickups.Add(Pickup.Heart_Whole, content.Load<Texture2D>("Textures/Pickup/Heart_Whole"));
            Sprite_Pickups.Add(Pickup.BonusHeart, content.Load<Texture2D>("Textures/Pickup/BonusHeart_Whole"));
            Sprite_Pickups.Add(Pickup.Coin, content.Load<Texture2D>("Textures/Pickup/Coin"));
            Sprite_Pickups.Add(Pickup.Coin_5, content.Load<Texture2D>("Textures/Pickup/Coin_5"));
            Sprite_Pickups.Add(Pickup.PowerCell, content.Load<Texture2D>("Textures/Pickup/EnergyCell"));

            Sprite_MinimapIcons = new Dictionary<MinimapIcon, Texture2D>(3); //change when all icons are sprited
            Sprite_MinimapIcons.Add(MinimapIcon.Current, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Current"));
            Sprite_MinimapIcons.Add(MinimapIcon.Explored, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Explored"));
            Sprite_MinimapIcons.Add(MinimapIcon.Unexplored, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Unexplored"));




            Font_Debug = content.Load<SpriteFont>("Fonts/arial");


            Debug.Log("Textures loaded.");
        }
    }
}
