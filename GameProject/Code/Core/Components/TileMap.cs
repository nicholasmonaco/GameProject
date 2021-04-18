using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Components {
    public class TileMap<T> : Component {
        public TileMap(GameObject attached) : base(attached) { }


        private Tile<T>[,] _map;
        public  Collider2D[,] ColliderMap;
        public Action<Tile<T>, TileMap<T>> TileChangeAction { private get; set; } = (tile, parentMap) => { };

        public Point MapSize { get; private set; }
        public Vector2 MapOffset { get; private set; } = Vector2.Zero;
        public Vector2 TileSize { get; private set; }
        public Vector2 TileSpacing { get; private set; } = Vector2.Zero;


        public void SetMap(T[,] dataMap, int xSize, int ySize, Vector2 tileSize, Vector2 tileSpacing, Vector2 mapOffset) {
            TileSize = tileSize;
            TileSpacing = tileSpacing;
            MapSize = new Point(xSize, ySize);
            MapOffset = mapOffset;
            _map = new Tile<T>[xSize, ySize];
            ColliderMap = new Collider2D[xSize, ySize]; 

            for(int y = 0; y < ySize; y++) {
                for(int x = 0; x < xSize; x++) {
                    SpriteRenderer tileRend = gameObject.AddComponent<SpriteRenderer>();
                    tileRend.DrawLayer = DrawLayer.ID[DrawLayers.WorldStructs];
                    tileRend.OrderInLayer = 10;
                    _map[x, y] = new Tile<T>(dataMap[x, y], tileRend, new Point(x, y), this);
                    _map[x, y].DataChangeAction += TileChangeAction;
                    _map[x, y].RefreshData();
                }
            }
        }

        public void SetMap(T[,] dataMap, int xSize, int ySize, Vector2 tileSize) {
            SetMap(dataMap, xSize, ySize, tileSize, new Vector2(26, 28), Vector2.Zero);
        }

        public void RefreshMap() {
            for (int y = 0; y < MapSize.Y; y++) {
                for (int x = 0; x < MapSize.X; x++) {
                    _map[x, y].RefreshData();
                }
            }
        }

        public void RefreshTile(Point gridPos) {
            _map[gridPos.X, gridPos.Y].RefreshData();
        }

        public void RefreshTile(int x, int y) {
            _map[x, y].RefreshData();
        }


        public void ChangeTile(T newData, int x, int y) {
            _map[x, y].ChangeData(newData);
        }

        public T GetTile(int x, int y) {
            return _map[x, y].Data;
        }

        public T GetTile(Point p) {
            return GetTile(p.X, p.Y);
        }



        public T GetTileAtWorldPosition(Vector3 position) {
            return GetTileAtWorldPosition(position.ToVector2());
        }

        public T GetTileAtWorldPosition(Vector2 position) {
            Point pos = GetGridPosFromWorldPosition(position);

            return GetTile(pos.X, pos.Y);
        }

        public Vector2 GetWorldPosFromGridPos(Point gridPos) {
            return transform.Position.ToVector2() + _map[gridPos.X, gridPos.Y].Offset;
        }


        public Point GetGridPosFromWorldPosition(Vector3 position) {
            return GetGridPosFromWorldPosition(position.ToVector2());
        }

        public Point GetGridPosFromWorldPosition(Vector2 position) {
            Vector2 posV = (position - MapOffset) / (TileSize + TileSpacing); //if this is right first try i'd be amazed
            // Logic walkthrough:
            // the world position, then the reversed offset to go to where the tilemap actually is, then divide it by the 

            Point pos = posV.ToPoint();

            if (pos.X < 0) {
                pos.X = 0;
            } else if (pos.X > MapSize.X - 1) {
                pos.X = MapSize.X - 1;
            }

            if (pos.Y < 0) {
                pos.Y = 0;
            } else if (pos.Y > MapSize.Y - 1) {
                pos.Y = MapSize.X - 1;
            }

            return pos;
        }


        public override void OnDestroy() {
            foreach(Tile<T> t in _map) {
                t.Destroy();
            }
            _map = null;

            foreach(Collider2D coll in ColliderMap) {
                if(coll != null) coll.Destroy();
            }
            ColliderMap = null;

            TileChangeAction = (tile, parentMap) => { };
        }


        
    }
}