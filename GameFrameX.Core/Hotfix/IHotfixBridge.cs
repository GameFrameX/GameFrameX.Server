using GameFrameX.Setting;

namespace GameFrameX.Core.Hotfix;

/// <summary>
/// 热更新桥接基础接口
/// </summary>
public interface IHotfixBridge
{
    /// <summary>
    /// 加载成功
    /// </summary>
    /// <param name="setting">应用设置，包含热更新相关的配置信息</param>
    /// <param name="reload">是否为重新加载，表示此次加载是首次加载还是重新加载</param>
    /// <returns>一个任务，表示异步操作的结果，并返回加载是否成功的布尔值</returns>
    Task<bool> OnLoadSuccess(AppSetting setting, bool reload);

    /// <summary>
    /// 停止
    /// </summary>
    /// <returns>一个任务，表示异步操作的结果</returns>
    Task Stop();
}