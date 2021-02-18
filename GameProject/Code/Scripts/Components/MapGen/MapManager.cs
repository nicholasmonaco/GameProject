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

            Input.OnReset_Down += OnResetPressed;
            Input.OnReset_Released += OnResetReleased;
        }


        //public static Vector2 RoomSize = new Vector2(512, 216); //idk if this is right, check later
        public static Vector2 RoomSize = new Vector2(468, 312); //idk if this is right, check later

        public Dictionary<Point, Room> RoomGrid = null;
        public Room CurrentRoom { get; private set; }
        public Point CurrentGridPos;
        public bool ChangingRooms = false;

        private bool _resetDown = false;
        private float _resetHoldTimer = 0;
        private const float _resetTime = 2.3f;

        public bool Generated { get; set; } = false;

        public Point GridPos_StartingRoom { get; private set; }
        public Point GridPos_BossRoom { get; private set; }
        public Point GridPos_ItemRoom { get; private set; }
        public Point GridPos_ShopRoom { get; private set; }
        public Point GridPos_SecretRoom { get; private set; }
        public Point GridPos_TechRoom { get; private set; }
        public Point GridPos_MagicRoom { get; private set; }




        public IEnumerator GenerateLevel(LevelID level) {
            yield return null;

            Generated = false;

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
            int minRooms = 7 /*+ GetRandomRoomsForFloor()*/;
            int maxRooms = 10;
            

            // Set values used for map generation for each level
            switch (level) {
                default:
                case LevelID.QuarantineLevel:
                    break;
            }

            GameManager.CurLevelID = level;

            Resources.UnloadRooms(); //maybe this should be called right after the map is generated, idk
            Resources.LoadRooms(level);


            //generate random room layout
            //get number of end rooms 
            //if < 3, restart
            //else, continue
            //fill in special rooms, making boss the farthest path from start
            //boss is not allowed to be adjacent to starting room. if it is, restart.


            #region Main Map Generation

            if(RoomGrid == null) RoomGrid = new Dictionary<Point, Room>();
            bool mapGenned = false;
            bool forceRegen = false;
            int attempts = 1;

            while (!mapGenned) {
                Debug.Log("Generating level: Attempt " + attempts);

                foreach(Room oldRoom in RoomGrid.Values) {
                    Debug.Log($"Destroyed room at {oldRoom.GridPos}");
                    Destroy(oldRoom.gameObject);
                }
                RoomGrid.Clear();
                //^^This is somehow leaving some part of the old map in place. stop it from doing that.

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
                        if(validDoors.Count <= 1) {
                            forceRegen = true;
                        } else {
                            validDoors.Remove(newDir);
                            Debug.Log($"MAP GENERATION ERROR SOURCE | {validDoors.Count}");
                            //this is where the crash is coming from.
                            if (validDoors.Count < 2) {
                                refix = true;
                            }
                        }
                        continue;
                    }

                    directionIntoBaseRoom = newDir.InvertDirection();

                    Room newRoom = Instantiate<Prefab_Room>(new Vector3(newPoint.X * RoomSize.X, newPoint.Y * RoomSize.Y, 0), transform).GetComponent<Room>();
                    newRoom.GridPos = newPoint;
                    newRoom.RoomType = RoomType.Normal;
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
            CurrentRoom.RoomType = RoomType.Starting;

            List<(RoomType, RoomStyle, DoorType)> neededRooms = new List<(RoomType, RoomStyle, DoorType)>() {
                (RoomType.Item, RoomStyle.Item, DoorType.Item),
                (RoomType.Boss, RoomStyle.QuarantineLevel_01, DoorType.Boss)
                // Boss room should always be last here, as it always will get first dibs.
            };

            foreach(Point pos in RoomGrid.Keys) {
                Room room = RoomGrid[pos];
                room.DeleteUnconnectedDoors();
                //RoomGrid[pos].GenerateRoomLayout();

                if(neededRooms.Count > 0 && room.Doors.Count == 1 && room.RoomType == RoomType.Normal && DoesRoomHaveExactlyOneNeighbor(room.GridPos)) {
                    int index = neededRooms.Count - 1;
                    room.ResetType(neededRooms[index].Item1, neededRooms[index].Item2, neededRooms[index].Item3);
                    //Debug.Log($"Created {neededRooms[index].Item1} room at {pos}");
                    neededRooms.RemoveAt(index);
                }
            }

            // If the needed rooms aren't all set, generate some extras to fix it
            foreach((RoomType, RoomStyle, DoorType) needData in neededRooms) {
                //Room targetRoom = FindNormalRoomWithout4Neighbors();

                List<Direction> potentialSpawnDirs;
                Room targetRoom = FindNormalRoomWithout4Neighbors_Modified(out potentialSpawnDirs);

                if (targetRoom == null) {
                    yield break; // Exits the coroutine without setting Generated to true, so it will run the coroutine again.
                }

                //List<Direction> potentialSpawnDirs = GetAvailableNeighborPositions(targetRoom.GridPos);
                Direction createDirection = potentialSpawnDirs[GameManager.WorldRandom.Next(0, potentialSpawnDirs.Count)];

                Point newPoint = targetRoom.GridPos + createDirection.GetDirectionPoint();
                Room newRoom = Instantiate<Prefab_Room>(new Vector3(newPoint.X * RoomSize.X, newPoint.Y * RoomSize.Y, 0), transform).GetComponent<Room>();
                newRoom.GridPos = newPoint;
                newRoom.GenerateRoom();
                newRoom.SetDoor(createDirection.InvertDirection(), true);
                newRoom.FillEmptyDoorSlots();
                targetRoom.SetDoor(createDirection, true);
                newRoom.ResetType(needData.Item1, needData.Item2, needData.Item3);

                RoomGrid.Add(newPoint, newRoom);
            }


            foreach (Point pos in RoomGrid.Keys) {
                RoomGrid[pos].LoadLayout();

                if (pos != CurrentGridPos) UnloadRoom(RoomGrid[pos]);
            }

            Generated = true;
            Debug.Log("Map generated.");

            #endregion
        }



        private Room FindNormalRoomWithout4Neighbors() {
            foreach(Room room in RoomGrid.Values) {
                if (room.RoomType == RoomType.Normal && 
                    !(RoomGrid.ContainsKey(room.GridPos + Direction.Up.GetDirectionPoint()) &&
                      RoomGrid.ContainsKey(room.GridPos + Direction.Down.GetDirectionPoint()) &&
                      RoomGrid.ContainsKey(room.GridPos + Direction.Left.GetDirectionPoint()) &&
                      RoomGrid.ContainsKey(room.GridPos + Direction.Right.GetDirectionPoint()))) return room;
            }
            return null;
        }

        private Room FindNormalRoomWithout4Neighbors_Modified(out List<Direction> availableSingleNeighborDirections) {
            availableSingleNeighborDirections = null;

            foreach (Room room in RoomGrid.Values) {
                // If the room is normal, we can generate off of it
                if(room.RoomType == RoomType.Normal) {

                    // Does the room have any potential rooms it can spawn that will only be neightboring this room?
                    availableSingleNeighborDirections = GetAvailableNeighborPositionsWithOneAdjacent(room.GridPos);
                    // If so, we did it.
                    if (availableSingleNeighborDirections.Count > 0) return room;


                    //bool up = !RoomGrid.ContainsKey(room.GridPos + Direction.Up.GetDirectionPoint()); //There is no neighbor up
                    //if (up) {
                        
                    //}

                    //bool down = !RoomGrid.ContainsKey(room.GridPos + Direction.Down.GetDirectionPoint());
                    //if (down) {
                    //    availableSingleNeighborDirections = GetAvailableNeighborPositionsWithOneAdjacent(room.GridPos + Direction.Down.GetDirectionPoint());
                    //    if (availableSingleNeighborDirections != null) return room;
                    //}

                    //bool left = !RoomGrid.ContainsKey(room.GridPos + Direction.Left.GetDirectionPoint());
                    //if (left) {
                    //    availableSingleNeighborDirections = GetAvailableNeighborPositionsWithOneAdjacent(room.GridPos + Direction.Left.GetDirectionPoint());
                    //    if (availableSingleNeighborDirections != null) return room;
                    //}

                    //bool right = !RoomGrid.ContainsKey(room.GridPos + Direction.Right.GetDirectionPoint());
                    //if (right) {
                    //    availableSingleNeighborDirections = GetAvailableNeighborPositionsWithOneAdjacent(room.GridPos + Direction.Right.GetDirectionPoint());
                    //    if (availableSingleNeighborDirections != null) return room;
                    //}
                }
            }
            return null;
        }

        private List<Direction> GetAvailableNeighborPositions(Point roomPos) {
            List<Direction> possibleDirs = new List<Direction>(4) { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            List<Direction> emptyDirs = new List<Direction>(3);

            for(int i=0;i<possibleDirs.Count;i++) {
                if (!RoomGrid.ContainsKey(roomPos + possibleDirs[i].GetDirectionPoint())) {
                    emptyDirs.Add(possibleDirs[i]);
                }
            }
            return emptyDirs;
        }

        private List<Direction> GetAvailableNeighborPositionsWithOneAdjacent(Point roomPos) {
            List<Direction> possibleDirs = new List<Direction>(4) { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            List<Direction> emptyDirs = new List<Direction>(3);

            for (int i = 0; i < possibleDirs.Count; i++) {
                Point potential = roomPos + possibleDirs[i].GetDirectionPoint();
                if (!RoomGrid.ContainsKey(potential) && DoesRoomHaveExactlyOneNeighbor(potential)) {
                    emptyDirs.Add(possibleDirs[i]);
                }
            }
            return emptyDirs;
        }

        private bool DoesRoomHaveExactlyOneNeighbor(Point roomPos) {
            int neighbors = 0;

            neighbors += RoomGrid.ContainsKey(roomPos + Direction.Up.GetDirectionPoint()) ? 1 : 0;
            neighbors += RoomGrid.ContainsKey(roomPos + Direction.Down.GetDirectionPoint()) ? 1 : 0;
            neighbors += RoomGrid.ContainsKey(roomPos + Direction.Left.GetDirectionPoint()) ? 1 : 0;
            neighbors += RoomGrid.ContainsKey(roomPos + Direction.Right.GetDirectionPoint()) ? 1 : 0;

            return neighbors == 1;
        }




        public override void Update() {
            ReloadMap();
        }

        private void ReloadMap() {
            if (_resetDown) {
                _resetHoldTimer += Time.deltaTime;
                if (_resetHoldTimer >= _resetTime) {
                    GameManager.CurrentScene.ResetScene();
                    _resetHoldTimer = 0;
                }
            }else if (_resetHoldTimer != 0) {
                _resetHoldTimer = 0;
            }
        }

        private void OnResetPressed() {
            _resetDown = true;
        }

        private void OnResetReleased() {
            _resetDown = false;
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