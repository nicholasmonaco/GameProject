using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core {
    public class GameObject {
        public List<Component> _components;
        public Transform transform;

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

        public GameObject(string name, string tag = null, int layer = 0) {
            Name = name;
            Tag = tag;
            Layer = layer;
        }

        public Coroutine StartCoroutine(IEnumerator routine) {
            return GameManager.CurrentScene.StartCoroutine(routine);
        }


        // Standard scene methods

        public void Awake() {
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

        
    }
}
