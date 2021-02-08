using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Prefabs;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class MapManager : Component {
        public MapManager(GameObject attached) : base(attached) {
            GameManager.Map = this;
        }


        //public static Vector2 RoomSize = new Vector2(512, 216); //idk if this is right, check later
        public static Vector2 RoomSize = new Vector2(468, 312); //idk if this is right, check later

        public Dictionary<Point, Room> RoomGrid;
        public Room CurrentRoom { get; private set; }
        public Point CurrentGridPos;
        public bool ChangingRooms = false;

        public Point GridPos_StartingRoom { get; private set; }
        public Point GridPos_BossRoom { get; private set; }
        public Point GridPos_ItemRoom { get; private set; }
        public Point GridPos_ShopRoom { get; private set; }
        public Point GridPos_SecretRoom { get; private set; }
        public Point GridPos_TechRoom { get; private set; }
        public Point GridPos_MagicRoom { get; private set; }




        public IEnumerator GenerateLevel(LevelID level) {
            yield return null;

            // All levels MUST have:
            //  Empty starting room
            //  Boss room
            //  Item room
            //  Shop room
            //  Secret room
            //  Techno Dealer room (not connected)
            //  Magical Blessing room (not connected)

            // All levels CAN have:
            //  Challenge room
            //  Super secret room
            //  Casino room
            //  Mini-Boss room



            // Define map generation values
            int minRooms = 5 /*+ GetRandomRoomsForFloor()*/;
            int maxRooms = 10;
            

            // Set values used for map generation for each level
            switch (level) {
                default:
                case LevelID.QuarantineLevel:
                    break;
            }



            //generate random room layout
            //get number of end rooms 
            //if < 3, restart
            //else, continue
            //fill in special rooms, making boss the farthest path from start
            //boss is not allowed to be adjacent to starting room. if it is, restart.


            #region Main Map Generation

            RoomGrid = new Dictionary<Point, Room>();
            bool mapGenned = false;
            bool forceRegen = false;
            int attempts = 1;

            while (!mapGenned) {
                Debug.Log("Generating level: Attempt " + attempts);

                RoomGrid.Clear();

                int targetRoomCount = GameManager.WorldRandom.Next(minRooms, maxRooms + 1);
                int curRoomCount = 1;
                Point lastGridPointBase = new Point(0, 0); // Room generation always starts at 0,0

                // Generate starting room
                Room startingRoom = Instantiate<Prefab_Room>(Vector3.Zero, transform).GetComponent<Room>();
                startingRoom.GridPos = lastGridPointBase;
                RoomGrid.Add(lastGridPointBase, startingRoom);
                startingRoom.GenerateRoom();

                // Always have the starting room have two doorways
                startingRoom.ForceSetDoors(2);
                Debug.Log($"Starting room has {startingRoom.Doors.Keys.Count} doors");

                // Initialize the room that the next room will be spawned from
                Room baseRoom = startingRoom;
                Direction directionIntoBaseRoom = Direction.None;

                List<Direction> validDoors = new List<Direction>(4);
                HashSet<Point> invalidPositions = new HashSet<Point>();

                int noValidDoorCount = 0;
                bool refix = false;

                // Until there is a sufficient amount of rooms
                while (curRoomCount < targetRoomCount && !forceRegen) {

                    

                    // Reload valid doors if needed
                    if (validDoors.Count == 0) {
                        Debug.Log("no valid doors");
                        foreach (Direction d in baseRoom.Doors.Keys) {
                            if (d != directionIntoBaseRoom) {
                                validDoors.Add(d);
                            }
                        }

                        if(validDoors.Count == 0 || refix) {
                            noValidDoorCount++;
                            if(noValidDoorCount >= 2) {
                                //pick another room to generate from
                                baseRoom = RoomGrid[lastGridPointBase - directionIntoBaseRoom.InvertDirection().GetDirectionPoint()];
                                while (!baseRoom.CanGenerateInAnyDirection()) {
                                    invalidPositions.Add(baseRoom.GridPos);

                                    if(invalidPositions.Count == RoomGrid.Keys.Count) {
                                        forceRegen = true;
                                        break;
                                    }

                                    foreach(Point p in RoomGrid.Keys) {
                                        if (!invalidPositions.Contains(p)) {
                                            baseRoom = RoomGrid[p];
                                            refix = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        } else {
                            noValidDoorCount = 0;
                        }

                        if (forceRegen) break;
                        else continue;
                    }

                    Debug.Log("valid doors was not 0");

                    // Pick a random door
                    Direction newDir = validDoors[GameManager.WorldRandom.Next(0, validDoors.Count)];
                    Point dirAdditive = newDir.GetDirectionPoint();

                    // In that direction, create a new room that has (at least) the door of the opposite direction
                    Point newPoint = lastGridPointBase + dirAdditive;
                    if (RoomGrid.ContainsKey(newPoint)) {
                        validDoors.Remove(newDir);
                        Debug.Log($"MAP GENERATION ERROR SOURCE | {validDoors.Count}");
                        //this is where the crash is coming from.
                        if(validDoors.Count < 2) {
                            refix = true;
                        }
                        continue;
                    }

                    directionIntoBaseRoom = newDir.InvertDirection();

                    Room newRoom = Instantiate<Prefab_Room>(new Vector3(newPoint.X * RoomSize.X, newPoint.Y * RoomSize.Y, 0), transform).GetComponent<Room>();
                    newRoom.GridPos = newPoint;
                    newRoom.GenerateRoom();
                    //baseRoom.SetDoor(newDir, true);
                    newRoom.SetDoor(newDir.InvertDirection(), true);
                    newRoom.GenerateExtraDoors();

                    RoomGrid.Add(newPoint, newRoom);

                    curRoomCount++;

                    if(newRoom.Doors.Count > 1) {
                        baseRoom = newRoom;
                        lastGridPointBase += dirAdditive;
                        validDoors.Clear();
                    } else {
                        validDoors.Remove(newDir);
                    }

                    Debug.Log($"Generated room at ({newPoint.X}, {newPoint.Y})");
                    yield return null;
                }

                if (!forceRegen) mapGenned = true;
                else {
                    forceRegen = false;
                    attempts++;
                }
            }

            CurrentGridPos = new Point(0, 0);
            CurrentRoom = RoomGrid[CurrentGridPos];

            foreach(Point pos in RoomGrid.Keys) {
                RoomGrid[pos].DeleteUnconnectedDoors();
                //RoomGrid[pos].GenerateRoomLayout();
            }

            foreach (Point pos in RoomGrid.Keys) {
                if(pos != CurrentGridPos) UnloadRoom(RoomGrid[pos]);
            }

            Debug.Log("Map generated.");

            #endregion
        }

        public Room LoadRoom(Point gridPoint) {
            Room room = RoomGrid[gridPoint];
            room.gameObject.Enabled = true;
            return room;
        }

        public void UnloadRoom(Point gridPoint) {
            Room room = RoomGrid[gridPoint];
            room.gameObject.Enabled = false;
        }

        public void UnloadRoom(Room room) {
            room.gameObject.Enabled = false;
        }

        public void UnloadCurrentRoom() {
            CurrentRoom.gameObject.Enabled = false;
        }

        public void SetCurrentRoom(Point gridPos) {
            CurrentGridPos = gridPos;
            CurrentRoom = RoomGrid[gridPos];
        }

        public void SetCurrentRoomInDirection(Point additive) {
            CurrentGridPos += additive;
            CurrentRoom = RoomGrid[CurrentGridPos];
        }

        public bool RoomAtGridCoords(Point gridPos) {
            return RoomGrid.TryGetValue(gridPos, out _);
        }

        public bool RoomInDirection(Point gridAdditive) {
            return RoomGrid.TryGetValue(CurrentGridPos + gridAdditive, out _);
        }

    }
}