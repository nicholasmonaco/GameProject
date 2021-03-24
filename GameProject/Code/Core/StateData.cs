using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core {
    public abstract class StateData {
        public StateTransitionType TransitionType = StateTransitionType.Instant;
        public float TransitionData = 0;

        public List<Action<int, int, int>> TransitionToLogics = new List<Action<int, int, int>>(1);


        public abstract void Update(float progress = 1);

        public virtual void OnStateReset() { }

        public abstract IEnumerator Lerp_Coroutine(StateData otherState);

        public abstract IEnumerator SmoothStep_Coroutine(StateData otherState);

        public abstract IEnumerator Delayed_Coroutine(StateData otherState);

        public abstract void Destroy();
    }
}
