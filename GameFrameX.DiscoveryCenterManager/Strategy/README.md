# GameFrameX 负载均衡系统

GameFrameX 负载均衡系统提供了一个统一、高性能且可扩展的负载均衡解决方案，集成了多种策略和增强功能。

## 功能特性

### 🔧 核心功能
- **多种负载均衡策略**：轮询、随机、最少连接、一致性哈希、加权轮询、响应时间优化、自适应
- **向后兼容**：完全兼容现有代码，无缝升级
- **性能监控**：详细的实例指标和统计信息
- **健康检查**：自动故障检测和恢复
- **熔断器支持**：防止级联故障

### 🚀 增强功能
- **实时指标收集**：响应时间、成功率、资源使用率
- **地理位置路由**：基于距离的智能路由
- **自适应算法**：根据实时性能动态选择最优策略
- **请求亲和性**：会话保持和一致性哈希支持
- **优先级路由**：基于请求优先级的智能分发

## 快速开始

### 基础用法

```csharp
// 1. 创建简单的轮询负载均衡器
var loadBalancer = LoadBalancerFactory.Create(LoadBalanceStrategy.RoundRobin);

// 2. 选择实例
var availableInstances = new List<long> { 1, 2, 3, 4, 5 };
var selectedInstance = loadBalancer.SelectInstance("GameServer", availableInstances);
```

### 增强用法

```csharp
// 1. 创建增强型负载均衡器
var enhancedLoadBalancer = LoadBalancerFactory.CreateEnhanced(LoadBalanceStrategy.ResponseTime);

// 2. 创建负载均衡上下文
var context = new LoadBalancingContext
{
    ServerType = "GameServer",
    AvailableInstances = new List<long> { 1, 2, 3, 4, 5 },
    ClientId = "client123",
    UserId = "user456",
    Priority = RequestPriority.High,
    GeoLocation = new GeoLocation
    {
        Latitude = 39.9042,
        Longitude = 116.4074,
        City = "Beijing"
    }
};

// 3. 使用增强方法选择实例
var selection = enhancedLoadBalancer.SelectInstanceEnhanced(context);

if (selection.IsSuccess)
{
    Console.WriteLine($"选择实例: {selection.InstanceId}, 原因: {selection.Reason}, 分数: {selection.Score}");
}
else
{
    Console.WriteLine($"选择失败: {selection.ErrorMessage}");
}
```

## 策略详解

### 1. 轮询 (Round Robin)
最简单的策略，按顺序轮流选择实例。

```csharp
var loadBalancer = LoadBalancerFactory.Create< RoundRobinLoadBalancer>();
```

**适用场景**：
- 实例性能相似
- 请求量均匀分布
- 简单可靠

### 2. 随机 (Random)
随机选择实例。

```csharp
var loadBalancer = LoadBalancerFactory.Create(LoadBalanceStrategy.Random);
```

**适用场景**：
- 实例性能相似
- 避免热点问题
- 分布式环境

### 3. 最少连接 (Least Connections)
选择当前连接数最少的实例。

```csharp
var loadBalancer = LoadBalancerFactory.Create< LeastConnectionsLoadBalancer>();
```

**适用场景**：
- 连接处理时间差异较大
- 长连接应用
- 资源消耗敏感

### 4. 一致性哈希 (Consistent Hash)
基于键的哈希值选择实例，确保相同键总是路由到相同实例。

```csharp
var loadBalancer = LoadBalancerFactory.Create(LoadBalanceStrategy.ConsistentHash);
var instanceId = loadBalancer.SelectInstance("GameServer", instances, "user123");
```

**适用场景**：
- 会话保持
- 缓存一致性
- 数据分片

### 5. 加权轮询 (Weighted Round Robin)
根据权重分配请求，权重越高获得越多请求。

```csharp
var weightedLoadBalancer = LoadBalancerFactory.CreateWeightedRoundRobin(new Dictionary<long, int>
{
    { 1, 1 },  // 实例1权重为1
    { 2, 2 },  // 实例2权重为2
    { 3, 3 }   // 实例3权重为3
});
```

**适用场景**：
- 实例性能差异明显
- 资源配置不均
- 精细化流量控制

### 6. 响应时间优化 (Response Time)
选择平均响应时间最短的实例。

```csharp
var responseTimeLoadBalancer = LoadBalancerFactory.CreateResponseTime();
```

**适用场景**：
- 延迟敏感应用
- 实时系统
- 性能优先场景

### 7. 自适应 (Adaptive)
根据实时性能指标动态选择最优策略。

```csharp
var adaptiveConfig = new AdaptiveConfiguration
{
    StrategyUpdateIntervalSeconds = 60,
    EnableContextAwareScoring = true,
    MinimumInstanceCount = 2
};

var adaptiveLoadBalancer = LoadBalancerFactory.CreateAdaptive(adaptiveConfig);
```

**适用场景**：
- 复杂多变的环境
- 自动化运维
- 性能优化要求高


## 实例指标管理

### 更新基础指标

```csharp
// 更新实例连接数
loadBalancer.UpdateInstanceMetrics(instanceId: 1, connections: 25);
```

### 更新增强指标

```csharp
var metrics = new InstanceMetrics
{
    InstanceId = 1,
    TotalRequests = 1000,
    SuccessfulRequests = 950,
    FailedRequests = 50,
    ActiveConnections = 25,
    AverageResponseTimeMs = 85.5,
    CpuUsage = 65.2,
    MemoryUsage = 78.1,
    NetworkThroughputKbps = 1024.5
};

enhancedLoadBalancer.UpdateInstanceMetricsEnhanced(metrics);
```

## 健康检查和故障处理

### 标记实例状态

```csharp
// 标记实例为不可用
loadBalancer.MarkInstanceUnavailable(instanceId: 1);

// 标记实例为可用
loadBalancer.MarkInstanceAvailable(instanceId: 1);
```

### 健康检查

```csharp
var isHealthy = loadBalancer.PerformHealthCheck();
if (!isHealthy)
{
    Console.WriteLine("负载均衡器健康检查失败");
}
```

## 统计信息

### 获取基础统计

```csharp
var stats = loadBalancer.GetStatistics();
Console.WriteLine($"总选择次数: {stats.TotalSelections}");
Console.WriteLine($"成功率: {stats.SuccessRate:F2}%");
Console.WriteLine($"平均选择时间: {stats.AverageSelectionTimeMs:F2}ms");
```

### 获取策略特定统计

```csharp
if (loadBalancer is WeightedRoundRobinLoadBalancer weighted)
{
    var weightStats = weighted.GetWeightStatistics();
    Console.WriteLine($"总权重: {weightStats.TotalWeight}");
    Console.WriteLine($"平均权重: {weightStats.AverageWeight:F2}");
}

if (loadBalancer is ResponseTimeLoadBalancer responseTime)
{
    var responseStats = responseTime.GetResponseTimeStatistics();
    Console.WriteLine($"平均响应时间: {responseStats.AverageResponseTime:F2}ms");
    Console.WriteLine($"P95响应时间: {responseStats.P95ResponseTime:F2}ms");
}

if (loadBalancer is AdaptiveLoadBalancer adaptive)
{
    var adaptiveStats = adaptive.GetAdaptiveStatistics();
    Console.WriteLine($"最优策略: {adaptiveStats.BestStrategy}");
}

if (loadBalancer is GeoLocationLoadBalancer geo)
{
    var geoStats = geo.GetGeoLocationStatistics();
    Console.WriteLine($"平均路由距离: {geoStats.AverageRoutingDistance:F2}km");
    Console.WriteLine($"活跃客户端数: {geoStats.ActiveClientCount}");
}
```

## 高级配置

### 使用配置对象创建

```csharp
var config = new LoadBalancerConfiguration
{
    Strategy = LoadBalanceStrategy.WeightedRoundRobin,
    InstanceWeights = new Dictionary<long, int>
    {
        { 1, 3 },
        { 2, 2 },
        { 3, 1 }
    },
    EnableStatistics = true,
    HealthCheckIntervalSeconds = 30
};

var loadBalancer = LoadBalancerFactory.Create(config);
```

### 批量创建

```csharp
var loadBalancers = LoadBalancerFactory.CreateBatch(
    LoadBalanceStrategy.RoundRobin,
    LoadBalanceStrategy.LeastConnections,
    LoadBalanceStrategy.ResponseTime
);
```

### 缓存管理

```csharp
// 预热缓存
LoadBalancerFactory.WarmupCache();

// 获取工厂统计
var factoryStats = LoadBalancerFactory.GetStatistics();
Console.WriteLine($"缓存命中率: {factoryStats.CacheHitRatio:P2}");

// 清除缓存
LoadBalancerFactory.ClearCache();
```

## 最佳实践

### 1. 选择合适的策略

- **简单场景**：使用轮询或随机
- **性能差异大**：使用加权轮询或响应时间优化
- **需要会话保持**：使用一致性哈希
- **复杂环境**：使用自适应

### 2. 监控和调优

```csharp
// 定期收集统计信息
var timer = new System.Timers.Timer(60000); // 每分钟
timer.Elapsed += (s, e) =>
{
    var stats = loadBalancer.GetStatistics();
    if (stats.SuccessRate < 95)
    {
        LogHelper.Warning($"负载均衡成功率较低: {stats.SuccessRate:F2}%");
    }
};
timer.Start();
```

### 3. 故障处理

```csharp
try
{
    var selection = enhancedLoadBalancer.SelectInstanceEnhanced(context);
    if (selection.IsSuccess)
    {
        // 使用选中的实例
        await ProcessRequestAsync(selection.InstanceId, context);
    }
    else
    {
        // 处理选择失败
        LogHelper.Error($"实例选择失败: {selection.ErrorMessage}");
        await HandleSelectionFailureAsync(context);
    }
}
catch (Exception ex)
{
    LogHelper.Error($"负载均衡异常: {ex.Message}");
    await HandleLoadBalancerExceptionAsync(ex, context);
}
```

### 4. 性能优化

```csharp
// 预热缓存
LoadBalancerFactory.WarmupCache();

// 使用对象池
var contextPool = new ObjectPool<LoadBalancingContext>(
    () => new LoadBalancingContext(),
    context =>
    {
        context.AvailableInstances.Clear();
        context.Metadata.Clear();
        return context;
    });

// 异步更新指标
_ = Task.Run(async () =>
{
    while (true)
    {
        var metrics = await CollectMetricsAsync();
        enhancedLoadBalancer.UpdateInstanceMetricsEnhanced(metrics);
        await Task.Delay(TimeSpan.FromSeconds(30));
    }
});
```

## 迁移指南

### 从现有代码迁移

如果你正在使用旧的负载均衡系统，迁移非常简单：

```csharp
// 旧代码
var oldBalancer = new OldLoadBalancer();
var instance = oldBalancer.SelectInstance(serverType, instances);

// 新代码 - 方式1：使用工厂
var newBalancer = LoadBalancerFactory.Create(LoadBalanceStrategy.RoundRobin);
var instance = newBalancer.SelectInstance(serverType, instances);

// 新代码 - 方式2：使用泛型
var newBalancer = LoadBalancerFactory.Create<RoundRobinLoadBalancer>();
var instance = newBalancer.SelectInstance(serverType, instances);
```

### 渐进式升级

1. **第一阶段**：使用新的工厂替换旧代码中的实例创建
2. **第二阶段**：启用基础指标收集
3. **第三阶段**：使用增强功能和上下文感知选择
4. **第四阶段**：启用高级策略如自适应和地理位置

## 故障排查

### 常见问题

1. **选择结果为0**
   - 检查可用实例列表是否为空
   - 确认实例是否被标记为不可用
   - 验证健康检查状态

2. **性能问题**
   - 检查指标更新频率
   - 确认缓存是否预热
   - 分析选择时间统计

3. **分布不均**
   - 验证权重配置
   - 检查实例指标准确性
   - 考虑切换到自适应策略

### 调试工具

```csharp
// 启用详细日志
var stats = loadBalancer.GetStatistics();
foreach (var kvp in stats.SelectionsByInstance)
{
    Console.WriteLine($"实例 {kvp.Key}: {kvp.Value} 次选择");
}

// 检查实例状态
foreach (var instanceId in instances)
{
    var metrics = GetInstanceMetrics(instanceId);
    Console.WriteLine($"实例 {instanceId}: 连接数={metrics?.ActiveConnections ?? 0}, 响应时间={metrics?.AverageResponseTimeMs ?? 0}ms");
}
```

## 总结

GameFrameX 负载均衡系统提供了：

- ✅ **完整的向后兼容性**
- ✅ **丰富的策略选择**
- ✅ **强大的监控能力**
- ✅ **灵活的配置选项**
- ✅ **优秀的性能表现**

通过合理使用这些功能，你可以构建出高性能、高可用的分布式系统。