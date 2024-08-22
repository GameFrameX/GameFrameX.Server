using GameFrameX.Setting;

namespace GameFrameX.StartUp.Abstractions;

/// <summary>
/// 程序异常退出的执行器
/// </summary>
public interface IFetalExceptionExitHandler
{
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="tag">标签</param>
    /// <param name="setting">服务器设置</param>
    /// <param name="message">退出原因</param>
    void Run(string tag, BaseSetting setting, string message);
}