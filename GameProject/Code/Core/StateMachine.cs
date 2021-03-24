using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core {
    public class StateMachine<T> where T : StateData {

        private T[] States;

        public int CurrentStateID { get; private set; }
        public T CurrentState => States[CurrentStateID];

        private bool _changingState = false;

        public Guid GUID = Guid.Empty;




        public StateMachine(int stateCount, int startStateID) {
            States = new T[stateCount];
            CurrentStateID = startStateID;
        }

        public StateMachine(int stateCount) : this(stateCount, -1) { }

        public StateMachine() : this(1, -1) { }



        public T AddState(int stateID, T stateData) {
            if (stateID > States.Length - 1) return null;

            States[stateID] = stateData;

            if(CurrentStateID == -1) {
                ChangeState(stateID);
            }

            return stateData;
        }

        public T GetState(int stateID) {
            return States[stateID];
        }


        public void ChangeState(int newStateID) {
            if (_changingState || newStateID >= States.Length) return;

            T stateInfo = States[newStateID];

            if(stateInfo.TransitionType != StateTransitionType.Instant) {
                _changingState = true;
                //GameManager.CurrentScene.StartCoroutine(StateChangeCoroutine(stateInfo, newStateID));
            } else {
                //stateInfo.Update(); //why is this here?

                CurrentStateID = newStateID;
                _changingState = false;
                CurrentState.OnStateReset();
            }
        }

        private IEnumerator StateChangeCoroutine(T stateData, int newID) {
            switch (stateData.TransitionType) {
                case StateTransitionType.Lerp:
                    yield return GameManager.CurrentScene.StartCoroutine(stateData.Lerp_Coroutine(CurrentState), GUID);
                    break;
                case StateTransitionType.SmoothStep:
                    yield return GameManager.CurrentScene.StartCoroutine(stateData.SmoothStep_Coroutine(CurrentState), GUID);
                    break;
                case StateTransitionType.Delayed:
                    yield return GameManager.CurrentScene.StartCoroutine(stateData.Delayed_Coroutine(CurrentState), GUID);
                    break;
            }

            CurrentStateID = newID;
            _changingState = false;
            CurrentState.OnStateReset();
        }
    }

    public enum StateTransitionType {
        Instant,
        Lerp,
        SmoothStep,
        Delayed
    }
}
