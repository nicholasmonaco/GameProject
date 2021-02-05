using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class Room : Component {
        public Room(GameObject attached) : base(attached) { }

        //epic music riff is the background of Division Ruine

        // Each room has:
        //  Texture ID
        //  Corner texture ids
        //  Doors/door fillers
        //  Hole/Obstacle layout
        //  Enemies
        //  Entities
        //  Prespawned Pickups
        //  Loot pickups
        //  Entered
        //  Beaten
        //  RevealedOnMinimap
        //  MinimapIcon


        public Point GridPos;

        public LevelID TextureID = 0;
        public int[] CornerTextureIDs;
        public bool[] HasDoor;
        public Dictionary<Direction, DoorController> Doors;
        public Dictionary<ObstacleID, Vector2[]> ObstaclePositions;
        //public List<Enemy> Enemies;
        //public List<Entity> Entities; //this includes pickups that are prespawned
        //public List<Pickups> Loot;
        public bool Entered = false;
        public bool Beaten = true;
        public bool RevealedOnMinimap = false;
        public RoomType RoomType = RoomType.Normal;



        public void GenerateRoom() {
            for (int x = -1; x < 2; x += 2) {
                for (int y = -1; y < 2; y += 2) {
                    GameObject wallCorner = Instantiate<GameObject>();
                    wallCorner.transform.Parent = transform;
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
                                                          new Vector2(-110, -58) };
                    wallCorner._components.Add(new PolygonCollider2D(wallCorner, cornerPolygonPoints, false));

                }
            }

            for (int i = 0; i < 4; i++) {
                Direction dir = (Direction)i;
                float rotation = 0;
                Vector3 pos = new Vector3(206, 128, 0);
                Point gridDir;
                switch (dir) {
                    default:
                    case Direction.Up:
                        pos.X = 0;
                        gridDir = new Point(0, 1);
                        break;
                    case Direction.Down:
                        pos.X = 0;
                        pos.Y *= -1;
                        rotation = 180;
                        gridDir = new Point(0, -1);
                        break;
                    case Direction.Left:
                        pos.Y = 0;
                        rotation = 270;
                        gridDir = new Point(-1, 0);
                        break;
                    case Direction.Right:
                        pos.Y = 0;
                        pos.X *= -1;
                        rotation = 90;
                        gridDir = new Point(1, 0);
                        break;
                }

                // Make sure to not make a door if there is literally no room in that direction at all.
                if (!GameManager.Map.RoomInDirection(gridDir.InvertPoint())) continue;

                // Generate door if relevant
                GameObject door = Instantiate<GameObject>();
                door.transform.Parent = transform;
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

                DoorController dc = door.AddComponent<DoorController>();
                dc.DoorDirection = dir;
            }
        }


    }
}