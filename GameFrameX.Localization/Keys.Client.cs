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
/// 本地化键常量定义 - Client 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// 客户端相关日志消息资源键
    /// </summary>
    public static class Client
    {
        /// <summary>
        /// 客户端尝试连接到服务器的消息
        /// </summary>
        /// <remarks>
        /// 键名: Client.AttemptingToConnect
        /// 用途: 客户端开始连接到服务器时记录
        /// </remarks>
        public const string AttemptingToConnect = "Client.AttemptingToConnect";

        /// <summary>
        /// 客户端重试连接的消息
        /// </summary>
        /// <remarks>
        /// 键名: Client.RetryConnect
        /// 用途: 客户端未连接到服务器，准备重试时记录
        /// 参数: {0} - 当前尝试次数, {1} - 最大重试次数
        /// </remarks>
        public const string RetryConnect = "Client.RetryConnect";

        /// <summary>
        /// 客户端达到最大重试次数的消息
        /// </summary>
        /// <remarks>
        /// 键名: Client.MaxRetryReached
        /// 用途: 客户端重连次数达到上限时记录
        /// </remarks>
        public const string MaxRetryReached = "Client.MaxRetryReached";

        /// <summary>
        /// 客户端发生错误的消息
        /// </summary>
        /// <remarks>
        /// 键名: Client.ErrorOccurred
        /// 用途: 客户端操作过程中发生错误时记录
        /// 参数: {0} - 错误信息
        /// </remarks>
        public const string ErrorOccurred = "Client.ErrorOccurred";

        /// <summary>
        /// 客户端断开连接的消息
        /// </summary>
        /// <remarks>
        /// 键名: Client.Disconnected
        /// 用途: 客户端与服务器断开连接时记录
        /// </remarks>
        public const string Disconnected = "Client.Disconnected";

        /// <summary>
        /// 客户端成功连接到服务器的消息
        /// </summary>
        /// <remarks>
        /// 键名: Client.ConnectedSuccessfully
        /// 用途: 客户端成功连接到服务器时记录
        /// </remarks>
        public const string ConnectedSuccessfully = "Client.ConnectedSuccessfully";
    }
}