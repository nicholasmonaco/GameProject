// Time.cs - Nick Monaco

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Variables related to time.
    /// </summary>
    public static class Time {
        public static float time;
        public static float deltaTime;
        //public static readonly float fixedDeltaTime = 0.02f; // 50 times per second; standard
        //public static readonly float fixedDeltaTime = 0.015625f; // 64 times per second  
        public static readonly float fixedDeltaTime = 1/60f; // 60 times per second
    }
}
