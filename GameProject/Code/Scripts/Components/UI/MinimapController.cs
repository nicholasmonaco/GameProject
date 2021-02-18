using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Prefabs;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class MinimapController : Component {
        public MinimapController(GameObject attached) : base(attached) {
            GameManager.Minimap = this;
        }


        private MapManager Map;
        public static readonly Point MinimapIconSize = new Point(15, 15);

        private Dictionary<Point, MinimapTile> _minimapIcons;

        public void InitMinimap() {
            Map = GameManager.Map;

            
            _minimapIcons = new Dictionary<Point, MinimapTile>(Map.RoomGrid.Count);
            foreach(Point p in Map.RoomGrid.Keys) {
                MinimapTile mapTile = Instantiate<Prefab_MinimapTile>(Vector3.Zero, transform).GetComponent<MinimapTile>();
                //mapTile.transform.Scale = new Vector3(0.75f);
                //mapTile.transform.LocalScale = Vector3.One;

                mapTile.transform.LocalPosition = (p * MinimapIconSize).ToVector2().ToVector3();
                mapTile.transform.LocalScale = Vector3.One;
                mapTile.InitMinimapTile(GetIconFromRoom(Map.RoomGrid[p].RoomType), p);

                _minimapIcons.Add(p, mapTile);
            }

            // Set the starting room to be explored
            _minimapIcons[Map.GridPos_StartingRoom].SetExplored();

            // Set all adjacent rooms to be seen
            foreach (Direction dir in Map.RoomGrid[Map.GridPos_StartingRoom].Doors.Keys) {
                _minimapIcons[Map.GridPos_StartingRoom + dir.GetDirectionPoint()].SetSeen();
            }
        }

        public void ShiftDirection(Point newMapPos) {
            _minimapIcons[newMapPos].SetExplored();
            
            // Set all adjacent rooms to be seen
            foreach(Direction dir in Map.RoomGrid[newMapPos].Doors.Keys) {
                _minimapIcons[newMapPos + dir.GetDirectionPoint()].SetSeen();
            }
        }

        public static MinimapIcon GetIconFromRoom(RoomType type) {
            switch (type) {
                default:
                    return MinimapIcon.Normal;
                case RoomType.Item:
                    return MinimapIcon.Item;
                case RoomType.Boss:
                    return MinimapIcon.Boss;
            }
        }

    }

    public enum MinimapIcon {
        Current,
        Unexplored,
        Explored,

        Normal,
        Item,
        Boss,
        Shop,
        Casino
    }
}