// GameObject.cs - Nick Monaco

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// The base of all GameObjects, which is how everything in a scene is structured.
    /// </summary>
    public class GameObject {
        public List<Component> _components;
        public Transform transform;
        public Rigidbody2D rigidbody2D = null;

        public string Name = "GameObject";
        public string Tag = null;
        public int Layer = 0; // Default layer


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
        }

        public void Start() {
            foreach (Component c in _components) {
                c.Start();
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


        public static void Destroy(GameObject g) {
            GameManager.CurrentScene.GameObjects.Remove(g);
            // As far as I know, this is the best way to do this. The gameobjects only exist in the list in the scene, so this should be fine.
        }

        //public override int GetHashCode() {
        //    return Guid.NewGuid();
        //}

    }
}
