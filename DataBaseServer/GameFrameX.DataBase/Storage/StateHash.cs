using GameFrameX.Log;
using GameFrameX.Utility;
using Standart.Hash.xxHash;

namespace GameFrameX.DataBase.Storage;

/// <summary>
/// 数据状态Hash计算处理器
/// </summary>
internal sealed class StateHash
{
    public StateHash(BaseCacheState state, bool isNew)
    {
        State = state;
        if (!isNew)
        {
            CacheHash = GetHashAndData(state).md5;
        }
    }

    private BaseCacheState State { get; }

    /// <summary>
    /// 缓存的Hash
    /// </summary>
    private uint128 CacheHash { get; set; }

    /// <summary>
    /// 保存的Hash
    /// </summary>
    private uint128 ToSaveHash { get; set; }

    /// <summary>
    /// 判断是否需要保存
    /// </summary>
    /// <returns></returns>
    public (bool, byte[]) IsChanged()
    {
        var (toSaveHash, data) = GetHashAndData(State);
        ToSaveHash = toSaveHash;
        return (Hash.XXHash.IsDefault(CacheHash) || !toSaveHash.Equals(CacheHash), data);
    }

    /// <summary>
    /// 保存到数据库之后的操作
    /// </summary>
    public void SaveToDbPostHandler()
    {
        if (CacheHash.Equals(ToSaveHash))
        {
            LogHelper.Error($"调用AfterSaveToDB前CacheHash已经等于ToSaveHash {State}");
        }

        CacheHash = ToSaveHash;
    }

    private static (uint128 md5, byte[] data) GetHashAndData(BaseCacheState state)
    {
        var data = state.ToBytes();
        var uint128 = Hash.XXHash.Hash128(data);
        return (uint128, data);
    }
}