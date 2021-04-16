using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace GameProject.Code.Scripts.Components.Entity {
    public abstract class AbstractPickup : AbstractEntity {
        public AbstractPickup(GameObject attached) : base(attached, EntityID.Pickup_Coin) { }

        private Pickup _pickupType;
        protected SpriteRenderer _pickupRenderer;


        public virtual void InitPickup(Pickup type, SpriteRenderer pickupRenderer) {
            _pickupType = type;
            ID = (EntityID)((int)type); // Note: This only works because the id's match. Be sure to continue to do so in the future.

            _pickupRenderer = pickupRenderer;
            _pickupRenderer.Sprite = Resources.Sprite_Pickups[_pickupType];

            DeathAction = () => { StartCoroutine(YScaleFadeOut()); };

            StartCoroutine(YScaleFadeIn());
        }


        protected abstract bool CanPickup();
        protected abstract void OnPickup();


        protected IEnumerator YScaleFadeOut() {
            GetComponent<Collider2D>().Destroy();
            GetComponent<Rigidbody2D>().Velocity = Vector2.Zero;
            
            float dur_Total = 0.3f;
            float dur = dur_Total;
            Vector2 origScale = _pickupRenderer.SpriteScale;
            Vector2 yZero = new Vector2(origScale.X, 0);

            while(dur >= 0) {
                dur -= Time.deltaTime;
                _pickupRenderer.SpriteScale = Vector2.Lerp(yZero, origScale, dur / dur_Total);
                yield return null;
            }

            _pickupRenderer.SpriteScale = yZero;
            yield return new WaitForEndOfFrame();

            Destroy(this);
        }

        protected IEnumerator YScaleFadeIn() {
            

            float dur_Total = 0.3f;
            float dur = dur_Total;
            Vector2 origScale = _pickupRenderer.SpriteScale;
            Vector2 yZero = new Vector2(origScale.X, 0);

            while (dur >= 0) {
                dur -= Time.deltaTime;
                _pickupRenderer.SpriteScale = Vector2.Lerp(origScale, yZero, dur / dur_Total);
                yield return null;
            }

            _pickupRenderer.SpriteScale = origScale;
            GetComponent<Collider2D>().Enabled = true;
            //GetComponent<Rigidbody2D>().Enabled = true;
            //GetComponent<Rigidbody2D>().Velocity = Vector2.Zero;
            yield return new WaitForEndOfFrame();
        }

        protected Action DeathAction = () => { };



        public override void OnCollisionEnter2D(Collider2D other) {
            if (other.gameObject.Layer == LayerID.Player && CanPickup()) {
                OnPickup();
                DeathAction();
            }
        }

        public override void OnCollisionStay2D(Collider2D other) {
            if(other.gameObject.Layer == LayerID.Player && CanPickup()) {
                OnPickup();
                DeathAction();
            }
        }

    }

    public enum Pickup {
        Heart_Half = 201,
        Heart_Whole = 202,
        BonusHeart = 203,
        Key = 204,
        PowerCell = 205,
        Coin = 206,
        Coin_5 = 207,
        Bomb = 215,

        Chest_Free = 208,
        Chest_Free_Opened = 209,
        Chest_Locked = 210,
        Chest_Locked_Opened = 211,

        Heart_Double = 212,
        Key_Double = 213,
        Coin_Double = 214,
        Bomb_Double = 216
    }
}