using System.Collections.Concurrent;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Timer;
using GameFrameX.Core.Utility;
using MongoDB.Bson;
using MongoDB.Driver;
using GameFrameX.Core.Actors;
using GameFrameX.DBServer;
using GameFrameX.DBServer.Mongo;
using GameFrameX.DBServer.State;
using GameFrameX.Extension;
using GameFrameX.Log;

namespace GameFrameX.Core.Comps
{
    /// <summary>
    /// 数据状态组件
    /// </summary>
    public sealed class StateComponent
    {
        private static readonly ConcurrentBag<Func<bool, bool, Task>> SaveFuncMap = new ConcurrentBag<Func<bool, bool, Task>>();

        /// <summary>
        /// 注册回存
        /// </summary>
        /// <param name="shutdown"></param>
        public static void AddShutdownSaveFunc(Func<bool, bool, Task> shutdown)
        {
            SaveFuncMap.Add(shutdown);
        }

        /// <summary>
        /// 当游戏出现异常，导致无法正常回存，才需要将force=true
        /// 由后台http指令调度
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public static async Task SaveAll(bool force = false)
        {
            try
            {
                var begin = DateTime.Now;
                var tasks = new List<Task>();
                foreach (var saveFunc in SaveFuncMap)
                {
                    tasks.Add(saveFunc(true, force));
                }

                await Task.WhenAll(tasks);
                LogHelper.Info($"save all state, use: {(DateTime.Now - begin).TotalMilliseconds}ms");
            }
            catch (Exception e)
            {
                LogHelper.Error($"save all state error \n{e}");
            }
        }

        /// <summary>
        /// 定时回存所有数据
        /// </summary>
        public static async Task TimerSave()
        {
            try
            {
                foreach (var func in SaveFuncMap)
                {
                    await func(false, false);
                    if (!GlobalTimer.IsWorking)
                        return;
                }
            }
            catch (Exception e)
            {
                LogHelper.Info("timer save state error");
                LogHelper.Error(e.ToString());
            }
        }

        /// <summary>
        /// 统计工具
        /// </summary>
        public static readonly StatisticsTool StatisticsTool = new();
    }

    /// <summary>
    /// 数据状态组件
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public abstract class StateComponent<TState> : BaseComponent, IState where TState : CacheState, new()
    {
        private static readonly ConcurrentDictionary<long, TState> StateDic = new ConcurrentDictionary<long, TState>();

        /// <summary>
        /// 数据对象
        /// </summary>
        public TState State { get; private set; }

        static StateComponent()
        {
            StateComponent.AddShutdownSaveFunc(SaveAll);
        }

        /// <summary>
        /// 激活组件
        /// </summary>
        /// <returns></returns>
        public override async Task Active()
        {
            await base.Active();
            if (State != null)
            {
                return;
            }

            await ReadStateAsync();
        }

        /// <summary>
        /// 反激活组件
        /// </summary>
        public override Task Inactive()
        {
            StateDic.TryRemove(ActorId, out _);
            return base.Inactive();
        }


        internal override bool ReadyToInactive
        {
            get { return State == null || !State.IsChanged().isChanged; }
        }

        internal override async Task SaveState()
        {
            try
            {
                await GameDb.UpdateAsync(State);
            }
            catch (Exception e)
            {
                LogHelper.Fatal($"StateComp.SaveState.Failed.StateId:{State.Id},{e}");
            }
        }

        /// <summary>
        /// 准备状态
        /// </summary>
        /// <returns></returns>
        public async Task ReadStateAsync()
        {
            State = await GameDb.LoadState<TState>(ActorId);

            StateDic.TryRemove(State.Id, out _);
            StateDic.TryAdd(State.Id, State);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <returns></returns>
        public Task WriteStateAsync()
        {
            return GameDb.UpdateAsync(State);
        }


        #region 仅DBModel.Mongodb调用

        const int ONCE_SAVE_COUNT = 500;

        /// <summary>
        /// 保存全部数据
        /// </summary>
        /// <param name="shutdown"></param>
        /// <param name="force"></param>
        public static async Task SaveAll(bool shutdown, bool force = false)
        {
            var idList    = new List<long>();
            var writeList = new List<ReplaceOneModel<MongoDB.Bson.BsonDocument>>();
            if (shutdown)
            {
                foreach (var state in StateDic.Values)
                {
                    if (state.IsModify)
                    {
                        var bsonDoc = state.ToBsonDocument();
                        lock (writeList)
                        {
                            var filter = Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("_id", state.Id);
                            writeList.Add(new ReplaceOneModel<MongoDB.Bson.BsonDocument>(filter, bsonDoc) { IsUpsert = true });
                            idList.Add(state.Id);
                        }
                    }
                }
            }
            else
            {
                var tasks = new List<Task>();

                foreach (var state in StateDic.Values)
                {
                    var actor = ActorManager.GetActor(state.Id);
                    if (actor != null)
                    {
                        tasks.Add(actor.SendAsync(() =>
                                                  {
                                                      if (!force && !state.IsModify)
                                                          return;
                                                      var bsonDoc = state.ToBsonDocument();
                                                      lock (writeList)
                                                      {
                                                          var filter = Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("_id", state.Id);
                                                          writeList.Add(new ReplaceOneModel<MongoDB.Bson.BsonDocument>(filter, bsonDoc) { IsUpsert = true });
                                                          idList.Add(state.Id);
                                                      }
                                                  }));
                    }
                }

                await Task.WhenAll(tasks);
            }

            if (!writeList.IsNullOrEmpty())
            {
                var stateName = typeof(TState).Name;
                StateComponent.StatisticsTool.Count(stateName, writeList.Count);
                LogHelper.Debug($"[StateComp] 状态回存 {stateName} count:{writeList.Count}");
                var currentDatabase = GameDb.As<MongoDbService>().CurrentDatabase;
                var collection      = currentDatabase.GetCollection<MongoDB.Bson.BsonDocument>(stateName);
                for (int idx = 0; idx < writeList.Count; idx += ONCE_SAVE_COUNT)
                {
                    var docs = writeList.GetRange(idx, Math.Min(ONCE_SAVE_COUNT, writeList.Count - idx));
                    var ids  = idList.GetRange(idx, docs.Count);

                    bool save = false;
                    try
                    {
                        var result = await collection.BulkWriteAsync(docs, MongoDbService.BulkWriteOptions);
                        if (result.IsAcknowledged)
                        {
                            foreach (var id in ids)
                            {
                                StateDic.TryGetValue(id, out var state);
                                if (state == null)
                                    continue;
                                state.AfterSaveToDb();
                            }

                            save = true;
                        }
                        else
                        {
                            LogHelper.Error($"保存数据失败，类型:{typeof(TState).FullName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error($"保存数据异常，类型:{typeof(TState).FullName}，{ex}");
                    }

                    if (!save && shutdown)
                    {
                        LogHelper.Error($"保存数据失败，类型:{typeof(TState).FullName}");
                    }
                }
            }
        }

        #endregion
    }
}