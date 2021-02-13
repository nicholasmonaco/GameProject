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
        public Action<T, SpriteRenderer> TileChangeAction { private get; set; } = (data, renderer) => { };


        public void SetMap(T[,] dataMap, int xSize, int ySize) {
            _map = new Tile<T>[xSize, ySize];
            for(int y = 0; y < ySize; y++) {
                for(int x = 0; x < xSize; x++) {
                    SpriteRenderer tileRend = gameObject.AddComponent<SpriteRenderer>();
                    _map[y, x] = new Tile<T>(dataMap[y, x], tileRend, new Point(x, y));
                    _map[y, x].DataChangeAction += TileChangeAction;
                    _map[y, x].RefreshData();
                }
            }
        }

        public void ChangeTile(T newData, int x, int y) {
            _map[y, x].ChangeData(newData);
        }



        public override void OnDestroy() {
            foreach(Tile<T> t in _map) {
                t.Destroy();
            }
            _map = null;

            TileChangeAction = (data, renderer) => { };
        }

    }
}