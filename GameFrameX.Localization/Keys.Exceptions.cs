// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

namespace GameFrameX.Localization;

/// <summary>
/// 本地化键常量定义 - Exceptions 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// 异常消息资源键
    /// </summary>
    public static class Exceptions
    {
        /// <summary>
        /// 数据库启动失败的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.Database.StartFailed
        /// 用途: 数据库服务启动失败时抛出此异常
        /// 参数: {0} - 数据库类型, {1} - 详细错误信息
        /// </remarks>
        public const string Database_Start_Failed = "Exceptions.Database.StartFailed";

        /// <summary>
        /// 处理器未初始化的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.Handler.NotInitialized
        /// 用途: 当尝试使用未初始化的处理器时抛出此异常
        /// 参数: {0} - 处理器类型名称
        /// </remarks>
        public const string Handler_Not_Initialized = "Exceptions.Handler.NotInitialized";

        /// <summary>
        /// 计时器参数无效的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.Timer.InvalidParameters
        /// 用途: 当创建计时器时传入无效参数时抛出此异常
        /// 参数: {0} - 参数名称, {1} - 参数值
        /// </remarks>
        public const string Timer_Invalid_Parameters = "Exceptions.Timer.InvalidParameters";

        /// <summary>
        /// 每周计时器日期为空的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.Timer.Weekly.EmptyDays
        /// 用途: 当创建每周计时器时未指定任何执行日期时抛出此异常
        /// </remarks>
        public const string Timer_Weekly_Empty_Days = "Exceptions.Timer.Weekly.EmptyDays";

        /// <summary>
        /// 计时器代码在热更新中的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.Timer.CodeInHotfix
        /// 用途: 当尝试在热更新中使用不支持的计时器功能时抛出此异常
        /// </remarks>
        public const string Timer_Code_In_Hotfix = "Exceptions.Timer.CodeInHotfix";

        /// <summary>
        /// IP地址格式无效的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.Network.InvalidIpFormat
        /// 用途: 当传入的IP地址格式不正确时抛出此异常
        /// 参数: {0} - 无效的IP地址字符串
        /// </remarks>
        public const string Invalid_Ip_Format = "Exceptions.Network.InvalidIpFormat";

        /// <summary>
        /// BigInteger构造函数溢出的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.BigInteger.ConstructorOverflow
        /// 用途: 当创建BigInteger时数值溢出时抛出此异常
        /// 参数: {0} - 溢出的数值
        /// </remarks>
        public const string Biginteger_Constructor_Overflow = "Exceptions.BigInteger.ConstructorOverflow";

        /// <summary>
        /// BigInteger构造函数下溢的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.BigInteger.ConstructorUnderflow
        /// 用途: 当创建BigInteger时数值下溢时抛出此异常
        /// 参数: {0} - 下溢的数值
        /// </remarks>
        public const string Biginteger_Constructor_Underflow = "Exceptions.BigInteger.ConstructorUnderflow";

        /// <summary>
        /// BigInteger无效字符串的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.BigInteger.InvalidString
        /// 用途: 当使用无效字符串创建BigInteger时抛出此异常
        /// 参数: {0} - 无效的字符串
        /// </remarks>
        public const string Biginteger_Invalid_String = "Exceptions.BigInteger.InvalidString";

        /// <summary>
        /// BigInteger构造函数字节溢出的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Exceptions.BigInteger.ConstructorByteOverflow
        /// 用途: 当使用字节数组构造BigInteger时发生溢出时使用
        /// </remarks>
        public const string Biginteger_Constructor_Byte_Overflow = "Exceptions.BigInteger.ConstructorByteOverflow";

        /// <summary>
        /// BigInteger异常消息资源键
        /// </summary>
        public static class BigInteger
        {
            /// <summary>
            /// BigInteger构造函数字节溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.BigInteger.ConstructorByteOverflow
            /// 用途: 当使用字节数组构造BigInteger时发生溢出时使用
            /// </remarks>
            public const string ConstructorByteOverflow = "Exceptions.BigInteger.ConstructorByteOverflow";
        }
    }
}