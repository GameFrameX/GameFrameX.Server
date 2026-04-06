// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using GameFrameX.Foundation.Hash;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using Standart.Hash.xxHash;

namespace GameFrameX.DataBase.Storage;

/// <summary>
/// 数据状态Hash计算处理器。
/// </summary>
/// <remarks>
/// Data state hash calculation processor.
/// </remarks>
internal sealed class StateHash
{
    /// <summary>
    /// 初始化 <see cref="StateHash"/> 类的新实例。
    /// </summary>
    /// <remarks>
    /// Initialize a new instance of the <see cref="StateHash"/> class.
    /// </remarks>
    /// <param name="state">缓存状态对象 / Cache state object</param>
    /// <param name="isNew">是否是新创建的实例 / Whether it is a newly created instance</param>
    public StateHash(BaseCacheState state, bool isNew = false)
    {
        State = state;
        if (!isNew)
        {
            CacheHash = GetHashAndData(state).md5;
        }
    }

    /// <summary>
    /// 缓存状态对象。
    /// </summary>
    /// <remarks>
    /// Cache state object.
    /// </remarks>
    private BaseCacheState State { get; }

    /// <summary>
    /// 缓存的Hash值。
    /// </summary>
    /// <remarks>
    /// Cached hash value.
    /// </remarks>
    private uint128 CacheHash { get; set; }

    /// <summary>
    /// 待保存的Hash值。
    /// </summary>
    /// <remarks>
    /// Hash value to save.
    /// </remarks>
    private uint128 ToSaveHash { get; set; }

    /// <summary>
    /// 判断是否需要保存。
    /// </summary>
    /// <remarks>
    /// Determine if save is needed.
    /// </remarks>
    /// <returns>包含是否需要保存和字节数据的元组 / Tuple containing whether save is needed and byte data</returns>
    public (bool, byte[]) IsChanged()
    {
        var (toSaveHash, data) = GetHashAndData(State);
        ToSaveHash = toSaveHash;
        return (XxHashHelper.IsDefault(CacheHash) || !(toSaveHash.high64 == CacheHash.high64 && toSaveHash.low64 == CacheHash.low64), data);
    }

    /// <summary>
    /// 保存到数据库之后的操作。
    /// </summary>
    /// <remarks>
    /// Operation after saving to database.
    /// </remarks>
    public void SaveToDbPostHandler()
    {
        if (CacheHash.high64 == ToSaveHash.high64 && CacheHash.low64 == ToSaveHash.low64)
        {
            // LogHelper.Warning<string>("StateHash.SaveToDbPostHandler {cacheHashEquals}", LocalizationService.GetString(Localization.Keys.Storage.CacheHashEquals, State.GetType().FullName));
            return;
        }

        CacheHash = ToSaveHash;
    }

    /// <summary>
    /// 获取状态对象的Hash值和字节数据。
    /// </summary>
    /// <remarks>
    /// Get the hash value and byte data of the state object.
    /// </remarks>
    /// <param name="state">缓存状态对象 / Cache state object</param>
    /// <returns>包含Hash值和字节数据的元组 / Tuple containing hash value and byte data</returns>
    private static (uint128 md5, byte[] data) GetHashAndData(BaseCacheState state)
    {
        var data = state.ToBytes();
        var uint128 = XxHashHelper.Hash128(data);
        return (uint128, data);
    }
}