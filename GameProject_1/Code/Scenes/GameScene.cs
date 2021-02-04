// GameScene.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Prefabs;
using Microsoft.Xna.Framework;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scenes {
    
    /// <summary>
    /// The scene where the main game occurs.
    /// </summary>
    public class GameScene : Scene {

        //public GameScene() : base() {
            
        //}

        public override void Init() {
            base.Init();

            // This is essentially where there should be a list of default GameObjects in the scene.
            GameObjects.Add(new Prefab_MainCamera());


            for (int x = -1; x < 2; x += 2) {
                for (int y = -1; y < 2; y += 2) {
                    GameObject wallCorner = GameObjects.AddReturn(new GameObject());
                    wallCorner.transform.Position = new Vector3(x * 117, y * 78, 0);
                    wallCorner.transform.Scale = new Vector3(-x, y, 0);
                    SpriteRenderer wallCornerRend = wallCorner.AddComponent<SpriteRenderer>();
                    wallCornerRend.Sprite = Resources.Sprite_RoomCorner_2[GameManager.WorldRandom.Next(0, Resources.Sprite_RoomCorner_2.Length)];
                    wallCornerRend.DrawLayer = DrawLayer.ID["Background"];
                    wallCornerRend.OrderInLayer = 21;

                    Vector2[] cornerPolygonPoints = new Vector2[] { new Vector2(-110, 70),
                                                                    new Vector2(97, 70),
                                                                    new Vector2(97, 55),
                                                                    new Vector2(106, 28),
                                                                    new Vector2(-67, 28) };
                    wallCorner._components.Add(new PolygonCollider2D(wallCorner, cornerPolygonPoints, false));

                    cornerPolygonPoints = new Vector2[] { new Vector2(-110, 70),
                                                          new Vector2(-67, 28),
                                                          new Vector2(-67, -67),
                                                          new Vector2(-92, -58),
                                                          new Vector2(-110, -70) };
                    wallCorner._components.Add(new PolygonCollider2D(wallCorner, cornerPolygonPoints, false));

                }
            }

            for (int i = 0; i < 4; i++) {
                Direction dir = (Direction)i;
                float rotation = 0;
                Vector3 pos = new Vector3(206, 128, 0);
                switch (dir) {
                    default:
                    case Direction.Up:
                        pos.X = 0;
                        break;
                    case Direction.Down:
                        pos.X = 0;
                        pos.Y *= -1;
                        rotation = 180;
                        break;
                    case Direction.Left:
                        pos.Y = 0;
                        rotation = 270;
                        break;
                    case Direction.Right:
                        pos.Y = 0;
                        pos.X *= -1;
                        rotation = 90;
                        break;
                }

                GameObject door = GameObjects.AddReturn(new GameObject());
                door.transform.Position = pos;
                door.transform.Rotation = rotation;
                SpriteRenderer sr = door.AddComponent<SpriteRenderer>();
                sr.Sprite = Resources.Sprite_Door_Normal_Base;
                sr.DrawLayer = DrawLayer.ID["WorldStructs"];
                sr.OrderInLayer = 25;

                sr = door.AddComponent<SpriteRenderer>();
                sr.Sprite = Resources.Sprite_Door_Inside;
                sr.DrawLayer = DrawLayer.ID["WorldStructs"];
                sr.OrderInLayer = 20;

                Vector2[] doorBounds = new Vector2[] { new Vector2(-20, 3.5f), new Vector2(20, 3.5f), new Vector2(10, -24), new Vector2(-10, -24) };
                PolygonCollider2D pc = door._components.AddReturn(new PolygonCollider2D(door, doorBounds, false)) as PolygonCollider2D;
                pc.IsTrigger = true;
            }



            GameObjects.Add(new Prefab_Player());

        }



        //public override void Awake() {
        //    
        //}

        //public override void Start() {
        //    
        //}

        //public override void LoadContent(ContentManager content) {

        //}

        //public override void UnloadContent() {

        //}

        //public override void Update() {

        //}

        //public override void FixedUpdate() {

        //}

        //public override void LateUpdate() {

        //}

        //public override void Draw(SpriteBatch sb) {

        //}
    }
}
