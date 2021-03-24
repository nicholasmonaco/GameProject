using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Animation {
    public class AnimationController : Component {

        public AnimationController(GameObject attached, int stateCount) : base(attached) {
            StateMachine = new StateMachine<Animation>(stateCount);
        }



        public Animation CurrentAnimation => StateMachine.CurrentState;

        public int CurrentAnimationID { get; private set; }


        public StateMachine<Animation> StateMachine;


        public void ChangeAnimationState(int stateID) {
            if (stateID == CurrentAnimationID) return; //Disallow looping into it's own state

            StateMachine.ChangeState(stateID);
            CurrentAnimationID = stateID;
        }



        public override void Update() {
            CheckStateSwitch();

            CurrentAnimation.Update();
        }


        public void CheckStateSwitch() {
            foreach(Action<int, int, int> check in CurrentAnimation.TransitionToLogics) {
                check(CurrentAnimationID, CurrentAnimation.CurrentFrameIndex, CurrentAnimation.FrameDuration);
            }
        }
    }
}
