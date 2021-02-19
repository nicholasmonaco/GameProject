using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.UI {
    public class UI_Bounce : Component {
        public UI_Bounce(GameObject attached) : base(attached) { }


        private float _duration = 5;
        private float _timer = 0;
        private bool _movingUp = false;

        private Vector3 _origPos;
        private Vector3 _finalPos;


        public void InitBounce(float duration, Vector3 distance) {
            _duration = duration;
            _timer = duration;

            _origPos = transform.Position;
            _finalPos = _origPos + distance;
        }


        public override void Update() {
            _timer -= Time.deltaTime;

            if(_timer <= 0) {
                _timer = _duration;
                _movingUp = !_movingUp;
            }

            if (_movingUp) {
                transform.Position = Vector3.SmoothStep(_origPos, _finalPos, _timer / _duration);
            } else {
                transform.Position = Vector3.SmoothStep(_finalPos, _origPos, _timer / _duration);
            }
        }

    }
}