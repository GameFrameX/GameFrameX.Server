## [1.7.1] - 2026-04-16

* 移除KCP服务器启动代码并注释相关依赖## [1.7.0] - 2026-04-16

* 添加好友关系状态实体
* 实现好友增删查业务逻辑和消息处理器
* 添加好友场景测试流程
* 添加跨进程服务发现环境变量配置
* 机器人脚本支持 scenario 参数
* 跨进程好友操作传递源玩家ID## [1.6.1] - 2026-04-10

* 修正跨天逻辑中driver角色计数与跨天天数计算## [1.6.0] - 2026-04-09

* 升级到 .NET 10 并重构 docker-compose
* 重构启动选项类继承结构
* 支持单进程/多进程启动模式
* 合并 LauncherOptions 到 StartupOptions
* 为 Program.cs 添加双语文档注释
* 简化 TopologyProfile 配置，移除默认值和预设模板
* 单进程模式强制要求显式指定 ServerType
* 移除 TopologyProfile，ServerType 统一支持单值和逗号分隔列表
* 更新 ServerType 属性的注释和描述以更清晰
* 忽略StartupOptions.ServerType属性的空值检查
* 替换日志方法为控制台输出并调整执行顺序
* 移除 StopServerAsync 中的冗余日志刷新调用
* 为LogHelper方法调用添加泛型类型参数
* 移除冗余的缓存哈希相等警告日志
* 修复LogHelper.Error调用缺少泛型参数的问题
* 修复日志辅助方法调用时的泛型参数缺失问题
* 优化日志记录调用并调整应用退出流程
* 修正日志方法调用中的泛型参数
* 修正日志记录中泛型方法调用的类型参数
* 修复调试日志泛型调用错误
* 修复LogHelper.Error方法调用缺少泛型参数的问题
* 将同步发送方法替换为异步发送
* 将 KCP 网络模块项目设置为不可打包
* 替换 MongoDB.Entities 为 MongoDB.Driver
* 添加 MongoDB 服务查询接口的集成测试
* 添加 MongoDB 服务查询的并发与边界条件测试
* 修复MongoDB索引一致性检查并添加异步关闭方法
* 为异步方法添加取消令牌支持并扩展功能
* 为MongoDB服务添加取消令牌支持和新增操作方法
* 为MongoDB服务添加取消令牌及GameDb门面层测试
* 修复SendAsync方法未实现和参数校验问题
* 为MongoDB服务方法添加XML文档注释
* 添加GameDb的CRUD操作方法
* 更新多个项目中的 GameFrameX.Foundation 包版本至 2.2.8
* 修改minor_pattern以匹配feat前缀
* 统一使用 GetCurrentTimestamp 方法获取时间戳
* 将IsUseTimeZone默认值改为false
* 修复数据库连接失败时未释放服务实例的问题
* 添加数据库不可用异常类
* 增加连接重试机制和超时设置
* 更新多个项目中的 GameFrameX.Foundation 包版本至 2.2.10
* 增加数据库时区时间记录配置选项
* 在数据库初始化时添加时区设置选项
* 优化MongoDB连接超时设置与连接复用
* 添加MongoDB健康检查的连续失败阈值
* 添加 MongoDB 服务连接与健康检查的单元测试
* 添加带重试机制的读取和写入操作，优化MongoDB操作的稳定性
* 为MongoDbService添加异常消息的本地化支持
* 添加MongoDbService重试判定方法的单元测试
* 增强 MongoDB 连接与操作的监控和异常处理
* 移除 BaseCacheState 中的重复字段
* 添加数据库可用性状态枚举
* 添加数据库连接健康检查和自动恢复机制
* 将硬编码配置迁移至 DbOptions 并支持运行时调整
* 提取运行时配置到独立类以分离关注点
* 添加 MongoDB 数据库健康检查支持
* 添加获取端口占用进程信息的功能
* 增强HTTP端口占用错误日志，显示占用进程详情
* 在WebSocket服务器启动失败时添加端口占用详情日志
* 在TCP服务器启动失败时增加端口占用详情日志
* 在KCP服务器启动失败时记录端口占用详情
* 修正全局ID计算以包含最大Actor类型值
* 添加MongoDbService可用性状态机相关单元测试
* 为多个文件添加开源许可证头部注释
* 新增远程消息治理与调用编排统一客户端库
* 替换 Visual Studio 解决方案文件为 slnx 格式
* 将表达式体属性转换为完整属性语法
* 新增远程消息重试语义枚举类型
* 添加远程消息通信统一客户端框架
* 统一文件结尾格式并移除多余空行
* 为远程消息通信组件添加英文注释
* 修正泛型约束以使用 IResponseMessage 接口
* 移除接口实现方法上的多余inheritdoc注释
* 为接口实现方法添加 XML 文档注释
* 为远程消息组件添加中文XML文档注释
* 使用池化缓冲区优化消息编解码性能
* 为远程调用可观测性组件添加详细的中文注释
* 简化 PooledBuffer 的内存管理逻辑
* 添加消息压缩配置支持
* 添加元数据支持和创建上下文工厂方法
* 添加远程调用便捷方法和单向发送支持
* 添加可扩展的消息压缩算法支持
* 为远程消息组件添加XML注释的英文翻译
* 移除重复的版权声明并添加远程消息项目引用
* 移除重复的许可证和版权声明注释
* 将 AlgorithmId 属性从表达式体改为标准 getter
* 添加远程消息模块的单元测试
* 添加缺失的项目引用以支持热更新和网络功能测试
* 移除网络通道中的 RPC 会话相关代码
* 修复文件末尾缺少换行符的问题
* 添加 GameFrameX.NetWork.Abstractions 项目引用
* 引入网络发送器提供者模式以支持多种会话类型
* 移除未使用的WebSocketSession引用并简化DefaultNetWorkChannel构造
* 将Memory属性重构为完整属性语法
* 重新排序类成员并简化代码结构
* 移除接口文件中未使用的命名空间引用
* 重构服务常量到独立的类中
* 更新服务ID常量并新增多个服务定义
* 为服务常量类添加区域分组并重命名类
* 引入可插拔传输协议适配器以支持多协议扩展
* 引入统一消息发送器基础设施
* 新增跨服转发处理器与默认路由组件
* 迁移业务调用点到统一消息入口
* 统一使用 GameServerConst 替代 GlobalConst 服务常量
* 移除 RemoteMessageClientHolder 全局入口
* 新增统一消息系统单元测试
* 更新 Dockerfile 和 docker-compose 配置
* 移除未使用的 GameFrameX.NetWork.Message 全局引用
* 激活账号登录处理器并实现登录业务逻辑
* 重构 Bot 客户端支持参数化配置和断线重连
* 新增多服务部署配置和热更依赖同步
* 更新 README 添加多进程联调和机器人压测说明
* 修正目标框架属性名从复数到单数
* 移除硬编码环境变量并添加多个游戏服务实例## [1.5.1] - 2026-03-24

* 禁用非发布项目的 NuGet 打包
* 移除 DiscoveryCenterManager 模块
* 更新启动逻辑以使用 Aspire 服务发现
* 添加基于 Aspire 的服务拓扑监控
* 修复 HTTP 路由重复注册问题
* 添加 HTTP 路由注册单元测试## [1.5.0] - 2026-03-24

* 修复日志格式化字符串中的多余花括号
* 升级 Foundation 包至 2.2.5 并适配大端序 API
* 迁移至新的 TimerHelper API
* 使用 RandomElement 替代 Random
* 为 BaseCacheState 添加审计字段
* 为 GetAgent 泛型方法添加 class 约束
* 重命名发布工作流文件
* 统一代码格式
* 重构数据表 API 并优化性能
* 使用 TryGet 替代废弃的 Get 方法
* 为 SerializationException 添加 sealed 修饰符
* 添加 ThreadStatic 修复线程安全问题
* 规范静态字段命名
* 移除注释掉的 GetGenerateTime 方法
* 更新测试以适配字段重命名
* 将发布工作流触发分支改为 src
* 修复代码缺陷和拼写错误
* 使用 auto-property 替代 readonly 字段
* 修复消息管道过滤器对零长度数据包的处理逻辑
* 移除不必要的zipFlag负数检查
* 使用sizeof计算消息头长度以提高可维护性
* 将消息头长度注释从英文更新为中文并明确结构
* 重命名 HttpStatusCode 为 GameHttpStatusCode 避免与系统类型冲突
* 代码清理和资源管理优化
* 添加 SwaggerOperationFilter 反射缓存优化
* 避免重复 JSON 序列化/反序列化操作
* 添加 GET/PUT/DELETE 请求方法支持
* 添加HTTP请求日志开关配置
* 修复健康检查标签数组语法错误
* 简化本地化键引用路径以提升可维护性
* 简化本地化键路径的引用方式
* 简化本地化键的命名空间引用
* 标准化计时器处理器文件的行尾和空格格式
* 简化本地化键的命名空间引用
* 统一文件换行符格式并调整条件编译块缩进
* 统一变量声明方式并添加访问修饰符
* 统一文件换行符格式为 LF
* 简化本地化键的命名空间引用
* 修复 Program.cs 文件中的换行符和缩进格式问题
* 统一接口文件格式并移除BOM头
* 统一文件编码和格式化代码
* 统一文件中的换行符格式
* 将 switch 表达式重构为 switch 语句以提高可读性
* 统一监控帮助类文件的换行符和末尾逗号格式
* 添加 LangVersion 配置以支持 C# 12 功能
* 将 C# 语言版本从 12 降级到 10
* 支持 URL 路由同时兼容驼峰和下划线格式
* 添加发现中心功能开关配置
* 提取 StartUpTypes 为独立的注册表类
* 拆分 Entry 函数为多个小函数
* 添加 IsUseTimeZone 时区配置开关
* 添加本地化 key 检查和自动翻译脚本
* 补充缺失的本地化翻译
* 注释冗余的启动日志输出
* 翻译 Resources.zh-CN.resx 中的英文条目为中文
* 增强 MongoDB 服务的安全性和健壮性
* 优化 MongoDB 服务性能和异步安全
* 提取 ReplaceOptions 为静态字段
* 优化数据库模块代码质量
* 优化配置模块代码组织
* 更新本地开发配置
* 添加配置模块双语文档注释
* 添加工具模块双语文档注释
* 添加数学模块双语文档注释
* 更新双语文档注释
* 添加双语文档注释
* 添加双语文档注释
* 添加双语文档注释
* 添加双语文档注释
* 添加双语文档注释
* 添加双语文档注释
* 为HTTP服务器添加CORS支持
* 修复发布流水线分支推送和CHANGELOG配置
* 添加 KCP 网络模块
* 集成 KCP 服务器启动## [2.0.0.2-dev11] - 2026-02-24

* 将项目目标框架从 net10.0 调整为支持 net10.0 和 net8.0
* 将 .NET SDK 版本从 8.0 升级到 10.0.x
* 移除 .NET 8 目标框架，仅保留 .NET 10.0## [2.0.0.2-dev10] - 2026-02-24

* 更新README文档结构和内容
* 更新基础依赖包版本至2.0.0.2
* 更新多个项目的依赖包版本至2.0.0.2
* 移除各项目中的TargetFramework并统一配置多目标框架
* 更新 MongoDB.Entities 依赖至 25.0.0 版本
* 修改MongoDbContext继承并调整数据库初始化逻辑
* 将目标框架从多版本支持改为仅支持.NET 8.0
* 重构项目文件配置，统一管理公共属性
* 将目标框架从net8.0升级到net10.0
* 添加新的AppHost项目及相关配置
* 添加数据库密码配置选项
* 修复未初始化变量可能导致空引用异常的问题
* 修复GrafanaLoki标签已存在时的本地化键引用
* 添加MongoDB支持并配置持久化容器
* 添加MongoDB健康检查实现
* 添加服务默认配置模块和OpenTelemetry支持
* 移除自定义健康检查和OpenTelemetry扩展，使用ServiceDefaults替代
* 添加 GameFrameX.AppHost.ServiceDefaults 项目到解决方案
* Update README with detailed configuration guide and usage examples
* 更新多个项目的 GameFrameX.Foundation 包版本至 2.2.1
* 修复压缩辅助类中日志记录异常信息的格式
* 统一非法词检测的日志格式
* 修复LNumber溢出错误日志中的本地化键名错误
* 修复日志记录中缺少参数占位符的问题
* 为警告日志添加统一的上下文前缀
* 修复响应消息错误码处理和日志记录格式
* 优化FormatMessage错误日志记录以包含更多上下文信息
* 修复日志记录异常信息不完整的问题
* 优化日志记录错误信息格式
* 修复日志记录异常时缺少描述信息的问题
* 优化日志记录以使用结构化日志格式
* 简化本地化键引用并移除多余空行
* 将心跳消息列表从List改为HashSet以避免重复添加
* 将字符串拼接日志改为结构化日志记录
* 为事件分发器扩展添加结构化日志记录
* 修复缓存哈希相等时未提前返回的问题
* 优化 MongoDB 服务日志格式以增强可读性
* 修复日志记录缺少关键参数的问题
* 添加服务器时区配置支持
* 移除Entry方法中的多余空行
* 调整 TimeZone 属性在 LauncherOptions 中的声明位置
* 添加日志文件写入配置选项
* 重构日志记录以使用结构化消息格式
* 改进日志输出格式以包含上下文信息
* 改进日志记录以包含更多上下文信息
* 修复日志本地化调用并优化错误码判断
* 移除本地化依赖并改进日志记录
* 优化日志信息格式，移除本地化调用
* 改进日志记录，使用结构化参数并修复异常处理
* 标记过时的 Get 方法为 [Obsolete]
* 更新多个项目的 GameFrameX.Foundation 包版本至 2.2.2
* 更新过时警告信息为双语提示
* 增加时区时间记录配置选项
* 修复消息编码中长度字段类型转换问题
* 移除消息编码中OperationType的冗余类型转换
* 在消息编码处理中添加注释说明
* 重构 OpenTelemetry 配置以支持动态导出器## [2.0.0.2-dev09] - 2025-11-24

* 统一将CreateTime重命名为CreatedTime及相关属性
* 修复BaseCacheState全局过滤条件逻辑## [2.0.0.2-dev08] - 2025-11-24

* 修改默认查询表达式以包含null检查## [2.0.0.2-dev07] - 2025-11-20

* 添加GameFrameX.Localization模块并集成到各项目
* 为启动模块添加多语言支持
* 为网络消息模块添加本地化支持
* 为HTTP模块添加多语言支持
* 为网络抽象模块添加重复ID错误消息的本地化支持
* 添加网络模块本地化支持
* 移除MetricsHelper类及其相关监控功能
* 添加启动器模块的日志和错误消息本地化支持
* 添加DiscoveryCenterManager模块的本地化键和资源
* 更新本地化键的注释描述为更详细的信息
* 扩展数据表功能并添加类型约束
* 删除未使用的CustomAudioType枚举
* 添加核心模块日志和异常消息的本地化支持
* 添加会话管理和BSON类映射的本地化支持
* 添加代码生成器代理错误消息的本地化支持
* 统一中文资源文件格式并移除冗余英文翻译
* 添加 GameFrameX.Localization 项目到解决方案
* 为网络监控助手类添加摘要注释
* 移除BaseComponentAgent的冗余泛型参数注释
* 更新 GameFrameX.Foundation 相关包版本至 1.7.1.10
* 添加程序集资源提供者注册以支持本地化
* 改进异常消息和参数验证
* 添加实用工具模块的本地化支持
* 将硬编码日志消息替换为本地化字符串
* 优化常量命名和本地化引用路径
* 添加启动模块相关消息的本地化支持
* 移除GameFrameX.Serialize项目及其相关文件
* 实现HTTP模块的国际化支持
* 添加网络模块RPC超时和消息编码异常的本地化支持
* 添加玩家和服务上下线回调的本地化支持
* 为服务连接管理添加多语言支持
* 移除多余空行并格式化代码
* 清理无用引用并优化本地化字符串引用
* 更新多个项目的依赖包版本至1.7.1.11
* 重构缓存状态接口和删除标记
* 更新依赖版本并优化本地化引用路径## [2.0.0.2-dev06] - 2025-11-19

* 添加空检查防止空引用异常
* 添加获取域名IPv4和IPv6地址的方法
* 为GameAppServiceClient添加ID并更新回调参数
* 添加解析主机名或IP地址到EndPoint的方法
* 为回调方法添加实例ID参数以增强日志追踪
* 使用NetHelper验证并解析DiscoveryCenterHost配置
* 添加 LangVersion 10 到项目配置
* 修复_serverMap赋值语法错误
* 添加游戏应用服务客户端及配置管理
* 在RespConnectServer中添加ServerInstanceId字段
* 添加服务连接管理功能实现
* 使用对象池优化消息对象创建
* 修改GameAppClient方法以支持id参数
* 添加游戏启动配置项和标签名称
* 添加 MongoDB 数据库支持并移除内置 Proto 项目
* 添加ProtoBuf序列化测试及对象池测试
* 添加版权声明并重构消息ID生成方式
* 添加社交服务启动类及发现中心注册功能
* 添加好友组件及相关请求处理器
* 将日志控制台输出默认值改为true
* 移除未使用的RPC调用代码并清理命名空间
* 更新多个项目的依赖包版本至1.7.1.8## [2.0.0.2-dev05] - 2025-11-07

* 添加游戏客户端连接配置参数
* 修复消息发送时未设置MessageId的问题## [2.0.0.2-dev4] - 2025-11-05

* 修复配置文件加载失败时的日志显示顺序
* 调整日志初始化顺序以避免潜在的空引用异常
* 改进MongoDB服务初始化失败时的错误处理
* 添加启动任务为空的检查并输出警告信息
* 将Close方法改为异步返回Task## [2.0.0.2-dev3] - 2025-11-04

* 修复日志标签设置问题并添加日志类型配置
* 优化日志标签名称的构建逻辑
* 更新多个项目的依赖包版本至1.7.1.7## [2.0.0.2-dev2] - 2025-11-04

* 将定时任务相关代码拆分到单独文件
* 添加WriteStateAsync方法的高消耗操作警告注释
* 移除不必要的IComponentAgent接口继承
* 完善服务客户端类和配置的文档注释
* 完善启动类和属性的文档注释
* 完善代码注释和文档说明
* 更新接口文档注释为双语格式
* 添加路径空值检查并优化日志输出
* 移除PlatformRuntimeHelper及其测试文件
* 统一使用LogHelper替代LogHelper.Console方法
* 更新 GameFrameX.Foundation 相关包版本至 1.7.1.5
* 统一GrafanaLoki用户名属性命名
* 移除未使用的日志类型拼接逻辑
* 将LogOptions.Default.LogType改为LogTagName以更准确描述用途
* 优化服务器启动任务管理逻辑
* 修复启动逻辑错误并增强日志记录
* 将LogHelper.InfoConsole和LogHelper.DebugConsole替换为LogHelper.Info和LogHelper.Debug## [2.0.0.2-dev1] - 2025-10-31

* 添加HTTP服务启用开关配置
* 添加TCP服务开关配置
* 添加UDP支持配置及实现
* 扩展RPC会话功能并重构数据管理
* 添加游戏应用TCP服务客户端实现
* 重构服务客户端配置为独立类并整合功能
* 重构游戏应用客户端为服务客户端并添加RPC支持
* 统一将_gameAppClient重命名为_gameAppServiceClient以保持命名一致性
* 添加发现中心组件调用方法
* 添加MessageId有效性检查防止无效消息发送
* 为Send和Call方法添加异常注释说明## [2.0.0.1-dev9] - 2025-10-25

* 更新多个项目的 GameFrameX.Foundation 包版本至 1.7.0.1
* 更新 GameFrameX.Foundation 相关包版本至 1.7.0.7
* 将EnvironmentHelper迁移至Foundation.Utility命名空间
* 删除过时的TimeHelper测试文件
* 将ConsoleHelper移动到Foundation.Utility命名空间
* 替换Yitter.IdGenerator为自定义雪花ID实现
* 将TimeHelper重命名为TimerHelper并统一引用
* 统一使用TimerHelper替换TimeHelper的时间方法
* 添加 GameFrameX.Apps 项目引用到测试项目
* 添加MongoDB数据库服务实现
* 修正项目描述中的错误库名引用
* 移除未使用的引用并添加MongoDB数据库项目引用
* 更新RPC处理器返回类型注释为IMessageHandler
* 移除BaseComponentAgent中未使用的using和接口依赖
* 替换MongoIndexAttribute为EntityIndexAttribute
* 添加 GameFrameX ORM 相关依赖包
* 添加 GameFrameX.DataBase.Mongo 项目到解决方案
* 将CacheComponent的set访问器改为private
* 移除未使用的扩展方法及相关测试
* 更新多个项目的依赖包版本至1.7.1
* 优化MongoDB查询服务的命名空间引用
* 移除LoadFromDbPostHandler的冗余参数
* 移除 MongoDB.Entities 和 GameFrameX.Foundation.Hash 的包引用
* 完善基础组件代理类的日志记录器文档
* 添加ActorIdGenerator工具类用于生成Actor唯一ID## [2.0.0.1-dev8] - 2025-10-20

* 优化心跳消息处理并使用对象池
* 修复玩家登录查询时未禁用跟踪的问题
* 在登录流程中添加玩家ID绑定
* 调整控制台logo显示格式和内容
* 修复消息对象池管理问题
* 为BaseComponentAgent添加日志功能支持## [2.0.0.1-dev7] - 2025-10-14

* 添加网络操作错误码枚举类
* 添加执行超时错误码
* 更新错误码的英文描述和注释
* 修复RPC消息处理超时未返回错误响应的问题
* 捕获异常时设置默认错误码## [2.0.0.1-dev6] - 2025-10-13

* 移除sealed修饰符并修复代码格式## [2.0.0.1-dev5] - 2025-10-13

* 添加 RPC 异常基类用于远程过程调用错误处理
* 添加基础消息处理器接口
* 添加IMessageHandler接口定义网络消息处理能力
* 添加获取RPC处理器的方法
* 更新消息操作类型命名并添加服务注销功能
* 添加玩家上下线通知功能
* 添加服务上下线日志输出
* 使用全局常量替换硬编码的ServerId值
* 修正日志中消息类型显示错误的问题
* 移除未使用的MessageCodeDescriptionAttribute类
* 重构登录处理逻辑和组件结构
* 重构玩家登录逻辑，移除冗余代码
* 更新 Lib.Harmony 依赖版本至 2.4.1
* 删除未使用的PetComponentAgent类
* 添加基础消息处理器和组件处理器基类
* 添加基础RPC消息处理器和组件处理器实现
* 为基类处理器添加泛型参数文档注释
* 为基类处理器添加泛型参数和继承说明文档
* 重构全局组件处理器为泛型版本
* 重构玩家组件处理器为泛型版本
* 添加全局RPC组件处理器基类
* 添加玩家RPC组件处理器基类
* 将消息处理器返回类型改为IMessageHandler接口
* 重构背包相关消息处理器为RPC风格
* 重构玩家登录处理逻辑使用RPC模式
* 为IMessage接口添加Clear方法
* 添加抽象方法Clear用于清除消息内容
* 重构消息接口继承关系
* 使INetworkMessageHeader继承IMessageObject并添加Clear方法
* 添加Clear方法用于清除消息内容
* 删除内部消息对象头类 InnerMessageObjectHeader
* 移除未使用的InnerMessageObjectHeader并优化调试日志
* 添加支持合并模式的反序列化方法
* 添加消息对象池帮助类用于高性能对象重用
* 为消息类添加Clear方法实现
* 在消息编码器中添加finally块释放消息对象
* 使用对象池优化RPC消息处理性能
* 移除未使用的Register方法
* 提取对象池创建逻辑到独立方法
* 使用对象池优化心跳消息对象创建
* 更新 GameFrameX.Foundation 相关依赖包至 1.6.0 版本
* 合并属性注解中的DefaultValue到Option特性中## [2.0.0.1-dev4] - 2025-10-10

* 移除废弃的外部网络消息接口和实现类
* 移除内部网络消息接口及实现类
* 添加网络消息包接口及实现类
* 修正内部消息处理逻辑，仅处理消息ID小于0的内部消息
* 统一使用NetworkMessagePackage创建消息对象
* 将IInnerNetworkMessage重命名为INetworkMessagePackage
* 将OuterNetworkMessage替换为NetworkMessagePackage类型
* 修复服务器类型日志显示问题## [2.0.0.1-dev3] - 2025-10-10

* 统一服务名称格式并添加服务器ID常量
* 修正服务器类型检查条件中的常量名称错误
* 将GameName替换为GameServiceName以保持命名一致性
* 使用常量替换硬编码的服务名称和ID
* 移除ServerId的默认值以避免合服问题
* 移除已废弃的UnSchedule方法
* 移除已过时的UTC时间转换方法
* 移除重复的时间转换测试用例
* 移除可空类型标记以简化代码## [2.0.0.1-dev2] - 2025-10-10

* 修正ReqServiceUnRegister消息类型处理器的消息ID
* 将日志消息从中文翻译为英文
* 优化错误提示和参数验证逻辑
* 移除未使用的命名空间引用
* 将HTTP服务器相关日志和错误信息翻译为英文
* 将健康检查消息从中文改为英文
* 将服务器异常日志从Info级别改为Warning级别
* 将日志和异常消息从中文改为英文
* 将错误信息和日志从中文改为英文
* 完善类和方法注释的英文翻译
* 更新压缩解压缩辅助类的文档注释
* 统一客户端连接和断开日志的英文格式
* 完善环境帮助类的文档注释## [2.0.0.1-dev1] - 2025-09-29

* 修复WebSocket服务器启动逻辑并改进错误处理
* 添加服务器ID和内部端口配置
* 移除ServerType枚举定义
* 将ServerType从枚举类型改为字符串类型
* 将ServerType从枚举类型改为字符串类型
* 使用全局常量替换硬编码的服务类型检查
* 将ServerType从枚举类型改为字符串类型
* 将ServerType改为string类型以增加灵活性
* 将ServerType从枚举类型改为字符串类型
* 将ServerType从枚举类型改为字符串类型
* 移除已废弃的ClientMessage编解码处理器
* 添加心跳接口IActorHeartBeat用于标识具备心跳时间戳的Actor
* 添加INotifyActorHeartBeat接口用于处理心跳结果
* 新增服务下线通知接口IServiceNotifyOffline
* 添加服务上线通知接口定义
* 添加服务注册接口定义
* 重构消息操作类型枚举并添加明确值
* 将MessageOperationType从枚举类型改为byte类型
* 添加内置服务注册相关消息类型
* 移除未使用的服务器消息类
* 重命名连接服务器消息类型并添加相关协议
* 修复消息操作类型比较和枚举值转换问题
* 重命名玩家上下线消息类以统一命名规范
* 修复心跳消息处理器的类型转换问题
* 统一服务注册与通知的消息类型命名
* 使用全局常量替换硬编码的服务类型
* 添加游戏服务相关的常量名称
* 将ServerType.Game替换为GlobalConst.GameName## [2.0.0-dev4] - 2025-09-29

* 修改HttpPort默认值为8080
* 为InnerPort属性添加默认值8888
* 为ServerId属性添加默认值2000
* 调整HTTP服务器端口范围下限至5000
* 更新启动配置环境变量
* 添加 WebSocket 启用开关配置## [2.0.0-dev3] - 2025-09-29

* 重构服务信息接口和实现类
* 统一服务器信息字段命名
* 重构服务节点管理方法并改进注释
* 修复移除节点时使用的方法调用错误
* 修复服务发现中服务器实例ID的过滤条件
* 修复断开连接时会话移除方法调用错误
* 重命名TrySessionRemove为TryRemoveBySessionId以提高可读性
* 修复获取节点信息时使用错误的方法名
* 统一服务器地址字段命名从Ip改为Host
* 添加游戏客户端心跳处理逻辑
* 添加消息处理器异常捕获并记录致命错误
* 添加空会话检查防止空引用异常
* 重命名API相关属性名以更准确表达用途
* 添加客户端API和HubAPI地址配置项
* 将IP地址属性重命名为主机地址属性## [2.0.0-dev2] - 2025-09-29

* 提取终结点变量提升代码可读性
* 移除未使用的命名空间引用以简化代码
* 重构心跳发送逻辑以支持自定义心跳消息
* 将私有字段_mRetryCount改为公共属性RetryCount
* 将_maxRetryCount改为公共属性并移除_reqActorHeartBeat
* 将发现中心服务器连接日志从Info级别调整为Debug级别
* 移除未使用的GameFrameX.Proto.BuiltIn命名空间引用
* 将DiscoveryCenterIp重命名为DiscoveryCenterHost
* 重构 GameAppClient 配置参数为选项类
* 添加发现中心主机和端口的配置检查
* 将返回null改为返回default以符合代码规范
* 移除GameFrameX.Proto.BuiltIn项目并迁移内置协议到GameFrameX.Proto
* 重构游戏应用客户端配置使用方式
* 简化GameAppClient初始化逻辑并移除多余空行
* 移除未使用的程序集引用并简化消息协议初始化## [2.0.0-dev1] - 2025-09-28

* 修复当actorId为默认值时日志格式错误的问题
* 移除未使用的WebSocket和SuperSocket配置代码
* 添加发现中心客户端功能实现
* 移除未使用的网关连接相关代码
* 将NotifyHeartBeat更改为NotifyActorHeartBeat以匹配协议
* 修复发送消息时服务器类型显示错误
* 修复服务器启动时传入空HTTP处理器列表的问题
* 优化网络消息日志输出逻辑
* 添加通过发现中心注册热更新游戏服务器的功能
* 移除未使用的命名空间引用
* 将LogHelper.Warn改为LogHelper.Warning以保持命名一致性
* 将消息头长度属性类型从int改为ushort和uint
* 标记客户端消息编解码处理器为过时
* 重构消息头解析逻辑以提高可读性
* 修改消息头长度属性类型为ushort
* 优化消息编码处理，使用ArrayPool替代对象池
* 添加消息头长度属性
* 移除未使用的GameFrameX.ProtoBuf.Net引用
* 移除IMessageEncoderHandler中的IInnerNetworkMessage接口方法
* 移除未使用的内部消息处理逻辑
* 在反序列化消息时设置消息ID和操作类型
* 修复会话为空时未处理的异常
* 修复消息编码时使用错误参数的问题
* 使用默认消息编解码器替换客户端特定实现
* 修复服务器启动时注册发现中心的逻辑错误
* 添加支持指定版本的热更新模块加载方法
* 重构NamingServiceManager以支持多服务实例
* 优化服务管理器的初始化和节点获取逻辑
* 添加热更新支持并初始化组件注册
* 添加服务器列表HTTP处理接口
* 实现获取外部服务器列表的HTTP接口
* 添加获取所有节点列表的HTTP接口
* 添加根据服务器ID获取节点信息的HTTP处理器
* 添加获取节点总数的HTTP处理器
* 添加根据SessionId获取节点信息的HTTP处理器
* 添加获取自身服务信息的HTTP处理器
* 添加移除节点的HTTP处理器实现
* 添加根据服务器类型获取节点列表的HTTP处理器
* 为测试文件添加版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 统一添加项目版权声明和许可证信息
* 更新HTTP处理器并统一添加版权声明
* 启用DiscoveryCenter的StartUpTag标记
* 更新选项描述以提供更清晰的运维信息
* 移除未使用的数据库和HTTP相关检查方法
* 添加Hotfix模块支持并优化容器配置
* 移除LauncherOptionsValidate方法
* 移除内部参数检查方法
* 移除注释掉的性能监控代码以保持代码整洁
* 重构健康检查响应模型并使用强类型
* 重命名APMPort为MetricsPort并移除无用配置
* 使用Mapster映射健康检查设置
* 添加Json序列化选项以使用驼峰命名
* 更新AppSettingTests测试文件中的格式和空白行
* 添加内置协议消息类实现
* 重构服务发现中心模块并重命名
* 添加玩家信息接口、实体类及管理器实现
* 修复日志中服务器类型显示错误的问题
* 添加玩家注册在线和离线消息类型
* 添加 DiscoveryCenterManager 项目到构建流程
* 重构服务发现中心模块的命名空间和玩家管理
* 将网络消息日志级别从Info调整为Debug
* 重命名服务器变更回调变量名以简化
* 修改节点添加时的空值检查逻辑
* 使用JsonHelper序列化节点信息并重命名事件
* 移除已注释的节点状态设置方法
* 添加发现中心组件及相关状态和代理类
* 在热更新启动流程中添加发现中心组件注册
* 添加玩家上下线时向发现中心发送通知
* 添加服务器类型和ID到日志选项
* 添加根据启动选项设置日志标签名的功能
* 在发送心跳时更新唯一标识符
* 清理未使用的消息操作类型并修复代码格式## [1.5.1-dev37] - 2025-06-17

* 增加雪花ID的配置参数
* 增加雪花ID的参数的配置化## [1.5.1-dev36] - 2025-06-17

* 增加数据存档的参数
* 增加存档参数的配置化
* 增加Actor的配置参数
* 增加Actor的超时参数的配置化
* 增加环境变量的参数打印## [release-v1.0.0.0] - 2024-05-15
<!-- generated by git-cliff -->
