// Scene.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// A container for everything that is present in the game.
    /// </summary>
    public abstract class Scene {

        public List<GameObject> GameObjects; //change this to use guids and be made of a GameObjectReferencer class, so that we can just set the gameobject reference in there to be null when we want it to be destroyed
        public List<Collider2D> Collider2Ds;

        private List<Coroutine> _coroutines;
        private Action _coroutineQueue = () => { };

        private Action _instantiateList = () => { };



        public Scene() { }

        public virtual void LoadContent(ContentManager content) { }

        public virtual void UnloadContent() { }


        public virtual void Init() {
            GameObjects = new List<GameObject>();
            Collider2Ds = new List<Collider2D>();
            _coroutines = new List<Coroutine>();
        }

        public virtual void Awake() {
            // Instantiate initial gameobjects
            //_instantiateList();
            //_instantiateList = () => { };

            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                if (g.Enabled) {
                    g.Awake();
                    g.OnEnable();
                }
            }
        }

        public virtual void Start() {
            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                if(g.Enabled) g.Start();
            }
        }

        public virtual void Update() {
            // Instantiate new GameObjects
            _instantiateList();
            _instantiateList = () => { };

            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                if(g.Enabled && g._everAwaked) g.Update();
            }

            // Handle coroutines
            _coroutineQueue();
            _coroutineQueue = () => { };

            Action removeQueue = () => { };

            foreach (Coroutine routine in _coroutines) {
                // Update the coroutine
                routine.Update();
                // If the coroutine is finished, remove it from the list.
                if (routine.Finished) removeQueue += () => { _coroutines.Remove(routine); };
            }

            removeQueue();
        }

        public virtual void FixedUpdate() {
            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                if(g.Enabled && g._everAwaked) g.FixedUpdate();
            }
        }

        public virtual void LateUpdate() {
            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                if(g.Enabled && g._everAwaked) g.LateUpdate();
            }

            // Handle coroutines
            Action removeQueue = () => { };

            foreach (Coroutine routine in _coroutines) {
                // Update the coroutine
                routine.LateUpdate();
                // If the coroutine is finished, remove it from the list.
                if (routine.Finished) removeQueue += () => { _coroutines.Remove(routine); };
            }

            removeQueue();
        }

        public virtual void Draw(SpriteBatch sb) {
            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                if(g.Enabled) g.Draw(sb);
            }
        }




        public void PhysicsUpdate() {
            // Do internal physics updates here
            Action _triggerActions = () => { };
            Action _collisionActions = () => { };

            foreach (GameObject go in GameObjects) {
                if (!go.Enabled) continue;
                //search for rigidbody in children
                if(go.rigidbody2D != null) {
                    go.rigidbody2D._PhysicsUpdate();
                    // Now, queue up trigger and collision events
                    _triggerActions += go.rigidbody2D.CallTriggerEvents;
                    _collisionActions += go.rigidbody2D.CallCollisionEvents;
                }
            }

            _triggerActions();
            _collisionActions();
            // End internal physics updates

            // Handle coroutines
            Action removeQueue = () => { };

            foreach (Coroutine routine in _coroutines) {
                // Update the coroutine
                routine.FixedUpdate();
                // If the coroutine is finished, remove it from the list.
                if (routine.Finished) removeQueue += () => { _coroutines.Remove(routine); };
            }

            removeQueue();
        }

        public Coroutine StartCoroutine(IEnumerator routine) {
            Coroutine coroutine = new Coroutine(routine);

            _coroutineQueue += () => { _coroutines.Add(coroutine); };

            // This StepThrough makes it so the first section of code in the coroutine (before the first yield) happens immediately
            coroutine.StepThrough(); 

            return coroutine;
        }

        //public void RemoveGameObject(GameObject obj) {
        //    GameObjects.Remove(obj);
        //}


        public void Instantiate(GameObject obj) {
            _instantiateList += () => {
                GameManager.CurrentScene.GameObjects.Add(obj);
                obj.Awake();
                if (obj.Enabled) obj.OnEnable();
                obj.Start();
            };
        }


        public static void LoadScene(Scene scene, ContentManager content) {
            scene.Init();
            scene.LoadContent(content);
            scene.Awake();
            scene.Start();
        }

        public static void UnloadScene(Scene scene) {
            scene.UnloadContent();
        }

        
    }
}
