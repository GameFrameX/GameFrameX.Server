using GameFrameX.Core.Actors;

namespace GameFrameX.Core.Comps
{
    /// <summary>
    /// 组件类型标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentTypeAttribute : Attribute
    {
        public ComponentTypeAttribute(ActorType type)
        {
            ActorType = type;
        }

        public ActorType ActorType { get; }
    }
}