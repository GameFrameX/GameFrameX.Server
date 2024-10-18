using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Setting;
using ProtoBuf;

namespace GameFrameX.Proto.BuiltIn;

/// <summary>
/// 请求链接的服务
/// </summary>
[MessageTypeHandler(((-1) << 16) + 100, MessageOperationType.RequestConnectServer)]
public partial class ReqConnectServer : MessageObject, IRequestMessage
{
    /// <summary>
    ///  服务器类型
    /// </summary>
    [ProtoMember(1)]
    public ServerType ServerType { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(2)]
    public long ServerId { get; set; }
}

/// <summary>
/// 请求链接的服务返回
/// </summary>
[MessageTypeHandler(((-1) << 16) + 101, MessageOperationType.RequestConnectServer)]
public partial class RespConnectServer : MessageObject, IResponseMessage
{
    /// <summary>
    ///  服务器类型
    /// </summary>
    [ProtoMember(1)]
    public ServerType ServerType { get; set; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    [ProtoMember(2)]
    public string ServerName { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(3)]
    public long ServerId { get; set; }

    /// <summary>
    /// 服务器IP
    /// </summary>
    [ProtoMember(5)]
    public string TargetIp { get; set; }

    /// <summary>
    /// 服务器端口
    /// </summary>
    [ProtoMember(6)]
    public ushort TargetPort { get; set; }

    /// <summary>
    /// 返回的错误码
    /// </summary>
    [ProtoMember(888)]
    public int ErrorCode { get; set; }
}

/// <summary>
/// 服务上线
/// </summary>
[MessageTypeHandler(((-1) << 16) + 102, MessageOperationType.Notify)]
public partial class RespServerOnlineServer : MessageObject, IResponseMessage
{
    /// <summary>
    ///  服务器类型
    /// </summary>
    [ProtoMember(1)]
    public ServerType ServerType { get; set; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    [ProtoMember(2)]
    public string ServerName { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(3)]
    public long ServerId { get; set; }

    /// <summary>
    /// 返回的错误码
    /// </summary>
    [ProtoMember(888)]
    public int ErrorCode { get; set; }
}

/// <summary>
/// 服务下线
/// </summary>
[MessageTypeHandler(((-1) << 16) + 103, MessageOperationType.Notify)]
public partial class RespServerOfflineServer : MessageObject, IResponseMessage
{
    /// <summary>
    ///  服务器类型
    /// </summary>
    [ProtoMember(1)]
    public ServerType ServerType { get; set; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    [ProtoMember(2)]
    public string ServerName { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(3)]
    public long ServerId { get; set; }

    /// <summary>
    /// 返回的错误码
    /// </summary>
    [ProtoMember(888)]
    public int ErrorCode { get; set; }
}

/// <summary>
/// 请求注册服务
/// </summary>
[MessageTypeHandler(((-1) << 16) + 104, MessageOperationType.Register)]
public partial class ReqRegisterServer : MessageObject, IRequestMessage
{
    /// <summary>
    ///  服务器类型
    /// </summary>
    [ProtoMember(1)]
    public ServerType ServerType { get; set; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    [ProtoMember(2)]
    public string ServerName { get; set; }

    /// <summary>
    /// 内部服务器IP
    /// </summary>
    [ProtoMember(3)]
    public string InnerIp { get; set; }

    /// <summary>
    /// 内部服务器端口
    /// </summary>
    [ProtoMember(4)]
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 外部服务器IP
    /// </summary>
    [ProtoMember(5)]
    public string OuterIp { get; set; }

    /// <summary>
    /// 外部服务器端口
    /// </summary>
    [ProtoMember(6)]
    public ushort OuterPort { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(7)]
    public long ServerId { get; set; }
}

/// <summary>
/// 请求注册游戏服务
/// </summary>
[MessageTypeHandler(((-1) << 16) + 204, MessageOperationType.Register)]
public partial class ReqRegisterGameServer : MessageObject, IRequestMessage
{
    /// <summary>
    ///  服务器类型
    /// </summary>
    [ProtoMember(1)]
    public ServerType ServerType { get; set; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    [ProtoMember(2)]
    public string ServerName { get; set; }

    /// <summary>
    /// 最小模块消息ID
    /// </summary>
    [ProtoMember(3)]
    public short MinModuleMessageId { get; set; }

    /// <summary>
    /// 最大模块消息ID
    /// </summary>
    [ProtoMember(4)]
    public short MaxModuleMessageId { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(7)]
    public long ServerId { get; set; }
}