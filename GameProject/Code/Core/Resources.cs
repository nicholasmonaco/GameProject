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
using GameProject.Code.Pipeline;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Loads/unloads all resources (assets) into the game.
    /// </summary>
    public static class Resources {

        #region Sprite Resources
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

        public static Dictionary<ObstacleID, Texture2D> Sprites_GlobalObstacles;
        public static Dictionary<LevelID, Dictionary<ObstacleID, Texture2D>> Sprites_Obstacles;
        #endregion

        #region Font Resources
        public static SpriteFont Font_Debug;
        #endregion

        #region Sound Resources
        public static Dictionary<Pickup, SoundEffect> Sounds_PickupSpawn;
        public static Dictionary<Pickup, SoundEffect> Sounds_PickupCollect;
        #endregion

        #region RoomData Resources
        private static ContentManager RoomContent;

        private static Dictionary<RoomType, bool> ValidRoomTypes = new Dictionary<RoomType, bool>(12) { //true if pulled from floor-specific
            { RoomType.Normal, true }, 
            { RoomType.Boss, true },
            { RoomType.Starting, false },
            { RoomType.Item, false },
            { RoomType.Secret, false },
            { RoomType.SuperSecret, false },
            { RoomType.Casino, false },
            { RoomType.Shop, false },
            { RoomType.MiniBoss, false },
            { RoomType.Magic, false },
            { RoomType.Techno, false },
            { RoomType.Challenge, false },
            { RoomType.Decision, false } 
        };

        private static Dictionary<LevelID, Dictionary<RoomType, int>> RoomTypeCounts;

        public static Dictionary<RoomType, Dictionary<int, RoomData>> CurRooms_All;
        public static Dictionary<RoomType, Dictionary<int, RoomData>> CurRooms_Up;
        public static Dictionary<RoomType, Dictionary<int, RoomData>> CurRooms_Down;
        public static Dictionary<RoomType, Dictionary<int, RoomData>> CurRooms_Left;
        public static Dictionary<RoomType, Dictionary<int, RoomData>> CurRooms_Right;
        #endregion






        public static void LoadContent(ContentManager content) {
            LoadTextures(content);
            //LoadSounds(content);
            //LoadRooms(content);

            RoomContent = new ContentManager(content.ServiceProvider);


            InitRooms();
        }


        private static void InitRooms() {
            CurRooms_All = new Dictionary<RoomType, Dictionary<int, RoomData>>();
            CurRooms_Up = new Dictionary<RoomType, Dictionary<int, RoomData>>();
            CurRooms_Down = new Dictionary<RoomType, Dictionary<int, RoomData>>();
            CurRooms_Left = new Dictionary<RoomType, Dictionary<int, RoomData>>();
            CurRooms_Right = new Dictionary<RoomType, Dictionary<int, RoomData>>();

            RoomTypeCounts = new Dictionary<LevelID, Dictionary<RoomType, int>>();

            RoomTypeCounts.Add(LevelID.QuarantineLevel, new Dictionary<RoomType, int>() { 
                { RoomType.Normal, 91 }, //change to 161 later
                { RoomType.Boss, 0 },
                { RoomType.Item, 1 },
            });
        }


        private static void InitRoomLoad() {
            CurRooms_All.Clear();

            CurRooms_Up.Clear();
            CurRooms_Down.Clear();
            CurRooms_Left.Clear();
            CurRooms_Right.Clear();
        }

        public static void LoadRooms(LevelID levelID) {
            // Load it in when you generate the level, then unload it when you go to the next one
            InitRoomLoad();

            foreach(RoomType type in ValidRoomTypes.Keys) {
                if (!RoomTypeCounts[levelID].ContainsKey(type) || RoomTypeCounts[levelID][type] == 0) continue;

                int roomCount = RoomTypeCounts[levelID][type]; 

                Dictionary<int, RoomData> all = new Dictionary<int, RoomData>(roomCount);
                Dictionary<int, RoomData> up = new Dictionary<int, RoomData>();
                Dictionary<int, RoomData> down = new Dictionary<int, RoomData>();
                Dictionary<int, RoomData> left = new Dictionary<int, RoomData>();
                Dictionary<int, RoomData> right = new Dictionary<int, RoomData>();

                for (int i = 0; i < roomCount; i++) {
                    RoomData room;
                    if(ValidRoomTypes[type] == true) {
                        room = RoomContent.Load<RoomData>($"Content/RoomData/QuarantineLevel/{type}/{i.ToString("D3")}");
                        if (i == 12) {
                            Debug.Log($"up: {room.Door_Up}  down: {room.Door_Down}  left: {room.Door_Left}   right: {room.Door_Right}");
                        }
                    } else {
                        room = RoomContent.Load<RoomData>($"Content/RoomData/All/{type}/{i.ToString("D3")}");
                    }

                    all.Add(room.RoomID, room);

                    if (room.Door_Up) up.Add(room.RoomID, room);
                    if (room.Door_Down) down.Add(room.RoomID, room);
                    if (room.Door_Left) left.Add(room.RoomID, room);
                    if (room.Door_Right) right.Add(room.RoomID, room);
                }

                CurRooms_All[type] = all;
                CurRooms_Up[type] = up;
                CurRooms_Down[type] = down;
                CurRooms_Left[type] = left;
                CurRooms_Right[type] = right;
            }
        }

        public static void UnloadRooms() {
            RoomContent.Unload();
        }




        private static void LoadSounds(ContentManager content) {
            Sounds_PickupSpawn = new Dictionary<Pickup, SoundEffect>();
            Sounds_PickupSpawn.Add(Pickup.Heart_Whole, content.Load<SoundEffect>("Sounds/Pickup/Spawn/Heart"));
            Sounds_PickupSpawn.Add(Pickup.Coin, content.Load<SoundEffect>("Sounds/Pickup/Spawn/Coin"));
            Sounds_PickupSpawn.Add(Pickup.Key, content.Load<SoundEffect>("Sounds/Pickup/Spawn/Key"));
            Sounds_PickupSpawn.Add(Pickup.Chest_Free, content.Load<SoundEffect>("Sounds/Pickup/Spawn/Chest"));

            Sounds_PickupCollect = new Dictionary<Pickup, SoundEffect>();
            Sounds_PickupSpawn.Add(Pickup.Heart_Whole, content.Load<SoundEffect>("Sounds/Pickup/Collect/Heart"));
            Sounds_PickupSpawn.Add(Pickup.Coin, content.Load<SoundEffect>("Sounds/Pickup/Collect/Coin"));
            Sounds_PickupSpawn.Add(Pickup.Key, content.Load<SoundEffect>("Sounds/Pickup/Collect/Key"));
            Sounds_PickupSpawn.Add(Pickup.Chest_Free, content.Load<SoundEffect>("Sounds/Pickup/Collect/Chest"));
            Sounds_PickupSpawn.Add(Pickup.PowerCell, content.Load<SoundEffect>("Sounds/Pickup/Collect/PowerCell"));
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

            Sprite_MinimapIcons = new Dictionary<MinimapIcon, Texture2D>(5); //change when all icons are sprited
            Sprite_MinimapIcons.Add(MinimapIcon.Current, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Current"));
            Sprite_MinimapIcons.Add(MinimapIcon.Explored, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Explored"));
            Sprite_MinimapIcons.Add(MinimapIcon.Unexplored, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Unexplored"));
            Sprite_MinimapIcons.Add(MinimapIcon.Item, content.Load<Texture2D>("Textures/UI/Minimap/Icon_Item"));
            Sprite_MinimapIcons.Add(MinimapIcon.Boss, content.Load<Texture2D>("Textures/UI/Minimap/Icon_Boss"));


            LoadObstacleSprites(content);



            Font_Debug = content.Load<SpriteFont>("Fonts/arial");


            Debug.Log("Textures loaded.");
        }


        private static void LoadObstacleSprites(ContentManager content) {
            Sprites_GlobalObstacles = new Dictionary<ObstacleID, Texture2D>(11);

            Sprites_GlobalObstacles.Add(ObstacleID.None, Sprite_Invisible);
            Sprites_GlobalObstacles.Add(ObstacleID.Hole, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Hole"));
            Sprites_GlobalObstacles.Add(ObstacleID.Destructable0, Sprite_Invisible);
            Sprites_GlobalObstacles.Add(ObstacleID.Destructable1, Sprite_Invisible);
            Sprites_GlobalObstacles.Add(ObstacleID.Destructable2, Sprite_Invisible);
            Sprites_GlobalObstacles.Add(ObstacleID.Destructable3, Sprite_Invisible);
            Sprites_GlobalObstacles.Add(ObstacleID.Destructable4, Sprite_Invisible);
            Sprites_GlobalObstacles.Add(ObstacleID.Destructable5, Sprite_Invisible);
            Sprites_GlobalObstacles.Add(ObstacleID.Wall, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Wall0"));



            Sprites_Obstacles = new Dictionary<LevelID, Dictionary<ObstacleID, Texture2D>>(15);

            Sprites_Obstacles.Add(LevelID.QuarantineLevel, new Dictionary<ObstacleID, Texture2D>(9));
            Sprites_Obstacles[LevelID.QuarantineLevel].Add(ObstacleID.Rock0, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Rock0"));
            Sprites_Obstacles[LevelID.QuarantineLevel].Add(ObstacleID.Rock1, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Rock1"));
            Sprites_Obstacles[LevelID.QuarantineLevel].Add(ObstacleID.Rock2, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Rock2"));
            Sprites_Obstacles[LevelID.QuarantineLevel].Add(ObstacleID.Rock3, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Rock0"));
            Sprites_Obstacles[LevelID.QuarantineLevel].Add(ObstacleID.Rock4, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Rock0"));
            Sprites_Obstacles[LevelID.QuarantineLevel].Add(ObstacleID.Rock5, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Rock0"));
            Sprites_Obstacles[LevelID.QuarantineLevel].Add(ObstacleID.Damage0, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Damage0"));
            Sprites_Obstacles[LevelID.QuarantineLevel].Add(ObstacleID.Damage1, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Damage0"));
            Sprites_Obstacles[LevelID.QuarantineLevel].Add(ObstacleID.Damage2, content.Load<Texture2D>("Textures/Level/Obstacles/QuarantineLevel/Damage0"));


        }
    }
}
