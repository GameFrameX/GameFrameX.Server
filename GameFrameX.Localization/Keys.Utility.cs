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
/// 本地化键常量定义 - Utility 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// Utility模块异常消息资源键
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// ActorId小于最小服务器ID的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Utility.Exceptions.ActorIdLessThanMinServerId
        /// 用途: 当传入的ActorId小于最小服务器ID时使用
        /// 参数: {0} - 最小服务器ID值
        /// </remarks>
        public const string ActorIdLessThanMinServerId = "Utility.Exceptions.ActorIdLessThanMinServerId";

        /// <summary>
        /// ActorId小于最小服务器ID的详细错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Utility.Exceptions.ActorIdLessThanMinServerIdDetail
        /// 用途: 当传入的ActorId小于最小服务器ID时使用，包含具体的ActorId值
        /// 参数: {0} - ActorId值, {1} - 最小服务器ID值
        /// </remarks>
        public const string ActorIdLessThanMinServerIdDetail = "Utility.Exceptions.ActorIdLessThanMinServerIdDetail";

        /// <summary>
        /// Actor类型错误的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Utility.Exceptions.ActorTypeError
        /// 用途: 当传入的Actor类型无效时使用
        /// 参数: {0} - 错误的Actor类型值
        /// </remarks>
        public const string ActorTypeError = "Utility.Exceptions.ActorTypeError";

        /// <summary>
        /// 服务器ID为负数的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Utility.Exceptions.ServerIdNegative
        /// 用途: 当服务器ID为负数时使用
        /// 参数: {0} - 负数的服务器ID值
        /// </remarks>
        public const string ServerIdNegative = "Utility.Exceptions.ServerIdNegative";

        /// <summary>
        /// 服务器ID小于等于0的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Utility.Exceptions.ServerIdLessThanOrEqualZero
        /// 用途: 当服务器ID小于等于0时使用
        /// </remarks>
        public const string ServerIdLessThanOrEqualZero = "Utility.Exceptions.ServerIdLessThanOrEqualZero";

        /// <summary>
        /// Actor类型无效的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Utility.Exceptions.ActorTypeInvalid
        /// 用途: 当Actor类型超出有效范围时使用
        /// </remarks>
        public const string ActorTypeInvalid = "Utility.Exceptions.ActorTypeInvalid";

        /// <summary>
        /// 模块ID无效的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Utility.Exceptions.ModuleInvalid
        /// 用途: 当模块ID超出有效范围时使用
        /// </remarks>
        public const string ModuleInvalid = "Utility.Exceptions.ModuleInvalid";

        /// <summary>
        /// 构造函数溢出的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: Utility.Exceptions.ConstructorOverflow
        /// 用途: 当构造函数发生正溢出时使用
        /// </remarks>
        public const string ConstructorOverflow = "Utility.Exceptions.ConstructorOverflow";

        /// <summary>
        /// 异常消息资源键
        /// </summary>
        public static class Exceptions
        {
            /// <summary>
            /// ActorId小于最小服务器ID的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorIdLessThanMinServerId
            /// 用途: 当传入的ActorId小于最小服务器ID时使用
            /// 参数: {0} - 最小服务器ID值
            /// </remarks>
            public const string ActorIdLessThanMinServerId = "Utility.Exceptions.ActorIdLessThanMinServerId";

            /// <summary>
            /// ActorId小于最小服务器ID的详细错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorIdLessThanMinServerIdDetail
            /// 用途: 当传入的ActorId小于最小服务器ID时使用，包含具体的ActorId值
            /// 参数: {0} - ActorId值, {1} - 最小服务器ID值
            /// </remarks>
            public const string ActorIdLessThanMinServerIdDetail = "Utility.Exceptions.ActorIdLessThanMinServerIdDetail";

            /// <summary>
            /// 输入为全局ID的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.InputIsGlobalId
            /// 用途: 当尝试对全局ID执行不允许的操作时使用
            /// 参数: {0} - 全局ID值
            /// </remarks>
            public const string InputIsGlobalId = "Utility.Exceptions.InputIsGlobalId";

            /// <summary>
            /// Actor类型错误的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorTypeError
            /// 用途: 当传入的Actor类型无效时使用
            /// 参数: {0} - 错误的Actor类型值
            /// </remarks>
            public const string ActorTypeError = "Utility.Exceptions.ActorTypeError";

            /// <summary>
            /// 服务器ID为负数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ServerIdNegative
            /// 用途: 当服务器ID为负数时使用
            /// 参数: {0} - 负数的服务器ID值
            /// </remarks>
            public const string ServerIdNegative = "Utility.Exceptions.ServerIdNegative";

            /// <summary>
            /// 服务器ID小于等于0的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ServerIdLessThanOrEqualZero
            /// 用途: 当服务器ID小于等于0时使用
            /// </remarks>
            public const string ServerIdLessThanOrEqualZero = "Utility.Exceptions.ServerIdLessThanOrEqualZero";

            /// <summary>
            /// Actor类型无效的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorTypeInvalid
            /// 用途: 当Actor类型超出有效范围时使用
            /// </remarks>
            public const string ActorTypeInvalid = "Utility.Exceptions.ActorTypeInvalid";

            /// <summary>
            /// 模块ID无效的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ModuleInvalid
            /// 用途: 当模块ID超出有效范围时使用
            /// </remarks>
            public const string ModuleInvalid = "Utility.Exceptions.ModuleInvalid";

            /// <summary>
            /// 基数超出范围的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.RadixOutOfRange
            /// 用途: 当ToString方法的基数参数不在2-36范围内时使用
            /// </remarks>
            public const string RadixOutOfRange = "Utility.Exceptions.RadixOutOfRange";

            /// <summary>
            /// 仅支持正指数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.PositiveExponentsOnly
            /// 用途: 当模幂运算的指数为负数时使用
            /// </remarks>
            public const string PositiveExponentsOnly = "Utility.Exceptions.PositiveExponentsOnly";

            /// <summary>
            /// 参数k必须为奇数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ParameterKMustBeOdd
            /// 用途: 当Lucas序列参数k不为奇数时使用
            /// </remarks>
            public const string ParameterKMustBeOdd = "Utility.Exceptions.ParameterKMustBeOdd";

            /// <summary>
            /// 雅可比符号仅定义于奇数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.JacobiSymbolDefinedForOddOnly
            /// 用途: 当雅可比符号计算的参数不为奇数时使用
            /// </remarks>
            public const string JacobiSymbolDefinedForOddOnly = "Utility.Exceptions.JacobiSymbolDefinedForOddOnly";

            /// <summary>
            /// 没有逆的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.NoInverse
            /// 用途: 当模逆不存在时使用
            /// </remarks>
            public const string NoInverse = "Utility.Exceptions.NoInverse";

            /// <summary>
            /// 所需位数超过最大长度的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.RequiredBitsExceedMaxLength
            /// 用途: 当生成随机位数超过BigInteger最大长度时使用
            /// </remarks>
            public const string RequiredBitsExceedMaxLength = "Utility.Exceptions.RequiredBitsExceedMaxLength";

            /// <summary>
            /// 乘法溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.MultiplicationOverflow
            /// 用途: 当BigInteger乘法运算溢出时使用
            /// </remarks>
            public const string MultiplicationOverflow = "Utility.Exceptions.MultiplicationOverflow";

            /// <summary>
            /// 自增溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.IncrementOverflow
            /// 用途: 当BigInteger自增运算溢出时使用
            /// </remarks>
            public const string IncrementOverflow = "Utility.Exceptions.IncrementOverflow";

            /// <summary>
            /// 自减下溢的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.DecrementUnderflow
            /// 用途: 当BigInteger自减运算下溢时使用
            /// </remarks>
            public const string DecrementUnderflow = "Utility.Exceptions.DecrementUnderflow";

            /// <summary>
            /// 取负溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.NegationOverflow
            /// 用途: 当BigInteger取负运算溢出时使用
            /// </remarks>
            public const string NegationOverflow = "Utility.Exceptions.NegationOverflow";

            /// <summary>
            /// 构造函数字节溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ConstructorByteOverflow
            /// 用途: 当使用字节数组构造BigInteger时发生溢出时使用
            /// </remarks>
            public const string ConstructorByteOverflow = "Utility.Exceptions.ConstructorByteOverflow";

            /// <summary>
            /// 构造函数正溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ConstructorOverflow
            /// 用途: 当构造函数发生正溢出时使用
            /// </remarks>
            public const string ConstructorOverflow = "Utility.Exceptions.ConstructorOverflow";

            /// <summary>
            /// 构造函数负下溢的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ConstructorUnderflow
            /// 用途: 当构造函数发生负下溢时使用
            /// </remarks>
            public const string ConstructorUnderflow = "Utility.Exceptions.ConstructorUnderflow";

            /// <summary>
            /// 构造函数无效字符串的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ConstructorInvalidString
            /// 用途: 当构造函数接收到无效字符串时使用
            /// </remarks>
            public const string ConstructorInvalidString = "Utility.Exceptions.ConstructorInvalidString";

            /// <summary>
            /// 时间戳超出范围的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.TimestampOutOfRange
            /// 用途: 当传入的时间戳无法转换为有效的DateTime时使用
            /// </remarks>
            public const string TimestampOutOfRange = "Utility.Exceptions.TimestampOutOfRange";

            /// <summary>
            /// 系统时钟回退的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ClockMovedBackwards
            /// 用途: 当检测到系统时钟回退时，雪花算法ID生成器抛出此异常
            /// 参数: {0} - 回退的毫秒数
            /// </remarks>
            public const string ClockMovedBackwards = "Utility.Exceptions.ClockMovedBackwards";

            /// <summary>
            /// Worker ID 超出范围的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.WorkerIdOutOfRange
            /// 用途: 当创建IdWorker时传入的WorkerId超出有效范围时使用
            /// 参数: {0} - 最大允许的WorkerId值
            /// </remarks>
            public const string WorkerIdOutOfRange = "Utility.Exceptions.WorkerIdOutOfRange";

            /// <summary>
            /// Datacenter ID 超出范围的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.DatacenterIdOutOfRange
            /// 用途: 当创建IdWorker时传入的DatacenterId超出有效范围时使用
            /// 参数: {0} - 最大允许的DatacenterId值
            /// </remarks>
            public const string DatacenterIdOutOfRange = "Utility.Exceptions.DatacenterIdOutOfRange";
        }

        /// <summary>
        /// LNumber模块相关日志和错误消息资源键
        /// </summary>
        public static class LNumber
        {
            /// <summary>
            /// Xnumber创建失败的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.LNumber.CreateFailed
            /// 用途: 当创建Xnumber时传入无效参数时记录
            /// 参数: {0} - 整数部分, {1} - 小数部分
            /// </remarks>
            public const string CreateFailed = "Utility.LNumber.CreateFailed";

            /// <summary>
            /// Number数据超上限的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.LNumber.DataExceedsLimit
            /// 用途: 当Number数据超过上限时记录
            /// 参数: {0} - 操作描述, {1} - 数值
            /// </remarks>
            public const string DataExceedsLimit = "Utility.LNumber.DataExceedsLimit";

            /// <summary>
            /// LNumber乘法越界的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.LNumber.MultiplicationOverflow
            /// 用途: 当LNumber乘法运算越界时记录
            /// 参数: {0} - 计算结果
            /// </remarks>
            public const string MultiplicationOverflow = "Utility.LNumber.MultiplicationOverflow";

            /// <summary>
            /// LNumber除法越界的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.LNumber.DivisionOverflow
            /// 用途: 当LNumber除法运算越界时记录
            /// 参数: {0} - 计算结果
            /// </remarks>
            public const string DivisionOverflow = "Utility.LNumber.DivisionOverflow";
        }

        /// <summary>
        /// 网络模块相关错误消息资源键
        /// </summary>
        public static class Network
        {
            /// <summary>
            /// 无效IP地址格式的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Network.InvalidIpAddressFormat
            /// 用途: 当传入的IP地址格式无效时使用
            /// </remarks>
            public const string InvalidIpAddressFormat = "Utility.Network.InvalidIpAddressFormat";
        }

        /// <summary>
        /// 敏感词检测模块相关日志消息资源键
        /// </summary>
        public static class IllegalWordDetection
        {
            /// <summary>
            /// 敏感词初始化完成的消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.IllegalWordDetection.InitializationComplete
            /// 用途: 敏感词系统初始化完成时记录
            /// 参数: {0} - 耗时(毫秒), {1} - 有效数量
            /// </remarks>
            public const string InitializationComplete = "Utility.IllegalWordDetection.InitializationComplete";
        }

        /// <summary>
        /// 设置模块相关日志和错误消息资源键
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// 应用程序已退出，不能再次开启的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Settings.AppAlreadyExited
            /// 用途: 当尝试开启已经退出的应用程序时记录
            /// </remarks>
            public const string AppAlreadyExited = "Utility.Settings.AppAlreadyExited";

            /// <summary>
            /// 保存数据间隔过小的警告消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Settings.SaveDataIntervalTooSmall
            /// 用途: 当SaveDataInterval小于5000毫秒时记录警告
            /// 参数: {0} - 使用的默认值(毫秒)
            /// </remarks>
            public const string SaveDataIntervalTooSmall = "Utility.Settings.SaveDataIntervalTooSmall";
        }
    }
}