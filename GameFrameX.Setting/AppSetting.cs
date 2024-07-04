namespace GameFrameX.Setting
{
    /// <summary>
    /// App 配置
    /// </summary>
    public sealed class AppSetting : BaseSetting
    {
        /// <summary>
        /// 服务列表
        /// </summary>
        private readonly List<int> _servers = new List<int>();

        /// <summary>
        /// 是否是本地
        /// </summary>
        /// <param name="serverId"> 服务ID</param>
        /// <returns> 返回是否是本地</returns>
        public override bool IsLocal(int serverId)
        {
            return base.IsLocal(serverId) || _servers.Contains(serverId);
        }

        /// <summary>
        /// 
        /// </summary>
        public AppSetting()
        {
#if DEBUG
            IsDebug = true;
            IsDebugReceive = true;
            IsDebugSend = true;
#endif
        }
    }
}