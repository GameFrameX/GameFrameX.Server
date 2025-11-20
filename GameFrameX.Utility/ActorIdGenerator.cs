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

using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Utility;

/// <summary>
/// ActorId 生成器
/// 14   +   7  + 30 +  12   = 63
/// 服务器id 类型 时间戳 自增
/// 玩家
/// 公会
/// 服务器id * 100000 + 全局功能id
/// 全局玩法
/// </summary>
public static class ActorIdGenerator
{
    private static long _genSecond;
    private static long _incrNum;


    private static readonly object LockObj = new();

    /// <summary>
    /// 根据ActorId获取服务器id
    /// </summary>
    /// <param name="actorId">ActorId</param>
    /// <returns>服务器id</returns>
    public static int GetServerId(long actorId)
    {
        if (actorId < GlobalConst.MinServerId)
        {
            throw new ArgumentOutOfRangeException(nameof(actorId), LocalizationService.GetString(Localization.Keys.Utility.ActorIdLessThanMinServerId, GlobalConst.MinServerId));
        }

        if (actorId < GlobalConst.MaxGlobalId)
        {
            return (int)(actorId / 1000);
        }

        return (int)(actorId >> GlobalConst.ServerIdOrModuleIdMask);
    }

    /*/// <summary>
    /// 根据ActorId获取生成时间
    /// </summary>
    /// <param name="actorId">ActorId</param>
    /// <param name="isUtc">是否使用UTC</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static DateTime GetGenerateTime(long actorId, bool isUtc = false)
    {
        if (actorId < GlobalConst.MaxGlobalId)
        {
            throw new ArgumentException(LocalizationService.GetString(GameFrameX.Localization.Keys.Utility.ActorIdGenerator.InputIsGlobalId, actorId));
        }

        var  serverId = GetServerId(actorId);
        long seconds;
        if (serverId < GlobalConst.MinServerId)
        {
            // IDModule unique_id
            seconds = (actorId >> GlobalConst.ModuleIdTimestampMask) & GlobalConst.SecondMask;
        }
        else
        {
            seconds = (actorId >> GlobalConst.TimestampMask) & GlobalConst.SecondMask;
        }

        var date = IdGenerator.UtcTimeStart.AddSeconds(seconds);
        return isUtc ? date : date.ToLocalTime();
    }*/

    /// <summary>
    /// 根据ActorId获取ActorType
    /// </summary>
    /// <param name="actorId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ushort GetActorType(long actorId)
    {
        if (actorId < GlobalConst.MinServerId)
        {
            throw new ArgumentOutOfRangeException(nameof(actorId), LocalizationService.GetString(GameFrameX.Localization.Keys.Utility.ActorIdLessThanMinServerIdDetail, actorId, GlobalConst.MinServerId));
        }

        if (actorId < GlobalConst.MaxGlobalId)
        {
            // 全局actor
            return (ushort)(actorId % 1000);
        }

        return (ushort)((actorId >> GlobalConst.ActorTypeMask) & 0xF);
    }


    /// <summary>
    /// 根据ActorType获取ActorId
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serverId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static long GetActorId(ushort type, int serverId = 0)
    {
        if (type == GlobalConst.ActorTypeSeparator)
        {
            throw new ArgumentException(LocalizationService.GetString(GameFrameX.Localization.Keys.Utility.ActorTypeError, type));
        }

        if (serverId < 0)
        {
            throw new ArgumentException(LocalizationService.GetString(GameFrameX.Localization.Keys.Utility.ServerIdNegative, serverId));
        }

        if (serverId == 0)
        {
            serverId = GlobalSettings.CurrentSetting.ServerId;
        }

        if (type < GlobalConst.ActorTypeSeparator)
        {
            return GetMultiActorId(type, serverId);
        }

        return GetGlobalActorId(type, serverId);
    }

    /// <summary>
    /// 根据ActorType类型和服务器id获取ActorId
    /// </summary>
    /// <param name="actorType"></param>
    /// <param name="serverId">服务器ID</param>
    /// <returns></returns>
    private static long GetGlobalActorId(ushort actorType, int serverId)
    {
        if (serverId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(serverId), LocalizationService.GetString(GameFrameX.Localization.Keys.Utility.ServerIdLessThanOrEqualZero));
        }

        if (actorType >= GlobalConst.ActorTypeMax || actorType == GlobalConst.ActorTypeSeparator || actorType == GlobalConst.ActorTypeNone)
        {
            throw new ArgumentOutOfRangeException(nameof(actorType), LocalizationService.GetString(GameFrameX.Localization.Keys.Utility.ActorTypeInvalid));
        }

        return serverId * 1000 + actorType;
    }

    private static long GetMultiActorId(ushort type, int serverId)
    {
        var second = (long)(DateTime.UtcNow - IdGenerator.UtcTimeStart).TotalSeconds;
        lock (LockObj)
        {
            if (second > _genSecond)
            {
                _genSecond = second;
                _incrNum = 0L;
            }
            else if (_incrNum >= GlobalConst.MaxActorIncrease)
            {
                ++_genSecond;
                _incrNum = 0L;
            }
            else
            {
                ++_incrNum;
            }
        }

        var actorId = (long)serverId << GlobalConst.ServerIdOrModuleIdMask; // serverId-14位, 支持1000~9999
        actorId |= (long)type << GlobalConst.ActorTypeMask; // 多actor类型-7位, 支持0~127
        actorId |= _genSecond << GlobalConst.TimestampMask; // 时间戳-30位, 支持34年
        actorId |= _incrNum; // 自增-12位, 每秒4096个
        return actorId;
    }

    /// <summary>
    /// 根据模块获取唯一ID
    /// </summary>
    /// <param name="module">默认最大值.</param>
    /// <returns></returns>
    public static long GetUniqueId(ushort module = GlobalConst.IdModuleMax)
    {
        return GetUniqueIdByModule(module);
    }

    /// <summary>
    /// 根据模块获取唯一ID
    /// </summary>
    /// <param name="module">默认最大值. 最大值不能超过999</param>
    /// <returns></returns>
    public static long GetUniqueIdByModule(ushort module = 999)
    {
        if (module > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(module), LocalizationService.GetString(GameFrameX.Localization.Keys.Utility.ModuleInvalid));
        }

        var second = (long)(DateTime.UtcNow - IdGenerator.UtcTimeStart).TotalSeconds;
        lock (LockObj)
        {
            if (second > _genSecond)
            {
                _genSecond = second;
                _incrNum = 0L;
            }
            else if (_incrNum >= GlobalConst.MaxUniqueIncrease)
            {
                ++_genSecond;
                _incrNum = 0L;
            }
            else
            {
                ++_incrNum;
            }
        }

        var id = (long)module << GlobalConst.ServerIdOrModuleIdMask; // 模块id 14位 支持 0~9999
        lock (LockObj)
        {
            id |= _genSecond << GlobalConst.ModuleIdTimestampMask; // 时间戳 30位
        }

        id |= _incrNum; // 自增 19位
        return id;
    }
}