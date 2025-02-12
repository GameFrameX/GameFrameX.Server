// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace GameFrameX.Apps.Player.Player.Entity;

public sealed class PlayerState : CacheState
{
    /// <summary>
    /// 账号ID
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// 玩家名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 玩家等级
    /// </summary>
    public uint Level { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public uint Avatar { get; set; }

    /// <summary>
    /// 玩家状态
    /// </summary>
    public int State { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// 离线时间
    /// </summary>
    public DateTime OfflineTime { get; set; }
}