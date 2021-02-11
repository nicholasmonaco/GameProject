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
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Loads/unloads all resources (assets) into the game.
    /// </summary>
    public static class Resources {
        public static Texture2D Sprite_TestSprite;
        public static Texture2D Sprite_TestArrowSprite;
        public static Texture2D Sprite_TestSquare;
        public static Texture2D Sprite_Pixel;
        public static Texture2D Sprite_Invisible;

        public static Dictionary<RoomStyle, List<Texture2D>> Sprites_RoomCorners;

        public static Texture2D Sprite_Door_Inside;
        public static Dictionary<DoorType, Texture2D> Sprites_DoorFrames;

        public static List<Texture2D> Sprites_BossDoorEyeAnim;


        public static Texture2D Sprite_Bullet_Standard;

        public static Texture2D[] Sprite_UI_Reticles;
        public static Dictionary<HeartContainer, Texture2D> Sprites_HeartContainers;

        public static Dictionary<Pickup, Texture2D> Sprite_Pickups;

        public static Texture2D Sprite_MinimapBackground;
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
            Sprite_Invisible = content.Load<Texture2D>("Textures/Misc/Invisible");


            Sprites_RoomCorners = new Dictionary<RoomStyle, List<Texture2D>>(1);

            Sprites_RoomCorners.Add(RoomStyle.QuarantineLevel_01, new List<Texture2D>(2) {
                content.Load<Texture2D>("Textures/Level/Wall_2-01"),
                content.Load<Texture2D>("Textures/Level/Wall_2-02") });

            Sprites_RoomCorners.Add(RoomStyle.Item, new List<Texture2D>(2) {
                content.Load<Texture2D>("Textures/Level/Wall_2-01"),
                content.Load<Texture2D>("Textures/Level/Wall_2-02") });

            Sprites_RoomCorners.Add(RoomStyle.Shop, new List<Texture2D>(2) {
                content.Load<Texture2D>("Textures/Level/Wall_2-01"),
                content.Load<Texture2D>("Textures/Level/Wall_2-02") });



            Sprite_Door_Inside = content.Load<Texture2D>("Textures/Level/Door/Door_Inside");

            Sprites_DoorFrames = new Dictionary<DoorType, Texture2D>(3);
            Sprites_DoorFrames.Add(DoorType.Item, content.Load<Texture2D>("Textures/Level/Door/Door_Item"));
            Sprites_DoorFrames.Add(DoorType.Boss, content.Load<Texture2D>("Textures/Level/Door/Door_Boss"));
            Sprites_DoorFrames.Add(DoorType.Normal, content.Load<Texture2D>("Textures/Level/Door/Door_QuarantineZone_Base"));


            Sprites_BossDoorEyeAnim = new List<Texture2D>(6);
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_0"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_1"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_2"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_3"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_4"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_5"));


            Sprite_Bullet_Standard = content.Load<Texture2D>("Textures/Bullet/PhotonShot");

            Sprite_UI_Reticles = new Texture2D[1];
            Sprite_UI_Reticles[0] = content.Load<Texture2D>("Textures/UI/Reticle_0");

            Sprites_HeartContainers = new Dictionary<HeartContainer, Texture2D>(6);
            Sprites_HeartContainers.Add(HeartContainer.Invisible, Sprite_Invisible);
            Sprites_HeartContainers.Add(HeartContainer.Empty, content.Load<Texture2D>("Textures/UI/Heart_Empty"));
            Sprites_HeartContainers.Add(HeartContainer.Red_Full, content.Load<Texture2D>("Textures/UI/Heart_Whole"));
            Sprites_HeartContainers.Add(HeartContainer.Red_Half, content.Load<Texture2D>("Textures/UI/Heart_Half"));
            Sprites_HeartContainers.Add(HeartContainer.Bonus_Full, content.Load<Texture2D>("Textures/UI/BonusHeart_Whole"));
            Sprites_HeartContainers.Add(HeartContainer.Bonus_Half, content.Load<Texture2D>("Textures/UI/BonusHeart_Half"));


            Sprite_Pickups = new Dictionary<Pickup, Texture2D>(6);//change when all pickups are sprited
            Sprite_Pickups.Add(Pickup.Heart_Half, content.Load<Texture2D>("Textures/Pickup/Heart_Half"));
            Sprite_Pickups.Add(Pickup.Heart_Whole, content.Load<Texture2D>("Textures/Pickup/Heart_Whole"));
            Sprite_Pickups.Add(Pickup.BonusHeart, content.Load<Texture2D>("Textures/Pickup/BonusHeart_Whole"));
            Sprite_Pickups.Add(Pickup.Coin, content.Load<Texture2D>("Textures/Pickup/Coin"));
            Sprite_Pickups.Add(Pickup.Coin_5, content.Load<Texture2D>("Textures/Pickup/Coin_5"));
            Sprite_Pickups.Add(Pickup.PowerCell, content.Load<Texture2D>("Textures/Pickup/EnergyCell"));

            Sprite_MinimapBackground = content.Load<Texture2D>("Textures/UI/Minimap/Background");

            Sprite_MinimapIcons = new Dictionary<MinimapIcon, Texture2D>(3); //change when all icons are sprited
            Sprite_MinimapIcons.Add(MinimapIcon.Current, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Current"));
            Sprite_MinimapIcons.Add(MinimapIcon.Explored, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Explored"));
            Sprite_MinimapIcons.Add(MinimapIcon.Unexplored, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Unexplored"));
            Sprite_MinimapIcons.Add(MinimapIcon.Item, content.Load<Texture2D>("Textures/UI/Minimap/Icon_Item"));
            Sprite_MinimapIcons.Add(MinimapIcon.Boss, content.Load<Texture2D>("Textures/UI/Minimap/Icon_Boss"));




            Font_Debug = content.Load<SpriteFont>("Fonts/arial");


            Debug.Log("Textures loaded.");
        }
    }
}
