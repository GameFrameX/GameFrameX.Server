using GameAnalyticsSDK.Net;
using GameFrameX.Extension;

namespace GameFrameX.GameAnalytics;

/// <summary>
/// 游戏分析帮助类
/// </summary>
public static class GameAnalyticsHelper
{
    private static bool _isInit = false;

    /// <summary>
    /// 初始化,
    /// https://docs.gameanalytics.com/integrations/sdk/c-sharp#initialize-the-sdk
    /// </summary>
    /// <param name="gameKey"></param>
    /// <param name="gameSecret"></param>
    /// <param name="version">程序集版本</param>
    public static void Init(string gameKey, string gameSecret, string version)
    {
        gameKey.CheckNotNullOrEmptyOrWhiteSpace(nameof(gameKey));
        gameSecret.CheckNotNullOrEmptyOrWhiteSpace(nameof(gameSecret));
        version.CheckNotNullOrEmptyOrWhiteSpace(nameof(version));

        GameAnalyticsSDK.Net.GameAnalytics.ConfigureBuild("game_frame_x server " + version);
        GameAnalyticsSDK.Net.GameAnalytics.Initialize(gameKey, gameSecret);
        _isInit = true;
    }

    /// <summary>
    /// 设置全局自定义事件字段
    /// </summary>
    /// <param name="customFields">自定义参数</param>
    public static void SetGlobalCustomEventFields(Dictionary<string, object> customFields)
    {
        if (_isInit == false)
        {
            return;
        }

        customFields.CheckNotNull(nameof(customFields));
        GameAnalyticsSDK.Net.GameAnalytics.SetGlobalCustomEventFields(customFields);
    }

    /// <summary>
    /// 提交自定义事件 ID。用于跟踪游戏特别需要的指标。
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <param name="customFields">自定义参数</param>
    public static void Event(string eventId, Dictionary<string, object> customFields = null)
    {
        if (_isInit == false)
        {
            return;
        }

        eventId.CheckNotNullOrEmptyOrWhiteSpace(nameof(eventId));
        GameAnalyticsSDK.Net.GameAnalytics.AddDesignEvent(eventId, customFields);
    }

    /// <summary>
    /// 在 GA 服务器上支持收据验证的应用内购买。
    /// </summary>
    /// <param name="currency">货币类型</param>
    /// <param name="amount">数量</param>
    /// <param name="itemType">道具类型</param>
    /// <param name="itemId">道具ID</param>
    /// <param name="cartType">购买方式</param>
    /// <param name="customFields">自定义参数</param>
    /// <param name="mergeFields">是否合并参数</param>
    public static void Business(string currency, int amount, string itemType, string itemId, string cartType, IDictionary<string, object> customFields = null, bool mergeFields = false)
    {
        if (_isInit == false)
        {
            return;
        }

        GameAnalyticsSDK.Net.GameAnalytics.AddBusinessEvent(currency, amount, itemType, itemId, cartType, customFields, mergeFields);
    }

    /// <summary>
    /// 级别尝试与开始，失败和完成事件。
    /// </summary>
    /// <param name="progressionStatus">进程状态</param>
    /// <param name="progression01">进程1</param>
    /// <param name="progression02">进程2</param>
    /// <param name="progression03">进程3</param>
    /// <param name="score">分数</param>
    /// <param name="customFields">自定义参数</param>
    /// <param name="mergeFields">是否合并参数</param>
    public static void Progression(ProgressionStatus progressionStatus, string progression01, string progression02, string progression03, double score, IDictionary<string, object> customFields = null, bool mergeFields = false)
    {
        if (_isInit == false)
        {
            return;
        }

        EGAProgressionStatus status = EGAProgressionStatus.Undefined;
        switch (progressionStatus)
        {
            case ProgressionStatus.Start:
                status = EGAProgressionStatus.Start;
                break;
            case ProgressionStatus.Complete:
                status = EGAProgressionStatus.Complete;
                break;
            case ProgressionStatus.Fail:
                status = EGAProgressionStatus.Fail;
                break;
        }

        GameAnalyticsSDK.Net.GameAnalytics.AddProgressionEvent(status, progression01, progression02, progression03, score, customFields, mergeFields);
    }

    /// <summary>
    /// 管理虚拟货币的流动 - 如 gems 或 lives
    /// </summary>
    /// <param name="resourceFlowType">资源流动类型</param>
    /// <param name="currency">货币</param>
    /// <param name="amount">数量</param>
    /// <param name="itemType">道具类型</param>
    /// <param name="itemId">道具ID</param>
    /// <param name="customFields">自定义字段</param>
    /// <param name="mergeFields">是否合并字段</param>
    public static void Resource(ResourceFlowType resourceFlowType, string currency, float amount, string itemType, string itemId, IDictionary<string, object> customFields = null, bool mergeFields = false)
    {
        if (_isInit == false)
        {
            return;
        }

        EGAResourceFlowType flowType = EGAResourceFlowType.Undefined;
        switch (resourceFlowType)
        {
            case ResourceFlowType.Source:
                flowType = EGAResourceFlowType.Source;
                break;
            case ResourceFlowType.Sink:
                flowType = EGAResourceFlowType.Sink;
                break;
        }

        GameAnalyticsSDK.Net.GameAnalytics.AddResourceEvent(flowType, currency, amount, itemType, itemId, customFields, mergeFields);
    }

    /// <summary>
    /// 提交异常堆栈跟踪或自定义错误消息。
    /// </summary>
    /// <param name="errorLevel">日志等级</param>
    /// <param name="message">消息内容</param>
    /// <param name="customFields">自定义字段</param>
    /// <param name="mergeFields">是否合并字段</param>
    public static void Error(ErrorLevel errorLevel, string message, IDictionary<string, object> customFields = null, bool mergeFields = false)
    {
        if (_isInit == false)
        {
            return;
        }

        var severity = EGAErrorSeverity.Undefined;
        switch (errorLevel)
        {
            case ErrorLevel.Undefined:
                severity = EGAErrorSeverity.Undefined;
                break;
            case ErrorLevel.Debug:
                severity = EGAErrorSeverity.Debug;
                break;
            case ErrorLevel.Info:
                severity = EGAErrorSeverity.Info;
                break;
            case ErrorLevel.Warning:
                severity = EGAErrorSeverity.Warning;
                break;
            case ErrorLevel.Error:
                severity = EGAErrorSeverity.Error;
                break;
            case ErrorLevel.Critical:
                severity = EGAErrorSeverity.Critical;
                break;
        }

        GameAnalyticsSDK.Net.GameAnalytics.AddErrorEvent(severity, message, customFields, mergeFields);
    }
}