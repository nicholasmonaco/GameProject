// KeyboardController.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using System.Collections;

namespace GameProject.Code.Scripts.Components {
    
    /// <summary>
    /// A test component that allows a rigidbody to be controlled with keyboard input.
    /// Also used to test coroutines.
    /// </summary>
    public class KeyboardController : Component {
        public KeyboardController(GameObject attached) : base(attached) { }


        private Vector2 _input = Vector2.Zero;
        private Rigidbody2D _rb;


        public override void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            
            //Input.OnShoot_Down += () => {
            //    StartCoroutine(CoroutineTest_01());
            //};
        }

        public override void Update() {
            _input = Input.MovementDirection;
        }

        public override void FixedUpdate() {
            _rb.Velocity = _input * 75;
        }


        private IEnumerator CoroutineTest_01() {
            //move up for 3 seconds, move right 2 seconds, move down+left for 2 seconds
            _rb.Velocity = new Vector2(0, 20);
            yield return new WaitForSeconds(3);
            _rb.Velocity = new Vector2(20, 0);
            yield return new WaitForSeconds(2);
            _rb.Velocity = new Vector2(-20, -20);
            yield return new WaitForSeconds(3);
            _rb.Velocity = Vector2.Zero;
        }
    }
}
