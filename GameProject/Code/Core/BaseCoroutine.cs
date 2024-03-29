﻿// BaseCoroutine.cs - Nick Monaco

namespace GameProject.Code.Core {
    
    /// <summary>
    /// The base class of all types of coroutines and yield instructions used within coroutines.
    /// </summary>
    public abstract class BaseCoroutine {

        public virtual bool Finished { get; set; } = false; // Due to how this is used in Coroutine.cs, it has to be a property.


        // These would be abstract, but there needs to be some type of default behavior, which I'm going to say is "nothing".
        public virtual void Run() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void FixedUpdate() { }

    }
}
