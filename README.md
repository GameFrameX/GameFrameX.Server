# GameFrameX

**高性能、跨平台的游戏服务器框架**

[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![平台](https://img.shields.io/badge/平台-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)]()
[![Actor模型](https://img.shields.io/badge/架构-Actor--模型-orange.svg)]()

### 🎯 框架简介

GameFrameX 是一个基于 C# .NET 8.0 开发的高性能、跨平台游戏服务器框架，采用 Actor 模型设计，支持热更新机制。专为多人在线游戏开发而设计，完美支持 Unity3D 客户端集成。

**设计理念**：大道至简，以简化繁

### ✨ 核心特性

#### 🚀 高性能架构

- **Actor 模型**：基于 TPL DataFlow 构建的无锁高并发系统
- **零锁设计**：通过消息传递机制避免传统锁性能损耗
- **全异步编程**：完整的 async/await 异步编程模型
- **内存优化**：自动垃圾回收和对象池管理

#### 🔄 热更新系统

- **零停机更新**：运行时逻辑更新，无需停止服务
- **状态逻辑分离**：持久化状态数据与可热更业务逻辑分离
- **回滚保护**：更新失败自动回滚到稳定版本
- **版本管理**：支持程序集版本控制和回退功能

#### 🌐 多协议网络通信

- **TCP/UDP/WebSocket/HTTP**：全面的协议支持
- **SuperSocket 集成**：高性能异步 I/O 模型
- **消息分帧**：内置数据包处理和流量控制
- **SSL/TLS 加密**：安全通信通道支持
- **连接池**：优化的资源利用率

#### 💾 数据库与持久化

- **MongoDB 主数据库**：完整的 MongoDB 集成和连接池管理
- **透明持久化**：自动序列化/反序列化，开发者无感知
- **状态管理**：智能缓存和生命周期管理
- **批量操作**：高性能批量处理

#### 📊 监控与可观测性

- **OpenTelemetry**：全面的指标、追踪和日志
- **Grafana 集成**：内置仪表板和告警支持
- **Prometheus 导出**：原生指标导出功能
- **健康检查**：实时系统健康监控
- **性能指标**：数据库、网络和业务逻辑监控

### 🏗️ 系统架构

```
┌─────────────────────────────────────────────────────────────┐
│                        客户端层                              │
├─────────────────────────────────────────────────────────────┤
│                        网络层                                │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │     TCP     │ │  WebSocket  │ │    HTTP     │           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
├─────────────────────────────────────────────────────────────┤
│                       消息处理层                             │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                  消息处理器                              │ │
│  └─────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────┤
│                       Actor 层                              │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │   玩家      │ │   服务器    │ │   账户      │           │
│  │   Actor     │ │   Actor     │ │   Actor     │           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
├─────────────────────────────────────────────────────────────┤
│                      组件层                                  │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │   组件      │ │   组件      │ │   组件      │           │
│  │  + 状态     │ │  + 状态     │ │  + 状态     │           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
├─────────────────────────────────────────────────────────────┤
│                      数据库层                                │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                    MongoDB                              │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### 🚀 快速开始

#### 环境要求

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB 4.x+](https://www.mongodb.com/try/download/community)
- Visual Studio 2022 或 JetBrains Rider

#### 安装步骤

1. **克隆仓库**
   ```bash
   git clone https://github.com/GameFrameX/GameFrameX.git
   cd GameFrameX
   ```

2. **还原依赖**
   ```bash
   dotnet restore
   ```

3. **配置数据库**
    - 启动 MongoDB 服务
    - 更新配置文件中的连接字符串

4. **构建运行**
   ```bash
   dotnet build
   dotnet run --project GameFrameX.Launcher --ServerType=Game --ServerId=1000
   ```

5. **验证部署**
    - 检查健康端点：`http://localhost:29090/health`
    - 查看日志确认启动成功

### 📁 项目结构

```
GameFrameX/
├── GameFrameX.Apps/              # 应用层（状态数据）
│   ├── Account/                  # 账户模块状态
│   ├── Player/                   # 玩家模块状态
│   └── Server/                   # 服务器模块状态
├── GameFrameX.Hotfix/            # 热更新层（业务逻辑）
│   ├── Logic/                    # 业务逻辑实现
│   └── StartUp/                  # 热更新启动逻辑
├── GameFrameX.Core/              # 核心框架
│   ├── Actors/                   # Actor 系统实现
│   ├── Components/               # 组件系统
│   └── Events/                   # 事件系统
├── GameFrameX.NetWork/           # 网络通信
├── GameFrameX.DataBase.Mongo/    # MongoDB 集成
├── GameFrameX.Config/            # 配置管理
├── GameFrameX.Monitor/           # 监控和指标
├── GameFrameX.Launcher/          # 应用入口点
└── GameFrameX.StartUp/           # 启动编排
```

### 🔧 业务逻辑开发

#### 基础示例：Hello World

快速创建一个 HTTP 接口来体验开发流程。

```csharp
using GameFrameX.NetWork.HTTP;

[HttpMessageMapping(typeof(TestHttpHandler))]
[HttpMessageResponse(typeof(HttpTestResponse))]
[Description("Hello World 示例接口")]
public sealed class TestHttpHandler : BaseHttpHandler
{
    public override Task<string> Action(string ip, string url, Dictionary<string, object> parameters)
    {
        var response = new HttpTestResponse
        {
            Message = "Hello World From GameFrameX",
            Time = DateTime.Now
        };
        // 返回标准 JSON 格式
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

public sealed class HttpTestResponse : HttpMessageResponseBase
{
    public string Message { get; set; }
    public DateTime Time { get; set; }
}
```

#### 组件-代理模式

**1. 定义状态（Apps 层 - 不可热更）**

```csharp
public class BagState : CacheState
{
    public List<ItemData> Items { get; set; } = new();
    public int MaxSlots { get; set; } = 50;
}
```

**2. 创建组件（Apps 层）**

```csharp
public class BagComponent : StateComponent<BagState>
{
    // 组件初始化逻辑
    protected override async Task OnInit()
    {
        await base.OnInit();
        // 初始化组件状态
    }
}
```

**3. 实现业务逻辑（Hotfix 层 - 可热更）**

```csharp
public class BagComponentAgent : StateComponentAgent<BagComponent, BagState>
{
    public async Task<bool> AddItem(int itemId, int count)
    {
        if (State.Items.Count >= State.MaxSlots)
            return false;

        var item = new ItemData { Id = itemId, Count = count };
        State.Items.Add(item);

        await Save();
        return true;
    }
}
```

#### 消息处理器模式

**HTTP 处理器示例：**

```csharp
[HttpMessageMapping(typeof(GetPlayerInventoryHttpHandler))]
[Description("获取玩家背包物品")]
public sealed class GetPlayerInventoryHttpHandler : BaseHttpHandler
{
    public override async Task<MessageObject> Action(string ip, string url, Dictionary<string, object> parameters, MessageObject messageObject)
    {
        var request = (GetPlayerInventoryRequest)messageObject;
        var response = new GetPlayerInventoryResponse();

        // 从参数中获取玩家ID
        if (!parameters.TryGetValue("playerId", out var playerIdObj))
        {
            response.ErrorCode = (int)ResultCode.InvalidParameter;
            return response;
        }

        var playerId = Convert.ToInt64(playerIdObj);
        var bagAgent = await ActorManager.GetComponentAgent<BagComponentAgent>(playerId);

        if (bagAgent == null)
        {
            response.ErrorCode = (int)ResultCode.PlayerNotFound;
            return response;
        }

        var items = await bagAgent.GetItems();
        response.Items = items;
        return response;
    }
}
```

**RPC 处理器示例：**

```csharp
[MessageMapping(typeof(ReqAddItem))]
internal sealed class AddItemHandler : PlayerRpcComponentHandler<BagComponentAgent, ReqAddItem, RespAddItem>
{
    protected override async Task ActionAsync(ReqAddItem request, RespAddItem response)
    {
        try
        {
            // ComponentAgent 由基类自动注入,无需手动获取
            await ComponentAgent.AddItem(request, response);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }
}
```

#### 事件处理模式

```csharp
[Event(EventId.PlayerLogin)]
internal sealed class PlayerLoginEventHandler : EventListener<PlayerComponentAgent>
{
    protected override Task HandleEvent(PlayerComponentAgent agent, GameEventArgs gameEventArgs)
    {
        if (agent == null)
        {
            LogHelper.Error("代理对象为空");
            return Task.CompletedTask;
        }

        // 处理登录事件逻辑
        return agent.OnLogin();
    }
}
```

### 🔄 热更新机制

#### 架构概述

热更新系统将**状态**（持久化数据）与**逻辑**（业务规则）分离：

1. **Apps 层**：包含状态定义和组件外壳（不可热更）
2. **Hotfix 层**：包含业务逻辑实现（可热更）
3. **代理模式**：作为状态和逻辑之间的桥梁

#### 热更新流程

1. **编译新逻辑**：构建更新的 `GameFrameX.Hotfix.dll`
2. **部署程序集**：复制到 `/hotfix` 目录
3. **触发重载**：通过 HTTP 端点或文件监视器
4. **优雅过渡**：新请求使用更新后的逻辑
5. **回滚支持**：失败时自动回滚

#### 热更新 API

```bash
# 通过 HTTP 触发热更新
curl -X POST http://localhost:29090/api/reload

# 检查重载状态
curl http://localhost:29090/api/reload/status
```

### 📊 配置管理

GameFrameX 使用扁平化的配置结构，支持命令行参数 (`--Key=Value`)、环境变量 (`Key=Value`) 和 `appsettings.json` 配置文件。

#### 核心配置 (Server)

| 配置项           | 说明                   | 默认值          | 示例       |
|:--------------|:---------------------|:-------------|:---------|
| `ServerType`  | 服务器类型 (如 Game, Gate) | 无            | `Game`   |
| `ServerId`    | 服务器唯一标识 ID           | 无            | `1000`   |
| `ServerName`  | 服务器名称                | 同 ServerType | `Game-1` |
| `MinModuleId` | 业务模块起始 ID            | 0            | `100`    |
| `MaxModuleId` | 业务模块结束 ID            | 0            | `1000`   |

#### 网络配置 (Network)

| 配置项         | 说明              | 默认值 | 示例        |
|:------------|:----------------|:----|:----------|
| `InnerHost` | 内部通信 IP (集群间)   | 无   | `0.0.0.0` |
| `InnerPort` | 内部通信端口          | 无   | `29100`   |
| `OuterHost` | 外部通信 IP (面向客户端) | 无   | `0.0.0.0` |
| `OuterPort` | 外部通信端口          | 无   | `29200`   |
| `HttpPort`  | HTTP 服务端口       | 0   | `8080`    |
| `WsPort`    | WebSocket 服务端口  | 0   | `29300`   |

#### 数据库配置 (Database)

| 配置项            | 说明            | 默认值 | 示例                          |
|:---------------|:--------------|:----|:----------------------------|
| `DataBaseUrl`  | MongoDB 连接字符串 | 无   | `mongodb://localhost:27017` |
| `DataBaseName` | 数据库名称         | 无   | `gameframex`                |

#### 监控配置 (Monitoring)

| 配置项               | 说明                 | 默认值         | 示例     |
|:------------------|:-------------------|:------------|:-------|
| `IsOpenTelemetry` | 是否启用 OpenTelemetry | `false`     | `true` |
| `MetricsPort`     | Prometheus 指标端口    | 0 (复用 HTTP) | `9090` |
| `IsDebug`         | 开启调试日志             | `false`     | `true` |

#### 启动命令示例

```bash
dotnet GameFrameX.Launcher.dll \
    --ServerType=Game \
    --ServerId=1000 \
    --OuterPort=10000 \
    --DataBaseUrl=mongodb://127.0.0.1:27017 \
    --DataBaseName=game_db
```

### 🐳 Docker 部署

```dockerfile
# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore && dotnet build -c Release

# 运行阶段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /src/bin/Release/net8.0/ .
EXPOSE 29100 29110 29090
ENTRYPOINT ["dotnet", "GameFrameX.Launcher.dll"]
```

```yaml
# docker-compose.yml
version: '3.8'
services:
  gameframex:
    build: .
    ports:
      - "29100:29100"  # TCP
      - "29110:29110"  # WebSocket
      - "29090:29090"  # 指标
    environment:
      - ServerType=Game
      - ServerId=1000
      - DataBaseUrl=mongodb://mongodb:27017
    depends_on:
      - mongodb

  mongodb:
    image: mongo:6.0
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db

volumes:
  mongodb_data:
```

### 🔍 监控与可观测性

#### 指标端点

- **健康检查**：`http://localhost:29090/health`
- **Prometheus 指标**：`http://localhost:29090/metrics`
- **Grafana 仪表板**：可通过环境配置

#### 关键指标

- **数据库**：查询次数、持续时间、连接池状态
- **网络**：连接数、消息吞吐量、字节传输量
- **业务**：玩家登录、活跃会话、游戏特定指标
- **系统**：CPU、内存、GC 性能、线程池状态

---

### 🤝 贡献指南

我们欢迎任何形式的贡献！如果你想为 GameFrameX 做出贡献，请遵循以下步骤：

1. Fork 本仓库
2. 创建功能分支（`git checkout -b feature/amazing-feature`）
3. 提交更改（`git commit -m '添加某个功能'`）
4. 推送到分支（`git push origin feature/amazing-feature`）
5. 创建 Pull Request

### 📄 许可证

本项目采用 Apache License 2.0 许可证 - 详见 [LICENSE](LICENSE) 文件。

### 🔗 相关链接

- [在线文档](https://gameframex.doc.alianblank.com)
- [Unity 客户端](https://github.com/GameFrameX/GameFrameX.Unity)
- [问题反馈](https://github.com/GameFrameX/GameFrameX/issues)
- [社区讨论](https://github.com/GameFrameX/GameFrameX/discussions)

---

<div align="center">

**如果这个项目对你有帮助，请给我们一个 ⭐**

**Made with ❤️ by GameFrameX Team**

</div>