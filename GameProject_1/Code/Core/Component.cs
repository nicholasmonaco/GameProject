// Component.cs - Nick Monaco

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// The base for all GameObject components.
    /// </summary>
    public class Component {
        public GameObject gameObject;
        public Transform transform;

        public Component(GameObject attached) {
            gameObject = attached;
            transform = attached.transform;
        }


        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void PreAwake() {
            Rigidbody2D rb = null;
            bool fixedToCollider = false;
            foreach (Component c in gameObject._components) {
                if (c is Collider2D collider) {
                    collider.OnCollisionEnter2D_Direct += OnCollisionEnter2D;
                    collider.OnCollisionStay2D_Direct += OnCollisionStay2D;
                    collider.OnCollisionExit2D_Direct += OnCollisionExit2D;
                    collider.OnTriggerEnter2D_Direct += OnTriggerEnter2D;
                    collider.OnTriggerStay2D_Direct += OnTriggerStay2D;
                    collider.OnTriggerExit2D_Direct += OnTriggerExit2D;
                    fixedToCollider = true;
                    break; // i have this as break, which means having two colliders on something is gonna be reallllly messed up
                }

                if (c is Rigidbody2D r) rb = r;
            }

            if (!fixedToCollider && rb != null) {
                //Then fix to a rigidbody, since that will be connected to all subcolliders
                rb.OnCollisionEnter2D_Direct += OnCollisionEnter2D;
                rb.OnCollisionStay2D_Direct += OnCollisionStay2D;
                rb.OnCollisionExit2D_Direct += OnCollisionExit2D;
                rb.OnTriggerEnter2D_Direct += OnTriggerEnter2D;
                rb.OnTriggerStay2D_Direct += OnTriggerStay2D;
                rb.OnTriggerExit2D_Direct += OnTriggerExit2D;
                rb.RefixActionsToSubcolliders(); //should this be here? maybe the colliders will just pull them from their attached rigidbodies
            }
        }

        public virtual void Awake() { }

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void LateUpdate() { }

        public virtual void Draw(SpriteBatch sb) { }

        public virtual void OnDestroy() { }



        public Coroutine StartCoroutine(IEnumerator routine) {
            return gameObject.StartCoroutine(routine);
        }


        public T GetComponent<T>() {
            return gameObject.GetComponent<T>();
        }

        public static void Destroy(Component c) {
            c.OnDestroy();
            c.gameObject._components.Remove(c);
        }

        public static void Destroy(GameObject g) {
            GameManager.CurrentScene.GameObjects.Remove(g);
        }



        public virtual void OnCollisionEnter2D(Collider2D other) { }
        public virtual void OnCollisionStay2D(Collider2D other) { }
        public virtual void OnCollisionExit2D(Collider2D other) { }
        public virtual void OnTriggerEnter2D(Collider2D other) { }
        public virtual void OnTriggerStay2D(Collider2D other) { }
        public virtual void OnTriggerExit2D(Collider2D other) { }
    }
}
