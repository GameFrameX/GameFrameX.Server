using GameFrameX.Extension;

namespace GameFrameX.NetWork.HTTP
{
    /// <summary>
    /// Http 消息处理器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HttpMessageMappingAttribute : Attribute
    {
        /// <summary>
        /// 原始命令
        /// </summary>
        public string OriginalCmd { get; }

        /// <summary>
        /// 标准化的命令
        /// </summary>
        public string StandardCmd { get; }

        /// <summary>
        /// 处理器命名前缀
        /// </summary>
        public const string HTTPprefix = "";

        /// <summary>
        /// 处理器命名后缀
        /// </summary>
        public const string HTTPsuffix = "HttpHandler";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public HttpMessageMappingAttribute(Type classType)
        {
            if (classType == null)
            {
                throw new ArgumentNullException(nameof(classType));
            }

            var className = classType.Name;
            if (!classType.IsSealed)
            {
                throw new InvalidOperationException($"{className} 必须是标记为sealed的类");
            }

            // if (!className.StartsWith(HTTPprefix, StringComparison.Ordinal))
            // {
            //     throw new InvalidOperationException($"{className} 必须以{HTTPprefix}开头");
            // }

            if (!className.EndsWith(HTTPsuffix, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"{className} 必须以{HTTPsuffix}结尾");
            }

            OriginalCmd = className.Substring(HTTPprefix.Length, className.Length - HTTPprefix.Length - HTTPsuffix.Length);
            StandardCmd = OriginalCmd.ConvertToSnakeCase();
        }
    }
}