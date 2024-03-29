﻿// Component.cs - Nick Monaco

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core.Components;
using System.Reflection;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// The base for all GameObject components.
    /// </summary>
    public abstract class Component {
        private const BindingFlags __bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public GameObject gameObject;
        public Transform transform;

        public bool _everAwaked = false;
        public bool _everStarted = false;

        protected bool Destroyed = false;

        private bool _enabled = true;
        //public bool Enabled => gameObject.Enabled && _enabled;
        public bool Enabled {
            get => gameObject.Enabled && _enabled;
            set {
                _enabled = value;

                if (value) {
                    if (!_everAwaked) {
                        PreAwake();
                        Awake(); 
                    }
                    OnEnable();
                    if (!_everStarted) Start();
                } else {
                    OnDisable();
                }
            }
        }

        protected Component() {
            GUID = Guid.NewGuid();
        }

        public Component(GameObject attached) {
            gameObject = attached;
            transform = attached.transform;

            GUID = Guid.NewGuid();
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

            _everAwaked = true;
        }

        public virtual void Awake() { }

        public virtual void Start() { }

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void LateUpdate() { }

        public virtual void Draw(SpriteBatch sb) { }
        public virtual void DebugDraw(SpriteBatch sb) { }

        public virtual void OnDestroy() { }



        public Coroutine StartCoroutine(IEnumerator routine) {
            return gameObject.StartCoroutine(routine);
        }


        public T GetComponent<T>() {
            return gameObject.GetComponent<T>();
        }

        public static GameObject Instantiate(GameObject obj) {
            return GameObject.Instantiate(obj);
        }

        public static T Instantiate<T>(Vector3 position, Transform parent) where T : GameObject {
            return GameObject.Instantiate<T>(position, parent);
        }

        public static T Instantiate<T>(Vector3 position) where T : GameObject {
            return GameObject.Instantiate<T>(position, null);
        }

        public static T Instantiate<T>() where T : GameObject {
            return GameObject.Instantiate<T>();
        }

        public T AddComponent<T>() where T : Component {
            return gameObject.AddComponent<T>();
        }

        public T AddComponent<T>(params object[] parameters) where T : Component {
            return gameObject.AddComponent<T>(parameters);
        }

        public bool RemoveComponent<T>() {
            return gameObject.RemoveComponent<T>();
        }


        public void Destroy() {
            OnDestroy();
            Destroyed = true;
            gameObject.RemoveComponent(this);
            //gameObject._components.Remove(this);
            Dispose();
        }

        public virtual void Dispose() {
            //transform = null;
            //gameObject = null;

            // Okay, this is some cursed programming.
            // For the class, get all of the global variables. Yeah, all of them.
            // For each of those, if it's not a struct, set it to be null.
            PropertyInfo[] props = this.GetType().GetProperties(__bindingFlags);
            foreach(PropertyInfo property in props) {
                if (!(property.PropertyType).IsValueType) {
                    //if (typeof(Component).IsAssignableFrom(property.PropertyType)) {
                    // This is where you would go down the chain, but I dont think we'll need to
                    //}

                    MethodInfo[] info = property.GetAccessors(true);
                    foreach(MethodInfo i in info) {
                        if(i.ReturnType == typeof(void)) {
                            // then it has a setter we can manipulate
                            property.SetValue(this, null);
                            break;
                        }
                    }
                }
            }
        }


        public static void Destroy(Component c) {
            c.Destroy();
        }

        public static void Destroy(GameObject g) {
            GameObject.Destroy(g);
        }

        
        public Guid GUID { get; private set; }



        public virtual void OnCollisionEnter2D(Collider2D other) { }
        public virtual void OnCollisionStay2D(Collider2D other) { }
        public virtual void OnCollisionExit2D(Collider2D other) { }
        public virtual void OnTriggerEnter2D(Collider2D other) { }
        public virtual void OnTriggerStay2D(Collider2D other) { }
        public virtual void OnTriggerExit2D(Collider2D other) { }
    }
}
