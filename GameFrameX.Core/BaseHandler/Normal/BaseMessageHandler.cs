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

using System.Diagnostics;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.BaseHandler.Normal;

/// <summary>
/// 基础消息处理器
/// </summary>
/// <typeparam name="TRequest">请求消息类型，必须实现 <see cref="IRequestMessage"/> 接口，且具备无参构造函数</typeparam>
/// <remarks>
/// 所有具体消息处理器均应继承自此抽象类，以实现统一的消息初始化、超时监控、性能统计与异常处理逻辑。
/// </remarks>
public abstract class BaseMessageHandler<TRequest> : IMessageHandler where TRequest : class, IRequestMessage, new()
{
    private bool _isInit;

    /// <summary>
    /// 监控器
    /// </summary>
    private Stopwatch _stopwatch;

    /// <summary>
    /// 网络频道
    /// </summary>
    public INetWorkChannel NetWorkChannel { get; private set; }

    /// <summary>
    /// 消息对象
    /// </summary>
    public TRequest Message { get; private set; }

    /// <summary>
    /// 初始化
    /// 子类实现必须调用
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <param name="netWorkChannel">网络渠道</param>
    /// <returns>返回是否初始化成功,true:成功,false:失败</returns>
    public virtual Task<bool> Init(INetworkMessage message, INetWorkChannel netWorkChannel)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        ArgumentNullException.ThrowIfNull(netWorkChannel, nameof(netWorkChannel));
        if (message is not TRequest requestMessage)
        {
            throw new InvalidCastException($"消息类型错误, {message.GetType().FullName} to: {typeof(TRequest).FullName}");
        }

        _stopwatch = new Stopwatch();
        Message = requestMessage;
        NetWorkChannel = netWorkChannel;
        _isInit = true;
        return Task.FromResult(true);
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="timeout">执行超时时间，单位毫秒，默认30秒</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>执行任务</returns>
    public virtual async Task InnerAction(int timeout = 30000, CancellationToken cancellationToken = default)
    {
        if (_isInit == false)
        {
            throw new Exception("消息处理器未初始化,请调用先Init方法，如果已经子类实现了Init方法，请调用在子类Init中调用父类Init方法");
        }

        try
        {
            var task = InnerActionAsync(Message);
            try
            {
                await task.WaitAsync(TimeSpan.FromMilliseconds(timeout), cancellationToken);
            }
            catch (TimeoutException timeoutException)
            {
                LogHelper.Fatal(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.MessageHandler.ExecutionTimeout, timeoutException.Message));
                //强制设状态-取消该操作
            }
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
        }
    }


    /// <summary>
    /// 动作异步
    /// </summary>
    /// <param name="request"></param>
    /// <returns>动作执行任务</returns>
    protected abstract Task ActionAsync(TRequest request);

    /// <summary>
    /// 内部动作异步
    /// 记录执行时间并调用 <see cref="ActionAsync" />
    /// </summary>
    /// <param name="message"></param>
    /// <returns>动作执行任务</returns>
    protected async Task InnerActionAsync(TRequest message)
    {
        if (GlobalSettings.CurrentSetting.IsMonitorTimeOut)
        {
            _stopwatch.Restart();
            await ActionAsync(message);
            _stopwatch.Stop();
            if (_stopwatch.Elapsed.Seconds >= GlobalSettings.CurrentSetting.MonitorTimeOutSeconds)
            {
                LogHelper.Warning(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.MessageHandler.ExecutionTimeWarning, GetType().Name, Message.UniqueId, _stopwatch.ElapsedMilliseconds));
            }

            return;
        }

        if (GlobalSettings.CurrentSetting.IsDebug)
        {
            _stopwatch.Restart();
            await ActionAsync(message);
            _stopwatch.Stop();
            LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.MessageHandler.ExecutionTimeDebug, GetType().Name, Message.UniqueId, _stopwatch.ElapsedMilliseconds));
            return;
        }

        await ActionAsync(message);
    }

    /// <summary>
    /// 获取当前消息处理器所处理的消息类型。
    /// 通常用于反射或泛型约束，便于框架自动识别和分发消息。
    /// </summary>
    /// <returns>消息类型的 <see cref="Type"/> 实例。</returns>
    public Type GetMessageType()
    {
        return Message.GetType();
    }

    /// <summary>
    /// 获取当前消息处理器返回的响应类型。
    /// 用于确定消息处理完成后返回的数据类型，便于类型安全和自动化处理。
    /// </summary>
    /// <returns>响应类型的 <see cref="Type"/> 实例。</returns>
    public Type GetResponseType()
    {
        return default;
    }
}