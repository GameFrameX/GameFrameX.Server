namespace GameFrameX.Apps.Server.Entity;

public class ServerState : BaseCacheState
{
    /// <summary>
    /// 世界等级
    /// </summary>
    public int WorldLevel { get; set; } = 1;

    /// <summary>
    /// 第一次启动时间，即开服时间
    /// </summary>
    public long FirstStartTime { get; set; }
}
