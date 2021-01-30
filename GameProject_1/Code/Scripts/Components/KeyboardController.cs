using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Scripts.Components {
    public class KeyboardController : Component {
        public KeyboardController(GameObject attached) : base(attached) { }


        private Vector2 _input = Vector2.Zero;
        private Rigidbody2D _rb;


        public override void Awake() {
            _rb = GetComponent<Rigidbody2D>();
        }

        public override void Update() {
            _input = Input.MovementDirection;
        }

        public override void FixedUpdate() {
            _rb.Velocity = _input * 75;
        }
    }
}
