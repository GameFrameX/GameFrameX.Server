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

using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Actors;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.NetWork.Abstractions;
using Serilog;

namespace GameFrameX.Core.BaseHandler.Normal;

/// <summary>
/// 基础组件处理器基类
/// </summary>
/// <typeparam name="TRequest">请求消息类型，必须实现 <see cref="IRequestMessage"/> 并具备无参构造函数</typeparam>
public abstract class BaseComponentHandler<TRequest> : BaseMessageHandler<TRequest> where TRequest : class, IRequestMessage, new()
{
    /// <summary>
    /// 组件代理ID
    /// </summary>
    protected long ActorId { get; set; }

    /// <summary>
    /// 组件代理类型
    /// </summary>
    protected abstract Type ComponentAgentType { get; }

    /// <summary>
    /// 缓存组件代理对象
    /// </summary>
    public IComponentAgent CacheComponent { get; protected set; }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns>初始化任务</returns>
    protected abstract Task<bool> InitActor();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="message">网络消息</param>
    /// <param name="netWorkChannel">网络通道</param>
    /// <returns>返回是否初始化成功,true:成功,false:失败</returns>
    public override async Task<bool> Init(INetworkMessage message, INetWorkChannel netWorkChannel)
    {
        var initSuccess = await base.Init(message, netWorkChannel);
        if (!initSuccess)
        {
            return false;
        }

        initSuccess = await InitActor();
        if (!initSuccess)
        {
            return false;
        }

        if (CacheComponent == null)
        {
            if (ActorId == default)
            {
                LogHelper.Fatal(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.MessageHandler.ActorIdIsZero, message.GetType().FullName));
                NetWorkChannel.Close();
                return false;
            }

            try
            {
                CacheComponent = await ActorManager.GetComponentAgent(ActorId, ComponentAgentType);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "get component failed, close channel");
                NetWorkChannel.Close();
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 内部执行
    /// </summary>
    /// <param name="timeout">执行超时时间，单位毫秒，默认30秒</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>内部执行任务</returns>
    public override Task InnerAction(int timeout = 30000, CancellationToken cancellationToken = default)
    {
        if (CacheComponent == null)
        {
            LogHelper.Fatal(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.MessageHandler.CacheComponentIsNull));
            NetWorkChannel.Close();
            return Task.CompletedTask;
        }

        CacheComponent.Tell(() => InnerActionAsync(Message), timeout, cancellationToken);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent
    /// </summary>
    /// <typeparam name="TOtherAgent">组件代理类型</typeparam>
    /// <returns>组件代理任务</returns>
    protected Task<TOtherAgent> GetComponentAgent<TOtherAgent>() where TOtherAgent : IComponentAgent
    {
        if (CacheComponent == null)
        {
            LogHelper.Fatal(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.MessageHandler.CacheComponentIsNull));
            NetWorkChannel.Close();
            return default;
        }

        return CacheComponent.GetComponentAgent<TOtherAgent>();
    }
}