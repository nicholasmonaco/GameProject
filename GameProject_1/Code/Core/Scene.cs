using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameProject.Code.Core {
    public abstract class Scene {

        private List<Coroutine> _coroutines;


        public Scene() {
            _coroutines = new List<Coroutine>();
        }

        public abstract void LoadContent(ContentManager content);

        public abstract void UnloadContent();

        public abstract void Awake();

        public abstract void Start();

        public virtual void Update() {
            // Handle coroutines
            foreach(Coroutine routine in _coroutines) {
                // Update the coroutine
                routine.Update();
                // If the coroutine is finished, remove it from the list.
                if (routine.Finished) _coroutines.Remove(routine);
            }
        }

        public abstract void FixedUpdate();

        public virtual void LateUpdate() {
            // Handle coroutines
            foreach (Coroutine routine in _coroutines) {
                // Update the coroutine
                routine.LateUpdate();
                // If the coroutine is finished, remove it from the list.
                if (routine.Finished) _coroutines.Remove(routine);
            }
        }

        public abstract void Draw(SpriteBatch sb);




        public void PhysicsUpdate() {
            // Do internal physics updates here

            // End internal physics updates

            // Handle coroutines
            foreach (Coroutine routine in _coroutines) {
                // Update the coroutine
                routine.FixedUpdate();
                // If the coroutine is finished, remove it from the list.
                if (routine.Finished) _coroutines.Remove(routine);
            }
        }

        public Coroutine StartCoroutine(IEnumerator routine) {
            Coroutine coroutine = new Coroutine(routine);
            _coroutines.Add(coroutine);
            coroutine.StepThrough();
            return coroutine;
        }


        public static void LoadScene(Scene scene, ContentManager content) {
            scene.LoadContent(content);
            scene.Awake();
            scene.Start();
        }

        public static void UnloadScene(Scene scene) {
            scene.UnloadContent();
        }
    }
}
