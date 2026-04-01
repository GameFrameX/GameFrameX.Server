using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameFrameX.DataBase;
using GameFrameX.DataBase.Abstractions;
using GameFrameX.DataBase.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace GameFrameX.Tests.DataBase;

/// <summary>
/// MongoDbService 查询接口集成测试。
/// </summary>
public sealed class MongoDbServiceQueryTests
{
    private static long _idSeed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    /// 测试按 ID 查询已存在数据。
    /// </summary>
    [Fact]
    public async Task FindAsync_ById_ShouldReturnExistingState()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("player-1", group: 1, score: 30);
            await service.AddAsync(state);

            var result = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);

            Assert.NotNull(result);
            Assert.Equal(state.Id, result.Id);
            Assert.Equal("player-1", result.Name);
            Assert.Equal(30, result.Score);
        });
    }

    /// <summary>
    /// 测试按 ID 查询不存在数据且不自动创建时返回空。
    /// </summary>
    [Fact]
    public async Task FindAsync_ById_WhenNotExistsAndNoCreate_ShouldReturnNull()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var result = await service.FindAsync<MongoQueryTestState>(Interlocked.Increment(ref _idSeed), isCreateIfNotExists: false);
            Assert.Null(result);
        });
    }

    /// <summary>
    /// 测试条件查询与软删除过滤的一致性。
    /// </summary>
    [Fact]
    public async Task FindList_Count_Any_ShouldRespectSoftDeleteFilter()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var a = CreateState("group1-a", group: 1, score: 10);
            var b = CreateState("group1-b", group: 1, score: 20);
            var c = CreateState("group2-c", group: 2, score: 30);
            await service.AddListAsync(new[] { a, b, c });
            await service.DeleteAsync(b);

            var list = await service.FindListAsync<MongoQueryTestState>(x => x.Group == 1);
            var count = await service.CountAsync<MongoQueryTestState>(x => x.Group == 1);
            var anyGroup1 = await service.AnyAsync<MongoQueryTestState>(x => x.Group == 1);
            var anyDeleted = await service.AnyAsync<MongoQueryTestState>(x => x.Id == b.Id);

            Assert.Single(list);
            Assert.Equal(a.Id, list[0].Id);
            Assert.Equal(1, count);
            Assert.True(anyGroup1);
            Assert.False(anyDeleted);
        });
    }

    /// <summary>
    /// 测试升降序首条查询与分页查询结果。
    /// </summary>
    [Fact]
    public async Task SortQueries_ShouldReturnExpectedOrder()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var s1 = CreateState("s1", group: 1, score: 10);
            var s2 = CreateState("s2", group: 1, score: 20);
            var s3 = CreateState("s3", group: 1, score: 30);
            await service.AddListAsync(new[] { s1, s2, s3 });

            var ascFirst = await service.FindSortAscendingFirstOneAsync<MongoQueryTestState>(x => x.Group == 1, x => x.Score);
            var descFirst = await service.FindSortDescendingFirstOneAsync<MongoQueryTestState>(x => x.Group == 1, x => x.Score);
            var ascPage = await service.FindSortAscendingAsync<MongoQueryTestState>(x => x.Group == 1, x => x.Score, pageIndex: 1, pageSize: 2);
            var descPage = await service.FindSortDescendingAsync<MongoQueryTestState>(x => x.Group == 1, x => x.Score, pageIndex: 0, pageSize: 2);

            Assert.NotNull(ascFirst);
            Assert.NotNull(descFirst);
            Assert.Equal(10, ascFirst.Score);
            Assert.Equal(30, descFirst.Score);
            Assert.Equal(new[] { 30, 20 }, descPage.Select(x => x.Score).ToArray());
            Assert.Single(ascPage);
            Assert.Equal(30, ascPage[0].Score);
        });
    }

    /// <summary>
    /// 测试按条件查询在不存在时自动创建新对象。
    /// </summary>
    [Fact]
    public async Task FindAsync_ByFilter_WhenNotExistsAndCreate_ShouldCreateState()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var result = await service.FindAsync<MongoQueryTestState>(x => x.Name == "new-state", isCreateIfNotExists: true);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.True(result.CreatedTime > 0);
            Assert.Null(result.Name);
        });
    }

    /// <summary>
    /// 测试新增单条数据接口。
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldInsertOneRecord()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("add-one", group: 9, score: 99);
            await service.AddAsync(state);

            var result = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.NotNull(result);
            Assert.Equal("add-one", result.Name);
            Assert.Equal(9, result.Group);
            Assert.Equal(99, result.Score);
            Assert.True(result.CreatedTime > 0);
        });
    }

    /// <summary>
    /// 测试批量新增空集合接口。
    /// </summary>
    [Fact]
    public async Task AddListAsync_WithEmptyCollection_ShouldNotInsertAnyRecord()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            await service.AddListAsync(Array.Empty<MongoQueryTestState>());
            var count = await service.CountAsync<MongoQueryTestState>(x => true);
            Assert.Equal(0, count);
        });
    }

    /// <summary>
    /// 测试新增或更新接口的插入与更新流程。
    /// </summary>
    [Fact]
    public async Task AddOrUpdateAsync_ShouldInsertThenUpdate()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("upsert-init", group: 2, score: 10);
            await service.AddOrUpdateAsync(state);

            var inserted = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.NotNull(inserted);
            Assert.Equal("upsert-init", inserted.Name);

            state.Name = "upsert-updated";
            state.Score = 88;
            await service.AddOrUpdateAsync(state);

            var updated = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.NotNull(updated);
            Assert.Equal("upsert-updated", updated.Name);
            Assert.Equal(88, updated.Score);
        });
    }

    /// <summary>
    /// 测试单条更新接口支持字段置空。
    /// </summary>
    [Fact]
    public async Task UpdateAsync_Single_ShouldSupportUnsetNullField()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("update-one", group: 3, score: 30);
            state.OptionalNote = "before-update";
            await service.AddAsync(state);

            var loaded = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.NotNull(loaded);
            loaded.LoadFromDbPostHandler();
            loaded.OptionalNote = null;
            loaded.Score = 33;

            await service.UpdateAsync(loaded);

            var collection = service.CurrentDatabase.GetCollection<BsonDocument>(nameof(MongoQueryTestState));
            var raw = await collection.Find(Builders<BsonDocument>.Filter.Eq("_id", state.Id)).FirstOrDefaultAsync();
            Assert.NotNull(raw);
            Assert.Equal(33, raw["Score"].AsInt32);
            Assert.False(raw.Contains(nameof(MongoQueryTestState.OptionalNote)));
        });
    }

    /// <summary>
    /// 测试批量更新接口仅更新已变更对象。
    /// </summary>
    [Fact]
    public async Task UpdateAsync_List_ShouldReturnChangedCount()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var a = CreateState("batch-a", group: 1, score: 10);
            var b = CreateState("batch-b", group: 1, score: 20);
            await service.AddListAsync(new[] { a, b });

            var loadedA = await service.FindAsync<MongoQueryTestState>(a.Id, isCreateIfNotExists: false);
            var loadedB = await service.FindAsync<MongoQueryTestState>(b.Id, isCreateIfNotExists: false);
            Assert.NotNull(loadedA);
            Assert.NotNull(loadedB);
            loadedA.LoadFromDbPostHandler();
            loadedB.LoadFromDbPostHandler();

            loadedA.Score = 11;
            var updatedCount = await service.UpdateAsync(new[] { loadedA, loadedB });
            Assert.Equal(1, updatedCount);

            var refreshedA = await service.FindAsync<MongoQueryTestState>(a.Id, isCreateIfNotExists: false);
            var refreshedB = await service.FindAsync<MongoQueryTestState>(b.Id, isCreateIfNotExists: false);
            Assert.NotNull(refreshedA);
            Assert.NotNull(refreshedB);
            Assert.Equal(11, refreshedA.Score);
            Assert.Equal(20, refreshedB.Score);
        });
    }

    /// <summary>
    /// 测试批量更新空集合接口。
    /// </summary>
    [Fact]
    public async Task UpdateAsync_List_WithEmptyCollection_ShouldReturnZero()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var count = await service.UpdateAsync(Array.Empty<MongoQueryTestState>());
            Assert.Equal(0, count);
        });
    }

    /// <summary>
    /// 测试按对象删除接口。
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ByState_ShouldSoftDeleteRecord()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("delete-state", group: 7, score: 70);
            await service.AddAsync(state);

            var affected = await service.DeleteAsync(state);
            Assert.Equal(1, affected);

            var visible = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.Null(visible);
        });
    }

    /// <summary>
    /// 测试按条件删除接口。
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ByFilter_ShouldSoftDeleteFirstMatchedRecord()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var a = CreateState("delete-filter-a", group: 5, score: 50);
            var b = CreateState("delete-filter-b", group: 5, score: 51);
            await service.AddListAsync(new[] { a, b });

            var affected = await service.DeleteAsync<MongoQueryTestState>(x => x.Id == a.Id);
            Assert.Equal(1, affected);

            var count = await service.CountAsync<MongoQueryTestState>(x => x.Group == 5);
            Assert.Equal(1, count);
        });
    }

    /// <summary>
    /// 测试按条件批量删除接口。
    /// </summary>
    [Fact]
    public async Task DeleteListAsync_ShouldSoftDeleteMatchedRecords()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var a = CreateState("delete-list-a", group: 6, score: 61);
            var b = CreateState("delete-list-b", group: 6, score: 62);
            var c = CreateState("delete-list-c", group: 8, score: 80);
            await service.AddListAsync(new[] { a, b, c });

            var affected = await service.DeleteListAsync<MongoQueryTestState>(x => x.Group == 6);
            Assert.Equal(2, affected);

            var countGroup6 = await service.CountAsync<MongoQueryTestState>(x => x.Group == 6);
            var countGroup8 = await service.CountAsync<MongoQueryTestState>(x => x.Group == 8);
            Assert.Equal(0, countGroup6);
            Assert.Equal(1, countGroup8);
        });
    }

    /// <summary>
    /// 测试按ID批量删除接口。
    /// </summary>
    [Fact]
    public async Task DeleteListIdAsync_ShouldSoftDeleteSpecifiedIds()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var a = CreateState("delete-id-a", group: 10, score: 101);
            var b = CreateState("delete-id-b", group: 10, score: 102);
            var c = CreateState("delete-id-c", group: 10, score: 103);
            await service.AddListAsync(new[] { a, b, c });

            var affected = await service.DeleteListIdAsync<MongoQueryTestState>(new[] { a.Id, c.Id });
            Assert.Equal(2, affected);

            var list = await service.FindSortAscendingAsync<MongoQueryTestState>(x => x.Group == 10, x => x.Score);
            Assert.Single(list);
            Assert.Equal(b.Id, list[0].Id);
        });
    }

    /// <summary>
    /// 测试按ID批量删除空集合接口。
    /// </summary>
    [Fact]
    public async Task DeleteListIdAsync_WithEmptyCollection_ShouldReturnZero()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var affected = await service.DeleteListIdAsync<MongoQueryTestState>(Array.Empty<long>());
            Assert.Equal(0, affected);
        });
    }

    [Fact]
    public async Task Concurrent_AddAsync_ShouldInsertAllRecords()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            const int count = 20;
            var tasks = Enumerable.Range(0, count)
                                  .Select(i => service.AddAsync(CreateState($"concurrent-add-{i}", group: 77, score: i)));

            await Task.WhenAll(tasks);

            var total = await service.CountAsync<MongoQueryTestState>(x => x.Group == 77);
            Assert.Equal(count, total);
        });
    }

    [Fact]
    public async Task Concurrent_DeleteAsync_SameRecord_ShouldRemainSoftDeleted()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("concurrent-delete", group: 78, score: 100);
            await service.AddAsync(state);

            var deleteTasks = Enumerable.Range(0, 10)
                                        .Select(_ => service.DeleteAsync<MongoQueryTestState>(x => x.Id == state.Id));
            var results = await Task.WhenAll(deleteTasks);

            Assert.True(results.Sum() >= 1);

            var visible = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.Null(visible);

            var collection = service.CurrentDatabase.GetCollection<BsonDocument>(nameof(MongoQueryTestState));
            var raw = await collection.Find(Builders<BsonDocument>.Filter.Eq("_id", state.Id)).FirstOrDefaultAsync();
            Assert.NotNull(raw);
            Assert.True(raw["IsDeleted"].AsBoolean);
            Assert.True(raw["DeleteTime"].ToInt64() > 0);
        });
    }

    [Fact]
    public async Task Concurrent_UpdateAndDelete_ShouldKeepRecordSoftDeleted()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("concurrent-conflict", group: 79, score: 40);
            await service.AddAsync(state);

            var loaded = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.NotNull(loaded);
            loaded.LoadFromDbPostHandler();
            loaded.Score = 999;

            var updateTask = service.UpdateAsync(loaded);
            var deleteTask = service.DeleteAsync<MongoQueryTestState>(x => x.Id == state.Id);
            await Task.WhenAll(updateTask, deleteTask);

            var visible = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.Null(visible);

            var collection = service.CurrentDatabase.GetCollection<BsonDocument>(nameof(MongoQueryTestState));
            var raw = await collection.Find(Builders<BsonDocument>.Filter.Eq("_id", state.Id)).FirstOrDefaultAsync();
            Assert.NotNull(raw);
            Assert.True(raw["IsDeleted"].AsBoolean);
            Assert.Contains(raw["Score"].AsInt32, new[] { 40, 999 });
        });
    }

    /// <summary>
    /// 执行一次带数据库生命周期的测试。
    /// </summary>
    private static async Task ExecuteWithServiceAsync(Func<MongoDbService, Task> action)
    {
        var connectionString = Environment.GetEnvironmentVariable("GAMEFRAMEX_TEST_MONGODB_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        var dbName = $"gameframex_test_{Guid.NewGuid():N}";
        var service = new MongoDbService();
        var options = new DbOptions
        {
            Type = "MongoDb",
            ConnectionString = connectionString,
            Name = dbName,
        };

        var opened = await service.Open(options);
        Assert.True(opened);

        try
        {
            await action(service);
        }
        finally
        {
            await service.Close();
            try
            {
                var client = new MongoClient(connectionString);
                await client.DropDatabaseAsync(dbName);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// 创建测试实体。
    /// </summary>
    private static MongoQueryTestState CreateState(string name, int group, int score)
    {
        return new MongoQueryTestState
        {
            Id = Interlocked.Increment(ref _idSeed),
            Name = name,
            Group = group,
            Score = score,
        };
    }

    /// <summary>
    /// 查询测试实体。
    /// </summary>
    private sealed class MongoQueryTestState : BaseCacheState
    {
        /// <summary>
        /// 名称字段。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分组字段。
        /// </summary>
        public int Group { get; set; }

        /// <summary>
        /// 分数字段。
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 可为空的备注字段。
        /// </summary>
        public string OptionalNote { get; set; }

        /// <summary>
        /// 转换字节数组。
        /// </summary>
        public override byte[] ToBytes()
        {
            var data = $"{Id}|{Name}|{Group}|{Score}|{OptionalNote}|{IsDeleted}|{DeleteTime}|{UpdateCount}|{UpdateTime}";
            return Encoding.UTF8.GetBytes(data);
        }
    }
}
