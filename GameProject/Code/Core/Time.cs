// Time.cs - Nick Monaco

using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Variables related to time.
    /// </summary>
    public static class Time {
        public static float time;
        public static float deltaTime { get; private set; }
        public static float unscaledDeltaTime { get; private set; }

        //public static readonly float fixedDeltaTime = 0.02f; // 50 times per second; standard
        //public static readonly float fixedDeltaTime = 0.015625f; // 64 times per second  
        public static float fixedDeltaTime { get; private set; } = 1 / 60f; // 60 times per second
        public static readonly float unscaledFixedDeltaTime = 1 / 60f;


        private static float _timeScale = 1;
        public static float TimeScale {
            get => _timeScale;
            set {
                _timeScale = MathHelper.Clamp(value, 0, 20);
                fixedDeltaTime = unscaledFixedDeltaTime * _timeScale;
            }
        }

        public static void SetDeltaTime(float newDeltaTime) {
            deltaTime = newDeltaTime * TimeScale;
            unscaledDeltaTime = newDeltaTime;
        }
    }
}
