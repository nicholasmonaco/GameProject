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
        #endregion

        #region Camera
        public static Viewport Viewport => _mainGame.GraphicsDevice.Viewport;
        public static Point Resolution { get { return _mainGame.Window.ClientBounds.Size; } }
        public static Matrix ProjectionMatrix => MainCamera.ProjectionMatrix;
        public static Matrix ViewMatrix => MainCamera.ViewMatrix;
        public static Vector3 ViewOffset => new Vector3(Resolution.X / 2, Resolution.Y / 2, 0) / (MainCamera.Size * 2);
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

        public static LevelID CurLevelID { get; set; }
        #endregion

        #region Sound & Music
        private static float _masterVolume = 1;
        private static float _soundVolume = 1;
        private static float _musicVolume = 1;
        public static float MasterVolume {
            get => _masterVolume;
            set { _masterVolume = MathHelper.Clamp(value, 0, 1); }
        }
        public static float SoundVolume {
            get => _soundVolume;
            set { _soundVolume = MathHelper.Clamp(value, 0, 1); }
        }
        public static float MusicVolume {
            get => _musicVolume;
            set { _musicVolume = MathHelper.Clamp(value, 0, 1); }
        }

        public static SoundEffectInstance FloorSong = null;         // This is what plays by default
        public static SoundEffectInstance CurRoomSong = null;       // This overrides the default floor song, if applicable
        public static SoundEffectInstance OverlayRoomSong = null;   // This overrides the current room song until the event is triggered that disables it
        #endregion





        public static void SetMainGame(MainGame game) {
            _mainGame = game;
        }

        public static void SwitchScene(int newSceneID) {
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

    }
}
