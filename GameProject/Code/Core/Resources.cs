// Resources.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
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
        public static Texture2D Sprite_ControlGuide;

        public static Texture2D Sprite_ParticleDefault;
        public static Texture2D Sprite_ParticleDense;
        public static Texture2D Sprite_ParticleSharp;
        public static Texture2D Sprite_ParticleEx;

        public static Dictionary<RoomStyle, List<Texture2D>> Sprites_RoomCorners;

        public static Texture2D Sprite_Door_Inside;
        public static Texture2D Sprite_Door_Inside_Boss;
        public static Dictionary<DoorType, Texture2D> Sprites_DoorFrames;
        public static List<Texture2D> Sprites_DoorCloseFrames;

        public static List<Texture2D> Sprites_BossDoorEyeAnim;
        public static Texture2D Sprite_BossDoorGlow;


        public static Texture2D Sprite_Bullet_Standard;

        public static Texture2D[] Sprite_UI_Reticles;
        public static Texture2D Sprite_UI_Money;
        public static Texture2D Sprite_UI_Keys;
        public static Texture2D Sprite_UI_Bombs;
        public static Dictionary<HeartContainer, Texture2D> Sprites_HeartContainers;

        public static Dictionary<Pickup, Texture2D> Sprite_Pickups;

        public static Texture2D Sprite_BombExplosion;

        public static Texture2D Sprite_MinimapBackground;
        public static Dictionary<MinimapIcon, Texture2D> Sprite_MinimapIcons;

        public static Dictionary<ObstacleID, Texture2D> Sprites_GlobalObstacles;
        public static Dictionary<LevelID, Dictionary<ObstacleID, Texture2D>> Sprites_Obstacles;

        public static Texture2D Sprite_ItemPedastal;
        public static Texture2D Sprite_Item_Unknown;
        public static Dictionary<ItemID, Texture2D> Sprites_Items;

        public static Dictionary<Direction, List<Texture2D>> Sprites_PlayerMove;

        public static Texture2D Sprite_Vignette;
        public static Texture2D Sprite_DeathMessage;
        public static Texture2D Sprite_WinMessage;

        public static Texture2D Sprite_MM_Background;
        public static Texture2D Sprite_MM_BackgroundGradient;
        public static Texture2D Sprite_MM_Ground;
        public static Texture2D Sprite_MM_Title;
        public static Texture2D Sprite_MM_Prompt;

        public static Dictionary<EntityID, Dictionary<EnemyAnimationAction, List<Texture2D>>> Sprites_EnemyAnimations;

        #region Player Sprites
        // Idle Sprites
        public static Texture2D Sprite_Player_IdleDown_0;
        public static Texture2D Sprite_Player_IdleDown_1;

        public static Texture2D Sprite_Player_IdleUp_0;
        public static Texture2D Sprite_Player_IdleUp_1;

        public static Texture2D Sprite_Player_IdleLeft_0;
        public static Texture2D Sprite_Player_IdleLeft_1;

        public static Texture2D Sprite_Player_IdleRight_0;
        public static Texture2D Sprite_Player_IdleRight_1;

        // Walking Sprites
        public static Texture2D Sprite_Player_WalkDown_0;
        public static Texture2D Sprite_Player_WalkDown_1;
        public static Texture2D Sprite_Player_WalkDown_2;

        public static Texture2D Sprite_Player_WalkUp_0;
        public static Texture2D Sprite_Player_WalkUp_1;
        public static Texture2D Sprite_Player_WalkUp_2;

        public static Texture2D Sprite_Player_WalkLeft_0;
        public static Texture2D Sprite_Player_WalkLeft_1;
        public static Texture2D Sprite_Player_WalkLeft_2;

        public static Texture2D Sprite_Player_WalkRight_0;
        public static Texture2D Sprite_Player_WalkRight_1;
        public static Texture2D Sprite_Player_WalkRight_2;

        // Other Sprites
        public static Texture2D Sprite_Player_ItemPickup;

        public static Texture2D Sprite_Player_Damage;

        public static Texture2D Sprite_Player_Die;
        #endregion

        #region Arm Sprites
        public static Texture2D Sprite_Arm_Outer;
        #endregion


        #endregion


        #region Font Resources
        public static SpriteFont Font_Debug;
        public static SpriteFont Font_Base_Inner;
        public static SpriteFont Font_Base_Outer;
        #endregion

        #region Sound Resources
        public static SoundEffect Sound_PlayerHurt;
        public static SoundEffect Sound_Death;

        public static SoundEffect Sound_CaveChaser;

        public static SoundEffect Sound_Doors_Close;

        public static Dictionary<Pickup, SoundEffect> Sounds_PickupSpawn;
        public static Dictionary<Pickup, SoundEffect> Sounds_PickupCollect;

        public static SoundEffect Sound_Explosion;

        public static SoundEffect Sound_Menu_Move;
        public static SoundEffect Sound_Menu_Next;
        public static SoundEffect Sound_Menu_Back;

        public static SoundEffect Sound_Player_Shoot;

        public static SoundEffect Sound_Punch_Impact;
        public static List<SoundEffect> Sounds_PunchRush;
        #endregion

        #region Music Resources
        public static SoundEffect Music_Store;

        public static SoundEffect Music_QuarantineLevel;

        public static SoundEffect Music_MagmaChambers;

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

        #region Effect Resources (Shaders)
        public static Effect Effect_Base;

        public static Effect Effect_Grayscale;
        public static Effect Effect_Outline;
        #endregion





        public static void LoadContent(ContentManager content) {
            LoadEffects(content);

            LoadTextures(content);

            LoadMusic(content);
            LoadSounds(content);

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



        private static void LoadEffects(ContentManager content) {
            Effect_Base = content.Load<Effect>("Effects/BaseSpriteEffect");

            Effect_Grayscale = content.Load<Effect>("Effects/Grayscale");
            Effect_Outline = content.Load<Effect>("Effects/Outline");
        }



        private static void LoadSounds(ContentManager content) {
            Sound_PlayerHurt = content.Load<SoundEffect>("Sounds/PlayerHurt");
            Sound_Death = content.Load<SoundEffect>("Sounds/Death");

            Sound_Explosion = content.Load<SoundEffect>("Sounds/Explosion_01");


            Sound_CaveChaser = content.Load<SoundEffect>("Sounds/CaveChaser");


            //Sounds_PickupSpawn = new Dictionary<Pickup, SoundEffect>();
            //Sounds_PickupSpawn.Add(Pickup.Heart_Whole, content.Load<SoundEffect>("Sounds/Pickup/Spawn/Heart"));
            //Sounds_PickupSpawn.Add(Pickup.Coin, content.Load<SoundEffect>("Sounds/Pickup/Spawn/Coin"));
            //Sounds_PickupSpawn.Add(Pickup.Key, content.Load<SoundEffect>("Sounds/Pickup/Spawn/Key"));
            //Sounds_PickupSpawn.Add(Pickup.Chest_Free, content.Load<SoundEffect>("Sounds/Pickup/Spawn/Chest"));

            //Sounds_PickupCollect = new Dictionary<Pickup, SoundEffect>();
            //Sounds_PickupSpawn.Add(Pickup.Heart_Whole, content.Load<SoundEffect>("Sounds/Pickup/Collect/Heart"));
            //Sounds_PickupSpawn.Add(Pickup.Coin, content.Load<SoundEffect>("Sounds/Pickup/Collect/Coin"));
            //Sounds_PickupSpawn.Add(Pickup.Key, content.Load<SoundEffect>("Sounds/Pickup/Collect/Key"));
            //Sounds_PickupSpawn.Add(Pickup.Chest_Free, content.Load<SoundEffect>("Sounds/Pickup/Collect/Chest"));
            //Sounds_PickupSpawn.Add(Pickup.PowerCell, content.Load<SoundEffect>("Sounds/Pickup/Collect/PowerCell"));


            Sound_Doors_Close = content.Load<SoundEffect>("Sounds/DoorClose");

            Sound_Menu_Move = content.Load<SoundEffect>("Sounds/UI/MenuMove");
            Sound_Menu_Next = content.Load<SoundEffect>("Sounds/UI/MenuChange");
            Sound_Menu_Back = content.Load<SoundEffect>("Sounds/UI/MenuBack");

            Sound_Player_Shoot = content.Load<SoundEffect>("Sounds/PlayerShoot");

            Sound_Punch_Impact = content.Load<SoundEffect>("Sounds/Combat/Punch_Impact");

            Sounds_PunchRush = new List<SoundEffect>(3); //3 for now
            Sounds_PunchRush.Add(content.Load<SoundEffect>("Sounds/Rush/Ora_01"));
            Sounds_PunchRush.Add(content.Load<SoundEffect>("Sounds/Rush/Ora_01"));
            //Sounds_PunchRush.Add(content.Load<SoundEffect>("Sounds/Rush/Ora_03"));
        }

        private static void LoadMusic(ContentManager content) {
            Music_Store = content.Load<SoundEffect>("Music/Store");

            Music_QuarantineLevel = content.Load<SoundEffect>("Music/QuarantineLevel");
            Music_MagmaChambers = content.Load<SoundEffect>("Music/MagmaChambers");
        }


        private static void LoadTextures(ContentManager content) {
            Sprite_TestSprite = content.Load<Texture2D>("Textures/Misc/Ball");
            Sprite_TestArrowSprite = content.Load<Texture2D>("Textures/Misc/Arrow");
            Sprite_TestSquare = content.Load<Texture2D>("Textures/Misc/Square_01");
            Sprite_Pixel = content.Load<Texture2D>("Textures/Misc/Pixel");
            Sprite_Invisible = content.Load<Texture2D>("Textures/Misc/Invisible");

            Sprite_ParticleDefault = content.Load<Texture2D>("Textures/Misc/Particle_Default");
            Sprite_ParticleDense = content.Load<Texture2D>("Textures/Misc/Particle_Dense");
            Sprite_ParticleSharp = content.Load<Texture2D>("Textures/Misc/Particle_Sharp");
            Sprite_ParticleEx = content.Load<Texture2D>("Textures/Misc/Particle_Ex");

            Sprite_ControlGuide = content.Load<Texture2D>("Textures/Misc/ControlGuide");


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
            Sprite_Door_Inside_Boss = content.Load<Texture2D>("Textures/Level/Door/Door_Inside_Boss");

            Sprites_DoorFrames = new Dictionary<DoorType, Texture2D>(3);
            Sprites_DoorFrames.Add(DoorType.Item, content.Load<Texture2D>("Textures/Level/Door/Door_Item"));
            Sprites_DoorFrames.Add(DoorType.Boss, content.Load<Texture2D>("Textures/Level/Door/Door_Boss"));
            Sprites_DoorFrames.Add(DoorType.Normal, content.Load<Texture2D>("Textures/Level/Door/Door_QuarantineZone_Base"));

            Sprites_DoorCloseFrames = new List<Texture2D>(6);
            Sprites_DoorCloseFrames.Add(content.Load<Texture2D>("Textures/Level/Door/Close/Close_00"));
            Sprites_DoorCloseFrames.Add(content.Load<Texture2D>("Textures/Level/Door/Close/Close_01"));
            Sprites_DoorCloseFrames.Add(content.Load<Texture2D>("Textures/Level/Door/Close/Close_02"));
            Sprites_DoorCloseFrames.Add(content.Load<Texture2D>("Textures/Level/Door/Close/Close_03"));
            Sprites_DoorCloseFrames.Add(content.Load<Texture2D>("Textures/Level/Door/Close/Close_04"));
            Sprites_DoorCloseFrames.Add(content.Load<Texture2D>("Textures/Level/Door/Close/Close_05"));

            Sprites_BossDoorEyeAnim = new List<Texture2D>(6);
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_0"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_1"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_2"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_3"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_4"));
            Sprites_BossDoorEyeAnim.Add(content.Load<Texture2D>("Textures/Level/Door/Boss_Eyes/BossEyes_5"));

            Sprite_BossDoorGlow = content.Load<Texture2D>("Textures/Level/Door/Boss_Glow");


            Sprite_Bullet_Standard = content.Load<Texture2D>("Textures/Bullet/PhotonShot");

            Sprite_UI_Reticles = new Texture2D[1];
            Sprite_UI_Reticles[0] = content.Load<Texture2D>("Textures/UI/Reticle_0");

            Sprite_UI_Money = content.Load<Texture2D>("Textures/UI/Money");
            Sprite_UI_Keys = content.Load<Texture2D>("Textures/UI/Key");
            Sprite_UI_Bombs = content.Load<Texture2D>("Textures/UI/Bomb");

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
            Sprite_Pickups.Add(Pickup.Key, content.Load<Texture2D>("Textures/Pickup/Key"));
            Sprite_Pickups.Add(Pickup.Bomb, content.Load<Texture2D>("Textures/Pickup/Bomb"));

            Sprite_BombExplosion = content.Load<Texture2D>("Textures/Particles/Explosion_Frames");

            Sprite_MinimapBackground = content.Load<Texture2D>("Textures/UI/Minimap/Background");

            Sprite_MinimapIcons = new Dictionary<MinimapIcon, Texture2D>(5); //change when all icons are sprited
            Sprite_MinimapIcons.Add(MinimapIcon.Current, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Current"));
            Sprite_MinimapIcons.Add(MinimapIcon.Explored, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Explored"));
            Sprite_MinimapIcons.Add(MinimapIcon.Unexplored, content.Load<Texture2D>("Textures/UI/Minimap/Minimap_Unexplored"));
            Sprite_MinimapIcons.Add(MinimapIcon.Item, content.Load<Texture2D>("Textures/UI/Minimap/Icon_Item"));
            Sprite_MinimapIcons.Add(MinimapIcon.Boss, content.Load<Texture2D>("Textures/UI/Minimap/Icon_Boss"));

            //item
            Sprite_ItemPedastal = content.Load<Texture2D>("Textures/Entities/Misc/ItemPedastal");
            LoadItemSprites(content);

            LoadObstacleSprites(content);

            LoadMainMenuSprites(content);

            LoadEnemySprites(content);

            //player
            LoadPlayerSprites(content);

            //Sprites_PlayerMove = new Dictionary<Direction, List<Texture2D>>(4);

            //List<Texture2D> upTex = new List<Texture2D>(7);
            //upTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Up_0"));
            //upTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Up_1"));
            //upTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Up_2"));
            //upTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Up_3"));
            //upTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Up_0"));
            //upTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Up_4"));
            //upTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Up_5"));

            //List<Texture2D> downTex = new List<Texture2D>(7);
            //downTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Down_0"));
            //downTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Down_1"));
            //downTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Down_2"));
            //downTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Down_3"));
            //downTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Down_0"));
            //downTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Down_4"));
            //downTex.Add(content.Load<Texture2D>("Textures/Entities/Player/Down_5"));

            //Sprites_PlayerMove.Add(Direction.Up, upTex);
            //Sprites_PlayerMove.Add(Direction.Down, downTex);
            //end player


            Font_Debug = content.Load<SpriteFont>("Fonts/arial");
            Font_Base_Inner = content.Load<SpriteFont>("Fonts/Base_Inner");
            Font_Base_Outer = content.Load<SpriteFont>("Fonts/Base_Outer");


            Debug.Log("Textures loaded.");
        }


        private static void LoadPlayerSprites(ContentManager content) {
            // Idle Sprites
            Sprite_Player_IdleDown_0 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Down_0");
            Sprite_Player_IdleDown_1 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Down_1");

            Sprite_Player_IdleUp_0 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Up_0");
            Sprite_Player_IdleUp_1 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Up_1");

            Sprite_Player_IdleLeft_0 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Left_0");
            Sprite_Player_IdleLeft_1 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Left_1");

            Sprite_Player_IdleRight_0 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Right_0");
            Sprite_Player_IdleRight_1 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Right_1");


            // Walking Sprites
            Sprite_Player_WalkDown_0 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Down_1");
            Sprite_Player_WalkDown_1 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Down_1");
            Sprite_Player_WalkDown_2 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Down_1");

            Sprite_Player_WalkUp_0 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Up_1");
            Sprite_Player_WalkUp_1 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Up_1");
            Sprite_Player_WalkUp_2 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Up_1");

            Sprite_Player_WalkLeft_0 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Left_1");
            Sprite_Player_WalkLeft_1 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Left_1");
            Sprite_Player_WalkLeft_2 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Left_1");

            Sprite_Player_WalkRight_0 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Right_1");
            Sprite_Player_WalkRight_1 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Right_1");
            Sprite_Player_WalkRight_2 = content.Load<Texture2D>("Textures/Entities/Player/Idle/Right_1");


            // Other Sprites
            Sprite_Player_ItemPickup = content.Load<Texture2D>("Textures/Entities/Player/Pickup");
            
            Sprite_Player_Damage = content.Load<Texture2D>("Textures/Entities/Player/Hurt");
            
            Sprite_Player_Die = content.Load<Texture2D>("Textures/Entities/Player/Die");


            // Arms
            Sprite_Arm_Outer = content.Load<Texture2D>("Textures/Entities/Player/Arms/Outer");
        }

        private static void LoadMainMenuSprites(ContentManager content) {
            Sprite_Vignette = content.Load<Texture2D>("Textures/UI/Misc/Vignette");
            Sprite_DeathMessage = content.Load<Texture2D>("Textures/UI/Misc/DeathMessage");
            Sprite_WinMessage = content.Load<Texture2D>("Textures/UI/Misc/WinMessage");

            Sprite_MM_Background = content.Load<Texture2D>("Textures/UI/MainMenu/Sky");
            Sprite_MM_BackgroundGradient = content.Load<Texture2D>("Textures/UI/MainMenu/BackGradient");
            Sprite_MM_Ground = content.Load<Texture2D>("Textures/UI/MainMenu/Ground");
            Sprite_MM_Title = content.Load<Texture2D>("Textures/UI/MainMenu/Title_Backed");
            Sprite_MM_Prompt = content.Load<Texture2D>("Textures/UI/MainMenu/Start-Prompt_Space");
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

        private static void LoadEnemySprites(ContentManager content) {
            Sprites_EnemyAnimations = new Dictionary<EntityID, Dictionary<EnemyAnimationAction, List<Texture2D>>>();

            for(int i = 301; i < 600; i++) {
                string contentlesspath = "Textures/Entities/Enemies/" + ((EntityID)i).ToString() + "/";
                string path = content.RootDirectory + "/" + contentlesspath;

                if (!new DirectoryInfo(path).Exists) continue;

                Sprites_EnemyAnimations[(EntityID)i] = new Dictionary<EnemyAnimationAction, List<Texture2D>>();

                for (int j = 0; j <= 5; j++) {
                    string contentlessAnimPath = contentlesspath + "/" + ((EnemyAnimationAction)j).ToString();
                    string animPath = path + "/" + ((EnemyAnimationAction)j).ToString();
                    
                    DirectoryInfo dir = new DirectoryInfo(animPath);
                    if (!dir.Exists) continue;

                    FileInfo[] files = dir.GetFiles("*.*");
                    List<Texture2D> textures = new List<Texture2D>(files.Length);
                    foreach(FileInfo file in files) {
                        string name = Path.GetFileNameWithoutExtension(file.Name);

                        textures.Add(content.Load<Texture2D>(contentlessAnimPath + "/" + name));
                    }

                    Sprites_EnemyAnimations[(EntityID)i][(EnemyAnimationAction)j] = textures;
                }
            }
        }


        private static void LoadItemSprites(ContentManager content) {
            Sprite_Item_Unknown = content.Load<Texture2D>("Textures/Items/Unknown");

            Sprites_Items = new Dictionary<ItemID, Texture2D>();

            Sprites_Items.Add(ItemID.None, Sprite_Invisible);
            Sprites_Items.Add(ItemID.VitaminH, content.Load<Texture2D>($"Textures/Items/{ItemID.VitaminH.ToString()}"));
            Sprites_Items.Add(ItemID.RationBar, content.Load<Texture2D>($"Textures/Items/{ItemID.RationBar.ToString()}"));
            Sprites_Items.Add(ItemID.Cake, content.Load<Texture2D>($"Textures/Items/{ItemID.Cake.ToString()}"));
            Sprites_Items.Add(ItemID.FocusLens, content.Load<Texture2D>($"Textures/Items/{ItemID.FocusLens.ToString()}"));
            Sprites_Items.Add(ItemID.HabaneroHotSauce, content.Load<Texture2D>($"Textures/Items/{ItemID.HabaneroHotSauce.ToString()}"));
            Sprites_Items.Add(ItemID.FourLeafClover, content.Load<Texture2D>($"Textures/Items/{ItemID.FourLeafClover.ToString()}"));
        }


    }
}
