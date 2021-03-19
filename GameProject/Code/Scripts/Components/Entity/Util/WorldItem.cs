using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class WorldItem : Component {
        public WorldItem(GameObject attached) : base(attached) { }

        private ItemID _id;
        public ItemID ID {
            get => _id;
            set {
                _id = value;
                if (ItemRenderer != null) ItemRenderer.Sprite = Resources.Sprites_Items[value];
            }
        }

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


        public override void OnCollisionStay2D(Collider2D other) {
            //if item is passive,
            //   add it to inventory
            //   set item pedastal to "none" item
            //if item is active,
            //   check if current active is none or not
            //   if current active is none,
            //       set active to item
            //       set itemid to none
            //   if current active is not none,
            //       swap active and pedastal item


            //do a coroutine here for the display of item name and stuff

            if (Item.IsActive(ID)) {
                if(PlayerStats.ActiveItem == ItemID.None) {
                    PlayerStats.ActiveItem = ID;
                    ID = ItemID.None;
                } else {
                    ItemID temp = PlayerStats.ActiveItem;
                    PlayerStats.ActiveItem = ID;
                    ID = temp;
                }
            } else if(ID != ItemID.None) {
                PlayerStats.AddItem(ID);
                ID = ItemID.None;
            }
        }

    }
}