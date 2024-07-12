using GameFrameX.Core.Actors;

namespace GameFrameX.Core.Comps
{
    /// <summary>
    /// 组件类型标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentTypeAttribute : Attribute
    {
        /// <summary>
        /// 组件类型
        /// </summary>
        /// <param name="type">组件类型</param>
        public ComponentTypeAttribute(ActorType type)
        {
            ActorType = type;
        }

        /// <summary>
        /// ActorType
        /// </summary>
        public ActorType ActorType { get; }
    }
}