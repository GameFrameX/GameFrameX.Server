using System.Collections.Concurrent;
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
        #region 仅DBModel.Mongodb调用

        private static readonly ConcurrentBag<Func<bool, bool, Task>> saveFuncs = new();

        /// <summary>
        /// 注册回存
        /// </summary>
        /// <param name="shutdown"></param>
        public static void AddShutdownSaveFunc(Func<bool, bool, Task> shutdown)
        {
            saveFuncs.Add(shutdown);
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
                foreach (var saveFunc in saveFuncs)
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
                foreach (var func in saveFuncs)
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

        public static readonly StatisticsTool statisticsTool = new();

        #endregion
    }

    public abstract class StateComponent<TState> : BaseComponent, IState where TState : CacheState, new()
    {
        static readonly ConcurrentDictionary<long, TState> stateDic = new();

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
            // if (GlobalSettings.DBModel == (int) DbModel.Mongodb)
            stateDic.TryRemove(ActorId, out _);
            return base.Inactive();
        }


        internal override bool ReadyToInactive => State == null || !State.IsChanged().isChanged;

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

        public async Task ReadStateAsync()
        {
            State = await GameDb.LoadState<TState>(ActorId);

            stateDic.TryRemove(State.Id, out _);
            stateDic.TryAdd(State.Id, State);
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
                foreach (var state in stateDic.Values)
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

                foreach (var state in stateDic.Values)
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
                StateComponent.statisticsTool.Count(stateName, writeList.Count);
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
                                stateDic.TryGetValue(id, out var state);
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