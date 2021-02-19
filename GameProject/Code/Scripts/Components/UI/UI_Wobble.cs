using GameProject.Code.Core;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Scripts.Components.UI {
    public class UI_Wobble : Component {
        public UI_Wobble(GameObject attached) : base(attached) { }

        private float _duration = 5;
        private float _timer = 0;
        private bool _clockwise = false;

        private float _origRot;
        private float _finalRot;


        public void Init(float duration, float maxAngle) {
            _duration = duration;
            _timer = duration;

            _origRot = transform.Rotation - maxAngle;
            _finalRot = transform.Rotation + maxAngle;
        }


        public override void Update() {
            _timer -= Time.deltaTime;

            if (_timer <= 0) {
                _timer = _duration;
                _clockwise = !_clockwise;
            }

            if (_clockwise) {
                transform.Rotation = MathHelper.SmoothStep(_origRot, _finalRot, _timer / _duration);
            } else {
                transform.Rotation = MathHelper.SmoothStep(_finalRot, _origRot, _timer / _duration);
            }
        }
    }
}