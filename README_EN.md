<div align="center">

# GameFrameX

**High-Performance, Cross-Platform Game Server Framework**

[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)]()
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

English | [简体中文](README.md)

</div>

## 📖 Introduction

GameFrameX is a high-performance game server framework developed with C# .NET 8.0, designed with the Actor model and supporting cross-platform deployment. The framework features built-in hot update mechanisms and can meet the requirements of most game types, especially suitable for collaborative development with Unity3D.

**Design Philosophy: Simplicity is the ultimate sophistication**

## ✨ Core Features

### 🚀 High-Performance Architecture
- **Actor Model**: High-performance Actor system built on TPL DataFlow
- **Full Asynchronous Programming**: Uses async/await patterns for clean and elegant code
- **Lock-Free Design**: Avoids performance loss from traditional locking mechanisms through Actor model
- **Memory Optimization**: Automatic recycling of inactive data to reduce memory usage

### 🔄 Hot Update Support
- **Zero-Downtime Updates**: Supports runtime hot updates of game logic
- **State-Logic Separation**: State persistence with hot-updatable logic
- **Safe and Reliable**: Rollback capability on update failure to ensure service stability
- **Incremental Updates**: Only updates modified parts for improved efficiency

### 🌐 Network Communication
- **Multi-Protocol Support**: TCP, UDP, WebSocket, HTTP
- **High Concurrency**: Asynchronous I/O model based on SuperSocket
- **Message Processing**: Built-in message fragmentation and sticky packet handling
- **Secure Transmission**: SSL/TLS encryption support

### 💾 Data Persistence
- **Transparent Persistence**: Automatic serialization/deserialization, developers don't need to worry about database operations
- **NoSQL Support**: Uses MongoDB by default, supports other NoSQL databases
- **Caching Mechanism**: Intelligent caching strategy to improve data access performance

### ⏰ Scheduled Tasks
- **Multiple Timers**: One-time, periodic, and scheduled tasks
- **Thread Safety**: Built-in thread-safe Timer and Scheduler
- **Event System**: Complete event-driven architecture

## 🏗️ Architecture Design

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

## 🚀 Quick Start

### Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB 4.x+](https://www.mongodb.com/try/download/community)
- Visual Studio 2022 or JetBrains Rider

### Installation Steps

1. **Clone the Project**
   ```bash
   git clone https://github.com/GameFrameX/GameFrameX.git
   cd GameFrameX/Server
   ```

2. **Install Dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure Database**
   - Start MongoDB service
   - Modify database connection string in configuration file

4. **Build and Run**
   ```bash
   dotnet build
   dotnet run --project GameFrameX.Launcher
   ```

5. **Verify Running**
   - Open Unity project
   - Run Launcher scene
   - Check console logs to confirm successful connection

## 📁 Project Structure

```
GameFrameX.Server/
├── GameFrameX.Apps/              # Application Layer (State Data)
│   ├── Account/                  # Account Module
│   ├── Player/                   # Player Module
│   └── Server/                   # Server Module
├── GameFrameX.Hotfix/            # Hot Update Layer (Business Logic)
│   ├── Logic/                    # Business Logic
│   └── StartUp/                  # Startup Logic
├── GameFrameX.Core/              # Core Framework
│   ├── Actors/                   # Actor System
│   ├── Components/               # Component System
│   └── Events/                   # Event System
├── GameFrameX.NetWork/           # Network Module
├── GameFrameX.DataBase/          # Database Module
├── GameFrameX.Config/            # Configuration Module
└── GameFrameX.Launcher/          # Launcher
```

## 🔧 Core Concepts

### Entity-Component-State Architecture

- **Entity**: Game entities (players, guilds, systems, etc.)
- **Component**: Functional components (inventory, quests, combat, etc.)
- **State**: State data (persistent data of components)

### Actor Model

Each Entity has a corresponding Actor, and all operations on Entity are performed through Actor to ensure thread safety:

```csharp
// Get player component agent
var playerAgent = await ActorManager.GetComponentAgent<PlayerComponentAgent>(playerId);

// Call component method (automatically queued, thread-safe)
var result = await playerAgent.AddExp(1000);
```

### Hot Update Mechanism

- **Apps Project**: Contains state data, not hot-updatable
- **Hotfix Project**: Contains business logic, supports hot updates
- **Proxy Pattern**: Implements logic-state separation through Agent proxy

## 📚 Development Guide

### Creating New Game Modules

1. **Define State Class** (Apps Project)
   ```csharp
   public class BagState : StateBase
   {
       public List<Item> Items { get; set; } = new();
       public int MaxSlots { get; set; } = 100;
   }
   ```

2. **Create Component Class** (Apps Project)
   ```csharp
   public class BagComponent : StateComponent<BagState>
   {
       // Component initialization logic
   }
   ```

3. **Implement Business Logic** (Hotfix Project)
   ```csharp
   public class BagComponentAgent : StateComponentAgent<BagComponent, BagState>
   {
       public async Task<bool> AddItem(int itemId, int count)
       {
           // Business logic implementation
           return true;
       }
   }
   ```

### Message Processing

```csharp
[MessageMapping(typeof(ReqLogin))]
public class LoginHandler : BaseMessageHandler
{
    public override async Task<MessageObject> Action(MessageObject message)
    {
        var request = (ReqLogin)message;
        // Handle login logic
        return new RespLogin { Success = true };
    }
}
```

## 🔍 Performance Optimization

### Actor Design Principles

1. **Independence**: Minimize dependencies between Actors
2. **Granularity Control**: Reasonably split Actors, avoid over-fragmentation
3. **Avoid Deadlocks**: Follow hierarchical calling principles (lower level calls higher level)

### Memory Management

- Automatic recycling of inactive player data
- Use object pools to reduce GC pressure
- Reasonable cache strategy settings

## 🛠️ Deployment Guide

### Docker Deployment

```bash
# Build image
docker build -t gameframex .

# Run container
docker run -d -p 8080:8080 gameframex
```

### Production Environment Configuration

1. **Database Configuration**: Configure MongoDB cluster
2. **Load Balancing**: Use Nginx or HAProxy
3. **Monitoring and Alerting**: Integrate Prometheus + Grafana
4. **Log Collection**: Use ELK or similar solutions

## 🤝 Contributing

We welcome all forms of contributions!

1. Fork this repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Create a Pull Request

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## 🔗 Related Links

- [📖 Detailed Documentation](https://gameframex.doc.alianblank.com)
- [🎥 Video Tutorials](https://www.bilibili.com/video/BV1yrpeepEn7)
- [🏠 Project Homepage](https://github.com/GameFrameX)
- [💬 Community Discussions](https://github.com/GameFrameX/GameFrameX/discussions)

## 🙏 Acknowledgments

Thanks to all developers who have contributed to GameFrameX!

---

<div align="center">

**If this project helps you, please give us a ⭐**

</div>