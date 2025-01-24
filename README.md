# GameFrameX介绍：

GameFrameX 是基于 `GeekServer` 是一个开源的[分区分服](https://mp.weixin.qq.com/s?__biz=MzI3MTQ1NzU2NA==&mid=2247483884&idx=1&sn=3547c769a300f1d82cc04e9b1852c6d5&chksm=eac0cd9fddb7448997e38a74e2d26bde259cd2127583e31bc488511bc1fdcd9f35caff27d4a3&scene=21#wechat_redirect)
的游戏服务器框架，采用C# .Netcore开发，开发效率高，性能强，跨平台，并内置不停服热更新机制。可以满足绝大部分游戏类型的需求，特别是和Unity3D协同开发更佳。

__设计理念:大道至简，以简化繁__

# GameFrameX功能：

## 1.跨平台

使用C# .Netcore开发（可以跨平台，可以跨平台，可以跨平台），.Netcore现在功能和性能都已经十分强大和稳健，不管是在windows还是linux上部署起来都很简便。

## 2.全面异步编程

全部采用异步编程（async/await），让逻辑代码变得整洁优雅，清晰易懂，让代码写起来行如流水。

## 3.TPL(Task Parallel Library) Actor模型

Actor模型构建于强大的TPL DataFlow之上，让Actor模型如虎添翼。（不了解Actor模型，可以搜一下相关资料，Akka，Orleans都是采用的Actor模型）

## 4.Actor入队透明化

内部会自动处理线程上下文, 编译期间会通过[Source Generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)自动生成入队代码, 开发人员无需关心多线程以及入队逻辑,
只需要像调用普通函数一样书写逻辑。

#### Actor入队透明

`GameFrameX` 编译期间会自动注入入队代码(**AgentWeaver**), 开发人员无需入队逻辑, 只需要像调用普通函数一样书写逻辑。

```c#
//编译期间会注入一个继承自xxxCompAgent的wrapper类,来实现自动入队
//同时SendAsync内部自动处理了线程上下文,开发者只需要像调用普通函数一样书写逻辑
public class ServerComponentAgentWrapper : ServerComponentAgent
{
	public override Task CheckCrossDay()
	{
		return base.Actor.SendAsync((Func<Task>)base.CheckCrossDay, isAwait: false, 10000);
	}

	public override Task<int> GetDaysFromOpenServer()
	{
		return base.Actor.SendAsync((Func<Task<int>>)base.GetDaysFromOpenServer, isAwait: true, 10000);
	}
}

var serverComp = await EntityMgr.GetCompAgent<ServerComponentAgent>(ActorType.Server);
//使用方式(就像调用普通函数一样,无需关心多线程或入队)
_ = serverComp.CheckCrossDay();

```

#### 线程上下文

`GameFrameX` 内部会自动处理线程上下文，由RuntimeContext实现，主要用于环路调用检测，以及判断是否需要入队，其内部使用**AsyncLocal**实现

```c#
internal class RuntimeContext
{
    internal static long Current => callCtx.Value;
    internal static AsyncLocal<long> callCtx = new AsyncLocal<long>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void SetContext(long callChainId)
    {
        callCtx.Value = callChainId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ResetContext()
    {
        callCtx.Value = 0;
    }
}
```

## 5.Actor死锁检测

#### Actor模型 ##

actor模型是什么可以自行查阅一下相关资料；actor模型在一定程度可以说是解决并发的最佳方案。

`GameFrameX` 的actor可以简单理解为一个线程(其实用的是线程池)，一个actor的逻辑只需保证在自己的队列里面执行即保证可线程安全，无需关心锁的问题。 `GameFrameX` 的基础就是actor，一切皆可为actor。 `GameFrameX` 的actor模型构建于强大的TPL DataFlow之上，底层使用内置线程池。 `GameFrameX` 同时使用异步变成（async/await），让逻辑代码更加清晰明了，符合人类的思维方式。

#### Actor死锁 ##

Actor模型本身是存在死锁的情况，且不容易被发现。 `GameFrameX` 内部可检测环路死锁(即:A->B->C->A)，并采用调用链重入机制消除环路死锁（类似与线程的可重入性）。  
还有另外一种情况（多路死锁），比如有2个actor，A和B，一段逻辑A调用B，同时有另外一段逻辑发起了B调用A，就会出现A等B，B再等A，此时发生死锁。同理 [A->B->C，C->D->A] [A->B->C，B->C->A],这样的调用路径都可能会存在死锁。  
这种死锁无法解决，而且不确定，因为它和调用时间节点有关系，只能从设计上去规避。

```csharp
class ActorA
{
	Task A1()
	{
        await Task.Delay(10); 
		var b = GetActorB();
		return b.SendAsync(b.B1);
	}
	
	Task A2()
	{
		var b = GetActorB();
		return b.SendAsync(b.B1);
	}
}

class ActorB
{
	Task B1()
	{
		await Task.Delay(5);
		var a = GetActorA();
		return a.SendAsync(a.A2);
	}
}

Task Call()
{
	var a = GetActorA();
	return a.SendAsync(a.A1);//这里就会触发死锁
}
```

如果得到打印日志【执行超时】很有可能就是触发了死锁。由于Actor设定了超时时间，在断点调试的时候需要手动将超时时间改长，默认10秒，在BaseActor.TIME_OUT修改。**（或者用一个Debug模式的宏来控制）**

#### 多路死锁解决方案 ##

由以上的分析可以看出，多路死锁其实是由于**Await**引起的，如果所有调用都不Await则永远不会有死锁。  
这里为大家罗列了几种解决方案：(在 `GameFrameX` 中可以通过ActorLimit来定制自己的检测规则)

1. **跨Actor调用，不能Await**  
   优点：规则简单，统一，绝对不会发生死锁  
   缺点：失去了异步语法的优势，所有需要跨Actor获取返回值的，只能使用回调，代码结构散乱，书写代码不方便。

2. **为Actor分配等级，只允许低等级await调用高等级（如：Role->Server 而Server不能等待调用Role, 推荐使用此方案）**  
   案例：公民可以去政府部门排队等待办理业务员，但是政府部门不可能去等待某个人去处理完自己的私事，再帮下一个人办理业务，顶多发短信或者打电话通知某个人做什么事情。  
   在方案1的基础上，给了使用者更多的异步调用空间。

3. **提供一个注册接口，注册哪些Actor可以Await调用哪些Actor，并在注册时候进行检测看是否可能存在死锁**    
   这是方案2的一个更加灵活的变种。

4. **允许交错执行** [了解更多](https://blog.csdn.net/uddiqpl/article/details/86294520)    
   如果你的某个组件不会操作数据或者对操作顺序不敏感，交错执行是很有用的。 `GameFrameX` 中的FuncComponent比较符合这个特点，但不绝对，仍需要开发人员自行判断。 （Orleans中有此方案）

5. **允许存在多路死锁的风险，由开发人员保证不会触发死锁**  
   优点：规则简单，统一，书写代码很方便，全程异步。  
   缺点：有可能发生死锁，对编码人员能力要求较高

6. **超时规则**  
   如果发生多路死锁，选择一条调用路径并终止，保证其中一条调用路径正确。（Orleans中有此方案）  
   优缺点和方案5一样。

7. **使用线程安全容器以及lock等**   
   但这与无锁化设计的理念冲突

## 6.支持不停服更新

采用组件+状态的分离设计实现不停服热更新:

1. 架构设计优势

- 状态与逻辑完全分离,状态类只包含数据属性
- 组件类只包含业务逻辑方法,不持有状态
- 通过代理模式将所有逻辑代码放入热更新dll
- 运行时只需重载dll即可更新全部业务逻辑

2. 热更新优势

- 无需停服即可更新游戏逻辑
- 玩家无感知,不会影响游戏体验
- 支持增量更新,只更新修改的逻辑
- 更新过程安全可靠,失败可回滚

3. 开发效率提升

- 逻辑bug修复无需重启服务器
- 新功能可以随时发布更新
- 减少维护成本和停机时间
- 便于快速验证和调试

#### 热更新

`GameFrameX` 支持不停服热更新逻辑。

#### 热更思路

游戏中的所有状态放在App工程中，始终存在于内存，不能热更。Actor和Component的逻辑使用代理模式（Agent）放到Hotfix工程。热更时载入新的dll（ `GameFrameX` .Hotfix.dll），清除所有老的Agent，所有新逻辑重新从Actor/Component获取新的Agent汇入新dll中执行热更后的逻辑，达到热更目的。正在执行老dll逻辑的代码获取的Agent依然来自热更前的老Dll中，等待老dll中的逻辑执行完后清理掉内存中老的dll。底层使用接口驱动热更dll中的逻辑。
需要注意的是，热更时新的dll需要放在新的目录下面，然后再载入内存，因为老的dll可能正在运行，是无法直接覆盖的。

##### 可以热更部分

可以热更的逻辑都应该放在 `GameFrameX` .Hotfix工程中

1. 所有Actor/Component的Agent，Agent中只有逻辑没有状态，状态全部放到Component的State
2. HttpHandler
3. TcpHandler
4. 协议
5. 配置表/配置表代码

##### 热更新流程

1. 游戏后台将新的 `GameFrameX` .Hotfix.dll及相关文件（对应pdb，json等）拷贝到游戏服特定目录下
2. 游戏后台向游戏服发送http命令，通知进行热更，并告知dll目录，md5等信息
3. 游戏服中热更HttpHandler根据后台信息，验证热更dll完整性，合法性，修改dllVersion.txt，发起热更调用

## 7.网络模块

网络模块采用SuperSocket作为底层网络框架，具有以下优势:

1. 高性能和可扩展性

- 基于.NET Core的高性能网络库
- 采用异步IO模型,支持高并发连接
- 内置连接池和内存池优化,减少GC压力
- 可配置的线程模型,支持灵活的扩展

2. 协议支持全面

- 支持TCP、UDP、WebSocket等多种传输协议
- 内置HTTP 1.1/2/3协议支持
- 支持SignalR实时通信
- 支持自定义二进制协议
- 支持SSL/TLS加密传输

3. 功能特性丰富

- 内置会话管理和心跳检测
- 支持消息过滤和管道处理
- 提供命令过滤器机制
- 支持消息分包和粘包处理
- 支持二进制和文本消息编解码

4. 开发便捷性

- 简单易用的API设计
- 完善的文档和示例
- 支持配置化开发
- 提供多种扩展点
- 活跃的社区支持

[了解更多SuperSocket详情](https://github.com/GameFrameX/GameFrameX.SuperSocket)

## 8.持久化透明

采用Nosql作为数据存储，状态的持久化全透明，框架会自动序列化/反序列,让开发人员更加专注于业务逻辑，无需操心数据库。

#### Entity和Component和State

`GameFrameX` 的Entity包含1个/多个Component，Component包含0个/1个State，你可以这样理解：Entity=实体，Component=功能系统，State=功能系统的数据。每个Entity都包含一个Actor成员变量，Entity的所有行为调度都有Actor完成，以保证线程安全。

Entity是 `GameFrameX` 的基础，一切功能都可以作为单独的Entity（比如一个角色，一个公会，一个玩法系统。。。），Component隶属于Entity，State隶属于Component，Component承载各种逻辑，State承载各种数据。Entity拆分方式根据项目的具体需求可以任意规划。

#### Entity(Actor)拆分

1. 尽可能独立（一个系统或者玩家的操作尽量不阻塞其他玩家的操作）
2. 在独立的前提下尽可能少（节约管理成本）
3. 一个角色，包含若干个功能系统（背包，宠物，任务。。。）
4. 一个公会，包含多个公会基础系统（基础信息，捐赠，工会副本。。。）

比如有2个全局玩法，世界boss和工会战，如果这2个系统归于一个Actor，那么当一个玩家挑战世界boss时，挑战逻辑到Actor线程中执行，此时另一个玩家要去报名工会战，报名逻辑也需要到Actor线程中执行，此时报名公会战的逻辑则需要等待前一个玩家挑战世界boss逻辑完成后才能执行，客户端表现可能就是更长时间的网络连接中，在cpu够用的情况下就产生了多余的等待时间，这就有些不合理了；这种功能之间没有任何交集的情况下我们建议将世界boss和工会战分别拆分为Actor，各自的逻辑便可以更加顺畅的执行，客户端等待的时间也越短。

## 9.Timer/Scheduler/Event

内置线程安全的Timer，Scheduler，Event系统，轻松应对游戏服务器常见的定时，任务计划，事件触发等业务需求。

#### 定时器&计划任务

`GameFrameX` 中没有使用传统意义中的Update，除MMO项目，其他大部分游戏类型的服务器基本没有必要使用Update，需要Update的模块添加一个Timer也可以实现
根据热更新设计方案，定时器和计划任务采用接口方式进行回调，任务使用扩展方法实现，

定时器支持：1次性delay,周期性timer。
计划任务：指定时间1次性任务，每天任务，每周任务，每周几任务，每月任务。
`GameFrameX` 中没有对定时器&计划任务做持久化，所以你可能需要在开服后、玩家上线或者Component激活时考虑一下计划任务逻辑是否需要被处理了。
回调函数继承TimerHandler<>，重写HandleTimer，里面处理定时器回调逻辑即可。
需要注意的是定时器是接入的Quartz，由于硬件精度问题（windows时间实际精度为10毫秒左右），回调时间可能会提前1-2毫秒，如果对时间依赖特别大的可能需要特殊处理下，比如在Timer回调后延时50毫秒再执行回调逻辑。

## 10.定期释放不活跃内存数据

以功能系统级别的粒度，定期剔除内存中不活跃的玩家数据，尽最大可能减少服务器内存开销。

# 运行

1. 安装[.NetCore8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
2. 安装[mongodb4.x](https://www.mongodb.com/try/download/community)
3. 打开git clone本项目
4. 用VisualStudio 或者Rider打开Server.sln 启动 `GameFrameX.Launcher`
5. 打开Unity工程，打开Launcher 场景，运行查看日志

# 文档&例子&Demo

[视频教程](https://www.bilibili.com/video/BV1yrpeepEn7)

[参考文档](https://gameframex.doc.alianblank.com)

[项目主页](https://github.com/GameFrameX)
