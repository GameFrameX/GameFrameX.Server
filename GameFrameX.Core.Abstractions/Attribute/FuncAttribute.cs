namespace GameFrameX.Core.Abstractions.Attribute
{
    /// <summary>
    /// 有关组件的功能
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FuncAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly short Func;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        public FuncAttribute(short func)
        {
            Func = func;
        }
    }
}