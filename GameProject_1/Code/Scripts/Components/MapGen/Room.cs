using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            Doors = new Dictionary<Direction, DoorController>(4);

            for (int x = -1; x < 2; x += 2) {
                for (int y = -1; y < 2; y += 2) {
                    GameObject wallCorner = Instantiate<GameObject>();
                    wallCorner.transform.Parent = transform;
                    wallCorner.transform.LocalPosition = new Vector3(x * 117, y * 78, 0);
                    wallCorner.transform.Scale = new Vector3(-x, y, 0);
                    wallCorner.Layer = (int)LayerID.EdgeWall;

                    SpriteRenderer wallCornerRend = wallCorner.AddComponent<SpriteRenderer>();
                    wallCornerRend.Sprite = GetRandomCornerTexture(RoomStyle.QuarantineLevel_01);
                    wallCornerRend.DrawLayer = DrawLayer.ID["Background"];
                    wallCornerRend.OrderInLayer = 21;

                    Vector2[] cornerPolygonPoints = new Vector2[] { new Vector2(-110, 70),
                                                                    new Vector2(97, 70),
                                                                    new Vector2(97, 55),
                                                                    new Vector2(100, 28),
                                                                    new Vector2(-67, 28) };
                    wallCorner._components.Add(new PolygonCollider2D(wallCorner, cornerPolygonPoints, false));

                    cornerPolygonPoints = new Vector2[] { new Vector2(-110, 70),
                                                          new Vector2(-67, 28),
                                                          new Vector2(-67, -61),
                                                          new Vector2(-92, -58),
                                                          new Vector2(-110, -58) };
                    wallCorner._components.Add(new PolygonCollider2D(wallCorner, cornerPolygonPoints, false));

                }
            }

        }

        public void SetDoor(Direction dir, bool isDoor) {
            //Direction dir = (Direction)i;
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
                case Direction.Right:
                    pos.Y = 0;
                    rotation = 270;
                    break;
                case Direction.Left:
                    pos.Y = 0;
                    pos.X *= -1;
                    rotation = 90;
                    break;
            }

            // Make sure to not make a door if there is literally no room in that direction at all.
            //if (GameManager.Map.RoomAtGridCoords(GridPos + gridDir)) {
            if (isDoor) {
                // Generate door if relevant
                GameObject door = Instantiate<GameObject>();
                door.transform.Parent = transform;
                door.transform.LocalPosition = pos;
                door.transform.Rotation = rotation;

                SpriteRenderer sr = door.AddComponent<SpriteRenderer>();
                sr.Sprite = Resources.Sprite_Door_Normal_Base;
                sr.DrawLayer = DrawLayer.ID["WorldStructs"];
                sr.OrderInLayer = 25;

                sr = door.AddComponent<SpriteRenderer>();
                sr.Sprite = Resources.Sprite_Door_Inside;
                sr.DrawLayer = DrawLayer.ID["WorldStructs"];
                sr.OrderInLayer = 20;

                Vector2[] doorBounds = new Vector2[] { new Vector2(-20, 3.5f), new Vector2(20, 3.5f), new Vector2(10, -22), new Vector2(-10, -22) };
                PolygonCollider2D pc = door._components.AddReturn(new PolygonCollider2D(door, doorBounds, false)) as PolygonCollider2D;
                pc.IsTrigger = true;

                DoorController dc = door.AddComponent<DoorController>();
                dc.DoorDirection = dir;
                Doors.Add(dir, dc);
                Debug.Log("new door added: " + dir);

            } else {
                GameObject doorFiller = Instantiate<GameObject>();
                doorFiller.transform.Parent = transform;
                doorFiller.transform.LocalPosition = pos;
                doorFiller.transform.Rotation = rotation;

                Vector2[] doorBounds = new Vector2[] { new Vector2(-20, 3.5f), new Vector2(20, 3.5f), new Vector2(11, -22), new Vector2(-11, -22) };
                PolygonCollider2D pc = doorFiller._components.AddReturn(new PolygonCollider2D(doorFiller, doorBounds, false)) as PolygonCollider2D;
                pc.IsTrigger = false;
            }
        }

        public void GenerateExtraDoors() {
            List<Direction> possible = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            
            foreach(Direction d in Doors.Keys) {
                possible.Remove(d);
            }

            foreach(Direction d in possible) {
                SetDoor(d, GameManager.WorldRandom.Next(0, 2) == 0 ? false : true);
            }
        }

        public void RegenerateFalseDoors() {
            List<Direction> possible = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

            foreach (Direction d in Doors.Keys) {
                if (!possible.Contains(d)) {
                    SetDoor(d, GameManager.WorldRandom.Next(0, 2) == 0 ? false : true);
                }
            }
        }

        public void ForceSetDoors(int doorCount) {
            int leftToGen = doorCount;
            List<Direction> possible = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

            while (leftToGen > 0) {
                int index = GameManager.WorldRandom.Next(0, possible.Count);
                Direction d = possible[index];
                possible.RemoveAt(index);

                SetDoor(d, true);

                leftToGen--;
            }
        }

        public bool CanGenerateInAnyDirection() {
            foreach(Direction dir in Doors.Keys) {
                if(GameManager.Map.RoomGrid.ContainsKey(GridPos + dir.GetDirectionPoint())) {
                    return true;
                }
            }

            return false;
        }

        public void DeleteUnconnectedDoors() {
            foreach(Direction dir in Doors.Keys) {
                Debug.Log($"{GridPos}, {dir} = {GameManager.Map.RoomAtGridCoords(GridPos + dir.GetDirectionPoint())}");

                if (!GameManager.Map.RoomAtGridCoords(GridPos + dir.GetDirectionPoint())) {
                    Destroy(Doors[dir].gameObject);
                    Doors.Remove(dir);
                    //roll to see if we want a connector there if there's also a door in the other room
                }
            }

            HashSet<Direction> possible = new HashSet<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            foreach(Direction dir in possible) {
                if (!Doors.ContainsKey(dir)) {
                    SetDoor(dir, false);
                }
            }
        }


        public static Texture2D GetRandomCornerTexture(RoomStyle style) {
            List<Texture2D> list = Resources.Sprites_RoomCorners[style];
            return list[GameManager.WorldRandom.Next(0, list.Count)];
        }

    }
}