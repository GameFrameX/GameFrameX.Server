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
