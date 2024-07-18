using GameFrameX.DBServer.State;
using GameFrameX.Log;

namespace GameFrameX.DBServer.Storage;

class StateHash
{
    private BaseCacheState State { get; }

    public StateHash(BaseCacheState state, bool isNew)
    {
        State = state;
        if (!isNew)
        {
            CacheHash = GetHashAndData(state).md5;
        }
    }

    private Standart.Hash.xxHash.uint128 CacheHash { get; set; }

    private Standart.Hash.xxHash.uint128 ToSaveHash { get; set; }

    public (bool, byte[]) IsChanged()
    {
        var (toSaveHash, data) = GetHashAndData(State);
        ToSaveHash             = toSaveHash;
        return (CacheHash.IsDefault() || !toSaveHash.Equals(CacheHash), data);
    }

    public void AfterSaveToDb()
    {
        if (CacheHash.Equals(ToSaveHash))
        {
            LogHelper.Error($"调用AfterSaveToDB前CacheHash已经等于ToSaveHash {State}");
        }

        CacheHash = ToSaveHash;
    }

    private static (Standart.Hash.xxHash.uint128 md5, byte[] data) GetHashAndData(BaseCacheState state)
    {
        var data   = state.ToBytes();
        var md5Str = Utility.Hash.XXHash.Hash128(data);
        return (md5Str, data);
    }
}