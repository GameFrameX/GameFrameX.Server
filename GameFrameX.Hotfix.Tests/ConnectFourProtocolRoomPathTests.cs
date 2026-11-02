// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证和 Apache 许可证（版本 2.0）双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录的 LICENSE 文件。
//   please see the LICENSE file in the root directory of the source tree for the full license text.
// ==========================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GameFrameX.Apps.Game.Room.Component;
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Abstractions.Hotfix;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Components;
using GameFrameX.Core.Hotfix;
using GameFrameX.Core.Hotfix.Agent;
using GameFrameX.DataBase;
using GameFrameX.DataBase.Abstractions;
using GameFrameX.DataBase.Sqlite;
using GameFrameX.Hotfix.Logic.Game.ConnectFour;
using GameFrameX.Hotfix.Logic.Game.Room;
using GameFrameX.Hotfix.Tests.Online;
using GameFrameX.Network.Abstractions;
using GameFrameX.Network.HTTP;
using GameFrameX.Proto.Proto;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;
using Xunit;

namespace GameFrameX.Hotfix.Tests.ConnectFour;

/// <summary>
/// 四子棋协议层全链验收：不绕过房间层直调桥——经真实房间代理（建房 / 加入 / 开局）与对局代理
/// （查询 / 落子 / 再来一局）驱动，覆盖「协议入口 → 房间规则 → 对局桥 → 平台权威裁决」的完整断链面，
/// 同时断言桥失败错误码显式透传、非成员查询拒绝与制胜后不经查询立即重开的结算窗口收敛。
/// </summary>
[Collection(CapabilityRuntimeCollection.Name)]
public sealed class ConnectFourProtocolRoomPathTests
{
    /// <summary>房主（先手，纵向四连决胜）。</summary>
    private const long OwnerPlayerId = 7001;

    /// <summary>对手（负方）。</summary>
    private const long GuestPlayerId = 7002;

    /// <summary>非房间成员（成员校验探针）。</summary>
    private const long OutsiderPlayerId = 7003;

    /// <summary>驱动的最大 tick 轮数（每轮覆盖 match-tick / match-settlement-tick / 投影重投 job）。</summary>
    private const int MaxTickRounds = 10;

    private static readonly object InitializeLock = new object();
    private static bool _serverSideInitialized;

    /// <summary>
    /// 验证协议层四子棋闭环：建房（房间规则命中）→ 加入 → 开局（桥创建平台对局）→
    /// 未开局查询显式失败（平台错误码透传）→ 非成员查询拒绝 → 落子至先手四连 →
    /// 结算落定后不经任何查询直接再来一局（重开路径自身刷新房间终态）。
    /// </summary>
    [Fact]
    public async Task ProtocolLayer_RoomRuleStartMoveQueryRestart_ClosesConnectFourLoop()
    {
        EnsureServerSideInitialized();

        await OnlineCapabilityTestHarness.WithRuntimeAsync(async runtime =>
        {
            await runtime.Host.StartAsync();

            var serverActorType = ComponentRegister.GetActorType(typeof(RoomComponent));
            var serverActorId = ActorIdGenerator.GetActorId(serverActorType);
            var roomAgent = await ActorManager.GetComponentAgentAsync<RoomComponentAgent>(serverActorId);
            var gameAgent = await ActorManager.GetComponentAgentAsync<ConnectFourGameComponentAgent>(serverActorId);
            Assert.NotNull(roomAgent);
            Assert.NotNull(gameAgent);

            // 建房：四子棋房型必须命中房间规则表（规则缺失时协议层建房恒 ParamErr，玩法整体不可达）。
            var createResponse = new RespCreateRoom();
            await roomAgent.OnCreateRoomAsync(OwnerPlayerId, new ReqCreateRoom { GameType = GameType.ConnectFour }, createResponse);
            Assert.Equal(0, createResponse.ErrorCode);
            Assert.NotNull(createResponse.Room);
            Assert.Equal(GameType.ConnectFour, createResponse.Room.GameType);
            var roomId = createResponse.Room.RoomId;

            // 对手加入 → 满员进入 Ready。
            var joinResponse = new RespJoinRoom();
            await roomAgent.OnJoinRoomAsync(GuestPlayerId, new ReqJoinRoom { RoomId = roomId }, joinResponse);
            Assert.Equal(0, joinResponse.ErrorCode);
            Assert.NotNull(joinResponse.Room);
            Assert.Equal(RoomStatus.Ready, joinResponse.Room.Status);

            // 未开局先查询：平台对局尚不存在，桥查询失败须显式回传平台错误码（不得「成功 + 空数据」）。
            var earlyQueryResponse = new RespConnectFourGameInfo();
            await gameAgent.OnReqGameInfoAsync(OwnerPlayerId, new ReqConnectFourGameInfo { RoomId = roomId }, earlyQueryResponse);
            Assert.Equal(-(int)OnlineErrorCode.ResourceNotFound, earlyQueryResponse.ErrorCode);
            Assert.Null(earlyQueryResponse.GameInfo);

            // 非成员查询：仅凭 RoomId 不得窥探他人对局。
            var outsiderQueryResponse = new RespConnectFourGameInfo();
            await gameAgent.OnReqGameInfoAsync(OutsiderPlayerId, new ReqConnectFourGameInfo { RoomId = roomId }, outsiderQueryResponse);
            Assert.Equal((int)OperationStatusCode.Forbidden, outsiderQueryResponse.ErrorCode);

            // 房主开局：桥先行创建平台对局，旧链完成房间 → Playing 推进。
            var startResponse = new RespStartRoomGame();
            await roomAgent.OnStartRoomGameAsync(OwnerPlayerId, new ReqStartRoomGame { RoomId = roomId }, startResponse);
            Assert.Equal(0, startResponse.ErrorCode);
            Assert.NotNull(startResponse.Room);
            Assert.Equal(RoomStatus.Playing, startResponse.Room.Status);

            // 成员查询：开局后对局视图可达，先手 = 房间成员表第一位。
            var infoResponse = new RespConnectFourGameInfo();
            await gameAgent.OnReqGameInfoAsync(OwnerPlayerId, new ReqConnectFourGameInfo { RoomId = roomId }, infoResponse);
            Assert.Equal(0, infoResponse.ErrorCode);
            Assert.NotNull(infoResponse.GameInfo);
            Assert.Equal(1, infoResponse.GameInfo.Round);
            Assert.Equal(OwnerPlayerId, infoResponse.GameInfo.CurrentPlayerId);

            // 落子至先手纵向四连（与进程内黄金路径同一确定性序列）。
            var moveSequence = new[]
            {
                (OwnerPlayerId, 0), (GuestPlayerId, 1),
                (OwnerPlayerId, 0), (GuestPlayerId, 1),
                (OwnerPlayerId, 0), (GuestPlayerId, 1),
                (OwnerPlayerId, 0),
            };

            RespSubmitConnectFourMove lastMoveResponse = null;
            foreach (var (playerId, column) in moveSequence)
            {
                lastMoveResponse = new RespSubmitConnectFourMove();
                await gameAgent.OnSubmitMoveAsync(playerId, new ReqSubmitConnectFourMove { RoomId = roomId, Column = column }, lastMoveResponse);
                Assert.Equal(0, lastMoveResponse.ErrorCode);
            }

            Assert.NotNull(lastMoveResponse.GameInfo);
            Assert.Equal(OwnerPlayerId, lastMoveResponse.GameInfo.WinnerPlayerId);

            // 平台结算落定（tick 驱动结算 outbox）；随后不发起任何查询，房主直接再来一局——
            // 重开路径必须自行以桥视图刷新房间终态（Settling → Settled），否则会被结算窗口误拒 Forbidden。
            for (var tickRound = 0; tickRound < MaxTickRounds; tickRound++)
            {
                await runtime.Host.Scheduler.TickAsync();
            }

            var restartResponse = new RespRestartConnectFourGame();
            await gameAgent.OnRestartGameAsync(OwnerPlayerId, new ReqRestartConnectFourGame { RoomId = roomId }, restartResponse);
            Assert.Equal(0, restartResponse.ErrorCode);
            Assert.NotNull(restartResponse.GameInfo);
            Assert.Equal(2, restartResponse.GameInfo.Round);
            Assert.Equal(0, restartResponse.GameInfo.MoveCount);
            Assert.Equal(OwnerPlayerId, restartResponse.GameInfo.CurrentPlayerId);
        }, null, options =>
        {
            options.Filter = new CapabilityAssemblyFilter(new[] { "online-foundation", "online-social", "online-matchmaking", "online-match", "online-assets", "online-leaderboard", "online-competition" });
            options.HostOptions.SchedulerIntervalMilliseconds = 3600000;
        });
    }

    /// <summary>
    /// 进程级服务面就绪（一次装配，全测试复用）：缺省设置、SQLite 测试库（组件状态读写走真实持久化面）、
    /// 组件注册（Apps 程序集）与代理解析注入（默认上下文解析器，见 <see cref="DefaultContextHotfixResolver"/>）。
    /// </summary>
    private static void EnsureServerSideInitialized()
    {
        lock (InitializeLock)
        {
            if (_serverSideInitialized)
            {
                return;
            }

            if (GlobalSettings.CurrentSetting == null)
            {
                GlobalSettings.SetCurrentSetting(new AppSetting { ServerId = (int)OnlineCapabilityTestHarness.ServerId });
            }

            var databaseFile = Path.Combine(Path.GetTempPath(), "gameframex-connectfour-protocol-" + Guid.NewGuid().ToString("N") + ".db");
            var databaseInitialized = GameDatabase.ProcessDefault.InitAsync<SqliteDbService>(new DbOptions
            {
                Provider = DatabaseProviderType.Sqlite,
                ConnectionString = "Data Source=" + databaseFile,
                Name = "connectfour-protocol-tests",
                IsDefault = true,
            }).GetAwaiter().GetResult();
            Assert.True(databaseInitialized, "SQLite 测试库初始化失败");

            ComponentRegister.InitAsync(typeof(RoomComponent).Assembly).GetAwaiter().GetResult();

            // 代理解析走默认加载上下文（测试引用的热更程序集），使代理类型与测试侧类型同一身份；
            // 生产热更装载（独立可收集上下文）会令跨上下文类型转换失败，不适合进程内直调代理的测试形态。
            var setResolverMethod = typeof(HotfixRuntime).GetMethod("SetResolverForTesting", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.NotNull(setResolverMethod);
            setResolverMethod.Invoke(HotfixRuntime.ProcessDefault, new object[] { new DefaultContextHotfixResolver() });

            _serverSideInitialized = true;
        }
    }

    /// <summary>
    /// 默认加载上下文的热更解析器：组件 → 代理的映射按「代理基类泛型参数」在热更程序集内扫描解析，
    /// 代理实例直接构造并绑定组件（消息处理器 / HTTP 处理器 / 事件监听器不在本测试面，返回空）。
    /// </summary>
    private sealed class DefaultContextHotfixResolver : IHotfixResolver
    {
        public System.Reflection.Assembly HotfixAssembly
        {
            get { return typeof(RoomComponentAgent).Assembly; }
        }

        public bool InTransition
        {
            get { return false; }
        }

        public Type GetAgentType(Type componentType)
        {
            return FindAgentType(componentType) ?? throw new KeyNotFoundException("未找到组件代理映射：" + componentType.FullName);
        }

        public Type GetComponentType(Type agentType)
        {
            return ResolveComponentType(agentType) ?? throw new KeyNotFoundException("代理基类型无效，无法推导组件类型：" + agentType.FullName);
        }

        public T GetAgent<T>(BaseComponent component, Type refAssemblyType) where T : class, IComponentAgent
        {
            var agentType = FindAgentType(component.GetType());
            if (agentType == null)
            {
                throw new KeyNotFoundException("未找到组件代理映射：" + component.GetType().FullName);
            }

            var agent = (T)Activator.CreateInstance(agentType);
            agent.SetOwner(component);
            return agent;
        }

        public IMessageHandler GetRpcHandler(Type requestType)
        {
            return null;
        }

        public IMessageHandler GetMessageHandler(int messageId)
        {
            return null;
        }

        public BaseHttpHandler GetHttpHandler(string command)
        {
            return null;
        }

        public List<BaseHttpHandler> GetListHttpHandler()
        {
            return new List<BaseHttpHandler>();
        }

        public List<IEventListener> FindListeners(ushort actorType, Type eventArgsType)
        {
            return new List<IEventListener>();
        }

        public List<IEventListener> FindListeners(Type eventArgsType)
        {
            return new List<IEventListener>();
        }

        public T GetInstance<T>(string typeName, Type refAssemblyType = null)
        {
            return default;
        }

        /// <summary>
        /// 在热更程序集内扫描组件对应的代理类型（排除热更包装器派生类，取真实代理本体）。
        /// </summary>
        private static Type FindAgentType(Type componentType)
        {
            foreach (var type in typeof(RoomComponentAgent).Assembly.GetTypes())
            {
                if (type.IsAbstract || !typeof(IComponentAgent).IsAssignableFrom(type) || type.Name.EndsWith("Wrapper", StringComparison.Ordinal))
                {
                    continue;
                }

                if (ResolveComponentType(type) == componentType)
                {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// 代理类型 → 组件类型：沿基类链找 <c>BaseComponentAgent&lt;TComponent&gt;</c> 的泛型参数。
        /// </summary>
        private static Type ResolveComponentType(Type agentType)
        {
            var current = agentType;
            while (current != null)
            {
                var baseType = current.BaseType;
                if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(BaseComponentAgent<>))
                {
                    return baseType.GetGenericArguments()[0];
                }

                current = baseType;
            }

            return null;
        }
    }
}
