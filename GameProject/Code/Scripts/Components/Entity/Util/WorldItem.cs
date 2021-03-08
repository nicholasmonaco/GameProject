using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class WorldItem : Component {
        public WorldItem(GameObject attached) : base(attached) { }

        public ItemID ID;
        public SpriteRenderer ItemRenderer;
        public SpriteRenderer PedastalRenderer;

        private Vector2 _origOffset;
        private Vector2 _upperOffset;
        private const float _hoverDur = 0.45f;
        private float _hoverTimer = 0;
        private int _hoverDir = 1;


        public void Init(ItemID id, SpriteRenderer spriteRend, SpriteRenderer pedastalRend) {
            ID = id;
            ItemRenderer = spriteRend;
            PedastalRenderer = pedastalRend;

            ItemRenderer.SpriteOffset = new Vector2(0, 18);
            _origOffset = ItemRenderer.SpriteOffset;
            _upperOffset = _origOffset + new Vector2(0, 3f);
        }

        public override void Update() {
            _hoverTimer += Time.deltaTime * _hoverDir;

            if(_hoverTimer >= _hoverDur) {
                _hoverTimer = _hoverDur;
                _hoverDir *= -1;
            }else if(_hoverTimer <= 0) {
                _hoverTimer = 0;
                _hoverDir *= -1;
            }

            ItemRenderer.SpriteOffset = Vector2.Lerp(_origOffset, _upperOffset, _hoverTimer / _hoverDur);
        }


        public static ItemID GetRandomItem(ItemPool pool) {
            //todo
            return ItemID.VitaminH;
        }

    }
}