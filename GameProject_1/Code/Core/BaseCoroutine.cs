namespace GameProject.Code.Core {
    public abstract class BaseCoroutine {

        public virtual bool Finished { get; set; } // Due to how this is used in Coroutine.cs, it has to be a property.


        // These would be abstract, but there needs to be some type of default behavior, which I'm going to say is "nothing".
        public virtual void Run() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void FixedUpdate() { }

    }
}
