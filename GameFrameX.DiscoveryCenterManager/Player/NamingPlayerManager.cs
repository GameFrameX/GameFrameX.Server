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

using System.Collections.Concurrent;
using GameFrameX.Utility;

namespace GameFrameX.DiscoveryCenterManager.Player;

/// <summary>
/// 命名玩家管理器，负责维护玩家ID与玩家信息的映射关系，并支持添加、移除及查询操作。
/// 采用单例模式，全局唯一实例。
/// </summary>
public sealed class NamingPlayerManager : Singleton<NamingPlayerManager>
{
    /// <summary>
    /// 玩家ID与玩家信息映射，线程安全字典
    /// </summary>
    private readonly ConcurrentDictionary<long, IPlayerInfo> _playerMap = new();

    /// <summary>
    /// 玩家添加成功时触发的回调
    /// </summary>
    private Action<IPlayerInfo> _onAdd;

    /// <summary>
    /// 玩家移除成功时触发的回调
    /// </summary>
    private Action<IPlayerInfo> _onRemove;

    /// <summary>
    /// 设置玩家添加与移除的回调
    /// </summary>
    /// <param name="onAdd">玩家添加回调</param>
    /// <param name="onRemove">玩家移除回调</param>
    public void SetChangeCallback(Action<IPlayerInfo> onAdd, Action<IPlayerInfo> onRemove)
    {
        _onAdd = onAdd;
        _onRemove = onRemove;
    }

    /// <summary>
    /// 添加玩家信息
    /// </summary>
    /// <param name="playerId">玩家ID</param>
    /// <param name="serverId">服务器ID</param>
    /// <param name="serverInstanceId">服务器实例ID</param>
    public void Add(long playerId, long serverId, long serverInstanceId)
    {
        var playerInfo = new NamingPlayerInfo(playerId, serverId, serverInstanceId);
        if (_playerMap.TryAdd(playerId, playerInfo))
        {
            _onAdd?.Invoke(playerInfo);
        }
    }

    /// <summary>
    /// 根据玩家ID尝试获取玩家信息
    /// </summary>
    /// <param name="playerId">玩家ID</param>
    /// <param name="playerInfo">返回的玩家信息</param>
    /// <returns>是否成功获取</returns>
    public bool TryGet(long playerId, out IPlayerInfo playerInfo)
    {
        return _playerMap.TryGetValue(playerId, out playerInfo);
    }

    /// <summary>
    /// 根据玩家ID尝试移除玩家信息
    /// </summary>
    /// <param name="playerId">玩家ID</param>
    /// <param name="playerInfo">返回的被移除玩家信息</param>
    /// <returns>是否成功移除</returns>
    public bool TryRemove(long playerId, out IPlayerInfo playerInfo)
    {
        if (_playerMap.TryRemove(playerId, out playerInfo))
        {
            _onRemove?.Invoke(playerInfo);
            return true;
        }

        return false;
    }
}