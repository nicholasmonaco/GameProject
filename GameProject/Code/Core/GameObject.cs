// GameObject.cs - Nick Monaco

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.UI;
using GameProject.Code.Scripts.Components.Bullet;

namespace GameProject.Code.Core {

    /// <summary>
    /// The base of all GameObjects, which is how everything in a scene is structured.
    /// </summary>
    public class GameObject {
        private const BindingFlags __bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private Action _emptyList = () => {};
        private Action _destroyList = () => { };

        public List<Component> _components;
        public Transform transform;
        public Rigidbody2D rigidbody2D = null;
        public List<Collider2D> collider2Ds;

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
                } else {
                    OnDisable();
                }

                foreach (Transform t in transform._children) {
                    t.gameObject.Enabled = value;
                }
            }
        }
        public string Tag = null;
        public LayerID Layer = LayerID.Default;

        public bool _everAwaked = false;
        protected bool _everStarted = false;


        public GameObject() {
            _components = new List<Component>(1);
            transform = new Transform(this);
            _components.Add(transform);

            collider2Ds = new List<Collider2D>(1);
        }

        public GameObject(string name) : this() {
            Name = name;
        }

        public GameObject(string name, string tag = null, LayerID layer = LayerID.Default) : this() {
            Name = name;
            Tag = tag;
            Layer = layer;
        }


        public RectTransform SwitchToRectTransform() {
            RectTransform rectTransform = new RectTransform(this);
            rectTransform.Parent = transform.Parent;
            rectTransform.Position = transform.Position;
            rectTransform.Scale = transform.Scale;
            rectTransform.Rotation = transform.Rotation;

            Transform[] newChildren = new Transform[transform._children.Count];
            transform._children.CopyTo(newChildren);
            rectTransform._children = new List<Transform>(newChildren);

            foreach(Transform t in newChildren) {
                t.Parent = rectTransform;
            }

            bool markedForCanvasChild = transform.UIParentFlag;
            rectTransform.UIParentFlag = markedForCanvasChild;

            Component.Destroy(transform);

            transform = rectTransform;
            foreach(Component c in _components) {
                c.transform = rectTransform;
            }

            _components[0] = rectTransform;

            return rectTransform;
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

        public bool HasComponent<T>() {
            return GetComponent<T>() != null;
        }

        public T GetComponent<T>() {
            foreach(Component c in _components) {
                if (c is T rightType) return rightType;
            }
            return default;
        }

        public T[] GetAllComponents<T>() {
            List<T> components = new List<T>();

            foreach (Component c in _components) {
                if (c is T rightType) components.Add(rightType);
            }

            foreach(Transform child in transform._children) {
                foreach (Component c in child.gameObject._components) {
                    if (c is T rightType) components.Add(rightType);
                }
            }

            return components.ToArray();
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
                    RemoveComponent(c);
                    return true;
                }
            }
            return false;
        }

        public void RemoveAllComponents<T>() {
            foreach (Component c in _components) {
                if (c is T _) {
                    Component.Destroy(c);
                    RemoveComponent(c);
                }
            }
        }

        public void RemoveComponent(Component c) {
            _destroyList += () => {
                _components.Remove(c);
                c.Dispose();
            };
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
                c._everStarted = true;
            }

            _everStarted = true;
        }

        public void OnEnable() {
            foreach (Component c in _components) {
                c.OnEnable();
            }
        }

        public void OnDisable() {
            foreach (Component c in _components) {
                c.OnDisable();
            }
        }

        public void Update() {
            foreach (Component c in _components) {
                if (c.Enabled && c._everAwaked) c.Update();
            }

            //if (_destroyList != _emptyList) {
                _destroyList();
                _destroyList = _emptyList;
            //}   
        }

        public void LateUpdate() {
            foreach (Component c in _components) {
                if (c.Enabled && c._everAwaked) c.LateUpdate();
            }

            //if (!_destroyList.Method.Equals(_emptyList)) {
                _destroyList();
                _destroyList = _emptyList;
            //}
        }

        public void FixedUpdate() {
            foreach (Component c in _components) {
                if (c.Enabled && c._everAwaked) c.FixedUpdate();
            }

            //if (!_destroyList.Method.Equals(_emptyList)) {
                _destroyList();
                _destroyList = _emptyList;
            //}


        }

        public void Draw(SpriteBatch sb) {
            foreach(Component c in _components) {
                if (!c.Enabled) continue;
                c.Draw(sb);
            }
        }

        // End standard scene methods


        public static T Instantiate<T>(Vector3 position, Transform parent) where T : GameObject {
            T obj = Activator.CreateInstance(typeof(T)) as T;
            obj.transform.Parent = parent;
            obj.transform.Position = position;
            //return GameManager.CurrentScene.GameObjects.AddReturn(obj) as T;
            GameManager.CurrentScene.Instantiate(obj);
            return obj;
        }

        public static T Instantiate<T>(Vector3 position) where T : GameObject {
            return Instantiate<T>(position, null);
        }

        public static T Instantiate<T>() where T : GameObject {
            return Instantiate<T>(Vector3.Zero, null);
        }


        public static GameObject Instantiate(GameObject obj) {
            obj.transform.Parent = null;
            obj.transform.Position = Vector3.Zero;

            GameManager.CurrentScene.Instantiate(obj);
            return obj;
        }


        public static void Destroy(GameObject g) {
            GameManager.CurrentScene.GameObjects.Remove(g);

            g.Dispose();
        }

        private void Dispose() {
            while(_components.Count > 0) {
                _components[0].Destroy();
                _components.RemoveAt(0);
            }
            //_components.Clear();

            // Okay, this is some cursed programming.
            // For the class, get all of the global variables. Yeah, all of them.
            // For each of those, if it's not a struct, set it to be null.
            PropertyInfo[] props = this.GetType().GetProperties(__bindingFlags);
            foreach (PropertyInfo property in props) {
                if (!(property.PropertyType).IsValueType) {
                    //property.SetValue(this, null);

                    MethodInfo[] info = property.GetAccessors(true);
                    foreach (MethodInfo i in info) {
                        if (i.ReturnType == typeof(void)) {
                            // then it has a setter we can manipulate
                            property.SetValue(this, null);
                            break;
                        }
                    }
                }
            }
        }


        //public override int GetHashCode() {
        //    return Guid.NewGuid();
        //}

    }
}
