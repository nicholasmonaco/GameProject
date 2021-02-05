using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Prefabs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class MapManager : Component {
        public MapManager(GameObject attached) : base(attached) {
            GameManager.Map = this;
        }


        public static Vector2 RoomSize = new Vector2(512, 216); //idk if this is right, check later

        public Dictionary<Point, Room> RoomGrid;
        public Room CurrentRoom;
        public Point CurrentGridPos;

        public Point GridPos_StartingRoom { get; private set; }
        public Point GridPos_BossRoom { get; private set; }
        public Point GridPos_ItemRoom { get; private set; }
        public Point GridPos_ShopRoom { get; private set; }
        public Point GridPos_SecretRoom { get; private set; }
        public Point GridPos_TechRoom { get; private set; }
        public Point GridPos_MagicRoom { get; private set; }




        public void GenerateLevel(LevelID level) {
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


            RoomGrid = new Dictionary<Point, Room>() { 
                { new Point(0, 0), null},
                { new Point(1, 0), null}
            };


            // Define map generation values
            int minRooms = 5 /*+ GetRandomRoomsForFloor()*/;
            int maxRooms = 5;
            
            int roomsToGenerate = 10; //arbitrary value -

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
            //


            // Generate level

            Dictionary<Point, Room> generatedGrid = new Dictionary<Point, Room>(roomsToGenerate);

            foreach (Point gridPoint in RoomGrid.Keys) {
                Room room = Instantiate<Prefab_Room>(new Vector3(gridPoint.X * RoomSize.X, gridPoint.Y * RoomSize.Y, 0), transform).GetComponent<Room>();
                room.GridPos = gridPoint;

                //todo: set room data from read in data

                if(gridPoint == Point.Zero) GridPos_StartingRoom = room.GridPos; //DEBUG - REMOVE WHEN LOADING DATA IS IMPLEMENTED

                room.GenerateRoom();

                generatedGrid[gridPoint] = room;
            }

            RoomGrid = generatedGrid;

            CurrentGridPos = GridPos_StartingRoom;
            CurrentRoom = RoomGrid[CurrentGridPos];
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

        public bool RoomAtGridCoords(Point gridPos) {
            return RoomGrid.TryGetValue(gridPos, out _);
        }

        public bool RoomInDirection(Point gridAdditive) {
            return RoomGrid.TryGetValue(CurrentGridPos + gridAdditive, out _);
        }

    }
}