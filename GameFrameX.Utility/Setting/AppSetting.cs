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

using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.Utility.Setting;

/// <summary>
/// 应用程序配置类
/// </summary>
public sealed class AppSetting
{
    /// <summary>
    /// 用于通知应用程序退出的任务完成源
    /// </summary>
    [JsonIgnore] public readonly TaskCompletionSource<bool> AppExitSource = new();

    private bool _appRunning;
    private string _serverType;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AppSetting()
    {
#if DEBUG
        IsDebug = true;
        IsDebugReceive = true;
        IsDebugSend = true;
        IsDebugSendHeartBeat = true;
        IsDebugReceiveHeartBeat = true;
#endif
        LaunchTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取应用程序退出的任务标记
    /// </summary>
    [JsonIgnore]
    public Task<bool> AppExitToken
    {
        get { return AppExitSource.Task; }
    }

    /// <summary>
    /// 应用程序启动时间
    /// </summary>
    public DateTime LaunchTime { get; set; }

    /// <summary>
    /// 获取或设置应用程序是否正在运行
    /// </summary>
    [JsonIgnore]
    public bool AppRunning
    {
        get { return _appRunning; }
        set
        {
            lock (AppExitSource)
            {
                if (AppExitSource.Task.IsCanceled)
                {
                    if (value)
                    {
                        LogHelper.Error("AppRunning已经被设置为退出，不能再次开启...");
                    }

                    _appRunning = false;
                    return;
                }

                _appRunning = value;
                if (!value && !AppExitSource.Task.IsCompleted)
                {
                    LogHelper.Info("Set AppRunning false...");
                    AppExitSource.TrySetCanceled();
                }
            }
        }
    }

    /// <summary>
    /// 获取或设置服务器类型
    /// </summary>
    public string ServerType
    {
        get { return _serverType; }
        set
        {
            _serverType = value;
            ServerName = value;
        }
    }

    /// <summary>
    /// 判断指定的服务ID是否为本地服务
    /// </summary>
    /// <param name="serverId">服务ID</param>
    /// <returns>返回是否是本地服务</returns>
    public bool IsLocal(int serverId)
    {
        return serverId == ServerId;
    }

    /// <summary>
    /// 将对象序列化为JSON字符串
    /// </summary>
    /// <returns>JSON字符串</returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }

    /// <summary>
    /// 将对象序列化为格式化的JSON字符串
    /// </summary>
    /// <returns>格式化的JSON字符串</returns>
    public string ToFormatString()
    {
        return JsonHelper.SerializeFormat(this);
    }

    #region 从配置文件读取的属性

    /// <summary>
    /// 是否启用指标收集功能,需要IsOpenTelemetry为true时有效
    /// <para>用于收集和监控应用程序的性能指标数据</para>
    /// <para>默认值为false</para>
    /// </summary>
    public bool IsOpenTelemetryMetrics { get; set; }

    /// <summary>
    /// 是否启用分布式追踪功能,需要IsOpenTelemetry为true时有效
    /// <para>用于跟踪和分析分布式系统中的请求流程</para>
    /// <para>默认值为false</para>
    /// </summary>
    public bool IsOpenTelemetryTracing { get; set; }

    /// <summary>
    /// 是否启用OpenTelemetry遥测功能
    /// <para>OpenTelemetry是一个开源的可观测性框架</para>
    /// <para>启用后可以统一管理指标、追踪和日志等可观测性数据</para>
    /// <para>默认值为false</para>
    /// </summary>
    public bool IsOpenTelemetry { get; set; }

    /// <summary>
    /// 是否是Debug打印日志模式,默认值为false
    /// </summary>
    public bool IsDebug { get; set; }

    /// <summary>
    /// 是否打印超时日志,
    /// </summary>
    public bool IsMonitorTimeOut { get; set; }

    /// <summary>
    /// 处理器超时时间（秒）,默认值为1秒
    /// </summary>
    public int MonitorTimeOutSeconds { get; set; }

    /// <summary>
    /// 网络发送等待超时时间（秒）,默认值为5秒
    /// </summary>
    public int NetWorkSendTimeOutSeconds { get; set; }

    /// <summary>
    /// 是否打印发送数据,只有在IsDebug为true时有效,默认值为false
    /// </summary>
    public bool IsDebugSend { get; set; }

    /// <summary>
    /// 是否打印发送的心跳数据,只有在IsDebugSend为true时有效,默认值为false
    /// </summary>
    public bool IsDebugSendHeartBeat { get; set; }

    /// <summary>
    /// 是否打印接收数据,只有在IsDebug为true时有效,默认值为false
    /// </summary>
    public bool IsDebugReceive { get; set; }

    /// <summary>
    /// 是否打印接收的心跳数据,只有在IsDebugReceive为true时有效,默认值为false
    /// </summary>
    public bool IsDebugReceiveHeartBeat { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    public int ServerId { get; set; }

    /// <summary>
    /// 服务器实例ID
    /// </summary>
    public long ServerInstanceId { get; set; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    public string ServerName { get; set; }

    /// <summary>
    /// 标记名称
    /// </summary>
    public string TagName { get; set; }

    /// <summary>
    /// 保存数据的时间间隔（毫秒）
    /// </summary>
    public int SaveDataInterval { get; set; } = 300_000;

    /// <summary>
    /// 保存数据的批量数量长度，默认为500
    /// </summary>
    public int SaveDataBatchCount { get; set; } = 500;

    /// <summary>
    /// 保存数据的超时时间（毫秒）,默认值为30秒
    /// </summary>
    public int SaveDataBatchTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 执行任务超时时间（毫秒）,默认值为30秒
    /// </summary>
    public int ActorTimeOut { get; set; } = 30_000;

    /// <summary>
    /// Actor 空闲多久回收,单位分钟,默认值为15分钟
    /// </summary>
    public int ActorRecycleTime { get; set; } = 15;

    /// <summary>
    /// Actor 执行任务队列超时时间（毫秒）,默认值为30秒
    /// </summary>
    public int ActorQueueTimeOut { get; set; } = 30_000;

    /// <summary>
    /// 是否启用TCP
    /// </summary>
    public bool IsEnableTcp { get; set; }

    /// <summary>
    /// 是否启用UDP
    /// </summary>
    public bool IsEnableUdp { get; set; } = false;

    /// <summary>
    /// 内部主机地址
    /// </summary>
    public string InnerHost { get; set; }

    /// <summary>
    /// 内部端口
    /// </summary>
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 雪花ID的工作ID
    /// </summary>
    public ushort WorkerId { get; set; }

    /// <summary>
    /// 雪花ID的数据中心ID
    /// </summary>
    public ushort DataCenterId { get; set; }

    /// <summary>
    /// 外部主机地址
    /// </summary>
    public string OuterHost { get; set; }

    /// <summary>
    /// 外部端口
    /// </summary>
    public ushort OuterPort { get; set; }

    /// <summary>
    /// HTTP地址
    /// </summary>
    public string HttpUrl { get; set; }

    /// <summary>
    /// 是否启用 HTTP 服务
    /// </summary>
    public bool IsEnableHttp { get; set; }

    /// <summary>
    /// HTTP 是否是开发模式
    /// </summary>
    public bool HttpIsDevelopment { get; set; }

    /// <summary>
    /// HTTP端口
    /// </summary>
    public ushort HttpPort { get; set; }

    /// <summary>
    /// HTTPS端口
    /// </summary>
    public ushort HttpsPort { get; set; }

    /// <summary>
    /// Prometheus指标端口（如果为0则使用HTTP端口）
    /// </summary>
    public ushort MetricsPort { get; set; }

    /// <summary>
    /// 是否启用 WebSocket 服务
    /// <para>开启后服务器将监听 WebSocket 端口，允许客户端通过 WebSocket 协议进行连接</para>
    /// <para>默认值为 false，即不启用</para>
    /// </summary>
    public bool IsEnableWebSocket { get; set; } = false;

    /// <summary>
    /// WebSocket端口
    /// </summary>
    public ushort WsPort { get; set; }

    /// <summary>
    /// WebSocket加密端口
    /// </summary>
    public ushort WssPort { get; set; }

    /// <summary>
    /// Wss使用的证书路径
    /// </summary>
    public string WssCertFilePath { get; set; }

    /// <summary>
    /// 数据库地址
    /// </summary>
    public string DataBaseUrl { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    public string DataBaseName { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// 数据中心
    /// </summary>
    public string DataCenter { get; set; }

    /// <summary>
    /// 发现中心地址
    /// </summary>
    public string DiscoveryCenterHost { get; set; }

    /// <summary>
    /// 发现中心端口
    /// </summary>
    public ushort DiscoveryCenterPort { get; set; }

    /// <summary>
    /// 最大客户端数量
    /// </summary>
    public int MaxClientCount { get; set; } = 3000;

    /// <summary>
    /// 游戏逻辑服务器的处理最小模块ID
    /// </summary>
    public short MinModuleId { get; set; }

    /// <summary>
    /// 游戏逻辑服务器的处理最大模块ID
    /// </summary>
    public short MaxModuleId { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 标签信息
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 客户端API地址
    /// </summary>
    public string ClientApiHost { get; set; }

    /// <summary>
    /// HubAPI地址
    /// </summary>
    public string HubApiHost { get; set; }

    /// <summary>
    /// 心跳间隔（毫秒），默认 5000 毫秒
    /// </summary>
    public int GameAppClientHeartBeatInterval { get; set; }

    /// <summary>
    /// 连接延迟（毫秒），默认 5000 毫秒
    /// </summary>
    public int GameAppClientConnectDelay { get; set; }

    /// <summary>
    /// 重试延迟（毫秒），默认 5000 毫秒
    /// </summary>
    public int GameAppClientRetryDelay { get; set; }

    /// <summary>
    /// 最大重试次数，默认 -1 表示无限重试
    /// </summary>
    public int GameAppClientMaxRetryCount { get; set; }

    #endregion
}