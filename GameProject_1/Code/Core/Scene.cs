using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    public abstract class Scene {

        public List<GameObject> GameObjects;
        public List<Collider2D> Collider2Ds;

        private List<Coroutine> _coroutines;
        private Action _coroutineQueue = () => { };



        public Scene() { }

        public virtual void LoadContent(ContentManager content) { }

        public virtual void UnloadContent() { }


        public virtual void Init() {
            GameObjects = new List<GameObject>();
            Collider2Ds = new List<Collider2D>();
            _coroutines = new List<Coroutine>();
        }

        public virtual void Awake() {
            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                g.Awake();
            }
        }

        public virtual void Start() {
            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                g.Start();
            }
        }

        public virtual void Update() {
            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                g.Update();
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
                g.FixedUpdate();
            }
        }

        public virtual void LateUpdate() {
            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                g.LateUpdate();
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
                g.Draw(sb);
            }
        }




        public void PhysicsUpdate() {
            // Do internal physics updates here
            Action _triggerActions = () => { };
            Action _collisionActions = () => { };

            foreach (GameObject go in GameObjects) {
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
