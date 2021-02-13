using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    public class Tile<T> {
        
        public T Data { get; private set; }
        private SpriteRenderer _tileRenderer;

        public Tile(T data, SpriteRenderer rend, Point pos) {
            Data = data;
            _tileRenderer = rend;
        }

        public void ChangeData(T newData) {
            Data = newData;
            DataChangeAction(newData, _tileRenderer);
        }

        public void RefreshData() {
            DataChangeAction(Data, _tileRenderer);
        }

        public Action<T, SpriteRenderer> DataChangeAction = (data, tileRenderer) => { };


        public void Destroy() {
            Data = default;
            _tileRenderer = null;
            DataChangeAction = (data, tileRend) => { };
        }
    }
}
