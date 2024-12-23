# GameFrameX介绍：

GameFrameX 是基于GeekServer是一个开源的[分区分服](https://mp.weixin.qq.com/s?__biz=MzI3MTQ1NzU2NA==&mid=2247483884&idx=1&sn=3547c769a300f1d82cc04e9b1852c6d5&chksm=eac0cd9fddb7448997e38a74e2d26bde259cd2127583e31bc488511bc1fdcd9f35caff27d4a3&scene=21#wechat_redirect)
的游戏服务器框架，采用C# .Netcore开发，开发效率高，性能强，跨平台，并内置不停服热更新机制。可以满足绝大部分游戏类型的需求，特别是和Unity3D协同开发更佳。    
__设计理念:大道至简，以简化繁__

# 程序集说明

|        程序集名称        |             介绍             |                               用途                               |
|:-------------------:|:--------------------------:|:--------------------------------------------------------------:|
| GameFrameX.Launcher |           程序启动入口           |                            用于编写启动逻辑                            |
|  GameFrameX.Hotfix  | 热更新逻辑和处理程序对象放置区(该部分为热更新内容) |               用于编写逻辑的地方(`主要工作区`,目录和`Apps`目录结构一致)               |
|   GameFrameX.Apps   |    组件和实体对象放置区(该部分不能热更新)    | 用于编写基础逻辑和数据存储(`功能的添加修改主要工作区`,目录结构按照`ServerType` 划分和`HotFix`对应) |
|  GameFrameX.Config  |       配置文件对象和配置文件放置区       |                 用于编写配置文件的类和文件的映射(目前由LuBan自动生成)                 |
|  GameFrameX.Proto   |        数据通讯协议对象放置区         |                           用于编写通讯协议对象                           |

# GameFrameX功能：

### 1.跨平台

使用C# .Netcore开发（可以跨平台，可以跨平台，可以跨平台），.Netcore现在功能和性能都已经十分强大和稳健，不管是在windows还是linux上部署起来都很简便。

### 2.全面异步编程

全部采用异步编程（async/await），让逻辑代码变得整洁优雅，清晰易懂，让代码写起来行如流水。

### 3.TPL(Task Parallel Library) Actor模型

Actor模型构建于强大的TPL DataFlow之上，让Actor模型如虎添翼。（不了解Actor模型，可以搜一下相关资料，Akka，Orleans都是采用的Actor模型）[了解更多](https://github.com/leeveel/GeekServer/blob/main/Docs/1.Actor%E6%A8%A1%E5%9E%8B.md)

### 4.Actor入队透明化

内部会自动处理线程上下文, 编译期间会通过[Source Generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)自动生成入队代码, 开发人员无需关心多线程以及入队逻辑,
只需要像调用普通函数一样书写逻辑。[了解更多](https://github.com/leeveel/GeekServer/blob/main/Docs/Actor%E5%85%A5%E9%98%9F.md)

### 5.Actor死锁检测

Actor模型本身是存在死锁的情况，且不容易被发现。内部可检测环路死锁(即:A->B->C->A)，并采用调用链重入机制消除环路死锁。[了解更多](https://github.com/leeveel/GeekServer/blob/main/Docs/1.Actor%E6%A8%A1%E5%9E%8B.md)

### 6.支持不停服更新

采用组件+状态的设计，状态只有属性，没有方法，组件只用方法，没有属性，并通过代理的方式全部放到热更dll中，运行时重新加载dll即可热更所有逻辑。[了解更多](https://github.com/leeveel/GeekServer/blob/main/Docs/%E7%83%AD%E6%9B%B4hotfix.md)

### 7.网络模块

网络模块采用SuperSocket的默认服务器Kestrel，支持协议多（Tcp，udp,Http123,websocket，signalr等），而且性能高[了解更多](https://github.com/leeveel/GeekServer/blob/main/Docs/%E7%BD%91%E7%BB%9CNet(tcp%26http).md)

### 8.持久化透明

采用Nosql作为数据存储，状态的持久化全透明，框架会自动序列化/反序列,让开发人员更加专注于业务逻辑，无需操心数据库。 [了解更多](https://github.com/leeveel/GeekServer/blob/main/Docs/2.Actor%26Component%26State.md)

### 9.Timer/Scheduler/Event

内置线程安全的Timer，Scheduler，Event系统，轻松应对游戏服务器常见的定时，任务计划，事件触发等业务需求。[了解更多](https://github.com/leeveel/GeekServer/blob/main/Docs/%E4%BA%8B%E4%BB%B6Event-timer.md)

### 10.定期释放不活跃内存数据

以功能系统级别的粒度，定期剔除内存中不活跃的玩家数据，尽最大可能减少服务器内存开销。

# 运行

1. 安装[.NetCore8.0](https://dotnet.microsoft.com/download/dotnet/6.0)
2. 安装[mongodb4.x](https://www.mongodb.com/try/download/community)
3. 打开git clone本项目
4. 用VisualStudio 或者Rider打开Server.sln 启动 `GameFrameX.Launcher`
5. 打开Unity工程，打开Launcher 场景，运行查看日志

# Doc (已经在写了,别催了!-_-!)

`所有站点内容一致，不存在内容不一致的情况`

文档地址 : https://gameframex.doc.alianblank.com

备用文档地址 : https://gameframex-docs.pages.dev

备用文档地址 : https://gameframex.doc.cloudflare.alianblank.com

备用文档地址 : https://gameframex.doc.vercel.alianblank.com

# 文档&例子&Demo

[视频教程](https://www.bilibili.com/video/BV1yrpeepEn7/)



