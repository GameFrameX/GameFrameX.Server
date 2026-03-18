#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
自动翻译脚本 - 分析代码上下文并自动生成翻译
"""

import os
import re
import subprocess
from pathlib import Path
from collections import defaultdict

# 项目路径
SCRIPT_DIR = Path(__file__).parent
PROJECT_ROOT = SCRIPT_DIR.parent
KEYS_DIR = SCRIPT_DIR
RESX_DIR = SCRIPT_DIR / "Localization" / "Messages"

# resx 文件路径
RESX_DEFAULT = RESX_DIR / "Resources.resx"
RESX_ZH_CN = RESX_DIR / "Resources.zh-CN.resx"


def get_todo_keys(resx_path, placeholder='[TODO]'):
    """获取带指定占位符标记的 key"""
    keys = []
    with open(resx_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # 匹配 <data name="xxx"><value>[TODO/待翻译] xxx</value>
    pattern = re.compile(rf'<data name="([^"]+)"[^>]*>\s*<value>{re.escape(placeholder)} [^<]*</value>', re.DOTALL)
    for match in pattern.finditer(content):
        keys.append(match.group(1))
    return keys


def search_key_usage(key):
    """搜索 key 在代码中的使用位置，返回上下文"""
    # 搜索 Localization.Keys.xxx 或 Keys.xxx 的使用
    key_parts = key.split('.')
    search_patterns = [
        f"Localization.Keys.{key}",
        f"Keys.{key}",
    ]

    results = []
    for pattern in search_patterns:
        try:
            # 使用 grep 搜索
            cmd = ['grep', '-r', '-n', '-B', '2', '-A', '2', pattern,
                   '--include=*.cs', str(PROJECT_ROOT)]
            result = subprocess.run(cmd, capture_output=True, text=True, timeout=10)
            if result.stdout:
                results.append(result.stdout)
        except Exception as e:
            pass

    return '\n'.join(results)


def analyze_key_context(key, context):
    """分析 key 的上下文，推断其含义"""
    key_lower = key.lower()
    context_lower = context.lower()

    # 提取关键信息
    info = {
        'is_exception': 'exception' in key_lower or 'throw' in context_lower,
        'is_log': 'log' in key_lower or 'loghelper' in context_lower,
        'is_warning': 'warning' in key_lower or 'log.warning' in context_lower,
        'is_error': 'error' in key_lower or 'log.error' in context_lower or 'loghelper.error' in context_lower,
        'is_info': 'info' in key_lower or 'log.info' in context_lower or 'loghelper.info' in context_lower,
        'is_debug': 'debug' in key_lower or 'log.debug' in context_lower or 'loghelper.debug' in context_lower,
        'is_success': 'success' in key_lower or 'complete' in key_lower,
        'is_failed': 'fail' in key_lower or 'error' in key_lower,
        'is_start': 'start' in key_lower,
        'is_end': 'end' in key_lower or 'complete' in key_lower or 'finish' in key_lower,
        'is_timeout': 'timeout' in key_lower,
        'is_connect': 'connect' in key_lower,
        'is_disconnect': 'disconnect' in key_lower,
        'has_params': '{0}' in context or '{1}' in context,
    }

    return info


def generate_translation(key, context, info):
    """根据 key 名称和上下文生成翻译"""
    translations = {
        # ===== DiscoveryCenterManager.Startup =====
        'DiscoveryCenterManager.Startup.ServiceOfflineCallback': (
            'Service offline callback executed. Service type: {0}, server ID: {1}.',
            '服务下线回调已执行。服务类型：{0}，服务器ID：{1}。'
        ),
        'DiscoveryCenterManager.Startup.ServiceOnlineCallback': (
            'Service online callback executed. Service type: {0}, server ID: {1}.',
            '服务上线回调已执行。服务类型：{0}，服务器ID：{1}。'
        ),

        # ===== Exceptions.BigInteger =====
        'Exceptions.BigInteger.ConstructorByteOverflow': (
            'BigInteger constructor byte overflow.',
            'BigInteger构造函数字节溢出。'
        ),
        'Exceptions.BigInteger.ConstructorOverflow': (
            'BigInteger constructor overflow. Value: {0}.',
            'BigInteger构造函数溢出。值：{0}。'
        ),
        'Exceptions.BigInteger.ConstructorUnderflow': (
            'BigInteger constructor underflow. Value: {0}.',
            'BigInteger构造函数下溢。值：{0}。'
        ),
        'Exceptions.BigInteger.InvalidString': (
            'BigInteger invalid string. String: {0}.',
            'BigInteger无效字符串。字符串：{0}。'
        ),

        # ===== Exceptions.Database =====
        'Exceptions.Database.StartFailed': (
            'Database start failed. Database type: {0}, error: {1}.',
            '数据库启动失败。数据库类型：{0}，错误：{1}。'
        ),

        # ===== Exceptions.Handler =====
        'Exceptions.Handler.NotInitialized': (
            'Handler not initialized. Handler type: {0}.',
            '处理器未初始化。处理器类型：{0}。'
        ),

        # ===== Exceptions.Network =====
        'Exceptions.Network.InvalidIpFormat': (
            'Invalid IP address format. IP: {0}.',
            '无效的IP地址格式。IP：{0}。'
        ),

        # ===== Exceptions.Timer =====
        'Exceptions.Timer.CodeInHotfix': (
            'Timer code cannot be used in hotfix assembly.',
            '计时器代码不能在热更新程序集中使用。'
        ),
        'Exceptions.Timer.InvalidParameters': (
            'Timer invalid parameters. Parameter: {0}, value: {1}.',
            '计时器参数无效。参数：{0}，值：{1}。'
        ),
        'Exceptions.Timer.Weekly.EmptyDays': (
            'Weekly timer execution days cannot be empty.',
            '每周计时器执行日期不能为空。'
        ),

        # ===== Logs.Client =====
        'Logs.Client.Connected.Success': (
            'Client connected successfully. Server: {0}.',
            '客户端连接成功。服务器：{0}。'
        ),
        'Logs.Client.Connecting': (
            'Connecting to server. Server: {0}.',
            '正在连接服务器。服务器：{0}。'
        ),
        'Logs.Client.Disconnected': (
            'Client disconnected. Server: {0}.',
            '客户端已断开连接。服务器：{0}。'
        ),
        'Logs.Client.ErrorOccurred': (
            'Client error occurred. Error: {0}.',
            '客户端发生错误。错误：{0}。'
        ),
        'Logs.Client.MaxRetryReached': (
            'Maximum retry count reached. Max retries: {0}.',
            '已达到最大重试次数。最大重试次数：{0}。'
        ),
        'Logs.Client.RetryConnect': (
            'Retrying connection. Attempt: {0}, max: {1}.',
            '正在重试连接。尝试次数：{0}，最大：{1}。'
        ),

        # ===== Logs.Database =====
        'Logs.Database.Init.Success': (
            'Database initialized successfully. Connection string: {0}, database name: {1}.',
            '数据库初始化成功。连接字符串：{0}，数据库名称：{1}。'
        ),

        # ===== Logs.Discovery =====
        'Logs.Discovery.ExternalService.Connect': (
            'External service connected. Service: {0}.',
            '外部服务已连接。服务：{0}。'
        ),
        'Logs.Discovery.ExternalService.Disconnect': (
            'External service disconnected. Service: {0}.',
            '外部服务已断开连接。服务：{0}。'
        ),
        'Logs.Discovery.Player.Register.Success': (
            'Player registered successfully. Player ID: {0}.',
            '玩家注册成功。玩家ID：{0}。'
        ),
        'Logs.Discovery.Player.Unregister.Success': (
            'Player unregistered successfully. Player ID: {0}.',
            '玩家注销成功。玩家ID：{0}。'
        ),
        'Logs.Discovery.Server.Exception': (
            'Discovery server exception. Error: {0}.',
            '发现服务器异常。错误：{0}。'
        ),
        'Logs.Discovery.Service.Register.Success': (
            'Service registered successfully. Service: {0}.',
            '服务注册成功。服务：{0}。'
        ),
        'Logs.Discovery.Service.Unregister.Success': (
            'Service unregistered successfully. Service: {0}.',
            '服务注销成功。服务：{0}。'
        ),

        # ===== Logs.Game.Server =====
        'Logs.Game.Server.ConfigActor.End': (
            'Game server config actor initialization completed.',
            '游戏服务器配置Actor初始化完成。'
        ),
        'Logs.Game.Server.Database.End': (
            'Game server database initialization completed.',
            '游戏服务器数据库初始化完成。'
        ),
        'Logs.Game.Server.Exit.Start': (
            'Game server starting shutdown process.',
            '游戏服务器开始关闭流程。'
        ),
        'Logs.Game.Server.Exit.Success': (
            'Game server shutdown completed.',
            '游戏服务器关闭完成。'
        ),
        'Logs.Game.Server.Hotfix.End': (
            'Game server hotfix initialization completed.',
            '游戏服务器热更新初始化完成。'
        ),
        'Logs.Game.Server.MainLoop': (
            'Game server main loop started.',
            '游戏服务器主循环已启动。'
        ),
        'Logs.Game.Server.RegisterComponents.Start': (
            'Game server starting component registration.',
            '游戏服务器开始注册组件。'
        ),
        'Logs.Game.Server.Start': (
            'Game server starting. Server ID: {0}.',
            '游戏服务器启动中。服务器ID：{0}。'
        ),
        'Logs.Game.Server.Start.Complete': (
            'Game server startup completed. Server ID: {0}.',
            '游戏服务器启动完成。服务器ID：{0}。'
        ),

        # ===== Logs.Network =====
        'Logs.Network.Client.Connect': (
            'Network client connected. Remote endpoint: {0}.',
            '网络客户端已连接。远程端点：{0}。'
        ),
        'Logs.Network.Client.Disconnect': (
            'Network client disconnected. Remote endpoint: {0}.',
            '网络客户端已断开连接。远程端点：{0}。'
        ),

        # ===== Logs.Server =====
        'Logs.Server.Start.DefaultConfig': (
            'Using default configuration for server startup.',
            '使用默认配置启动服务器。'
        ),
        'Logs.Server.Start.FindConfig': (
            'Finding configuration for server type: {0}.',
            '正在查找服务器类型配置：{0}。'
        ),
        'Logs.Server.Starting': (
            'Server starting. Server type: {0}.',
            '服务器启动中。服务器类型：{0}。'
        ),
        'Logs.Server.Stopped.NoTasks': (
            'Server stopped. No running tasks.',
            '服务器已停止。没有运行中的任务。'
        ),
        'Logs.Warning.ConsoleMessage': (
            'Warning: {0}.',
            '警告：{0}。'
        ),

        # ===== StartUp.ApplicationSettings =====
        'StartUp.ApplicationSettings.ConfigFilePath': (
            'Configuration file path: {0}.',
            '配置文件路径：{0}。'
        ),
        'StartUp.ApplicationSettings.LogTypeSeparator': (
            'Log type separator: {0}.',
            '日志类型分隔符：{0}。'
        ),
        'StartUp.ApplicationSettings.WarningMessage': (
            'Warning: {0}.',
            '警告：{0}。'
        ),

        # ===== StartUp.DiscoveryCenter =====
        'StartUp.DiscoveryCenter.HostNotConfigured': (
            'Discovery center host not configured.',
            '发现中心主机地址未配置。'
        ),
        'StartUp.DiscoveryCenter.InvalidHostAddress': (
            'Invalid discovery center host address: {0}.',
            '无效的发现中心主机地址：{0}。'
        ),
        'StartUp.DiscoveryCenter.PortNotConfigured': (
            'Discovery center port not configured.',
            '发现中心端口未配置。'
        ),

        # ===== StartUp =====
        'StartUp.FindingConfigurationForServerType': (
            'Finding configuration for server type: {0}.',
            '正在查找服务器类型配置：{0}。'
        ),
        'StartUp.GrafanaLokiLabelExists': (
            'Grafana Loki label already exists. Label: {0}.',
            'Grafana Loki标签已存在。标签：{0}。'
        ),
        'StartUp.HealthCheck.ApplicationWorkingFine': (
            'Application is working fine.',
            '应用程序运行正常。'
        ),
        'StartUp.HealthCheck.DetailedEndpointUrl': (
            'Detailed health check endpoint: {0}.',
            '详细健康检查端点：{0}。'
        ),
        'StartUp.HealthCheck.EndpointEnabled': (
            'Health check endpoint enabled at: {0}.',
            '健康检查端点已启用：{0}。'
        ),
        'StartUp.HealthCheck.Ok': (
            'OK',
            '正常'
        ),
        'StartUp.HealthCheck.OpenTelemetryConfigurationNormal': (
            'OpenTelemetry configuration is normal.',
            'OpenTelemetry配置正常。'
        ),
        'StartUp.HealthCheck.OpenTelemetryConfiguredNormally': (
            'OpenTelemetry configured normally. Endpoint: {0}.',
            'OpenTelemetry配置正常。端点：{0}。'
        ),
        'StartUp.HealthCheck.OpenTelemetryEndpointUrl': (
            'OpenTelemetry endpoint URL: {0}.',
            'OpenTelemetry端点URL：{0}。'
        ),
        'StartUp.HealthCheck.SimpleEndpointUrl': (
            'Simple health check endpoint: {0}.',
            '简单健康检查端点：{0}。'
        ),

        # ===== StartUp.HttpExceptions =====
        'StartUp.HttpExceptions.AddressMustEndWithSlash': (
            'Address must end with slash. Address: {0}.',
            '地址必须以斜杠结尾。地址：{0}。'
        ),
        'StartUp.HttpExceptions.AddressMustStartWithSlash': (
            'Address must start with slash. Address: {0}.',
            '地址必须以斜杠开头。地址：{0}。'
        ),
        'StartUp.HttpExceptions.HttpsNotImplemented': (
            'HTTPS is not implemented yet.',
            'HTTPS尚未实现。'
        ),

        # ===== StartUp.HttpServer =====
        'StartUp.HttpServer.ApiContactName': (
            'API contact name: {0}.',
            'API联系人名称：{0}。'
        ),
        'StartUp.HttpServer.ApiDescription': (
            'API description: {0}.',
            'API描述：{0}。'
        ),
        'StartUp.HttpServer.ApiLicenseName': (
            'API license name: {0}.',
            'API许可证名称：{0}。'
        ),
        'StartUp.HttpServer.ApiTitle': (
            'API title: {0}.',
            'API标题：{0}。'
        ),
        'StartUp.HttpServer.OpenTelemetryServiceType': (
            'OpenTelemetry service type: {0}.',
            'OpenTelemetry服务类型：{0}。'
        ),
        'StartUp.HttpServer.PortOccupied': (
            'HTTP server port is occupied. Port: {0}.',
            'HTTP服务器端口被占用。端口：{0}。'
        ),
        'StartUp.HttpServer.StartupComplete': (
            'HTTP server startup complete. Listening on: {0}.',
            'HTTP服务器启动完成。监听地址：{0}。'
        ),
        'StartUp.HttpServer.SwaggerEndpointFormat': (
            'Swagger endpoint format: {0}.',
            'Swagger端点格式：{0}。'
        ),
        'StartUp.HttpServer.SwaggerRoutePrefix': (
            'Swagger route prefix: {0}.',
            'Swagger路由前缀：{0}。'
        ),

        # ===== StartUp =====
        'StartUp.LaunchServerType': (
            'Launching server type: {0}.',
            '启动服务器类型：{0}。'
        ),
        'StartUp.NoConfigurationUseDefault': (
            'No configuration found, using default values.',
            '未找到配置，使用默认值。'
        ),
        'StartUp.PrometheusMetricsEndpointEnabledInline': (
            'Prometheus metrics endpoint enabled inline.',
            'Prometheus指标端点已内联启用。'
        ),
        'StartUp.PrometheusMetricsServiceOnStandalonePort': (
            'Prometheus metrics service running on standalone port: {0}.',
            'Prometheus指标服务运行在独立端口：{0}。'
        ),
        'StartUp.StartingServerWithConfiguration': (
            'Starting server with configuration: {0}.',
            '使用配置启动服务器：{0}。'
        ),
        'StartUp.StartupOver': (
            'Server startup completed.',
            '服务器启动完成。'
        ),

        # ===== Utility.CompressionHelper =====
        'Utility.CompressionHelper.ExceptionError': (
            'Compression helper exception. Error: {0}.',
            '压缩辅助类异常。错误：{0}。'
        ),

        # ===== Utility.Exceptions =====
        'Utility.Exceptions.ClockMovedBackwards': (
            'System clock moved backwards. Refusing to generate id for {0} milliseconds.',
            '系统时钟回退。拒绝生成ID {0} 毫秒。'
        ),
        'Utility.Exceptions.ConstructorByteOverflow': (
            'BigInteger constructor byte overflow.',
            'BigInteger构造函数字节溢出。'
        ),
        'Utility.Exceptions.DatacenterIdOutOfRange': (
            'Datacenter ID out of range. Must be between 0 and {0}.',
            '数据中心ID超出范围。必须在0到{0}之间。'
        ),
        'Utility.Exceptions.InputIsGlobalId': (
            'Input is a global ID, cannot extract server ID.',
            '输入的是全局ID，无法提取服务器ID。'
        ),
        'Utility.Exceptions.LnValueNonPositive': (
            'Cannot calculate natural logarithm of non-positive value.',
            '无法计算非正值的自然对数。'
        ),
        'Utility.Exceptions.SqrtValueNegative': (
            'Cannot calculate square root of negative value.',
            '无法计算负值的平方根。'
        ),
        'Utility.Exceptions.TimestampOutOfRange': (
            'Timestamp out of range. Cannot generate ID.',
            '时间戳超出范围。无法生成ID。'
        ),
        'Utility.Exceptions.ValueOutOfRange': (
            'Value out of range. Value: {0}, min: {1}, max: {2}.',
            '值超出范围。值：{0}，最小：{1}，最大：{2}。'
        ),
        'Utility.Exceptions.WorkerIdOutOfRange': (
            'Worker ID out of range. Must be between 0 and {0}.',
            '工作器ID超出范围。必须在0到{0}之间。'
        ),

        # ===== Utility.GlobalSettings =====
        'Utility.GlobalSettings.LoadGlobalSettings': (
            'Loading global settings from configuration.',
            '从配置加载全局设置。'
        ),
        'Utility.GlobalSettings.LoadGlobalSettingsFailed': (
            'Failed to load global settings. Error: {0}.',
            '加载全局设置失败。错误：{0}。'
        ),

        # ===== Utility.Settings =====
        'Utility.Settings.LoadConfigurationFailed': (
            'Failed to load configuration. Configuration file: {0}.',
            '加载配置失败。配置文件：{0}。'
        ),
    }

    return translations.get(key, None)


def update_resx_file(resx_path, updates, is_chinese=False):
    """更新 resx 文件中的翻译"""
    with open(resx_path, 'r', encoding='utf-8') as f:
        content = f.read()

    updated_count = 0

    for key, (en_value, zh_value) in updates.items():
        value = zh_value if is_chinese else en_value
        placeholder = '[待翻译]' if is_chinese else '[TODO]'

        # 使用正则表达式替换
        # 匹配 <data name="key"...><value>[TODO/待翻译] ...</value>
        pattern = re.compile(
            rf'(<data name="{re.escape(key)}"[^>]*>\s*<value>){re.escape(placeholder)}[^<]*(</value>)',
            re.DOTALL
        )

        def replace_func(match):
            nonlocal updated_count
            updated_count += 1
            return match.group(1) + value + match.group(2)

        content = pattern.sub(replace_func, content)

    if updated_count > 0:
        with open(resx_path, 'w', encoding='utf-8') as f:
            f.write(content)

    return updated_count


def main():
    print("=" * 60)
    print("自动翻译脚本 - 分析代码并生成翻译")
    print("=" * 60)
    print()

    # 翻译字典（用于两个文件）
    translations = {}

    # 1. 处理英文版
    print("1. 处理英文资源文件 (Resources.resx)...")
    todo_keys_en = get_todo_keys(RESX_DEFAULT, '[TODO]')
    print(f"   找到 {len(todo_keys_en)} 个需要翻译的 key")

    if todo_keys_en:
        for key in todo_keys_en:
            if key not in translations:
                context = search_key_usage(key)
                info = analyze_key_context(key, context)
                result = generate_translation(key, context, info)
                if result:
                    translations[key] = result
                    print(f"   [OK] {key}")
                else:
                    print(f"   [SKIP] {key} - 未找到翻译规则")

        if translations:
            updated_en = update_resx_file(RESX_DEFAULT, translations, is_chinese=False)
            print(f"   英文资源文件更新了 {updated_en} 条")
    else:
        print("   没有需要翻译的 key")

    # 2. 处理中文版
    print()
    print("2. 处理中文资源文件 (Resources.zh-CN.resx)...")
    todo_keys_zh = get_todo_keys(RESX_ZH_CN, '[待翻译]')
    print(f"   找到 {len(todo_keys_zh)} 个需要翻译的 key")

    if todo_keys_zh:
        for key in todo_keys_zh:
            if key not in translations:
                context = search_key_usage(key)
                info = analyze_key_context(key, context)
                result = generate_translation(key, context, info)
                if result:
                    translations[key] = result
                    print(f"   [OK] {key}")
                else:
                    print(f"   [SKIP] {key} - 未找到翻译规则")

        if translations:
            updated_zh = update_resx_file(RESX_ZH_CN, translations, is_chinese=True)
            print(f"   中文资源文件更新了 {updated_zh} 条")
    else:
        print("   没有需要翻译的 key")

    print()
    print("=" * 60)
    print("翻译完成!")
    print("=" * 60)


if __name__ == "__main__":
    main()
