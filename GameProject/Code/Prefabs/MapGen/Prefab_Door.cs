using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Components;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Prefabs.MapGen {
    public class Prefab_Door : GameObject {

        public Prefab_Door() : base() {
            Name = "Door";
            
            Layer = LayerID.Door;

            SpriteRenderer frameRend = AddComponent<SpriteRenderer>();

            frameRend.Sprite = Resources.Sprites_DoorFrames[DoorType.Normal];
            frameRend.DrawLayer = DrawLayer.ID[DrawLayers.WorldStructs];
            frameRend.OrderInLayer = 25;
            frameRend.Material.BatchID = BatchID.Room;


            SpriteRenderer insideRend = AddComponent<SpriteRenderer>();
            insideRend.Sprite = Resources.Sprite_Door_Inside;
            insideRend.DrawLayer = DrawLayer.ID[DrawLayers.WorldStructs];
            insideRend.OrderInLayer = 20;
            insideRend.Material.BatchID = BatchID.Room;

            SpriteRenderer doorRend = AddComponent<SpriteRenderer>();
            doorRend.Sprite = Resources.Sprite_Invisible;
            doorRend.DrawLayer = DrawLayer.ID[DrawLayers.WorldStructs];
            doorRend.OrderInLayer = 21;
            doorRend.Material.BatchID = BatchID.Room;

            Vector2[] doorBounds = new Vector2[] { new Vector2(-20, 3.5f), new Vector2(20, 3.5f), new Vector2(10, -22), new Vector2(-10, -22) };
            PolygonCollider2D doorCollider = _components.AddReturn(new PolygonCollider2D(this, doorBounds, false)) as PolygonCollider2D;
            doorCollider.IsTrigger = true;

            RectCollider2D fillerCollider = AddComponent<RectCollider2D>(30, 35, 0, -6.5f);
            fillerCollider.Enabled = false;
            //something is messed up with this. you can walk through the bottom and right doors when they're closed, which is weird. it's like theyre not enabled.

            DoorController dc = AddComponent<DoorController>();
            dc.FrameRenderer = frameRend;
            dc.DoorRenderer = doorRend;
            dc.DoorCollider = doorCollider;
            dc.FillerCollider = fillerCollider;
        }
    }
}
