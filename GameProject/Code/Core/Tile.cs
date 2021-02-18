using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    public class Tile<T> {
        
        public T Data { get; private set; }
        public Point TilemapPos { get; private set; }
        public SpriteRenderer TileRenderer { get; private set; }
        public Vector2 Offset { get; private set; } = Vector2.Zero;
        private TileMap<T> _parentMap;

        public Tile(T data, SpriteRenderer rend, Point pos, TileMap<T> parentMap) {
            Data = data;
            TilemapPos = pos;
            _parentMap = parentMap;
            TileRenderer = rend;

            Offset = pos.ToVector2() * (parentMap.TileSize + parentMap.TileSpacing * 2) + parentMap.MapOffset + parentMap.TileSpacing;
            TileRenderer.SpriteOffset = Offset;


        }

        public void ChangeData(T newData) {
            Data = newData;
            DataChangeAction(this, _parentMap);
        }

        public void RefreshData() {
            DataChangeAction(this, _parentMap);
        }

        public Action<Tile<T>, TileMap<T>> DataChangeAction = (tile, parentMap) => { };


        public void Destroy() {
            Data = default;
            TileRenderer.Destroy();
            TileRenderer = null;
            DataChangeAction = (tile, parentMap) => { };
        }
    }
}
