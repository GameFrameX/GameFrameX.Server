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
/// 本地化键常量定义 - NetWork 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// NetWork模块相关日志和错误消息资源键
    /// </summary>
    public static class NetWork
    {
        /// <summary>
        /// 发送消息的日志消息
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.MessageSent
        /// 用途: 记录发送消息的日志信息
        /// 参数: {0} - 格式化的消息内容
        /// </remarks>
        public const string MessageSent = "NetWork.MessageSent";

        /// <summary>
        /// 消息发送超时被取消的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.MessageSendTimeout
        /// 用途: 当消息发送超时被取消时记录错误
        /// 参数: {0} - 异常消息
        /// </remarks>
        public const string MessageSendTimeout = "NetWork.MessageSendTimeout";

        /// <summary>
        /// 类型必须实现接口的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.TypeMustImplementInterface
        /// 用途: 当类型未实现指定接口时抛出异常
        /// 参数: {0} - 类型名称, {1} - 接口名称
        /// </remarks>
        public const string TypeMustImplementInterface = "NetWork.TypeMustImplementInterface";

        /// <summary>
        /// 类型必须是类的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.TypeMustBeClass
        /// 用途: 当类型不是类时抛出异常
        /// 参数: {0} - 类型名称
        /// </remarks>
        public const string TypeMustBeClass = "NetWork.TypeMustBeClass";

        /// <summary>
        /// 类型必须有无参构造函数的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.TypeMustHaveParameterlessConstructor
        /// 用途: 当类型没有无参构造函数时抛出异常
        /// 参数: {0} - 类型名称
        /// </remarks>
        public const string TypeMustHaveParameterlessConstructor = "NetWork.TypeMustHaveParameterlessConstructor";

        /// <summary>
        /// 无法找到Create方法的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.CannotFindCreateMethod
        /// 用途: 当无法在ObjectPoolProvider上找到Create方法时抛出异常
        /// </remarks>
        public const string CannotFindCreateMethod = "NetWork.CannotFindCreateMethod";

        /// <summary>
        /// 创建对象池失败的错误消息
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.FailedToCreateObjectPool
        /// 用途: 当无法为类型创建对象池时抛出异常
        /// 参数: {0} - 类型名称
        /// </remarks>
        public const string FailedToCreateObjectPool = "NetWork.FailedToCreateObjectPool";
    }
}