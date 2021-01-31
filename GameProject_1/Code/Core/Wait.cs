using System;

namespace GameProject.Code.Core {
    class Wait { }

    public class WaitForSeconds : BaseCoroutine {
        private float _duration = 0;
        private bool _going = false;

        public WaitForSeconds(float seconds) : base() {
            _duration = seconds;
        }

        public override void Run() {
            _going = true;
        }

        public override void Update() {
            if (!_going) return;
            _duration -= Time.deltaTime;

            if (_duration <= 0) Finished = true;
        }
    }


    public class WaitForEndOfFrame : BaseCoroutine {
        public override void LateUpdate() {
            Finished = true;
        }
    }


    public class WaitForFixedUpdate : BaseCoroutine {
        public override void FixedUpdate() {
            Finished = true;
        }
    }
}
