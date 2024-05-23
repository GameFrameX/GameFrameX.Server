using System.Linq.Expressions;
using GameFrameX.DBServer.State;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GameFrameX.DBServer.Utility;

public static class DbUtility
{
    /// <summary>
    /// 查询条件转查询字符串
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public static string GetFindString<TState>(Expression<Func<TState, bool>> filter) where TState : ICacheState, new()
    {
        var filterDefinition = Builders<TState>.Filter.Where(filter);
        var sqlText = filterDefinition.Render(BsonSerializer.SerializerRegistry.GetSerializer<TState>(), BsonSerializer.SerializerRegistry);
        return sqlText.ToString();
    }

    /// <summary>
    /// 查询字符串转Filter
    /// </summary>
    /// <param name="filterJson"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public static FilterDefinition<TState> FindStringToFilter<TState>(string filterJson) where TState : ICacheState, new()
    {
        var filterDefinition = Builders<TState>.Filter.Text(filterJson);
        // var sqlText = filterDefinition.Render(BsonSerializer.SerializerRegistry.GetSerializer<TState>(), BsonSerializer.SerializerRegistry);
        return filterDefinition;
    }
}