// Wait.cs - Nick Monaco

using System;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// The container for all types of "waiting" yield instructions.
    /// </summary>
    class Wait { }


    /// <summary>
    /// A yield instruction that allows a certain amount of time to pass before finishing.
    /// </summary>
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


    /// <summary>
    /// A yield instruction that waits for the end of the current frame to finish.
    /// </summary>
    public class WaitForEndOfFrame : BaseCoroutine {
        public override void LateUpdate() {
            Finished = true;
        }
    }


    /// <summary>
    /// A yield instruction that waits for FixedUpdate() to be called in order to finish.
    /// </summary>
    public class WaitForFixedUpdate : BaseCoroutine {
        public override void FixedUpdate() {
            Finished = true;
        }
    }
}
