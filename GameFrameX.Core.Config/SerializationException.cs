#define CPU_SUPPORT_MEMORY_NOT_ALIGN //CPU 是否支持读取非对齐内存


/// <summary>
/// TODO  
/// 1. 整理代码
/// 2. 优化序列化 (像这样 data[endPos + 1] = (byte)(x >> 8) 挨个字节赋值总感觉很低效，能优化吗)
/// </summary>
namespace GameFrameX.Core.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class SerializationException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public SerializationException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public SerializationException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}