using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Scenes {
    public class GameScene : Scene {

        Texture2D _testBall;



        public override void Awake() {
            throw new NotImplementedException();
        }

        public override void Start() {
            throw new NotImplementedException();
        }

        public override void LoadContent(ContentManager content) {
            _testBall = content.Load<Texture2D>("Textures/Misc/Ball");
        }

        public override void UnloadContent() {
            throw new NotImplementedException();
        }

        public override void Update() {
            throw new NotImplementedException();
        }

        public override void FixedUpdate() {
            throw new NotImplementedException();
        }

        public override void LateUpdate() {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch sb) {
            
        }
    }
}
