﻿// GameObject.cs - Nick Monaco

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// The base of all GameObjects, which is how everything in a scene is structured.
    /// </summary>
    public class GameObject {
        private const BindingFlags __bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public List<Component> _components;
        public Transform transform;
        public Rigidbody2D rigidbody2D = null;

        public string Name = "GameObject";
        private bool _enabled = true;
        public bool Enabled {
            get { return _enabled; }
            set {
                _enabled = value;

                if (value) {
                    if (!_everAwaked) Awake();
                    OnEnable();
                    if (!_everStarted) Start();
                }

                foreach (Transform t in transform._children) {
                    t.gameObject.Enabled = value;
                }
            }
        }
        public string Tag = null;
        public int Layer = 0; // Default layer

        protected bool _everAwaked = false;
        protected bool _everStarted = false;


        public GameObject() {
            _components = new List<Component>(1);
            transform = new Transform(this);
            _components.Add(transform);
        }

        public GameObject(string name) : this() {
            Name = name;
        }

        public GameObject(string name, string tag = null, int layer = 0) : this() {
            Name = name;
            Tag = tag;
            Layer = layer;
        }

        public Coroutine StartCoroutine(IEnumerator routine) {
            return GameManager.CurrentScene.StartCoroutine(routine);
        }


        public T AddComponent<T>() where T : Component {
            T newComp = Activator.CreateInstance(typeof(T), this) as T;
            _components.Add(newComp);
            return newComp;
        }

        public T AddComponent<T>(params object[] parameters) where T: Component {
            object[] p = new object[parameters.Length + 1];
            p[0] = this;
            Array.Copy(parameters, 0, p, 1, parameters.Length);

            T newComp = Activator.CreateInstance(typeof(T), p) as T;
            _components.Add(newComp);
            return newComp;
        }

        public T GetComponent<T>() {
            foreach(Component c in _components) {
                if (c is T rightType) return rightType;
            }
            return default;
        }

        public bool TryGetComponent<T>(out T comp) {
            foreach (Component c in _components) {
                if (c is T rightType) { 
                    comp = rightType;
                    return true;
                }
            }
            comp = default;
            return false;
        }

        public bool RemoveComponent<T>() {
            foreach (Component c in _components) {
                if (c is T _) {
                    Component.Destroy(c);
                    return true;
                }
            }
            return false;
        }


        // Standard scene methods

        public void Awake() {
            foreach (Component c in _components) {
                c.PreAwake();
                if (rigidbody2D == null && c is Rigidbody2D rb) rigidbody2D = c as Rigidbody2D;
            }

            rigidbody2D?.ResetPosition();

            foreach (Component c in _components) {
                c.Awake();
            }

            _everAwaked = true;
        }

        public void Start() {
            foreach (Component c in _components) {
                c.Start();
            }

            _everStarted = true;
        }

        public void OnEnable() {
            foreach (Component c in _components) {
                c.OnEnable();
            }
        }

        public void Update() {
            foreach (Component c in _components) {
                c.Update();
            }
        }

        public void LateUpdate() {
            foreach (Component c in _components) {
                c.LateUpdate();
            }
        }

        public void FixedUpdate() {
            foreach (Component c in _components) {
                c.FixedUpdate();
            }
        }

        public void Draw(SpriteBatch sb) {
            foreach(Component c in _components) {
                c.Draw(sb);
            }
        }

        // End standard scene methods


        public static T Instantiate<T>(Vector3 position, Transform parent) where T : GameObject {
            T obj = Activator.CreateInstance(typeof(T)) as T;
            obj.transform.Parent = parent;
            obj.transform.Position = position;
            return GameManager.CurrentScene.GameObjects.AddReturn(obj) as T;
        }

        public static T Instantiate<T>(Vector3 position) where T : GameObject {
            return Instantiate<T>(position, null);
        }

        public static T Instantiate<T>() where T : GameObject {
            return Instantiate<T>(Vector3.Zero, null);
        }


        public static void Destroy(GameObject g) {
            GameManager.CurrentScene.GameObjects.Remove(g);

            g.Dispose();
        }

        private void Dispose() {
            foreach(Component comp in _components) {
                comp.Destroy();
            }


            // Okay, this is some cursed programming.
            // For the class, get all of the global variables. Yeah, all of them.
            // For each of those, if it's not a struct, set it to be null.
            PropertyInfo[] props = this.GetType().GetProperties(__bindingFlags);
            foreach (PropertyInfo property in props) {
                if (!(property.PropertyType).IsValueType) {
                    property.SetValue(this, null);
                }
            }
        }


        //public override int GetHashCode() {
        //    return Guid.NewGuid();
        //}

    }
}
