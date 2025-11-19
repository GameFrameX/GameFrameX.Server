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
/// 本地化键常量定义 - DiscoveryCenterManager 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// DiscoveryCenterManager模块相关日志和错误消息资源键
    /// </summary>
    public static class DiscoveryCenterManager
    {
        /// <summary>
        /// 尝试连接到目标服务器...{0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Client.TryConnectToServer
        /// 用途: 当客户端尝试连接到目标服务器时记录调试信息
        /// 参数: {0} - 服务器主机地址
        /// </remarks>
        public const string TryConnectToServer = "DiscoveryCenterManager.Client.TryConnectToServer";

        /// <summary>
        /// 第{0}次重连,断开连接,原因:{1}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Client.ReconnectAttempt
        /// 用途: 当客户端未连接到目标服务器且进行重连时记录日志
        /// 参数: {0} - 当前重连次数, {1} - 最大重连次数
        /// </remarks>
        public const string ReconnectAttempt = "DiscoveryCenterManager.Client.ReconnectAttempt";

        /// <summary>
        /// 重连次数已达上限: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Client.ReconnectLimitReached
        /// 用途: 当重连次数达到最大限制时记录日志
        /// 参数: {0} - 最大重连次数
        /// </remarks>
        public const string ReconnectLimitReached = "DiscoveryCenterManager.Client.ReconnectLimitReached";

        /// <summary>
        /// 发生错误:{0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Client.ErrorOccurred
        /// 用途: 当客户端发生错误时记录错误信息
        /// 参数: {0} - 异常消息
        /// </remarks>
        public const string ErrorOccurred = "DiscoveryCenterManager.Client.ErrorOccurred";

        /// <summary>
        /// 客户端与服务器断开连接: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Client.DisconnectedFromServer
        /// 用途: 当客户端与服务器断开连接时记录日志
        /// 参数: {0} - 服务器主机地址
        /// </remarks>
        public const string DisconnectedFromServer = "DiscoveryCenterManager.Client.DisconnectedFromServer";

        /// <summary>
        /// 客户端成功连接到服务器: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Client.ConnectedToServer
        /// 用途: 当客户端成功连接到服务器时记录日志
        /// 参数: {0} - 服务器主机地址
        /// </remarks>
        public const string ConnectedToServer = "DiscoveryCenterManager.Client.ConnectedToServer";

        /// <summary>
        /// 无效的IP地址格式: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Service.InvalidIpAddress
        /// 用途: 当DiscoveryCenterHost配置的端口号不是有效IP地址时记录错误
        /// 参数: {0} - 端口号
        /// </remarks>
        public const string InvalidIpAddress = "DiscoveryCenterManager.Service.InvalidIpAddress";

        /// <summary>
        /// 不能添加自己节点,{0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Server.CannotAddSelfNode
        /// 用途: 当尝试添加与自身服务器类型相同的节点时记录错误
        /// 参数: {0} - 自身服务类型, {1} - 节点信息
        /// </remarks>
        public const string CannotAddSelfNode = "DiscoveryCenterManager.Server.CannotAddSelfNode";

        /// <summary>
        /// 重复节点添加，已忽略: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Server.DuplicateNodeAddition
        /// 用途: 当尝试添加已存在的节点时记录警告
        /// 参数: {0} - 节点信息
        /// </remarks>
        public const string DuplicateNodeAddition = "DiscoveryCenterManager.Server.DuplicateNodeAddition";

        /// <summary>
        /// 当前网络节点总数: {0}，新节点信息: {1}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Server.NetworkNodeCount
        /// 用途: 记录当前网络节点数量和新节点信息
        /// 参数: {0} - 节点总数, {1} - 新节点信息的序列化
        /// </remarks>
        public const string NetworkNodeCount = "DiscoveryCenterManager.Server.NetworkNodeCount";

        /// <summary>
        /// 节点加入,{0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Server.NodeJoined
        /// 用途: 当有新节点加入时记录日志
        /// 参数: {0} - 节点信息
        /// </remarks>
        public const string NodeJoined = "DiscoveryCenterManager.Server.NodeJoined";

        /// <summary>
        /// 节点移除,{0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Server.NodeRemoved
        /// 用途: 当节点被移除时记录日志
        /// 参数: {0} - 节点信息
        /// </remarks>
        public const string NodeRemoved = "DiscoveryCenterManager.Server.NodeRemoved";

        /// <summary>
        /// 无效的URL格式: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.NamingService.InvalidUrlFormat
        /// 用途: 当URL格式无效时记录错误
        /// 参数: {0} - URL字符串
        /// </remarks>
        public const string NamingServiceInvalidUrlFormat = "DiscoveryCenterManager.NamingService.InvalidUrlFormat";

        /// <summary>
        /// 发生错误:{0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.NamingService.ErrorOccurred
        /// 用途: 当命名服务发生错误时记录错误
        /// 参数: {0} - 异常消息
        /// </remarks>
        public const string NamingServiceErrorOccurred = "DiscoveryCenterManager.NamingService.ErrorOccurred";

        /// <summary>
        /// 尝试连接服务: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.NamingService.ServiceConnectionAttempt
        /// 用途: 当尝试连接服务时记录日志
        /// 参数: {0} - 服务信息
        /// </remarks>
        public const string NamingServiceConnectionAttempt = "DiscoveryCenterManager.NamingService.ServiceConnectionAttempt";

        /// <summary>
        /// messageObject.MessageId必须小于0
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Exception.MessageIdMustBeLessThanZero
        /// 用途: 当消息对象的MessageId不小于0时抛出异常
        /// 参数: 无
        /// </remarks>
        public const string MessageIdMustBeLessThanZero = "DiscoveryCenterManager.Exception.MessageIdMustBeLessThanZero";

        /// <summary>
        /// 重连尝试已达上限({0})，将不再进行重连。
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Client.ReconnectLimitReachedMessage
        /// 用途: 当重连次数达到最大限制时记录日志
        /// 参数: {0} - 最大重连次数
        /// </remarks>
        public const string ReconnectLimitReachedMessage = "DiscoveryCenterManager.Client.ReconnectLimitReachedMessage";

        /// <summary>
        /// 客户端与服务器断开连接: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Client.DisconnectedMessage
        /// 用途: 当客户端断开连接时记录日志
        /// 参数: {0} - 服务器地址
        /// </remarks>
        public const string DisconnectedMessage = "DiscoveryCenterManager.Client.DisconnectedMessage";

        /// <summary>
        /// 客户端成功连接到服务器: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Client.ConnectedMessage
        /// 用途: 当客户端成功连接时记录日志
        /// 参数: {0} - 服务器地址
        /// </remarks>
        public const string ConnectedMessage = "DiscoveryCenterManager.Client.ConnectedMessage";

        /// <summary>
        /// 重复节点添加...已忽略: {0}
        /// </summary>
        /// <remarks>
        /// 键名: DiscoveryCenterManager.Server.DuplicateNodeWarning
        /// 用途: 当尝试添加重复节点时记录警告
        /// 参数: {0} - 节点信息
        /// </remarks>
        public const string DuplicateNodeWarning = "DiscoveryCenterManager.Server.DuplicateNodeWarning";
    }
}