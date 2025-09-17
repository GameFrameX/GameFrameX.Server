<div align="center">

# GameFrameX

**高性能、跨平台的游戏服务器框架**

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)]()
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

[English](README_EN.md) | 简体中文

</div>

## 📖 简介

GameFrameX 是一个基于 C# .NET 8.0 开发的高性能游戏服务器框架，采用 Actor 模型设计，支持跨平台部署。框架内置热更新机制，可以满足绝大部分游戏类型的需求，特别适合与 Unity3D 协同开发。

**设计理念：大道至简，以简化繁**

## ✨ 核心特性

### 🚀 高性能架构

- **Actor 模型**：基于 TPL DataFlow 构建的高性能 Actor 系统
- **全异步编程**：采用 async/await 模式，代码清晰优雅
- **无锁设计**：通过 Actor 模型避免传统锁机制的性能损耗
- **内存优化**：自动回收不活跃数据，减少内存占用

### 🔄 热更新支持

- **不停服更新**：支持运行时热更新游戏逻辑
- **状态逻辑分离**：状态持久化，逻辑可热更
- **安全可靠**：更新失败可回滚，保证服务稳定性
- **增量更新**：只更新修改的部分，提高更新效率

### 🌐 网络通信

- **多协议支持**：TCP、UDP、WebSocket、HTTP
- **高并发**：基于 SuperSocket 的异步 I/O 模型
- **消息处理**：内置消息分包、粘包处理机制
- **安全传输**：支持 SSL/TLS 加密

### 💾 数据持久化

- **透明持久化**：自动序列化/反序列化，开发者无需关心数据库操作
- **NoSQL 支持**：默认使用 MongoDB，支持其他 NoSQL 数据库
- **缓存机制**：智能缓存策略，提高数据访问性能

### ⏰ 定时任务

- **多种定时器**：一次性、周期性、计划任务
- **线程安全**：内置线程安全的 Timer 和 Scheduler
- **事件系统**：完整的事件驱动架构

## 🏗️ 架构设计

```
┌─────────────────────────────────────────────────────────────┐
│                        Client Layer                         │
├─────────────────────────────────────────────────────────────┤
│                      Network Layer                          │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │     TCP     │ │  WebSocket  │ │    HTTP     │           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
├─────────────────────────────────────────────────────────────┤
│                      Message Layer                          │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │              Message Handlers                           │ │
│  └─────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────┤
│                       Actor Layer                           │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │   Player    │ │   Server    │ │   Account   │           │
│  │   Actor     │ │   Actor     │ │   Actor     │           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
├─────────────────────────────────────────────────────────────┤
│                    Component Layer                          │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │  Component  │ │  Component  │ │  Component  │           │
│  │   + State   │ │   + State   │ │   + State   │           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
├─────────────────────────────────────────────────────────────┤
│                     Database Layer                          │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                    MongoDB                              │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 快速开始

### 环境要求

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB 4.x+](https://www.mongodb.com/try/download/community)
- Visual Studio 2022 或 JetBrains Rider

### 安装步骤

1. **克隆项目**
   ```bash
   git clone https://github.com/GameFrameX/GameFrameX.git
   cd GameFrameX/Server
   ```

2. **安装依赖**
   ```bash
   dotnet restore
   ```

3. **配置数据库**
    - 启动 MongoDB 服务
    - 修改配置文件中的数据库连接字符串

4. **编译运行**
   ```bash
   dotnet build
   dotnet run --project GameFrameX.Launcher
   ```

5. **验证运行**
    - 打开 Unity 工程
    - 运行 Launcher 场景
    - 查看控制台日志确认连接成功

## 📁 项目结构

```
GameFrameX.Server/
├── GameFrameX.Apps/              # 应用层（状态数据）
│   ├── Account/                  # 账号模块
│   ├── Player/                   # 玩家模块
│   └── Server/                   # 服务器模块
├── GameFrameX.Hotfix/            # 热更新层（业务逻辑）
│   ├── Logic/                    # 业务逻辑
│   └── StartUp/                  # 启动逻辑
├── GameFrameX.Core/              # 核心框架
│   ├── Actors/                   # Actor 系统
│   ├── Components/               # 组件系统
│   └── Events/                   # 事件系统
├── GameFrameX.NetWork/           # 网络模块
├── GameFrameX.DataBase/          # 数据库模块
├── GameFrameX.Config/            # 配置模块
└── GameFrameX.Launcher/          # 启动器
```

## 🔧 核心概念

### Entity-Component-State 架构

- **Entity**：游戏实体（玩家、公会、系统等）
- **Component**：功能组件（背包、任务、战斗等）
- **State**：状态数据（组件的持久化数据）

### Actor 模型

每个 Entity 都有一个对应的 Actor，所有对 Entity 的操作都通过 Actor 进行，保证线程安全：

```csharp
// 获取玩家组件代理
var playerAgent = await ActorManager.GetComponentAgent<PlayerComponentAgent>(playerId);

// 调用组件方法（自动入队，线程安全）
var result = await playerAgent.AddExp(1000);
```

### 热更新机制

- **Apps 工程**：包含状态数据，不可热更
- **Hotfix 工程**：包含业务逻辑，支持热更
- **代理模式**：通过 Agent 代理实现逻辑与状态分离

## 📚 开发指南

### 创建新的游戏模块

1. **定义状态类**（Apps 工程）
   ```csharp
   public class BagState : StateBase
   {
       public List<Item> Items { get; set; } = new();
       public int MaxSlots { get; set; } = 100;
   }
   ```

2. **创建组件类**（Apps 工程）
   ```csharp
   public class BagComponent : StateComponent<BagState>
   {
       // 组件初始化逻辑
   }
   ```

3. **实现业务逻辑**（Hotfix 工程）
   ```csharp
   public class BagComponentAgent : StateComponentAgent<BagComponent, BagState>
   {
       public async Task<bool> AddItem(int itemId, int count)
       {
           // 业务逻辑实现
           return true;
       }
   }
   ```

### 消息处理

```csharp
[MessageMapping(typeof(ReqLogin))]
public class LoginHandler : BaseMessageHandler
{
    public override async Task<MessageObject> Action(MessageObject message)
    {
        var request = (ReqLogin)message;
        // 处理登录逻辑
        return new RespLogin { Success = true };
    }
}
```

## 🔍 性能优化

### Actor 设计原则

1. **独立性**：尽可能减少 Actor 间的依赖
2. **粒度控制**：合理拆分 Actor，避免过度细分
3. **避免死锁**：遵循层级调用原则（低层级调用高层级）

### 内存管理

- 自动回收不活跃的玩家数据
- 使用对象池减少 GC 压力
- 合理设置缓存策略

## 🛠️ 部署指南

### Docker 部署

```bash
# 构建镜像
docker build -t gameframex .

# 运行容器
docker run -d -p 8080:8080 gameframex
```

# GameFrameX OpenTelemetry 指标配置文档

## 概述

本文档详细说明了 `OpenTelemetryExtensions.cs` 文件中配置的所有 OpenTelemetry 指标，包括指标的来源、用途和配置方式。

## 配置的指标列表

### 系统级指标

- **Microsoft.AspNetCore.Hosting**: ASP.NET Core 主机指标
    - 请求处理时间
    - 请求计数
    - 错误率
    - 并发连接数

- **System.Net.Http**: HTTP 客户端指标
    - HTTP 请求持续时间
    - HTTP 请求计数
    - HTTP 响应状态码分布

- **System.Runtime**: .NET 运行时指标
    - GC 收集次数
    - 内存使用情况
    - 线程池状态
    - 异常计数

### 数据库指标

- **MongoDB.Driver.Core.Extensions.DiagnosticSources**: MongoDB 数据库指标
    - 数据库连接状态
    - 查询执行时间
    - 命令执行计数
    - 连接池状态

### GameFrameX 自定义业务指标

- **GameFrameX.Database**: 数据库相关指标
    - `gameframex_database_query_total`: 数据库查询总次数
    - `gameframex_database_query_duration_seconds`: 数据库查询持续时间
    - `gameframex_database_connection_pool_size`: 数据库连接池大小
    - `gameframex_database_active_connections`: 数据库活跃连接数

- **GameFrameX.Network**: 网络相关指标
    - `gameframex_network_connection_total`: 网络连接总次数
    - `gameframex_network_message_sent_total`: 网络消息发送总数
    - `gameframex_network_message_received_total`: 网络消息接收总数
    - `gameframex_network_bytes_sent_total`: 网络字节发送总数
    - `gameframex_network_bytes_received_total`: 网络字节接收总数
    - `gameframex_network_current_connections`: 当前网络连接数

- **GameFrameX.Session**: 会话相关指标
    - `gameframex_session_created_total`: 会话创建总数
    - `gameframex_session_destroyed_total`: 会话销毁总数
    - `gameframex_session_duration_seconds`: 会话持续时间
    - `gameframex_session_active_count`: 当前活跃会话数

- **GameFrameX.Business**: 业务相关指标
    - `gameframex_player_login_total`: 玩家登录总数
    - `gameframex_player_register_total`: 玩家注册总数
    - `gameframex_game_room_created_total`: 游戏房间创建总数
    - `gameframex_online_player_count`: 当前在线玩家数
    - `gameframex_active_game_room_count`: 当前活跃游戏房间数
    - `gameframex_http_api_request_total`: HTTP API 请求总数
    - `gameframex_http_api_request_duration_seconds`: HTTP API 请求持续时间

### 1. ASP.NET Core 内置指标

#### 1.1 Microsoft.AspNetCore.Hosting

- **配置方式**: `configure.AddMeter("Microsoft.AspNetCore.Hosting")`
- **指标类型**: ASP.NET Core 托管相关指标
- **主要指标**:
    - `http.server.request.duration` - HTTP 服务器请求持续时间
    - `http.server.active_requests` - 当前活跃的 HTTP 请求数量
- **用途**: 监控 Web 应用程序的 HTTP 请求性能和负载情况
- **可用性**: .NET 8+

#### 1.2 Microsoft.AspNetCore.Server.Kestrel

- **配置方式**: `configure.AddMeter("Microsoft.AspNetCore.Server.Kestrel")`
- **指标类型**: Kestrel Web 服务器相关指标
- **主要指标**:
    - `kestrel.active_connections` - 当前活跃连接数
    - `kestrel.connection.duration` - 连接持续时间
    - `kestrel.rejected_connections` - 被拒绝的连接数
    - `kestrel.queued_connections` - 排队等待的连接数
    - `kestrel.queued_requests` - 排队等待的请求数
- **用途**: 监控 Kestrel 服务器的连接状态和性能
- **可用性**: .NET 8+

### 2. System.Net 网络库指标

#### 2.1 System.Net.Http

- **配置方式**: `configure.AddMeter("System.Net.Http")`
- **指标类型**: HTTP 客户端相关指标
- **主要指标**:
    - `http.client.request.duration` - HTTP 客户端请求持续时间
    - `http.client.active_requests` - 当前活跃的客户端请求数
    - `http.client.open_connections` - 打开的 HTTP 连接数
    - `http.client.connection.duration` - HTTP 连接持续时间
- **用途**: 监控应用程序发出的 HTTP 请求性能和连接状态
- **可用性**: .NET 8+

#### 2.2 System.Net.NameResolution

- **配置方式**: `configure.AddMeter("System.Net.NameResolution")`
- **指标类型**: DNS 解析相关指标
- **主要指标**:
    - `dns.lookup.duration` - DNS 查找持续时间
- **用途**: 监控 DNS 解析性能，帮助诊断网络连接问题
- **可用性**: .NET 8+

### 3. .NET 运行时指标

#### 3.1 System.Runtime

- **配置方式**: `configure.AddMeter("System.Runtime")`
- **指标类型**: .NET 运行时性能指标
- **主要指标**:
    - `dotnet.process.cpu.time` - 进程 CPU 使用时间
    - `dotnet.process.memory.working_set` - 进程工作集内存
    - `dotnet.gc.collections` - GC 收集次数
    - `dotnet.gc.heap.total_allocated` - 堆总分配内存
    - `dotnet.gc.last_collection.memory.committed_size` - 最后一次 GC 后提交的内存大小
    - `dotnet.gc.last_collection.heap.size` - 最后一次 GC 后堆大小
    - `dotnet.gc.last_collection.heap.fragmentation.size` - 最后一次 GC 后堆碎片大小
    - `dotnet.thread_pool.thread.count` - 线程池线程数量
    - `dotnet.thread_pool.work_item.count` - 线程池工作项数量
    - `dotnet.thread_pool.queue.length` - 线程池队列长度
- **用途**: 监控 .NET 应用程序的运行时性能，包括内存使用、GC 行为和线程池状态
- **可用性**: .NET 9+ (注意：在 .NET 8 中可能不会产生数据)

### 4. ASP.NET Core Blazor 组件指标（可选）

#### 4.1 Microsoft.AspNetCore.Components

- **配置方式**: `configure.AddMeter("Microsoft.AspNetCore.Components")` (当前已注释)
- **指标类型**: Blazor 组件相关指标
- **用途**: 监控 Blazor 组件的性能和行为
- **状态**: 已注释，仅在使用 Blazor 功能时启用

#### 4.2 Microsoft.AspNetCore.Components.Lifecycle

- **配置方式**: `configure.AddMeter("Microsoft.AspNetCore.Components.Lifecycle")` (当前已注释)
- **指标类型**: Blazor 组件生命周期指标
- **用途**: 监控 Blazor 组件的生命周期事件
- **状态**: 已注释，仅在使用 Blazor 功能时启用

#### 4.3 Microsoft.AspNetCore.Components.Server.Circuits

- **配置方式**: `configure.AddMeter("Microsoft.AspNetCore.Components.Server.Circuits")` (当前已注释)
- **指标类型**: Blazor Server 电路相关指标
- **用途**: 监控 Blazor Server 应用程序的电路状态
- **状态**: 已注释，仅在使用 Blazor Server 功能时启用

## 配置结构

### 主要配置方法

1. **AddGameFrameXOpenTelemetry**: 主要的 OpenTelemetry 配置方法
2. **AddGameFrameXHealthChecks**: 添加健康检查服务配置
3. **UseGameFrameXHealthChecks**: 配置健康检查端点
4. **CreateMetricsServerAsync**: 创建独立的指标服务器

### 配置特性

- **条件配置**: 通过 `setting.IsOpenTelemetry` 和 `setting.IsOpenTelemetryMetrics` 控制是否启用
- **开发环境支持**: 在开发环境下启用控制台导出器
- **Prometheus 导出**: 配置 Prometheus 格式的指标导出
- **Grafana 集成**: 通过 `UseGrafana()` 集成 Grafana 监控

### 导出器配置

- **Prometheus 导出器**: `configure.AddPrometheusExporter()`
- **控制台导出器**: `configure.AddConsoleExporter()` (仅开发环境)
- **Grafana 集成**: `builder.UseGrafana()`

## 独立指标服务器

### 功能特性

- **独立端口**: 在指定端口上运行独立的指标服务器
- **健康检查**: 提供 `/health` 端点进行健康检查
- **Prometheus 端点**: 提供 `/metrics` 端点供 Prometheus 抓取
- **端口检查**: 启动前检查端口可用性

### 端点说明

- **指标端点**: `http://{ip}:{port}/metrics`
- **健康检查端点**: `http://{ip}:{port}/health`

## 使用建议

### 1. 监控指标使用示例

```csharp
// 在业务代码中使用自定义指标
using GameFrameX.Monitor;

// 记录数据库查询
GameFrameXMetrics.DatabaseQueryCount.Add(1, new KeyValuePair<string, object?>("operation", "find"));

// 记录网络消息
GameFrameXMetrics.NetworkMessageSentCount.Add(1, new KeyValuePair<string, object?>("message_type", "login"));

// 记录玩家登录
GameFrameXMetrics.PlayerLoginCount.Add(1, new KeyValuePair<string, object?>("login_type", "normal"));
```

### 2. 指标标签建议

为了更好地分析监控数据，建议在记录指标时添加适当的标签：

- **数据库指标标签**：
    - `operation`: 操作类型（find, insert, update, delete）
    - `collection`: 集合名称
    - `database`: 数据库名称

- **网络指标标签**：
    - `message_type`: 消息类型
    - `protocol`: 协议类型（tcp, udp, websocket）
    - `endpoint`: 端点信息

- **会话指标标签**：
    - `session_type`: 会话类型
    - `user_type`: 用户类型

- **业务指标标签**：
    - `game_mode`: 游戏模式
    - `server_region`: 服务器区域
    - `user_level`: 用户等级

### 3. 性能考虑

- 避免创建过多的标签组合，以免影响性能
- 对于高频操作，考虑使用采样策略
- 定期清理不再使用的指标实例

### 4. 环境配置建议

1. **生产环境**: 建议启用所有相关指标以获得完整的监控覆盖
2. **开发环境**: 可以启用控制台导出器进行实时调试
3. **Blazor 应用**: 如果使用 Blazor 功能，取消注释相关的 Blazor 指标配置
4. **.NET 版本**: 注意 `System.Runtime` 指标需要 .NET 9+ 才能正常工作
5. **资源监控**: 定期监控指标收集对应用程序性能的影响

## 健康检查功能

### 健康检查服务配置

#### AddGameFrameXHealthChecks 方法

- **功能**: 添加健康检查服务到依赖注入容器
- **配置**:
    - 基本健康检查：检查应用程序自身状态
    - OpenTelemetry 健康检查：当启用 OpenTelemetry 时自动添加
- **用途**: 提供应用程序健康状态监控

#### UseGameFrameXHealthChecks 方法

- **功能**: 配置健康检查 HTTP 端点
- **端点**:
    - `/health`: 详细的 JSON 格式健康检查报告
    - `/health/simple`: 简单的 "OK" 响应（兼容性端点）
- **响应格式**: JSON 格式，包含状态、检查项详情、持续时间、服务器信息和时间戳

### 健康检查响应示例

```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "description": "应用程序运行正常",
      "duration": 0.1234
    },
    {
      "name": "opentelemetry",
      "status": "Healthy",
      "description": "OpenTelemetry 配置正常",
      "duration": 0.0567
    }
  ],
  "totalDuration": 0.1801,
  "serverName": "GameServer",
  "tagName": "Production",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

### 使用方式

```csharp
// 在 Program.cs 或 Startup.cs 中配置
builder.Services.AddGameFrameXHealthChecks(appSetting);

// 配置健康检查端点
app.UseGameFrameXHealthChecks(appSetting);
// 或指定自定义路径
app.UseGameFrameXHealthChecks(appSetting, "/api/health");
```

## 注意事项

- `System.Runtime` 指标在 .NET 8 中可能不会产生数据，建议在 .NET 9+ 中使用
- Blazor 相关指标仅在使用相应功能时才会产生有意义的数据
- 指标收集会对应用程序性能产生轻微影响，建议在生产环境中进行性能测试
- 独立指标服务器需要额外的端口资源，确保端口不被其他服务占用
- **健康检查功能独立于 Grafana 配置**，即使不启用 OpenTelemetry 也可以单独使用健康检查功能
- 健康检查端点应该被监控系统（如 Kubernetes、Docker Swarm、负载均衡器等）用于服务健康状态检测
-

### 生产环境配置

1. **数据库配置**：配置 MongoDB 集群
2. **负载均衡**：使用 Nginx 或 HAProxy
3. **监控告警**：集成 Prometheus + Grafana
4. **日志收集**：使用 ELK 或类似方案

## 🤝 贡献指南

我们欢迎所有形式的贡献！

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

## 📄 许可证

本项目采用 Apache License 2.0 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🔗 相关链接

- [📖 详细文档](https://gameframex.doc.alianblank.com)
- [🎥 视频教程](https://www.bilibili.com/video/BV1yrpeepEn7)
- [🏠 项目主页](https://github.com/GameFrameX)
- [💬 社区讨论](https://github.com/GameFrameX/GameFrameX/discussions)

## 🙏 致谢

感谢所有为 GameFrameX 做出贡献的开发者们！

---

<div align="center">

**如果这个项目对你有帮助，请给我们一个 ⭐**

</div>
