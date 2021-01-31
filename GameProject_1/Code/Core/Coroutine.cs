// Coroutine.cs - Nick Monaco

using System.Collections;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Governs the inner workings of coroutines.
    /// </summary>
    public class Coroutine : BaseCoroutine {
        private IEnumerator _routineCode;

        public Coroutine(IEnumerator routine) {
            _routineCode = routine;
        }

        public override void Update() {
            // So here we can essentially read what we "yield return" one-by-one.
            // Therefore: If it's null or the coroutine is finished, we just move on.
            // If waiting for end of frame, wait until then to update. (Handled in LateUpdate())
            // If waiting for a set amount of time, wait until that time is over.

            BaseCoroutine curRoutine = _routineCode.Current as BaseCoroutine;

            if (curRoutine == null || curRoutine.Finished) {
                StepThrough();
                return;
            }

            switch (_routineCode.Current) {
                case WaitForSeconds _: // Variables are unneeded here, Visual Studio reccommended these underscore discard variable things.
                case Coroutine _:
                    curRoutine.Update();
                    break;
            }
        }

        public override void LateUpdate() {
            BaseCoroutine curRoutine = _routineCode.Current as BaseCoroutine;
            if (curRoutine.Finished) StepThrough();

            switch (_routineCode.Current) {
                case WaitForEndOfFrame _:
                    curRoutine.LateUpdate();
                    break;
            }
        }

        public override void FixedUpdate() {
            BaseCoroutine curRoutine = _routineCode.Current as BaseCoroutine;
            if (curRoutine.Finished) StepThrough();

            switch (_routineCode.Current) {
                case WaitForFixedUpdate _:
                    curRoutine.FixedUpdate();
                    break;
            }
        }


        public void StepThrough() {
            if (_routineCode.MoveNext()) {
                // This "as" usage effectively just lets a null check be sufficient instead of dealing with error catching.
                BaseCoroutine steppedCoroutine = _routineCode.Current as BaseCoroutine;

                if (steppedCoroutine != null) {
                    steppedCoroutine.Run();
                }
            } else {
                Finished = true;
            }
        }
    }
}
