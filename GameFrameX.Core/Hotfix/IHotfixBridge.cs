using GameFrameX.Setting;

namespace GameFrameX.Core.Hotfix
{
    /// <summary>
    /// 热更新桥接基础接口
    /// </summary>
    public interface IHotfixBridge
    {
        /// <summary>
        /// 桥接类型
        /// </summary>
        ServerType BridgeType { get; }

        /// <summary>
        /// 加载成功
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="reload"></param>
        /// <returns></returns>
        Task<bool> OnLoadSuccess(AppSetting setting, bool reload);

        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        Task Stop();
    }
}