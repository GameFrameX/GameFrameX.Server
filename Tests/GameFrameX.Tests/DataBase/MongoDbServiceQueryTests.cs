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

    [Fact]
    public async Task Chunked_UpdateAsync_BatchedChunks_ShouldUpdateAllChangedRecords()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            const int count = 30;
            var source = Enumerable.Range(0, count)
                                   .Select(i => CreateState($"chunk-update-{i}", group: 83, score: i))
                                   .ToArray();
            await service.AddListAsync(source);

            var loaded = await service.FindListAsync<MongoQueryTestState>(x => x.Group == 83);
            loaded = loaded.OrderBy(x => x.Id).ToList();
            Assert.Equal(count, loaded.Count);

            for (var i = 0; i < loaded.Count; i++)
            {
                loaded[i].LoadFromDbPostHandler();
                loaded[i].Score = 1000 + i;
            }

            long updated = 0;
            foreach (var chunk in loaded.Chunk(10))
            {
                updated += await service.UpdateAsync(chunk);
            }

            Assert.Equal(count, updated);

            var refreshed = await service.FindListAsync<MongoQueryTestState>(x => x.Group == 83);
            refreshed = refreshed.OrderBy(x => x.Id).ToList();
            Assert.Equal(count, refreshed.Count);
            Assert.Equal(1000, refreshed[0].Score);
            Assert.Equal(1000 + count - 1, refreshed[^1].Score);
        });
    }

    [Fact]
    public async Task Concurrent_MixedDeleteAndUpdate_ByDifferentIdSets_ShouldKeepConsistentVisibility()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            const int total = 40;
            var source = Enumerable.Range(0, total)
                                   .Select(i => CreateState($"mixed-{i}", group: 84, score: i))
                                   .ToArray();
            await service.AddListAsync(source);

            var deleteIds = source.Take(20).Select(x => x.Id).ToArray();
            var updateIds = source.Skip(20).Select(x => x.Id).ToArray();

            var loadedToUpdate = new List<MongoQueryTestState>(updateIds.Length);
            foreach (var id in updateIds)
            {
                var item = await service.FindAsync<MongoQueryTestState>(id, isCreateIfNotExists: false);
                Assert.NotNull(item);
                item.LoadFromDbPostHandler();
                item.Score += 5000;
                loadedToUpdate.Add(item);
            }

            var deleteTask = service.DeleteListIdAsync<MongoQueryTestState>(deleteIds);
            var updateTask = service.UpdateAsync(loadedToUpdate);
            await Task.WhenAll(deleteTask, updateTask);

            var visible = await service.FindListAsync<MongoQueryTestState>(x => x.Group == 84);
            Assert.Equal(20, visible.Count);
            Assert.All(visible, x => Assert.DoesNotContain(x.Id, deleteIds));
            Assert.All(visible, x => Assert.True(x.Score >= 5020));
        });
    }

    [Fact]
    public async Task DeleteAsync_ByFilter_MultipleTimes_ShouldBeIdempotent()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("idempotent-delete", group: 85, score: 1);
            await service.AddAsync(state);

            var first = await service.DeleteAsync<MongoQueryTestState>(x => x.Id == state.Id);
            var second = await service.DeleteAsync<MongoQueryTestState>(x => x.Id == state.Id);

            Assert.Equal(1, first);
            Assert.Equal(0, second);

            var visible = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.Null(visible);
        });
    }

    /// <summary>
    /// 测试两个服务实例并发新增数据的一致性。
    /// </summary>
    [Fact]
    public async Task MultiService_ConcurrentAddAsync_ShouldInsertAllRecords()
    {
        await ExecuteWithTwoServicesAsync(async (serviceA, serviceB) =>
        {
            const int perServiceCount = 15;
            var tasksA = Enumerable.Range(0, perServiceCount)
                                   .Select(i => serviceA.AddAsync(CreateState($"ms-a-{i}", group: 90, score: i)));
            var tasksB = Enumerable.Range(0, perServiceCount)
                                   .Select(i => serviceB.AddAsync(CreateState($"ms-b-{i}", group: 90, score: i + 100)));

            await Task.WhenAll(tasksA.Concat(tasksB));

            var countA = await serviceA.CountAsync<MongoQueryTestState>(x => x.Group == 90);
            var countB = await serviceB.CountAsync<MongoQueryTestState>(x => x.Group == 90);
            Assert.Equal(perServiceCount * 2, countA);
            Assert.Equal(perServiceCount * 2, countB);
        });
    }

    /// <summary>
    /// 测试两个服务实例对同一记录并发更新时的最终可见性。
    /// </summary>
    [Fact]
    public async Task MultiService_ConcurrentAddOrUpdateSameId_ShouldKeepSingleRecord()
    {
        await ExecuteWithTwoServicesAsync(async (serviceA, serviceB) =>
        {
            var id = Interlocked.Increment(ref _idSeed);
            var statesA = Enumerable.Range(0, 10).Select(i => new MongoQueryTestState
            {
                Id = id,
                Name = $"winner-a-{i}",
                Group = 91,
                Score = 1000 + i,
            });
            var statesB = Enumerable.Range(0, 10).Select(i => new MongoQueryTestState
            {
                Id = id,
                Name = $"winner-b-{i}",
                Group = 91,
                Score = 2000 + i,
            });

            var tasks = statesA.Select(serviceA.AddOrUpdateAsync)
                               .Concat(statesB.Select(serviceB.AddOrUpdateAsync));
            await Task.WhenAll(tasks);

            var listA = await serviceA.FindListAsync<MongoQueryTestState>(x => x.Id == id);
            var listB = await serviceB.FindListAsync<MongoQueryTestState>(x => x.Id == id);
            Assert.Single(listA);
            Assert.Single(listB);
            Assert.Equal(listA[0].Id, listB[0].Id);
            Assert.Contains(listA[0].Score, Enumerable.Range(1000, 10).Concat(Enumerable.Range(2000, 10)));
        });
    }

    /// <summary>
    /// 测试两个服务实例并发删除与查询时软删除过滤保持一致。
    /// </summary>
    [Fact]
    public async Task MultiService_ConcurrentDeleteAndQuery_ShouldRespectSoftDeleteFilter()
    {
        await ExecuteWithTwoServicesAsync(async (serviceA, serviceB) =>
        {
            var states = Enumerable.Range(0, 20)
                                   .Select(i => CreateState($"ms-del-{i}", group: 92, score: i))
                                   .ToArray();
            await serviceA.AddListAsync(states);
            var deleteIds = states.Take(10).Select(x => x.Id).ToArray();

            var deleteTask = serviceA.DeleteListIdAsync<MongoQueryTestState>(deleteIds);
            var queryTask = serviceB.FindListAsync<MongoQueryTestState>(x => x.Group == 92);
            await Task.WhenAll(deleteTask, queryTask);

            var finalA = await serviceA.FindListAsync<MongoQueryTestState>(x => x.Group == 92);
            var finalB = await serviceB.FindListAsync<MongoQueryTestState>(x => x.Group == 92);
            Assert.Equal(10, finalA.Count);
            Assert.Equal(10, finalB.Count);
            Assert.All(finalA, x => Assert.DoesNotContain(x.Id, deleteIds));
            Assert.All(finalB, x => Assert.DoesNotContain(x.Id, deleteIds));
        });
    }

    /// <summary>
    /// 测试分页超出范围时返回空集合。
    /// </summary>
    [Fact]
    public async Task FindSortAscendingAsync_PageOutOfRange_ShouldReturnEmpty()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var states = Enumerable.Range(0, 5)
                                   .Select(i => CreateState($"page-edge-{i}", group: 93, score: i))
                                   .ToArray();
            await service.AddListAsync(states);

            var page = await service.FindSortAscendingAsync<MongoQueryTestState>(x => x.Group == 93, x => x.Score, pageIndex: 10, pageSize: 5);
            Assert.Empty(page);
        });
    }

    /// <summary>
    /// 测试条件批量删除无命中时返回0且数据保持可见。
    /// </summary>
    [Fact]
    public async Task DeleteListAsync_WhenNoMatch_ShouldReturnZeroAndKeepData()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("delete-no-match", group: 94, score: 1);
            await service.AddAsync(state);

            var affected = await service.DeleteListAsync<MongoQueryTestState>(x => x.Group == 9999);
            Assert.Equal(0, affected);

            var visible = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.NotNull(visible);
            Assert.Equal(94, visible.Group);
        });
    }

    /// <summary>
    /// 测试两个服务实例重叠ID并发删除后数据不泄露。
    /// </summary>
    [Fact]
    public async Task MultiService_ConcurrentDeleteListId_WithOverlappingIds_ShouldDeleteUnion()
    {
        await ExecuteWithTwoServicesAsync(async (serviceA, serviceB) =>
        {
            var states = Enumerable.Range(0, 30)
                                   .Select(i => CreateState($"union-del-{i}", group: 95, score: i))
                                   .ToArray();
            await serviceA.AddListAsync(states);

            var idsA = states.Take(20).Select(x => x.Id).ToArray();
            var idsB = states.Skip(10).Take(20).Select(x => x.Id).ToArray();

            var deleteTaskA = serviceA.DeleteListIdAsync<MongoQueryTestState>(idsA);
            var deleteTaskB = serviceB.DeleteListIdAsync<MongoQueryTestState>(idsB);
            await Task.WhenAll(deleteTaskA, deleteTaskB);

            var finalVisible = await serviceA.FindListAsync<MongoQueryTestState>(x => x.Group == 95);
            Assert.Empty(finalVisible);
        });
    }

    /// <summary>
    /// 测试软删除后使用新增或更新接口可恢复可见性。
    /// </summary>
    [Fact]
    public async Task AddOrUpdateAsync_AfterSoftDelete_ShouldRestoreVisibility()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("restore-target", group: 96, score: 10);
            await service.AddAsync(state);
            await service.DeleteAsync(state);

            var hidden = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.Null(hidden);

            var restore = new MongoQueryTestState
            {
                Id = state.Id,
                Name = "restore-success",
                Group = 96,
                Score = 99,
            };
            await service.AddOrUpdateAsync(restore);

            var visible = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            Assert.NotNull(visible);
            Assert.Equal("restore-success", visible.Name);
            Assert.Equal(99, visible.Score);
        });
    }

    /// <summary>
    /// 测试多轮混合更新与删除后计数和可见性保持一致。
    /// </summary>
    [Fact]
    public async Task MultiRound_MixedUpdateAndDelete_ShouldKeepConsistentCount()
    {
        await ExecuteWithTwoServicesAsync(async (serviceA, serviceB) =>
        {
            var states = Enumerable.Range(0, 50)
                                   .Select(i => CreateState($"round-mix-{i}", group: 97, score: i))
                                   .ToArray();
            await serviceA.AddListAsync(states);

            var deletedIdSet = new HashSet<long>();
            for (var round = 0; round < 3; round++)
            {
                var current = await serviceA.FindListAsync<MongoQueryTestState>(x => x.Group == 97);
                current = current.OrderBy(x => x.Id).ToList();
                Assert.True(current.Count >= 6);

                var updateTargets = current.Take(5).ToList();
                foreach (var item in updateTargets)
                {
                    item.LoadFromDbPostHandler();
                    item.Score += 1000 + round;
                }

                var deleteIds = current.TakeLast(3).Select(x => x.Id).ToArray();
                foreach (var id in deleteIds)
                {
                    deletedIdSet.Add(id);
                }

                var updateTask = serviceA.UpdateAsync(updateTargets);
                var deleteTask = serviceB.DeleteListIdAsync<MongoQueryTestState>(deleteIds);
                await Task.WhenAll(updateTask, deleteTask);
            }

            var finalList = await serviceA.FindListAsync<MongoQueryTestState>(x => x.Group == 97);
            Assert.Equal(50 - deletedIdSet.Count, finalList.Count);
            Assert.All(finalList, x => Assert.DoesNotContain(x.Id, deletedIdSet));
        });
    }

    /// <summary>
    /// 测试批量更新对象全部未修改时返回0。
    /// </summary>
    [Fact]
    public async Task UpdateAsync_List_AllUnchanged_ShouldReturnZero()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var a = CreateState("unchanged-a", group: 98, score: 1);
            var b = CreateState("unchanged-b", group: 98, score: 2);
            await service.AddListAsync(new[] { a, b });

            var loadedA = await service.FindAsync<MongoQueryTestState>(a.Id, isCreateIfNotExists: false);
            var loadedB = await service.FindAsync<MongoQueryTestState>(b.Id, isCreateIfNotExists: false);
            Assert.NotNull(loadedA);
            Assert.NotNull(loadedB);
            loadedA.LoadFromDbPostHandler();
            loadedB.LoadFromDbPostHandler();

            var updated = await service.UpdateAsync(new[] { loadedA, loadedB });
            Assert.Equal(0, updated);
        });
    }

    /// <summary>
    /// 测试按条件创建在命中软删除记录时返回未持久化新对象。
    /// </summary>
    [Fact]
    public async Task FindAsync_ByFilter_CreateAfterSoftDelete_ShouldReturnTransientNewState()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var state = CreateState("recreate-by-filter", group: 99, score: 1);
            await service.AddAsync(state);
            await service.DeleteAsync(state);

            var recreated = await service.FindAsync<MongoQueryTestState>(x => x.Name == "recreate-by-filter", isCreateIfNotExists: true);
            Assert.NotNull(recreated);
            Assert.NotEqual(state.Id, recreated.Id);

            var oldVisible = await service.FindAsync<MongoQueryTestState>(state.Id, isCreateIfNotExists: false);
            var newVisible = await service.FindAsync<MongoQueryTestState>(recreated.Id, isCreateIfNotExists: false);
            Assert.Null(oldVisible);
            Assert.Null(newVisible);
        });
    }

    /// <summary>
    /// 测试固定随机种子的跨服务混合回放后两侧快照一致。
    /// </summary>
    [Fact]
    public async Task MultiService_SeededReplayMixedOperations_ShouldKeepSameSnapshot()
    {
        await ExecuteWithTwoServicesAsync(async (serviceA, serviceB) =>
        {
            var states = Enumerable.Range(0, 30)
                                   .Select(i => CreateState($"seeded-{i}", group: 100, score: i))
                                   .ToArray();
            await serviceA.AddListAsync(states);

            var allIds = states.Select(x => x.Id).ToList();
            var random = new Random(20260401);
            for (var i = 0; i < 60; i++)
            {
                var targetId = allIds[random.Next(allIds.Count)];
                var useServiceA = i % 2 == 0;
                var writer = useServiceA ? serviceA : serviceB;

                switch (i % 3)
                {
                    case 0:
                    {
                        var loaded = await writer.FindAsync<MongoQueryTestState>(targetId, isCreateIfNotExists: false);
                        if (loaded != null)
                        {
                            loaded.LoadFromDbPostHandler();
                            loaded.Score = 5000 + i;
                            await writer.UpdateAsync(loaded);
                        }

                        break;
                    }
                    case 1:
                    {
                        await writer.DeleteListIdAsync<MongoQueryTestState>(new[] { targetId });
                        break;
                    }
                    default:
                    {
                        await writer.AddOrUpdateAsync(new MongoQueryTestState
                        {
                            Id = targetId,
                            Name = $"seeded-revive-{targetId}",
                            Group = 100,
                            Score = 8000 + i,
                        });
                        break;
                    }
                }
            }

            var snapshotA = await serviceA.FindSortAscendingAsync<MongoQueryTestState>(x => x.Group == 100, x => x.Id);
            var snapshotB = await serviceB.FindSortAscendingAsync<MongoQueryTestState>(x => x.Group == 100, x => x.Id);

            Assert.Equal(snapshotA.Count, snapshotB.Count);
            Assert.Equal(snapshotA.Select(x => x.Id).ToArray(), snapshotB.Select(x => x.Id).ToArray());
            Assert.Equal(snapshotA.Select(x => x.Score).ToArray(), snapshotB.Select(x => x.Score).ToArray());
        });
    }

    /// <summary>
    /// 测试按条件创建得到瞬态对象后通过新增或更新接口可持久化。
    /// </summary>
    [Fact]
    public async Task FindAsync_ByFilter_TransientThenAddOrUpdate_ShouldPersistState()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var transient = await service.FindAsync<MongoQueryTestState>(x => x.Name == "transient-persist", isCreateIfNotExists: true);
            Assert.NotNull(transient);

            var beforePersist = await service.FindAsync<MongoQueryTestState>(transient.Id, isCreateIfNotExists: false);
            Assert.Null(beforePersist);

            transient.Name = "transient-persist";
            transient.Group = 101;
            transient.Score = 1234;
            await service.AddOrUpdateAsync(transient);

            var persisted = await service.FindAsync<MongoQueryTestState>(transient.Id, isCreateIfNotExists: false);
            Assert.NotNull(persisted);
            Assert.Equal("transient-persist", persisted.Name);
            Assert.Equal(101, persisted.Group);
            Assert.Equal(1234, persisted.Score);
        });
    }

    /// <summary>
    /// 测试跨服务并发创建瞬态对象并持久化后最终可见集合一致。
    /// </summary>
    [Fact]
    public async Task MultiService_ConcurrentTransientCreateThenPersist_ShouldKeepConsistentVisibility()
    {
        await ExecuteWithTwoServicesAsync(async (serviceA, serviceB) =>
        {
            const int count = 20;
            var transientA = new List<MongoQueryTestState>(count);
            var transientB = new List<MongoQueryTestState>(count);

            for (var i = 0; i < count; i++)
            {
                var a = await serviceA.FindAsync<MongoQueryTestState>(x => x.Name == $"ms-transient-a-{i}", isCreateIfNotExists: true);
                var b = await serviceB.FindAsync<MongoQueryTestState>(x => x.Name == $"ms-transient-b-{i}", isCreateIfNotExists: true);
                transientA.Add(a);
                transientB.Add(b);
            }

            foreach (var item in transientA)
            {
                item.Name = $"persisted-{item.Id}";
                item.Group = 102;
                item.Score = 2000;
            }

            foreach (var item in transientB)
            {
                item.Name = $"persisted-{item.Id}";
                item.Group = 102;
                item.Score = 3000;
            }

            var persistTaskA = Task.WhenAll(transientA.Select(serviceA.AddOrUpdateAsync));
            var persistTaskB = Task.WhenAll(transientB.Select(serviceB.AddOrUpdateAsync));
            await Task.WhenAll(persistTaskA, persistTaskB);

            var expectedCount = transientA.Concat(transientB).Select(x => x.Id).Distinct().Count();
            var visibleA = await serviceA.FindListAsync<MongoQueryTestState>(x => x.Group == 102);
            var visibleB = await serviceB.FindListAsync<MongoQueryTestState>(x => x.Group == 102);
            visibleA = visibleA.OrderBy(x => x.Id).ToList();
            visibleB = visibleB.OrderBy(x => x.Id).ToList();
            Assert.Equal(expectedCount, visibleA.Count);
            Assert.Equal(expectedCount, visibleB.Count);
            Assert.Equal(visibleA.Select(x => x.Id).ToArray(), visibleB.Select(x => x.Id).ToArray());
        });
    }

    /// <summary>
    /// 测试瞬态对象重复持久化时保持单条记录并采用最后一次写入值。
    /// </summary>
    [Fact]
    public async Task FindAsync_ByFilter_TransientPersistTwice_ShouldKeepSingleRecordAndLastValue()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var transient = await service.FindAsync<MongoQueryTestState>(x => x.Name == "transient-idempotent", isCreateIfNotExists: true);
            Assert.NotNull(transient);

            transient.Name = "transient-idempotent";
            transient.Group = 103;
            transient.Score = 11;
            await service.AddOrUpdateAsync(transient);

            transient.Score = 22;
            await service.AddOrUpdateAsync(transient);

            var byId = await service.FindAsync<MongoQueryTestState>(transient.Id, isCreateIfNotExists: false);
            var byGroup = await service.FindListAsync<MongoQueryTestState>(x => x.Group == 103);
            Assert.NotNull(byId);
            Assert.Equal(22, byId.Score);
            Assert.Single(byGroup);
            Assert.Equal(transient.Id, byGroup[0].Id);
        });
    }

    /// <summary>
    /// 测试跨服务先软删除后同ID重建时最终可见记录保持一致。
    /// </summary>
    [Fact]
    public async Task MultiService_DeleteThenRecreateSameId_ShouldKeepSingleVisibleRecord()
    {
        await ExecuteWithTwoServicesAsync(async (serviceA, serviceB) =>
        {
            var seed = CreateState("delete-then-recreate", group: 104, score: 1);
            await serviceA.AddAsync(seed);

            var deleted = await serviceB.DeleteListIdAsync<MongoQueryTestState>(new[] { seed.Id });
            Assert.Equal(1, deleted);

            await serviceA.AddOrUpdateAsync(new MongoQueryTestState
            {
                Id = seed.Id,
                Name = "recreated-same-id",
                Group = 104,
                Score = 9001,
            });

            var listA = await serviceA.FindListAsync<MongoQueryTestState>(x => x.Id == seed.Id);
            var listB = await serviceB.FindListAsync<MongoQueryTestState>(x => x.Id == seed.Id);
            Assert.Single(listA);
            Assert.Single(listB);
            Assert.Equal(9001, listA[0].Score);
            Assert.Equal(9001, listB[0].Score);
        });
    }

    /// <summary>
    /// 测试同ID按删除更新重建顺序执行后最终以重建结果为准。
    /// </summary>
    [Fact]
    public async Task SameId_DeleteThenUpdateThenAddOrUpdate_ShouldUseRecreatedState()
    {
        await ExecuteWithServiceAsync(async service =>
        {
            var seed = CreateState("matrix-seq-1", group: 105, score: 10);
            await service.AddAsync(seed);

            var stale = await service.FindAsync<MongoQueryTestState>(seed.Id, isCreateIfNotExists: false);
            Assert.NotNull(stale);
            stale.LoadFromDbPostHandler();
            stale.Score = 200;

            await service.DeleteListIdAsync<MongoQueryTestState>(new[] { seed.Id });
            await service.UpdateAsync(stale);
            await service.AddOrUpdateAsync(new MongoQueryTestState
            {
                Id = seed.Id,
                Name = "matrix-seq-1-recreated",
                Group = 105,
                Score = 700,
            });

            var visible = await service.FindAsync<MongoQueryTestState>(seed.Id, isCreateIfNotExists: false);
            Assert.NotNull(visible);
            Assert.Equal("matrix-seq-1-recreated", visible.Name);
            Assert.Equal(700, visible.Score);
        });
    }

    /// <summary>
    /// 测试跨服务同ID三种顺序矩阵下最终可见性符合预期。
    /// </summary>
    [Fact]
    public async Task MultiService_SameId_OperationOrderMatrix_ShouldMatchExpectedVisibility()
    {
        await ExecuteWithTwoServicesAsync(async (serviceA, serviceB) =>
        {
            var idA = Interlocked.Increment(ref _idSeed);
            var idB = Interlocked.Increment(ref _idSeed);
            var idC = Interlocked.Increment(ref _idSeed);

            await serviceA.AddAsync(new MongoQueryTestState { Id = idA, Name = "matrix-a", Group = 106, Score = 1 });
            await serviceA.AddAsync(new MongoQueryTestState { Id = idB, Name = "matrix-b", Group = 106, Score = 2 });
            await serviceA.AddAsync(new MongoQueryTestState { Id = idC, Name = "matrix-c", Group = 106, Score = 3 });

            var staleA = await serviceB.FindAsync<MongoQueryTestState>(idA, isCreateIfNotExists: false);
            var staleB = await serviceA.FindAsync<MongoQueryTestState>(idB, isCreateIfNotExists: false);
            var staleC = await serviceB.FindAsync<MongoQueryTestState>(idC, isCreateIfNotExists: false);
            Assert.NotNull(staleA);
            Assert.NotNull(staleB);
            Assert.NotNull(staleC);
            staleA.LoadFromDbPostHandler();
            staleB.LoadFromDbPostHandler();
            staleC.LoadFromDbPostHandler();
            staleA.Score = 111;
            staleB.Score = 222;
            staleC.Score = 333;

            await serviceA.DeleteListIdAsync<MongoQueryTestState>(new[] { idA });
            await serviceB.UpdateAsync(staleA);
            await serviceA.AddOrUpdateAsync(new MongoQueryTestState { Id = idA, Name = "matrix-a-recreated", Group = 106, Score = 710 });

            await serviceB.AddOrUpdateAsync(new MongoQueryTestState { Id = idB, Name = "matrix-b-upserted", Group = 106, Score = 720 });
            await serviceA.DeleteListIdAsync<MongoQueryTestState>(new[] { idB });
            await serviceB.UpdateAsync(staleB);

            await serviceA.UpdateAsync(staleC);
            await serviceB.DeleteListIdAsync<MongoQueryTestState>(new[] { idC });
            await serviceA.AddOrUpdateAsync(new MongoQueryTestState { Id = idC, Name = "matrix-c-recreated", Group = 106, Score = 730 });

            var finalA = await serviceA.FindListAsync<MongoQueryTestState>(x => x.Group == 106);
            finalA = finalA.OrderBy(x => x.Id).ToList();
            Assert.Equal(2, finalA.Count);
            Assert.Equal(new[] { idA, idC }, finalA.Select(x => x.Id).ToArray());
            Assert.Equal(new[] { 710, 730 }, finalA.Select(x => x.Score).ToArray());
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
    /// 执行一次双服务实例共享数据库的测试。
    /// </summary>
    private static async Task ExecuteWithTwoServicesAsync(Func<MongoDbService, MongoDbService, Task> action)
    {
        var connectionString = Environment.GetEnvironmentVariable("GAMEFRAMEX_TEST_MONGODB_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        var dbName = $"gameframex_test_{Guid.NewGuid():N}";
        var options = new DbOptions
        {
            Type = "MongoDb",
            ConnectionString = connectionString,
            Name = dbName,
        };

        var serviceA = new MongoDbService();
        var serviceB = new MongoDbService();

        var openedA = await serviceA.Open(options);
        var openedB = await serviceB.Open(options);
        Assert.True(openedA);
        Assert.True(openedB);

        try
        {
            await action(serviceA, serviceB);
        }
        finally
        {
            await serviceA.Close();
            await serviceB.Close();
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
