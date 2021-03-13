// GameManager.cs - Nick Monaco

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scenes;
using Microsoft.Xna.Framework.Audio;
using GameProject.Code.Scripts.Components.UI;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Holds values and methods commonly needed across the game.
    /// </summary>
    public static class GameManager {

        #region Overarching Game Stuff
        private static MainGame _mainGame;
        private static int _curSceneID = -1;
        public static Scene CurrentScene => _mainGame.SceneList[_curSceneID];

        public static bool UsingKeyboardControls = false;
        #endregion

        #region Camera
        public static Viewport Viewport => _mainGame.GraphicsDevice.Viewport;
        public static Point Resolution { get { return _mainGame.Window.ClientBounds.Size; } }
        public static Matrix ProjectionMatrix => MainCamera.ProjectionMatrix;
        public static Matrix ViewMatrix => MainCamera.ViewMatrix;
        public static Vector3 ViewOffset => new Vector3(Resolution.X / 2, Resolution.Y / 2, 0) / (MainCamera.Size * 2);
        #endregion

        #region Menu Handling
        public static List<UI_LayoutItem> UILayoutMembers;

        private static int _curUIIndex = 0;
        public static int CurrentUIIndex {
            get => _curUIIndex;
            set {
                _curUIIndex = value;
                OnSelectIndexChange(value);
            }
        }

        public static Action<int> OnSelectIndexChange = (newIndex) => { };
        #endregion

        #region Game World
        public static Camera MainCamera;
        public static Canvas MainCanvas;
        public static Random WorldRandom;
        public static Random DeltaRandom;

        public static PlayerController Player;
        public static Transform PlayerTransform => Player.transform;
        public static MapManager Map;
        public static MinimapController Minimap;
        public static InventoryTracker Inventory;

        public static Transform BulletHolder;

        public static LevelID CurLevelID { get; set; }
        #endregion


        #region Sound & Music
        private static float _masterVolume = 1;
        private static float _soundVolume = 1;
        private static float _musicVolume = 1;
        public static float MasterVolume {
            get => _masterVolume;
            set { 
                _masterVolume = MathHelper.Clamp(value, 0, 1);
                RealSoundVolume = _soundVolume * _masterVolume;
            }
        }
        public static float SoundVolume {
            get => _soundVolume;
            set { 
                _soundVolume = MathHelper.Clamp(value, 0, 1);
                RealSoundVolume = _soundVolume * _masterVolume;
            }
        }
        public static float MusicVolume {
            get => _musicVolume;
            set { _musicVolume = MathHelper.Clamp(value, 0, 1); }
        }

        public static float RealSoundVolume = 1;

        public static SoundEffectInstance FloorSong = null;         // This is what plays by default
        public static SoundEffectInstance CurRoomSong = null;       // This overrides the default floor song, if applicable
        public static SoundEffectInstance OverlayRoomSong = null;   // This overrides the current room song until the event is triggered that disables it
        #endregion




        #region Initialization Methods
        public static void SetMainGame(MainGame game) {
            _mainGame = game;
        }

        public static void InitInternalValues() {
            UILayoutMembers = new List<UI_LayoutItem>();
        }

        public static void SetLayerRules() {
            CollisionMatrix.SetLayerIgnore(LayerID.Player, LayerID.Bullet_Good);

            CollisionMatrix.SetLayerIgnore(LayerID.Enemy, LayerID.Pickup);
            CollisionMatrix.SetLayerIgnore(LayerID.Enemy, LayerID.Item);
            CollisionMatrix.SetLayerIgnore(LayerID.Enemy, LayerID.Bullet_Evil);
            CollisionMatrix.SetLayerIgnore(LayerID.Enemy, LayerID.ShopItem);

            CollisionMatrix.SetLayerIgnore(LayerID.Enemy_Flying, LayerID.Pickup);
            CollisionMatrix.SetLayerIgnore(LayerID.Enemy_Flying, LayerID.Item);
            CollisionMatrix.SetLayerIgnore(LayerID.Enemy_Flying, LayerID.Bullet_Evil);
            CollisionMatrix.SetLayerIgnore(LayerID.Enemy_Flying, LayerID.ShopItem);
            CollisionMatrix.SetLayerIgnore(LayerID.Enemy_Flying, LayerID.Wall);
            CollisionMatrix.SetLayerIgnore(LayerID.Enemy_Flying, LayerID.Obstacle);
            CollisionMatrix.SetLayerIgnore(LayerID.Enemy_Flying, LayerID.Hole);

            CollisionMatrix.SetLayerIgnore(LayerID.Pickup, LayerID.Item);
            CollisionMatrix.SetLayerIgnore(LayerID.Pickup, LayerID.Bullet_Good);
            CollisionMatrix.SetLayerIgnore(LayerID.Pickup, LayerID.Bullet_Evil);
            CollisionMatrix.SetLayerIgnore(LayerID.Pickup, LayerID.ShopItem);

            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.Item);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.Wall);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.EdgeWall);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.Door);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.Bullet_Good);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.Bullet_Evil);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.Familiar);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.Obstacle);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.Hole);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.ShopItem);
            CollisionMatrix.SetLayerIgnore(LayerID.Item, LayerID.Special);

            CollisionMatrix.SetLayerIgnore(LayerID.Door, LayerID.Door);
            CollisionMatrix.SetLayerIgnore(LayerID.Door, LayerID.Wall);
            CollisionMatrix.SetLayerIgnore(LayerID.Door, LayerID.EdgeWall);
            CollisionMatrix.SetLayerIgnore(LayerID.Door, LayerID.Obstacle);
            CollisionMatrix.SetLayerIgnore(LayerID.Door, LayerID.Hole);
            CollisionMatrix.SetLayerIgnore(LayerID.Door, LayerID.ShopItem);

            CollisionMatrix.SetLayerIgnore(LayerID.Familiar, LayerID.Familiar);
            CollisionMatrix.SetLayerIgnore(LayerID.Familiar, LayerID.Wall);
            CollisionMatrix.SetLayerIgnore(LayerID.Familiar, LayerID.Bullet_Good);
            CollisionMatrix.SetLayerIgnore(LayerID.Familiar, LayerID.Familiar);
            CollisionMatrix.SetLayerIgnore(LayerID.Familiar, LayerID.Hole);
            CollisionMatrix.SetLayerIgnore(LayerID.Familiar, LayerID.ShopItem);

            CollisionMatrix.SetLayerIgnore(LayerID.Bullet_Good, LayerID.Bullet_Good);
            CollisionMatrix.SetLayerIgnore(LayerID.Bullet_Good, LayerID.Hole);
            CollisionMatrix.SetLayerIgnore(LayerID.Bullet_Good, LayerID.ShopItem);

            CollisionMatrix.SetLayerIgnore(LayerID.Bullet_Evil, LayerID.Bullet_Evil);
            CollisionMatrix.SetLayerIgnore(LayerID.Bullet_Evil, LayerID.Hole);
            CollisionMatrix.SetLayerIgnore(LayerID.Bullet_Evil, LayerID.ShopItem);

            CollisionMatrix.SetLayerIgnore(LayerID.ShopItem, LayerID.ShopItem);
            CollisionMatrix.SetLayerIgnore(LayerID.ShopItem, LayerID.Wall);
            CollisionMatrix.SetLayerIgnore(LayerID.ShopItem, LayerID.EdgeWall);
            CollisionMatrix.SetLayerIgnore(LayerID.ShopItem, LayerID.Obstacle);
            CollisionMatrix.SetLayerIgnore(LayerID.ShopItem, LayerID.Hole);
            CollisionMatrix.SetLayerIgnore(LayerID.ShopItem, LayerID.Special);

            CollisionMatrix.SetLayerIgnore(LayerID.Wall, LayerID.EdgeWall);
            CollisionMatrix.SetLayerIgnore(LayerID.Wall, LayerID.Obstacle);
            CollisionMatrix.SetLayerIgnore(LayerID.Wall, LayerID.Hole);

            CollisionMatrix.SetLayerIgnore(LayerID.EdgeWall, LayerID.Obstacle);
            CollisionMatrix.SetLayerIgnore(LayerID.EdgeWall, LayerID.Hole);

            CollisionMatrix.SetLayerIgnore(LayerID.Obstacle, LayerID.Hole);
        }
        #endregion


        #region Gameplay-related Methods
        public static void SwitchScene(int newSceneID) {
            OnSelectIndexChange = (newIndex) => { };
            CurrentUIIndex = 0;
            UILayoutMembers.Clear();

            if (_curSceneID == newSceneID) return;

            if (_curSceneID != -1 && CurrentScene != null) {
                Scene.UnloadScene(CurrentScene);
            }

            _curSceneID = newSceneID;

            Scene.LoadScene(CurrentScene, _mainGame.Content);
        }

        public static void Die() {
            if(CurrentScene is GameScene scene) {
                scene.Die();
            }
        }

        public static void ExitGame() {
            _mainGame.Exit();
        }
        #endregion



        #region Music and Sound methods
        public static void SetFloorSong(LevelID level) {
            if(FloorSong != null) {
                FloorSong.Stop();
                FloorSong.Dispose();
            }

            FloorSong = Resources.Music_QuarantineLevel.CreateInstance();
            FloorSong.IsLooped = true;
            FloorSong.Volume = _musicVolume * _masterVolume;
            FloorSong.Play();
        }

        public static void StopFloorSong() {
            FloorSong.Stop();
        }

        public static void ActivateRoomSong(SoundEffect roomSong) {
            CurRoomSong = roomSong.CreateInstance();
            CurRoomSong.IsLooped = true;
            CurRoomSong.Volume = _musicVolume * _masterVolume;
            CurRoomSong.Play();

            FloorSong.Volume = 0;
        }

        public static void DeactivateRoomSong() {
            if (CurRoomSong == null) return;

            CurRoomSong.Stop();
            CurRoomSong.Dispose();

            FloorSong.Volume = _musicVolume * _masterVolume;
        }
        #endregion

    }
}
