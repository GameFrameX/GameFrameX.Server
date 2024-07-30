using GameFrameX.DataBase.RDS;

namespace GameFrameX.Apps.Server.Server.Entity;

public class ServerState : CacheState
{
    /// <summary>
    /// 世界等级
    /// </summary>
    public int WorldLevel { get; set; } = 1;
}