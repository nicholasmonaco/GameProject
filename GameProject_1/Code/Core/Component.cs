using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core {
    public class Component {
        public GameObject gameObject;
        public Transform transform;

        public Component(GameObject attached) {
            gameObject = attached;
            transform = attached.transform;
        }


        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Awake() { }

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void LateUpdate() { }

        public virtual void Draw(SpriteBatch sb) { }



        public Coroutine StartCoroutine(IEnumerator routine) {
            return gameObject.StartCoroutine(routine);
        }
    }
}
