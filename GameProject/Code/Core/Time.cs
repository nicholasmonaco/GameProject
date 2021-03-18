// Time.cs - Nick Monaco

using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Variables related to time.
    /// </summary>
    public static class Time {
        public static float time;
        
        public static float unscaledDeltaTime { get; private set; }
        public static readonly float unscaledFixedDeltaTime = 1 / 60f; // 60 times per second

        public static float deltaTime { get; private set; }
        public static float fixedDeltaTime { get; private set; } = 1 / 60f; 

        public static float entityDeltaTime { get; private set; }
        public static float entityFixedDeltaTime { get; private set; } = 1 / 60f;



        private static float _timeScale = 1;
        public static float TimeScale {
            get => _timeScale;
            set {
                _timeScale = MathHelper.Clamp(value, 0, 20);
                fixedDeltaTime = unscaledFixedDeltaTime * _timeScale;
            }
        }

        private static float _entityTimeScale = 1;
        public static float EntityTimeScale {
            get => _entityTimeScale;
            set {
                _entityTimeScale = MathHelper.Clamp(value, 0, 20);
                entityFixedDeltaTime = fixedDeltaTime * _entityTimeScale;
            }
        }


        public static void SetDeltaTime(float newDeltaTime) {
            unscaledDeltaTime = newDeltaTime;
            deltaTime = newDeltaTime * TimeScale;
            entityDeltaTime = deltaTime * EntityTimeScale;
        }
    }
}
