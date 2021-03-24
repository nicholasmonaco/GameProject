using System;

namespace GameProject.Code.Core {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AnimatableValue : Attribute { }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AnimatableComponent : Attribute { }
}
