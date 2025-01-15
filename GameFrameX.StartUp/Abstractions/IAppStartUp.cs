using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp.Abstractions;

/// <summary>
/// 程序启动器基类接口定义
/// </summary>
public interface IAppStartUp
{
    /// <summary>
    /// 应用退出
    /// </summary>
    Task<string> AppExitToken { get; }

    /// <summary>
    /// 服务器类型
    /// </summary>
    ServerType ServerType { get; }

    /// <summary>
    /// 配置信息
    /// </summary>
    AppSetting Setting { get; }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="serverType">服务器类型</param>
    /// <param name="setting">启动设置</param>
    /// <param name="args">启动参数</param>
    /// <returns></returns>
    bool Init(ServerType serverType, AppSetting setting, string[] args);

    /// <summary>
    /// 启动
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// 终止服务器
    /// </summary>
    /// <param name="message">终止原因</param>
    Task StopAsync(string message = "");
}