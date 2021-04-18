using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Items;
using GameProject.Code.Scripts.Components.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
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

        private bool _changing = false;


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
            List<ItemID> possible = new List<ItemID>() {
                ItemID.VitaminH,
                ItemID.RationBar,
                ItemID.Cake,
                ItemID.FocusLens,
                ItemID.HabaneroHotSauce,
                ItemID.FourLeafClover
            };

            return possible[GameManager.WorldRandom.Next(0, possible.Count)];
        }


        public override void OnCollisionStay2D(Collider2D other) {
            if(other.Layer == LayerID.Player) {
                if (!_changing) {
                    _changing = true;
                    // Coroutine for the display of item name and delayed pickup logic
                    StartCoroutine(ItemGetCoroutine());
                }
            }
        }

        private IEnumerator ItemGetCoroutine() {
            if (ID == ItemID.None) yield break;

            float holdTime = 2f;


            //play pickup sfx


            //pop up ui display of item name
            StartCoroutine(FlashUI(holdTime));

            //switch to player pickup animation and pass in time
            StartCoroutine(GameManager.Player.PickupItem(holdTime));

            // Actually do item pickup logic
            PickupItem();

            //hold for 0.75f or whatever
            yield return new WaitForSeconds(holdTime);

            //get rid of ui display
            //happens automatically


            // Flip pickup flag
            _changing = false;
        }


        private IEnumerator FlashUI(float duration) {
            Type itemType = Item.GetItem(ID);
            if (itemType == null) yield break;
            Item realItem = (Item)Activator.CreateInstance(itemType);

            float bonusSlideTime = 0.5f;
            float holdDuration = duration - bonusSlideTime;

            //enable item ui
            ItemPickupUI ui = GameManager.ItemPickupUI;
            ui.gameObject.Enabled = true;

            //set text of it
            
            ui.NameRenderer.Text = realItem.Name;
            ui.FlavorTextRenderer.Text = realItem.FlavorText;

            //slide it in from the side
            float timer = bonusSlideTime;
            while(timer > 0) {
                ui.transform.LocalPosition = Vector3.Lerp(ui.CenteredPos, ui.OffscreenPos_Right, timer / bonusSlideTime);

                yield return null;
                timer -= Time.deltaTime;
            }

            ui.transform.LocalPosition = ui.CenteredPos;

            //wait duration
            yield return new WaitForSeconds(holdDuration);

            //slide it out to the other side
            timer = bonusSlideTime;
            while (timer > 0) {
                ui.transform.LocalPosition = Vector3.Lerp(ui.OffscreenPos_Left, ui.CenteredPos, timer / bonusSlideTime);

                yield return null;
                timer -= Time.deltaTime;
            }

            ui.transform.LocalPosition = ui.OffscreenPos_Left;
            ui.gameObject.Enabled = false;
        }



        private void PickupItem() {
            // If item is an active item:
            if (Item.IsActive(ID)) {
                // If the player doesn't already have an active item:
                if (PlayerStats.ActiveItem == ItemID.None) {
                    // Give the player the active item
                    PlayerStats.ActiveItem = ID;
                    ID = ItemID.None;
                } else {
                    // Swap the player's active item and the pedastal item
                    ItemID temp = PlayerStats.ActiveItem;
                    PlayerStats.ActiveItem = ID;
                    ID = temp;
                }

            // If item is passive (and also isn't nothing):
            } else if (ID != ItemID.None) {
                // Give the player the item
                PlayerStats.AddItem(ID);
                ID = ItemID.None;
            }
        }

    }
}