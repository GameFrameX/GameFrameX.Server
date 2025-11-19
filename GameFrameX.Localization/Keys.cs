namespace GameFrameX.Localization
{
    /// <summary>
    /// 本地化字符串键常量类
    /// Localization string keys constants class
    /// </summary>
    public static class Keys
    {
        /// <summary>
        /// 日志消息资源键
        /// </summary>
        public static class Logs
        {
            /// <summary>
            /// 服务器启动时找到配置文件的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Server.Start.FindConfig
            /// 用途: 服务器启动过程中成功找到配置文件时记录
            /// 参数: {0} - 配置文件路径
            /// </remarks>
            public const string Server_Start_Find_Config = "Logs.Server.Start.FindConfig";

            /// <summary>
            /// 服务器启动使用默认配置的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Server.Start.DefaultConfig
            /// 用途: 服务器启动时未找到配置文件，使用默认配置时记录
            /// </remarks>
            public const string Server_Start_Default_Config = "Logs.Server.Start.DefaultConfig";

            /// <summary>
            /// 服务器正在启动的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Server.Starting
            /// 用途: 服务器启动开始时记录
            /// 参数: {0} - 服务器名称, {1} - 版本号
            /// </remarks>
            public const string Server_Starting = "Logs.Server.Starting";

            /// <summary>
            /// 服务器停止时无正在运行任务的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Server.Stopped.NoTasks
            /// 用途: 服务器停止时确认没有正在运行的任务时记录
            /// </remarks>
            public const string Server_Stopped_No_Tasks = "Logs.Server.Stopped.NoTasks";

            /// <summary>
            /// 游戏服务器启动的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Game.Server.Start
            /// 用途: 游戏服务器启动开始时记录
            /// </remarks>
            public const string Game_Server_Start = "Logs.Game.Server.Start";

            /// <summary>
            /// 游戏服务器配置Actor完成的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Game.Server.ConfigActor.End
            /// 用途: 游戏服务器Actor系统配置完成时记录
            /// </remarks>
            public const string Game_Server_Config_Actor_End = "Logs.Game.Server.ConfigActor.End";

            /// <summary>
            /// 游戏服务器数据库配置完成的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Game.Server.Database.End
            /// 用途: 游戏服务器数据库配置完成时记录
            /// </remarks>
            public const string Game_Server_Database_End = "Logs.Game.Server.Database.End";

            /// <summary>
            /// 游戏服务器注册组件开始的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Game.Server.RegisterComponents.Start
            /// 用途: 游戏服务器开始注册组件时记录
            /// </remarks>
            public const string Game_Server_Register_Components_Start = "Logs.Game.Server.RegisterComponents.Start";

            /// <summary>
            /// 游戏服务器热更新完成的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Game.Server.Hotfix.End
            /// 用途: 游戏服务器热更新加载完成时记录
            /// </remarks>
            public const string Game_Server_Hotfix_End = "Logs.Game.Server.Hotfix.End";

            /// <summary>
            /// 游戏服务器主循环启动的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Game.Server.MainLoop
            /// 用途: 游戏服务器主循环开始运行时记录
            /// </remarks>
            public const string Game_Server_Main_Loop = "Logs.Game.Server.MainLoop";

            /// <summary>
            /// 游戏服务器启动完成的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Game.Server.Start.Complete
            /// 用途: 游戏服务器完全启动并准备好接收连接时记录
            /// </remarks>
            public const string Game_Server_Start_Complete = "Logs.Game.Server.Start.Complete";

            /// <summary>
            /// 游戏服务器开始退出的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Game.Server.Exit.Start
            /// 用途: 游戏服务器开始关闭流程时记录
            /// </remarks>
            public const string Game_Server_Exit_Start = "Logs.Game.Server.Exit.Start";

            /// <summary>
            /// 游戏服务器成功退出的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Game.Server.Exit.Success
            /// 用途: 游戏服务器完全关闭时记录
            /// </remarks>
            public const string Game_Server_Exit_Success = "Logs.Game.Server.Exit.Success";

            /// <summary>
            /// 数据库初始化成功的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Database.Init.Success
            /// 用途: 数据库连接和初始化成功时记录
            /// 参数: {0} - 数据库连接字符串, {1} - 数据库名称
            /// </remarks>
            public const string Database_Init_Success = "Logs.Database.Init.Success";

            /// <summary>
            /// 客户端正在连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Client.Connecting
            /// 用途: 客户端开始连接到服务器时记录
            /// 参数: {0} - 客户端ID, {1} - 服务器地址
            /// </remarks>
            public const string Client_Connecting = "Logs.Client.Connecting";

            /// <summary>
            /// 客户端重试连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Client.RetryConnect
            /// 用途: 客户端连接失败后准备重试时记录
            /// 参数: {0} - 客户端ID, {1} - 重试次数, {2} - 最大重试次数
            /// </remarks>
            public const string Client_Retry_Connect = "Logs.Client.RetryConnect";

            /// <summary>
            /// 客户端达到最大重试次数的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Client.MaxRetryReached
            /// 用途: 客户端连接重试次数达到上限时记录
            /// 参数: {0} - 客户端ID, {1} - 最大重试次数
            /// </remarks>
            public const string Client_Max_Retry_Reached = "Logs.Client.MaxRetryReached";

            /// <summary>
            /// 客户端发生错误的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Client.ErrorOccurred
            /// 用途: 客户端操作过程中发生错误时记录
            /// 参数: {0} - 客户端ID, {1} - 错误信息
            /// </remarks>
            public const string Client_Error_Occurred = "Logs.Client.ErrorOccurred";

            /// <summary>
            /// 客户端断开连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Client.Disconnected
            /// 用途: 客户端与服务器断开连接时记录
            /// 参数: {0} - 客户端ID, {1} - 断开原因
            /// </remarks>
            public const string Client_Disconnected = "Logs.Client.Disconnected";

            /// <summary>
            /// 客户端连接成功的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Client.Connected.Success
            /// 用途: 客户端成功连接到服务器时记录
            /// 参数: {0} - 客户端ID, {1} - 服务器地址
            /// </remarks>
            public const string Client_Connected_Success = "Logs.Client.Connected.Success";

            /// <summary>
            /// 网络客户端断开连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Network.Client.Disconnect
            /// 用途: 网络层客户端断开连接时记录
            /// 参数: {0} - 连接ID
            /// </remarks>
            public const string Network_Client_Disconnect = "Logs.Network.Client.Disconnect";

            /// <summary>
            /// 网络客户端连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Network.Client.Connect
            /// 用途: 网络层客户端建立连接时记录
            /// 参数: {0} - 连接ID, {1} - 远程地址
            /// </remarks>
            public const string Network_Client_Connect = "Logs.Network.Client.Connect";

            /// <summary>
            /// 发现中心服务器异常的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Discovery.Server.Exception
            /// 用途: 发现中心服务器发生异常时记录
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string Discovery_Server_Exception = "Logs.Discovery.Server.Exception";

            /// <summary>
            /// 玩家在发现中心注册成功的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Discovery.Player.Register.Success
            /// 用途: 玩家成功注册到发现中心时记录
            /// 参数: {0} - 玩家ID, {1} - 服务类型
            /// </remarks>
            public const string Discovery_Player_Register_Success = "Logs.Discovery.Player.Register.Success";

            /// <summary>
            /// 玩家在发现中心注销成功的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Discovery.Player.Unregister.Success
            /// 用途: 玩家成功从发现中心注销时记录
            /// 参数: {0} - 玩家ID
            /// </remarks>
            public const string Discovery_Player_Unregister_Success = "Logs.Discovery.Player.Unregister.Success";

            /// <summary>
            /// 服务在发现中心注册成功的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Discovery.Service.Register.Success
            /// 用途: 服务成功注册到发现中心时记录
            /// 参数: {0} - 服务ID, {1} - 服务类型
            /// </remarks>
            public const string Discovery_Service_Register_Success = "Logs.Discovery.Service.Register.Success";

            /// <summary>
            /// 服务在发现中心注销成功的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Discovery.Service.Unregister.Success
            /// 用途: 服务成功从发现中心注销时记录
            /// 参数: {0} - 服务ID
            /// </remarks>
            public const string Discovery_Service_Unregister_Success = "Logs.Discovery.Service.Unregister.Success";

            /// <summary>
            /// 连接外部服务的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Discovery.ExternalService.Connect
            /// 用途: 成功连接到外部服务时记录
            /// 参数: {0} - 服务地址, {1} - 服务类型
            /// </remarks>
            public const string Discovery_External_Service_Connect = "Logs.Discovery.ExternalService.Connect";

            /// <summary>
            /// 断开外部服务连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Discovery.ExternalService.Disconnect
            /// 用途: 与外部服务断开连接时记录
            /// 参数: {0} - 服务地址, {1} - 断开原因
            /// </remarks>
            public const string Discovery_External_Service_Disconnect = "Logs.Discovery.ExternalService.Disconnect";

            /// <summary>
            /// 控制台警告消息
            /// </summary>
            /// <remarks>
            /// 键名: Logs.Warning.ConsoleMessage
            /// 用途: 向控制台输出警告信息时使用
            /// 参数: {0} - 警告消息内容
            /// </remarks>
            public const string Warning_Console_Message = "Logs.Warning.ConsoleMessage";
        }

        /// <summary>
        /// 客户端相关日志消息资源键
        /// </summary>
        public static class Client
        {
            /// <summary>
            /// 客户端尝试连接到服务器的消息
            /// </summary>
            /// <remarks>
            /// 键名: Client.AttemptingToConnect
            /// 用途: 客户端开始连接到服务器时记录
            /// </remarks>
            public const string AttemptingToConnect = "Client.AttemptingToConnect";

            /// <summary>
            /// 客户端重试连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: Client.RetryConnect
            /// 用途: 客户端未连接到服务器，准备重试时记录
            /// 参数: {0} - 当前尝试次数, {1} - 最大重试次数
            /// </remarks>
            public const string RetryConnect = "Client.RetryConnect";

            /// <summary>
            /// 客户端达到最大重试次数的消息
            /// </summary>
            /// <remarks>
            /// 键名: Client.MaxRetryReached
            /// 用途: 客户端重连次数达到上限时记录
            /// </remarks>
            public const string MaxRetryReached = "Client.MaxRetryReached";

            /// <summary>
            /// 客户端发生错误的消息
            /// </summary>
            /// <remarks>
            /// 键名: Client.ErrorOccurred
            /// 用途: 客户端操作过程中发生错误时记录
            /// 参数: {0} - 错误信息
            /// </remarks>
            public const string ErrorOccurred = "Client.ErrorOccurred";

            /// <summary>
            /// 客户端断开连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: Client.Disconnected
            /// 用途: 客户端与服务器断开连接时记录
            /// </remarks>
            public const string Disconnected = "Client.Disconnected";

            /// <summary>
            /// 客户端成功连接到服务器的消息
            /// </summary>
            /// <remarks>
            /// 键名: Client.ConnectedSuccessfully
            /// 用途: 客户端成功连接到服务器时记录
            /// </remarks>
            public const string ConnectedSuccessfully = "Client.ConnectedSuccessfully";
        }

        /// <summary>
        /// 事件相关日志消息资源键
        /// </summary>
        public static class Events
        {
            /// <summary>
            /// 事件没有监听者的警告消息
            /// </summary>
            /// <remarks>
            /// 键名: Events.NoListenersFound
            /// 用途: 当事件分发时没有找到任何监听者时记录
            /// 参数: {0} - 事件ID
            /// </remarks>
            public const string NoListenersFound = "Events.NoListenersFound";
        }

        /// <summary>
        /// 数据存储相关日志消息资源键
        /// </summary>
        public static class Storage
        {
            /// <summary>
            /// 缓存哈希相等的警告消息
            /// </summary>
            /// <remarks>
            /// 键名: Storage.CacheHashEquals
            /// 用途: 当在保存到数据库前发现缓存哈希已经等于目标哈希时记录
            /// 参数: {0} - 状态类型名称
            /// </remarks>
            public const string CacheHashEquals = "Storage.CacheHashEquals";
        }

        /// <summary>
        /// 数据库模块日志键名
        /// </summary>
        public static class Database
        {
            /// <summary>
            /// MongoDB服务初始化成功
            /// </summary>
            /// <remarks>
            /// 键名: Database.MongoDb.InitializedSuccessfully
            /// 用途: MongoDB服务成功初始化时记录
            /// 参数: {0} - 连接字符串, {1} - 数据库名称
            /// </remarks>
            public const string MongoDbInitializedSuccessfully = "Database.MongoDb.InitializedSuccessfully";

            /// <summary>
            /// MongoDB服务初始化失败
            /// </summary>
            /// <remarks>
            /// 键名: Database.MongoDb.InitializationFailed
            /// 用途: MongoDB服务初始化失败时记录
            /// 参数: {0} - 连接字符串, {1} - 数据库名称
            /// </remarks>
            public const string MongoDbInitializationFailed = "Database.MongoDb.InitializationFailed";
        }

        /// <summary>
        /// 异常消息资源键
        /// </summary>
        public static class Exceptions
        {
            /// <summary>
            /// 数据库启动失败的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.Database.StartFailed
            /// 用途: 数据库服务启动失败时抛出此异常
            /// 参数: {0} - 数据库类型, {1} - 详细错误信息
            /// </remarks>
            public const string Database_Start_Failed = "Exceptions.Database.StartFailed";

            /// <summary>
            /// 处理器未初始化的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.Handler.NotInitialized
            /// 用途: 当尝试使用未初始化的处理器时抛出此异常
            /// 参数: {0} - 处理器类型名称
            /// </remarks>
            public const string Handler_Not_Initialized = "Exceptions.Handler.NotInitialized";

            /// <summary>
            /// 计时器参数无效的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.Timer.InvalidParameters
            /// 用途: 当创建计时器时传入无效参数时抛出此异常
            /// 参数: {0} - 参数名称, {1} - 参数值
            /// </remarks>
            public const string Timer_Invalid_Parameters = "Exceptions.Timer.InvalidParameters";

            /// <summary>
            /// 每周计时器日期为空的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.Timer.Weekly.EmptyDays
            /// 用途: 当创建每周计时器时未指定任何执行日期时抛出此异常
            /// </remarks>
            public const string Timer_Weekly_Empty_Days = "Exceptions.Timer.Weekly.EmptyDays";

            /// <summary>
            /// 计时器代码在热更新中的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.Timer.CodeInHotfix
            /// 用途: 当尝试在热更新中使用不支持的计时器功能时抛出此异常
            /// </remarks>
            public const string Timer_Code_In_Hotfix = "Exceptions.Timer.CodeInHotfix";

            /// <summary>
            /// IP地址格式无效的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.Network.InvalidIpFormat
            /// 用途: 当传入的IP地址格式不正确时抛出此异常
            /// 参数: {0} - 无效的IP地址字符串
            /// </remarks>
            public const string Invalid_Ip_Format = "Exceptions.Network.InvalidIpFormat";

            /// <summary>
            /// BigInteger构造函数溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.BigInteger.ConstructorOverflow
            /// 用途: 当创建BigInteger时数值溢出时抛出此异常
            /// 参数: {0} - 溢出的数值
            /// </remarks>
            public const string Biginteger_Constructor_Overflow = "Exceptions.BigInteger.ConstructorOverflow";

            /// <summary>
            /// BigInteger构造函数下溢的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.BigInteger.ConstructorUnderflow
            /// 用途: 当创建BigInteger时数值下溢时抛出此异常
            /// 参数: {0} - 下溢的数值
            /// </remarks>
            public const string Biginteger_Constructor_Underflow = "Exceptions.BigInteger.ConstructorUnderflow";

            /// <summary>
            /// BigInteger无效字符串的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.BigInteger.InvalidString
            /// 用途: 当使用无效字符串创建BigInteger时抛出此异常
            /// 参数: {0} - 无效的字符串
            /// </remarks>
            public const string Biginteger_Invalid_String = "Exceptions.BigInteger.InvalidString";

            /// <summary>
            /// BigInteger构造函数字节溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Exceptions.BigInteger.ConstructorByteOverflow
            /// 用途: 当使用字节数组构造BigInteger时发生溢出时使用
            /// </remarks>
            public const string Biginteger_Constructor_Byte_Overflow = "Exceptions.BigInteger.ConstructorByteOverflow";

            /// <summary>
            /// BigInteger异常消息资源键
            /// </summary>
            public static class BigInteger
            {
                /// <summary>
                /// BigInteger构造函数字节溢出的错误消息
                /// </summary>
                /// <remarks>
                /// 键名: Exceptions.BigInteger.ConstructorByteOverflow
                /// 用途: 当使用字节数组构造BigInteger时发生溢出时使用
                /// </remarks>
                public const string ConstructorByteOverflow = "Exceptions.BigInteger.ConstructorByteOverflow";
            }
        }

        /// <summary>
        /// Utility模块异常消息资源键
        /// </summary>
        public static class Utility
        {
            /// <summary>
            /// ActorId小于最小服务器ID的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorIdLessThanMinServerId
            /// 用途: 当传入的ActorId小于最小服务器ID时使用
            /// 参数: {0} - 最小服务器ID值
            /// </remarks>
            public const string ActorIdLessThanMinServerId = "Utility.Exceptions.ActorIdLessThanMinServerId";

            /// <summary>
            /// ActorId小于最小服务器ID的详细错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorIdLessThanMinServerIdDetail
            /// 用途: 当传入的ActorId小于最小服务器ID时使用，包含具体的ActorId值
            /// 参数: {0} - ActorId值, {1} - 最小服务器ID值
            /// </remarks>
            public const string ActorIdLessThanMinServerIdDetail = "Utility.Exceptions.ActorIdLessThanMinServerIdDetail";

            /// <summary>
            /// Actor类型错误的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorTypeError
            /// 用途: 当传入的Actor类型无效时使用
            /// 参数: {0} - 错误的Actor类型值
            /// </remarks>
            public const string ActorTypeError = "Utility.Exceptions.ActorTypeError";

            /// <summary>
            /// 服务器ID为负数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ServerIdNegative
            /// 用途: 当服务器ID为负数时使用
            /// 参数: {0} - 负数的服务器ID值
            /// </remarks>
            public const string ServerIdNegative = "Utility.Exceptions.ServerIdNegative";

            /// <summary>
            /// 服务器ID小于等于0的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ServerIdLessThanOrEqualZero
            /// 用途: 当服务器ID小于等于0时使用
            /// </remarks>
            public const string ServerIdLessThanOrEqualZero = "Utility.Exceptions.ServerIdLessThanOrEqualZero";

            /// <summary>
            /// Actor类型无效的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorTypeInvalid
            /// 用途: 当Actor类型超出有效范围时使用
            /// </remarks>
            public const string ActorTypeInvalid = "Utility.Exceptions.ActorTypeInvalid";

            /// <summary>
            /// 模块ID无效的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ModuleInvalid
            /// 用途: 当模块ID超出有效范围时使用
            /// </remarks>
            public const string ModuleInvalid = "Utility.Exceptions.ModuleInvalid";

            /// <summary>
            /// 构造函数溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ConstructorOverflow
            /// 用途: 当构造函数发生正溢出时使用
            /// </remarks>
            public const string ConstructorOverflow = "Utility.Exceptions.ConstructorOverflow";

            /// <summary>
            /// 异常消息资源键
            /// </summary>
            public static class Exceptions
            {
            /// <summary>
            /// ActorId小于最小服务器ID的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorIdLessThanMinServerId
            /// 用途: 当传入的ActorId小于最小服务器ID时使用
            /// 参数: {0} - 最小服务器ID值
            /// </remarks>
            public const string ActorIdLessThanMinServerId = "Utility.Exceptions.ActorIdLessThanMinServerId";

            /// <summary>
            /// ActorId小于最小服务器ID的详细错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorIdLessThanMinServerIdDetail
            /// 用途: 当传入的ActorId小于最小服务器ID时使用，包含具体的ActorId值
            /// 参数: {0} - ActorId值, {1} - 最小服务器ID值
            /// </remarks>
            public const string ActorIdLessThanMinServerIdDetail = "Utility.Exceptions.ActorIdLessThanMinServerIdDetail";

            /// <summary>
            /// 输入为全局ID的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.InputIsGlobalId
            /// 用途: 当尝试对全局ID执行不允许的操作时使用
            /// 参数: {0} - 全局ID值
            /// </remarks>
            public const string InputIsGlobalId = "Utility.Exceptions.InputIsGlobalId";

            /// <summary>
            /// Actor类型错误的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorTypeError
            /// 用途: 当传入的Actor类型无效时使用
            /// 参数: {0} - 错误的Actor类型值
            /// </remarks>
            public const string ActorTypeError = "Utility.Exceptions.ActorTypeError";

            /// <summary>
            /// 服务器ID为负数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ServerIdNegative
            /// 用途: 当服务器ID为负数时使用
            /// 参数: {0} - 负数的服务器ID值
            /// </remarks>
            public const string ServerIdNegative = "Utility.Exceptions.ServerIdNegative";

            /// <summary>
            /// 服务器ID小于等于0的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ServerIdLessThanOrEqualZero
            /// 用途: 当服务器ID小于等于0时使用
            /// </remarks>
            public const string ServerIdLessThanOrEqualZero = "Utility.Exceptions.ServerIdLessThanOrEqualZero";

            /// <summary>
            /// Actor类型无效的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ActorTypeInvalid
            /// 用途: 当Actor类型超出有效范围时使用
            /// </remarks>
            public const string ActorTypeInvalid = "Utility.Exceptions.ActorTypeInvalid";

            /// <summary>
            /// 模块ID无效的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ModuleInvalid
            /// 用途: 当模块ID超出有效范围时使用
            /// </remarks>
            public const string ModuleInvalid = "Utility.Exceptions.ModuleInvalid";

            /// <summary>
            /// 基数超出范围的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.RadixOutOfRange
            /// 用途: 当ToString方法的基数参数不在2-36范围内时使用
            /// </remarks>
            public const string RadixOutOfRange = "Utility.Exceptions.RadixOutOfRange";

            /// <summary>
            /// 仅支持正指数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.PositiveExponentsOnly
            /// 用途: 当模幂运算的指数为负数时使用
            /// </remarks>
            public const string PositiveExponentsOnly = "Utility.Exceptions.PositiveExponentsOnly";

            /// <summary>
            /// 参数k必须为奇数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ParameterKMustBeOdd
            /// 用途: 当Lucas序列参数k不为奇数时使用
            /// </remarks>
            public const string ParameterKMustBeOdd = "Utility.Exceptions.ParameterKMustBeOdd";

            /// <summary>
            /// 雅可比符号仅定义于奇数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.JacobiSymbolDefinedForOddOnly
            /// 用途: 当雅可比符号计算的参数不为奇数时使用
            /// </remarks>
            public const string JacobiSymbolDefinedForOddOnly = "Utility.Exceptions.JacobiSymbolDefinedForOddOnly";

            /// <summary>
            /// 没有逆的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.NoInverse
            /// 用途: 当模逆不存在时使用
            /// </remarks>
            public const string NoInverse = "Utility.Exceptions.NoInverse";

            /// <summary>
            /// 所需位数超过最大长度的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.RequiredBitsExceedMaxLength
            /// 用途: 当生成随机位数超过BigInteger最大长度时使用
            /// </remarks>
            public const string RequiredBitsExceedMaxLength = "Utility.Exceptions.RequiredBitsExceedMaxLength";

            /// <summary>
            /// 乘法溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.MultiplicationOverflow
            /// 用途: 当BigInteger乘法运算溢出时使用
            /// </remarks>
            public const string MultiplicationOverflow = "Utility.Exceptions.MultiplicationOverflow";

            /// <summary>
            /// 自增溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.IncrementOverflow
            /// 用途: 当BigInteger自增运算溢出时使用
            /// </remarks>
            public const string IncrementOverflow = "Utility.Exceptions.IncrementOverflow";

            /// <summary>
            /// 自减下溢的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.DecrementUnderflow
            /// 用途: 当BigInteger自减运算下溢时使用
            /// </remarks>
            public const string DecrementUnderflow = "Utility.Exceptions.DecrementUnderflow";

            /// <summary>
            /// 取负溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.NegationOverflow
            /// 用途: 当BigInteger取负运算溢出时使用
            /// </remarks>
            public const string NegationOverflow = "Utility.Exceptions.NegationOverflow";

            /// <summary>
            /// 构造函数字节溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ConstructorByteOverflow
            /// 用途: 当使用字节数组构造BigInteger时发生溢出时使用
            /// </remarks>
            public const string ConstructorByteOverflow = "Utility.Exceptions.ConstructorByteOverflow";

            /// <summary>
            /// 构造函数正溢出的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ConstructorOverflow
            /// 用途: 当构造函数发生正溢出时使用
            /// </remarks>
            public const string ConstructorOverflow = "Utility.Exceptions.ConstructorOverflow";

            /// <summary>
            /// 构造函数负下溢的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ConstructorUnderflow
            /// 用途: 当构造函数发生负下溢时使用
            /// </remarks>
            public const string ConstructorUnderflow = "Utility.Exceptions.ConstructorUnderflow";

            /// <summary>
            /// 构造函数无效字符串的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ConstructorInvalidString
            /// 用途: 当构造函数接收到无效字符串时使用
            /// </remarks>
            public const string ConstructorInvalidString = "Utility.Exceptions.ConstructorInvalidString";

            /// <summary>
            /// 时间戳超出范围的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.TimestampOutOfRange
            /// 用途: 当传入的时间戳无法转换为有效的DateTime时使用
            /// </remarks>
            public const string TimestampOutOfRange = "Utility.Exceptions.TimestampOutOfRange";

            /// <summary>
            /// 系统时钟回退的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.ClockMovedBackwards
            /// 用途: 当检测到系统时钟回退时，雪花算法ID生成器抛出此异常
            /// 参数: {0} - 回退的毫秒数
            /// </remarks>
            public const string ClockMovedBackwards = "Utility.Exceptions.ClockMovedBackwards";

            /// <summary>
            /// Worker ID 超出范围的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.WorkerIdOutOfRange
            /// 用途: 当创建IdWorker时传入的WorkerId超出有效范围时使用
            /// 参数: {0} - 最大允许的WorkerId值
            /// </remarks>
            public const string WorkerIdOutOfRange = "Utility.Exceptions.WorkerIdOutOfRange";

            /// <summary>
            /// Datacenter ID 超出范围的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: Utility.Exceptions.DatacenterIdOutOfRange
            /// 用途: 当创建IdWorker时传入的DatacenterId超出有效范围时使用
            /// 参数: {0} - 最大允许的DatacenterId值
            /// </remarks>
            public const string DatacenterIdOutOfRange = "Utility.Exceptions.DatacenterIdOutOfRange";
            }

            /// <summary>
            /// LNumber模块相关日志和错误消息资源键
            /// </summary>
            public static class LNumber
            {
                /// <summary>
                /// Xnumber创建失败的错误消息
                /// </summary>
                /// <remarks>
                /// 键名: Utility.LNumber.CreateFailed
                /// 用途: 当创建Xnumber时传入无效参数时记录
                /// 参数: {0} - 整数部分, {1} - 小数部分
                /// </remarks>
                public const string CreateFailed = "Utility.LNumber.CreateFailed";

                /// <summary>
                /// Number数据超上限的错误消息
                /// </summary>
                /// <remarks>
                /// 键名: Utility.LNumber.DataExceedsLimit
                /// 用途: 当Number数据超过上限时记录
                /// 参数: {0} - 操作描述, {1} - 数值
                /// </remarks>
                public const string DataExceedsLimit = "Utility.LNumber.DataExceedsLimit";

                /// <summary>
                /// LNumber乘法越界的错误消息
                /// </summary>
                /// <remarks>
                /// 键名: Utility.LNumber.MultiplicationOverflow
                /// 用途: 当LNumber乘法运算越界时记录
                /// 参数: {0} - 计算结果
                /// </remarks>
                public const string MultiplicationOverflow = "Utility.LNumber.MultiplicationOverflow";

                /// <summary>
                /// LNumber除法越界的错误消息
                /// </summary>
                /// <remarks>
                /// 键名: Utility.LNumber.DivisionOverflow
                /// 用途: 当LNumber除法运算越界时记录
                /// 参数: {0} - 计算结果
                /// </remarks>
                public const string DivisionOverflow = "Utility.LNumber.DivisionOverflow";
            }

            /// <summary>
            /// 网络模块相关错误消息资源键
            /// </summary>
            public static class Network
            {
                /// <summary>
                /// 无效IP地址格式的错误消息
                /// </summary>
                /// <remarks>
                /// 键名: Utility.Network.InvalidIpAddressFormat
                /// 用途: 当传入的IP地址格式无效时使用
                /// </remarks>
                public const string InvalidIpAddressFormat = "Utility.Network.InvalidIpAddressFormat";
            }

            /// <summary>
            /// 敏感词检测模块相关日志消息资源键
            /// </summary>
            public static class IllegalWordDetection
            {
                /// <summary>
                /// 敏感词初始化完成的消息
                /// </summary>
                /// <remarks>
                /// 键名: Utility.IllegalWordDetection.InitializationComplete
                /// 用途: 敏感词系统初始化完成时记录
                /// 参数: {0} - 耗时(毫秒), {1} - 有效数量
                /// </remarks>
                public const string InitializationComplete = "Utility.IllegalWordDetection.InitializationComplete";
            }

            /// <summary>
            /// 设置模块相关日志和错误消息资源键
            /// </summary>
            public static class Settings
            {
                /// <summary>
                /// 应用程序已退出，不能再次开启的错误消息
                /// </summary>
                /// <remarks>
                /// 键名: Utility.Settings.AppAlreadyExited
                /// 用途: 当尝试开启已经退出的应用程序时记录
                /// </remarks>
                public const string AppAlreadyExited = "Utility.Settings.AppAlreadyExited";

                /// <summary>
                /// 保存数据间隔过小的警告消息
                /// </summary>
                /// <remarks>
                /// 键名: Utility.Settings.SaveDataIntervalTooSmall
                /// 用途: 当SaveDataInterval小于5000毫秒时记录警告
                /// 参数: {0} - 使用的默认值(毫秒)
                /// </remarks>
                public const string SaveDataIntervalTooSmall = "Utility.Settings.SaveDataIntervalTooSmall";
            }
        }

        /// <summary>
        /// NetWork.Message模块相关日志和错误消息资源键
        /// </summary>
        public static class NetWorkMessage
        {
            /// <summary>
            /// 消息解码异常的致命错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Message.MessageDecodeFatalError
            /// 用途: 当消息解码过程中发生致命异常时记录
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string MessageDecodeFatalError = "NetWork.Message.MessageDecodeFatalError";

            /// <summary>
            /// 消息编码异常的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Message.MessageEncodeError
            /// 用途: 当消息对象编码异常时记录错误信息
            /// </remarks>
            public const string MessageEncodeError = "NetWork.Message.MessageEncodeError";

            /// <summary>
            /// 消息编码异常详情的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Message.MessageEncodeException
            /// 用途: 当消息编码过程中发生异常时记录详细信息
            /// 参数: {0} - 异常信息
            /// </remarks>
            public const string MessageEncodeException = "NetWork.Message.MessageEncodeException";

            /// <summary>
            /// 消息对象为空的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Message.MessageObjectNull
            /// 用途: 当消息对象为空导致编码异常时记录错误
            /// </remarks>
            public const string MessageObjectNull = "NetWork.Message.MessageObjectNull";

            /// <summary>
            /// 消息对象编码异常的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Message.MessageObjectEncodeException
            /// 用途: 当消息对象编码过程中发生异常时记录错误，提示检查错误日志
            /// </remarks>
            public const string MessageObjectEncodeException = "NetWork.Message.MessageObjectEncodeException";

            /// <summary>
            /// 消息对象为空导致编码异常的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Message.MessageObjectNullEncodeException
            /// 用途: 当消息对象为空导致编码异常时记录错误
            /// </remarks>
            public const string MessageObjectNullEncodeException = "NetWork.Message.MessageObjectNullEncodeException";
        }

        /// <summary>
        /// NetWork.HTTP模块相关日志和错误消息资源键
        /// </summary>
        public static class NetWorkHttp
        {
            /// <summary>
            /// 参数重复的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.ParameterDuplicate
            /// 用途: 当HTTP请求中参数重复时记录错误
            /// 参数: {0} - 重复的参数名
            /// </remarks>
            public const string ParameterDuplicate = "NetWork.Http.ParameterDuplicate";

            /// <summary>
            /// 不支持的内容类型的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.UnsupportedContentType
            /// 用途: 当HTTP请求的Content-Type不支持时记录错误
            /// 参数: {0} - 不支持的Content-Type值
            /// </remarks>
            public const string UnsupportedContentType = "NetWork.Http.UnsupportedContentType";

            /// <summary>
            /// 请求参数日志消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.RequestParameters
            /// 用途: 记录HTTP请求的参数信息
            /// 参数: {0} - 序列化后的参数
            /// </remarks>
            public const string RequestParameters = "NetWork.Http.RequestParameters";

            /// <summary>
            /// 服务器状态错误的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.ServerStatusError
            /// 用途: 当服务器状态不正常时返回错误
            /// </remarks>
            public const string ServerStatusError = "NetWork.Http.ServerStatusError";

            /// <summary>
            /// HTTP命令处理器不存在的警告消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.CommandHandlerNotFound
            /// 用途: 当找不到对应的HTTP命令处理器时记录警告
            /// 参数: {0} - 命令名称
            /// </remarks>
            public const string CommandHandlerNotFound = "NetWork.Http.CommandHandlerNotFound";

            /// <summary>
            /// 执行时间日志消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.ExecutionTime
            /// 用途: 记录HTTP请求的执行时间
            /// 参数: {0} - 日志头, {1} - 执行时间(毫秒), {2} - 结果
            /// </remarks>
            public const string ExecutionTime = "NetWork.Http.ExecutionTime";

            /// <summary>
            /// 发生异常的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.ExceptionOccurred
            /// 用途: 当HTTP处理过程中发生异常时记录错误
            /// 参数: {0} - 日志头, {1} - 异常消息, {2} - 堆栈跟踪
            /// </remarks>
            public const string ExceptionOccurred = "NetWork.Http.ExceptionOccurred";

            /// <summary>
            /// 消息类型继承错误
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.MessageTypeInheritanceError
            /// 用途: 当HTTP消息请求类型未继承自HttpMessageRequestBase时抛出异常
            /// 参数: {0} - 消息类型名称
            /// </remarks>
            public const string MessageTypeInheritanceError = "NetWork.Http.MessageTypeInheritanceError";

            /// <summary>
            /// 响应消息类型继承错误
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.ResponseMessageTypeInheritanceError
            /// 用途: 当HTTP消息响应类型未继承自HttpMessageResponseBase时抛出异常
            /// 参数: {0} - 消息类型名称
            /// </remarks>
            public const string ResponseMessageTypeInheritanceError = "NetWork.Http.ResponseMessageTypeInheritanceError";

            /// <summary>
            /// 类必须是密封类的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.ClassMustBeSealed
            /// 用途: 当类不是sealed时抛出异常
            /// 参数: {0} - 类名
            /// </remarks>
            public const string ClassMustBeSealed = "NetWork.Http.ClassMustBeSealed";

            /// <summary>
            /// 类必须以特定后缀结尾的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Http.ClassMustEndWithSuffix
            /// 用途: 当类名不以指定后缀结尾时抛出异常
            /// 参数: {0} - 类名, {1} - 要求的后缀
            /// </remarks>
            public const string ClassMustEndWithSuffix = "NetWork.Http.ClassMustEndWithSuffix";
        }

        /// <summary>
        /// NetWork.Abstractions模块相关日志和错误消息资源键
        /// </summary>
        public static class NetWorkAbstractions
        {
            /// <summary>
            /// 消息ID重复的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Abstractions.MessageIdDuplicate
            /// 用途: 当消息ID重复时抛出异常
            /// 参数: {0} - 重复的消息ID, {1} - 已存在的类型全名
            /// </remarks>
            public const string MessageIdDuplicate = "NetWork.Abstractions.MessageIdDuplicate";

            /// <summary>
            /// 心跳消息重复的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Abstractions.HeartbeatMessageDuplicate
            /// 用途: 当心跳消息类型重复时记录错误日志
            /// 参数: {0} - 重复的消息类型全名
            /// </remarks>
            public const string HeartbeatMessageDuplicate = "NetWork.Abstractions.HeartbeatMessageDuplicate";

            /// <summary>
            /// 请求ID重复的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Abstractions.RequestIdDuplicate
            /// 用途: 当请求ID重复时抛出异常
            /// 参数: {0} - 重复的请求ID, {1} - 已存在的类型全名
            /// </remarks>
            public const string RequestIdDuplicate = "NetWork.Abstractions.RequestIdDuplicate";

            /// <summary>
            /// 返回ID重复的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.Abstractions.ResponseIdDuplicate
            /// 用途: 当返回ID重复时抛出异常
            /// 参数: {0} - 重复的返回ID, {1} - 已存在的类型全名
            /// </remarks>
            public const string ResponseIdDuplicate = "NetWork.Abstractions.ResponseIdDuplicate";
        }

        /// <summary>
        /// NetWork模块相关日志和错误消息资源键
        /// </summary>
        public static class NetWork
        {
            /// <summary>
            /// 发送消息的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.MessageSent
            /// 用途: 记录发送消息的日志信息
            /// 参数: {0} - 格式化的消息内容
            /// </remarks>
            public const string MessageSent = "NetWork.MessageSent";

            /// <summary>
            /// 消息发送超时被取消的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.MessageSendTimeout
            /// 用途: 当消息发送超时被取消时记录错误
            /// 参数: {0} - 异常消息
            /// </remarks>
            public const string MessageSendTimeout = "NetWork.MessageSendTimeout";

            /// <summary>
            /// 类型必须实现接口的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.TypeMustImplementInterface
            /// 用途: 当类型未实现指定接口时抛出异常
            /// 参数: {0} - 类型名称, {1} - 接口名称
            /// </remarks>
            public const string TypeMustImplementInterface = "NetWork.TypeMustImplementInterface";

            /// <summary>
            /// 类型必须是类的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.TypeMustBeClass
            /// 用途: 当类型不是类时抛出异常
            /// 参数: {0} - 类型名称
            /// </remarks>
            public const string TypeMustBeClass = "NetWork.TypeMustBeClass";

            /// <summary>
            /// 类型必须有无参构造函数的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.TypeMustHaveParameterlessConstructor
            /// 用途: 当类型没有无参构造函数时抛出异常
            /// 参数: {0} - 类型名称
            /// </remarks>
            public const string TypeMustHaveParameterlessConstructor = "NetWork.TypeMustHaveParameterlessConstructor";

            /// <summary>
            /// 无法找到Create方法的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.CannotFindCreateMethod
            /// 用途: 当无法在ObjectPoolProvider上找到Create方法时抛出异常
            /// </remarks>
            public const string CannotFindCreateMethod = "NetWork.CannotFindCreateMethod";

            /// <summary>
            /// 创建对象池失败的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: NetWork.FailedToCreateObjectPool
            /// 用途: 当无法为类型创建对象池时抛出异常
            /// 参数: {0} - 类型名称
            /// </remarks>
            public const string FailedToCreateObjectPool = "NetWork.FailedToCreateObjectPool";
        }

        /// <summary>
        /// Launcher模块相关日志和错误消息资源键
        /// </summary>
        public static class Launcher
        {
            /// <summary>
            /// 开始启动服务器的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ServerStartBegin
            /// 用途: 记录服务器启动开始的信息
            /// 参数: {0} - 服务器类型
            /// </remarks>
            public const string ServerStartBegin = "Launcher.ServerStartBegin";

            /// <summary>
            /// 开始配置Actor限制逻辑的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ActorLimitConfigBegin
            /// 用途: 记录开始配置Actor限制逻辑的日志
            /// </remarks>
            public const string ActorLimitConfigBegin = "Launcher.ActorLimitConfigBegin";

            /// <summary>
            /// 配置Actor限制逻辑结束的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ActorLimitConfigEnd
            /// 用途: 记录配置Actor限制逻辑结束的日志
            /// </remarks>
            public const string ActorLimitConfigEnd = "Launcher.ActorLimitConfigEnd";

            /// <summary>
            /// 开始启动数据库服务的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.DatabaseServiceStartBegin
            /// 用途: 记录开始启动数据库服务的日志
            /// </remarks>
            public const string DatabaseServiceStartBegin = "Launcher.DatabaseServiceStartBegin";

            /// <summary>
            /// 数据库服务启动失败的异常消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.DatabaseServiceStartFailed
            /// 用途: 当数据库服务启动失败时抛出异常
            /// </remarks>
            public const string DatabaseServiceStartFailed = "Launcher.DatabaseServiceStartFailed";

            /// <summary>
            /// 启动数据库服务结束的控制台日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.DatabaseServiceStartEnd
            /// 用途: 记录数据库服务启动结束的控制台日志
            /// </remarks>
            public const string DatabaseServiceStartEnd = "Launcher.DatabaseServiceStartEnd";

            /// <summary>
            /// 注册组件开始的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ComponentRegisterBegin
            /// 用途: 记录组件注册开始的日志
            /// </remarks>
            public const string ComponentRegisterBegin = "Launcher.ComponentRegisterBegin";

            /// <summary>
            /// 注册组件结束的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ComponentRegisterEnd
            /// 用途: 记录组件注册结束的日志
            /// </remarks>
            public const string ComponentRegisterEnd = "Launcher.ComponentRegisterEnd";

            /// <summary>
            /// 开始加载热更新模块的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.HotfixModuleLoadBegin
            /// 用途: 记录开始加载热更新模块的日志
            /// </remarks>
            public const string HotfixModuleLoadBegin = "Launcher.HotfixModuleLoadBegin";

            /// <summary>
            /// 加载热更新模块结束的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.HotfixModuleLoadEnd
            /// 用途: 记录热更新模块加载结束的日志
            /// </remarks>
            public const string HotfixModuleLoadEnd = "Launcher.HotfixModuleLoadEnd";

            /// <summary>
            /// 进入游戏主循环的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.EnterMainLoop
            /// 用途: 记录进入游戏主循环的日志
            /// </remarks>
            public const string EnterMainLoop = "Launcher.EnterMainLoop";

            /// <summary>
            /// 服务器启动结束的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ServerStartEnd
            /// 用途: 记录服务器启动结束的信息
            /// 参数: {0} - 服务器类型
            /// </remarks>
            public const string ServerStartEnd = "Launcher.ServerStartEnd";

            /// <summary>
            /// 服务器执行异常的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ServerExecutionException
            /// 用途: 记录服务器执行异常的信息
            /// 参数: {0} - 异常详情
            /// </remarks>
            public const string ServerExecutionException = "Launcher.ServerExecutionException";

            /// <summary>
            /// 退出服务器开始的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ServerExitBegin
            /// 用途: 记录服务器开始退出的日志
            /// </remarks>
            public const string ServerExitBegin = "Launcher.ServerExitBegin";

            /// <summary>
            /// 退出服务器成功的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ServerExitSuccess
            /// 用途: 记录服务器退出成功的日志
            /// </remarks>
            public const string ServerExitSuccess = "Launcher.ServerExitSuccess";

            /// <summary>
            /// 发送消息的调试日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.SendMessage
            /// 用途: 记录发送消息的调试信息
            /// 参数: {0} - 发送方服务器类型, {1} - 接收方服务器类型, {2} - 消息内容
            /// </remarks>
            public const string SendMessage = "Launcher.SendMessage";

            /// <summary>
            /// 接收消息的调试日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ReceiveMessage
            /// 用途: 记录接收消息的调试信息
            /// 参数: {0} - 发送方服务器类型, {1} - 接收方服务器类型, {2} - 消息内容
            /// </remarks>
            public const string ReceiveMessage = "Launcher.ReceiveMessage";

            /// <summary>
            /// 注册玩家成功的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.PlayerRegisterSuccess
            /// 用途: 记录玩家注册成功的日志
            /// 参数: {0} - 玩家ID, {1} - 玩家信息
            /// </remarks>
            public const string PlayerRegisterSuccess = "Launcher.PlayerRegisterSuccess";

            /// <summary>
            /// 注销玩家成功的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.PlayerUnregisterSuccess
            /// 用途: 记录玩家注销成功的日志
            /// 参数: {0} - 玩家ID, {1} - 玩家信息
            /// </remarks>
            public const string PlayerUnregisterSuccess = "Launcher.PlayerUnregisterSuccess";

            /// <summary>
            /// 注册服务成功的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ServiceRegisterSuccess
            /// 用途: 记录服务注册成功的日志
            /// 参数: {0} - 服务器类型, {1} - 服务器名称, {2} - 注册信息
            /// </remarks>
            public const string ServiceRegisterSuccess = "Launcher.ServiceRegisterSuccess";

            /// <summary>
            /// 注销服务成功的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ServiceUnregisterSuccess
            /// 用途: 记录服务注销成功的日志
            /// 参数: {0} - 服务器ID, {1} - 服务器实例ID
            /// </remarks>
            public const string ServiceUnregisterSuccess = "Launcher.ServiceUnregisterSuccess";

            /// <summary>
            /// 外部服务连接成功的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ExternalServiceConnected
            /// 用途: 记录外部服务连接到中心服务器成功的日志
            /// 参数: {0} - SessionID, {1} - RemoteEndPoint
            /// </remarks>
            public const string ExternalServiceConnected = "Launcher.ExternalServiceConnected";

            /// <summary>
            /// 外部服务断开的日志消息
            /// </summary>
            /// <remarks>
            /// 键名: Launcher.ExternalServiceDisconnected
            /// 用途: 记录外部服务从中心服务器断开的日志
            /// 参数: {0} - 断开原因
            /// </remarks>
            public const string ExternalServiceDisconnected = "Launcher.ExternalServiceDisconnected";
        }

        /// <summary>
        /// StartUp模块相关日志和错误消息资源键
        /// </summary>
        public static class StartUp
        {
            /// <summary>
            /// 服务器停止的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.ServerStopped
            /// 用途: 当服务器停止时记录日志信息
            /// 参数: {0} - 服务器类型, {1} - 终止原因, {2} - 配置信息
            /// </remarks>
            public const string ServerStopped = "StartUp.ServerStopped";

            /// <summary>
            /// 与发现中心通信错误的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.DiscoveryCenterCommunicationError
            /// 用途: 当与发现中心通信发生错误时记录
            /// 参数: {0} - 服务器类型, {1} - 连接ID, {2} - 异常信息
            /// </remarks>
            public const string DiscoveryCenterCommunicationError = "StartUp.DiscoveryCenterCommunicationError";

            /// <summary>
            /// 接收到发现中心消息的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.DiscoveryCenterMessageReceived
            /// 用途: 当接收到发现中心消息时记录
            /// 参数: {0} - 服务器类型, {1} - 连接ID, {2} - 消息内容
            /// </remarks>
            public const string DiscoveryCenterMessageReceived = "StartUp.DiscoveryCenterMessageReceived";

            /// <summary>
            /// 与发现中心断开连接的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.DiscoveryCenterDisconnected
            /// 用途: 当与发现中心断开连接时记录
            /// 参数: {0} - 服务器类型, {1} - 连接ID
            /// </remarks>
            public const string DiscoveryCenterDisconnected = "StartUp.DiscoveryCenterDisconnected";

            /// <summary>
            /// 连接到发现中心成功的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.DiscoveryCenterConnected
            /// 用途: 当成功连接到发现中心时记录
            /// 参数: {0} - 服务器类型, {1} - 连接ID
            /// </remarks>
            public const string DiscoveryCenterConnected = "StartUp.DiscoveryCenterConnected";

            /// <summary>
            /// 服务器启动任务为空的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.NoStartupTaskFound
            /// 用途: 当没有找到启动任务时记录
            /// </remarks>
            public const string NoStartupTaskFound = "StartUp.NoStartupTaskFound";

            /// <summary>
            /// 指标端口被占用的错误消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.MetricsPortInUse
            /// 用途: 当指标端口被占用时记录错误
            /// 参数: {0} - 端口号
            /// </remarks>
            public const string MetricsPortInUse = "StartUp.MetricsPortInUse";

            /// <summary>
            /// Prometheus指标端点已启用的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.PrometheusMetricsEndpointEnabled
            /// 用途: 当Prometheus指标端点启用时记录
            /// 参数: {0} - IP地址, {1} - 端口号
            /// </remarks>
            public const string PrometheusMetricsEndpointEnabled = "StartUp.PrometheusMetricsEndpointEnabled";

            /// <summary>
            /// Metrics健康检查端点已启用的消息
            /// </summary>
            /// <remarks>
            /// 键名: StartUp.MetricsHealthCheckEndpointEnabled
            /// 用途: 当Metrics健康检查端点启用时记录
            /// 参数: {0} - IP地址, {1} - 端口号
            /// </remarks>
            public const string MetricsHealthCheckEndpointEnabled = "StartUp.MetricsHealthCheckEndpointEnabled";

            /// <summary>
            /// HTTP服务器相关消息
            /// </summary>
            public static class HttpServer
            {
                /// <summary>
                /// 处理POST请求的描述
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.HttpServer.HandlePostRequest
                /// 用途: HTTP API文档中使用
                /// </remarks>
                public const string HandlePostRequest = "StartUp.HttpServer.HandlePostRequest";

                /// <summary>
                /// 处理游戏客户端POST请求的描述
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.HttpServer.HandleGameClientPostRequest
                /// 用途: HTTP API文档中使用
                /// </remarks>
                public const string HandleGameClientPostRequest = "StartUp.HttpServer.HandleGameClientPostRequest";

                /// <summary>
                /// HTTP服务被禁用的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.HttpServer.ServiceDisabled
                /// 用途: 当HTTP服务被禁用时记录
                /// </remarks>
                public const string ServiceDisabled = "StartUp.HttpServer.ServiceDisabled";

                /// <summary>
                /// 启动HTTP服务器的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.HttpServer.StartingServer
                /// 用途: 当开始启动HTTP服务器时记录
                /// </remarks>
                public const string StartingServer = "StartUp.HttpServer.StartingServer";

                /// <summary>
                /// HTTP端口超出范围的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.HttpServer.PortOutOfRange
                /// 用途: 当HTTP端口超出允许范围时记录警告
                /// 参数: {0} - 端口号, {1} - 最小端口, {2} - 最大端口
                /// </remarks>
                public const string PortOutOfRange = "StartUp.HttpServer.PortOutOfRange";

                /// <summary>
                /// Swagger UI访问地址的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.HttpServer.SwaggerUiAccess
                /// 用途: 显示Swagger UI访问地址时记录
                /// 参数: {0} - IP地址, {1} - 端口号
                /// </remarks>
                public const string SwaggerUiAccess = "StartUp.HttpServer.SwaggerUiAccess";
            }

            /// <summary>
            /// TCP服务器相关消息
            /// </summary>
            public static class TcpServer
            {
                /// <summary>
                /// 客户端断开连接的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.TcpServer.ClientDisconnected
                /// 用途: 当客户端断开连接时记录
                /// 参数: {0} - 会话ID, {1} - 远程端点, {2} - 断开原因
                /// </remarks>
                public const string ClientDisconnected = "StartUp.TcpServer.ClientDisconnected";

                /// <summary>
                /// 新客户端连接的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.TcpServer.NewClientConnection
                /// 用途: 当新客户端连接时记录
                /// 参数: {0} - 会话ID, {1} - 远程端点
                /// </remarks>
                public const string NewClientConnection = "StartUp.TcpServer.NewClientConnection";

                /// <summary>
                /// 接收到消息的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.TcpServer.MessageReceived
                /// 用途: 当接收到消息时记录调试信息
                /// 参数: {0} - 服务器类型, {1} - 消息内容
                /// </remarks>
                public const string MessageReceived = "StartUp.TcpServer.MessageReceived";

                /// <summary>
                /// 启动TCP服务器的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.TcpServer.StartingServer
                /// 用途: 当开始启动TCP服务器时记录
                /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
                /// </remarks>
                public const string StartingServer = "StartUp.TcpServer.StartingServer";

                /// <summary>
                /// TCP服务器启动完成的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.TcpServer.StartupComplete
                /// 用途: 当TCP服务器启动完成时记录
                /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
                /// </remarks>
                public const string StartupComplete = "StartUp.TcpServer.StartupComplete";

                /// <summary>
                /// TCP服务器启动失败的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.TcpServer.StartupFailed
                /// 用途: 当TCP服务器启动失败时记录警告
                /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
                /// </remarks>
                public const string StartupFailed = "StartUp.TcpServer.StartupFailed";

                /// <summary>
                /// TCP服务器被禁用的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.TcpServer.ServerDisabled
                /// 用途: 当TCP服务器被禁用时记录
                /// 参数: {0} - 服务器类型, {1} - 地址, {2} - 端口
                /// </remarks>
                public const string ServerDisabled = "StartUp.TcpServer.ServerDisabled";
            }

            /// <summary>
            /// WebSocket服务器相关消息
            /// </summary>
            public static class WebSocketServer
            {
                /// <summary>
                /// 启动WebSocket服务器的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.WebSocketServer.StartingServer
                /// 用途: 当开始启动WebSocket服务器时记录
                /// 参数: {0} - 服务器类型, {1} - 端口
                /// </remarks>
                public const string StartingServer = "StartUp.WebSocketServer.StartingServer";

                /// <summary>
                /// WebSocket服务器启动完成的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.WebSocketServer.StartupComplete
                /// 用途: 当WebSocket服务器启动完成时记录
                /// 参数: {0} - 服务器类型, {1} - 端口
                /// </remarks>
                public const string StartupComplete = "StartUp.WebSocketServer.StartupComplete";

                /// <summary>
                /// WebSocket服务器启动失败的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.WebSocketServer.StartupFailed
                /// 用途: 当WebSocket服务器启动失败时记录警告
                /// 参数: {0} - 服务器类型, {1} - 端口
                /// </remarks>
                public const string StartupFailed = "StartUp.WebSocketServer.StartupFailed";

                /// <summary>
                /// WebSocket服务未启用的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.WebSocketServer.ServiceNotEnabled
                /// 用途: 当WebSocket服务未启用时记录警告
                /// 参数: {0} - 服务器类型, {1} - 端口
                /// </remarks>
                public const string ServiceNotEnabled = "StartUp.WebSocketServer.ServiceNotEnabled";
            }

            /// <summary>
            /// 应用程序通用消息
            /// </summary>
            public static class Application
            {
                /// <summary>
                /// 指标服务器启动的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.Application.MetricServerStarted
                /// 用途: 当独立指标服务器启动时记录
                /// 参数: {0} - 端口号
                /// </remarks>
                public const string MetricServerStarted = "StartUp.Application.MetricServerStarted";

                /// <summary>
                /// 监听程序退出消息的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.Application.ListeningExitMessage
                /// 用途: 当开始监听程序退出消息时记录
                /// </remarks>
                public const string ListeningExitMessage = "StartUp.Application.ListeningExitMessage";

                /// <summary>
                /// 执行退出程序的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.Application.ExecutingExitProcedure
                /// 用途: 当执行退出程序时记录
                /// </remarks>
                public const string ExecutingExitProcedure = "StartUp.Application.ExecutingExitProcedure";

                /// <summary>
                /// 获取未处理异常的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.Application.GetUnhandledException
                /// 用途: 当获取到未处理异常时记录
                /// 参数: {0} - 异常标签
                /// </remarks>
                public const string GetUnhandledException = "StartUp.Application.GetUnhandledException";

                /// <summary>
                /// 未处理异常的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.Application.UnhandledException
                /// 用途: 当发生未处理异常时记录
                /// 参数: {0} - 异常信息
                /// </remarks>
                public const string UnhandledException = "StartUp.Application.UnhandledException";

                /// <summary>
                /// 所有未处理异常的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.Application.AllUnhandledExceptions
                /// 用途: 当发生多个未处理异常时记录
                /// 参数: {0} - 异常信息集合
                /// </remarks>
                public const string AllUnhandledExceptions = "StartUp.Application.AllUnhandledExceptions";

                /// <summary>
                /// 未处理异常回调的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.Application.UnhandledExceptionCallback
                /// 用途: 在退出回调中记录未处理异常时使用
                /// 参数: {0} - 异常信息
                /// </remarks>
                public const string UnhandledExceptionCallback = "StartUp.Application.UnhandledExceptionCallback";

                /// <summary>
                /// SIGTERM信号注册的消息
                /// </summary>
                /// <remarks>
                /// 键名: StartUp.Application.SigtermSignalReceived
                /// 用途: 当接收到SIGTERM信号时记录
                /// </remarks>
                public const string SigtermSignalReceived = "StartUp.Application.SigtermSignalReceived";
            }
        }
    }
}