using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace GameProject.Code.Scripts.Components.Entity {
    public abstract class AbstractPickup : Component {
        public AbstractPickup(GameObject attached) : base(attached) { }

        private Pickup _pickupType;
        protected SpriteRenderer _pickupRenderer;


        public virtual void InitPickup(Pickup type, SpriteRenderer pickupRenderer) {
            _pickupType = type;

            _pickupRenderer = pickupRenderer;
            _pickupRenderer.Sprite = Resources.Sprite_Pickups[_pickupType];

            DeathAction = () => { StartCoroutine(YScaleFade()); };
        }


        protected abstract bool CanPickup();
        protected abstract void OnPickup();


        protected IEnumerator YScaleFade() {
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

        protected Action DeathAction = () => { };



        public override void OnCollisionEnter2D(Collider2D other) {
            if (other.gameObject.Layer == (int)LayerID.Player && CanPickup()) {
                OnPickup();
                DeathAction();
            }
        }

        public override void OnCollisionStay2D(Collider2D other) {
            if(other.gameObject.Layer == (int)LayerID.Player && CanPickup()) {
                OnPickup();
                DeathAction();
            }
        }

    }

    public enum Pickup {
        Heart_Half,
        Heart_Whole,
        BonusHeart,
        Key,
        PowerCell,
        Coin,
        Coin_5,

        Chest_Free,
        Chest_Free_Opened,
        Chest_Locked,
        Chest_Locked_Opened,

        Heart_Double,
        Key_Double,
        Coin_Double
    }
}