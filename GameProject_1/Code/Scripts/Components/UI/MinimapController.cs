using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Prefabs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class MinimapController : Component {
        public MinimapController(GameObject attached) : base(attached) { }


        private MapManager Map;
        private static readonly Point MinimapIconSize = new Point(15, 15);

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
                mapTile.InitMinimapTile(MinimapIcon.Normal);

                _minimapIcons.Add(p, mapTile);
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