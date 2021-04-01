using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_WorldItem : GameObject {
        public Prefab_WorldItem(ItemID id) {
            Name = $"Item Pedastal ({id.ToString()})";
            Layer = LayerID.Item;

            SpriteRenderer pedastalRend = AddComponent<SpriteRenderer>(Resources.Sprite_ItemPedastal);
            pedastalRend.DrawLayer = DrawLayer.ID[DrawLayers.Entities];
            pedastalRend.OrderInLayer = 100;

            Texture2D sprite = Resources.Sprites_Items.ContainsKey(id) ? Resources.Sprites_Items[id] : Resources.Sprite_Item_Unknown;
            SpriteRenderer itemRend = AddComponent<SpriteRenderer>(sprite);
            itemRend.DrawLayer = DrawLayer.ID[DrawLayers.Entities];
            itemRend.OrderInLayer = 101;
            itemRend.Material.BatchID = BatchID.AbovePlayer;

            WorldItem item = AddComponent<WorldItem>();
            item.Init(id, itemRend, pedastalRend);

            Collider2D coll = AddComponent<CircleCollider2D>(10);
            //coll.IsTrigger = true;
        }
    }
}
