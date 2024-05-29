using GameFrameX.NetWork.Messages;
using GameFrameX.Setting;
using ProtoBuf;

namespace GameFrameX.Proto.BuiltIn;

/// <summary>
/// 请求链接的服务
/// </summary>
[MessageTypeHandler((999 << 16) + 100)]
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
    public long ServerID { get; set; }
}

/// <summary>
/// 请求链接的服务返回
/// </summary>
[MessageTypeHandler((999 << 16) + 101)]
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
    public long ServerID { get; set; }

    /// <summary>
    /// 服务器IP
    /// </summary>
    [ProtoMember(5)]
    public string TargetIP { get; set; }

    /// <summary>
    /// 服务器端口
    /// </summary>
    [ProtoMember(6)]
    public ushort TargetPort { get; set; }
}

/// <summary>
/// 服务上线
/// </summary>
[MessageTypeHandler((999 << 16) + 102)]
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
    public long ServerID { get; set; }
}

/// <summary>
/// 服务下线
/// </summary>
[MessageTypeHandler((999 << 16) + 103)]
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
    public long ServerID { get; set; }
}

/// <summary>
/// 请求注册服务
/// </summary>
[MessageTypeHandler((999 << 16) + 104)]
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
    public string InnerIP { get; set; }

    /// <summary>
    /// 内部服务器端口
    /// </summary>
    [ProtoMember(4)]
    public ushort InnerPort { get; set; }

    /// <summary>
    /// 外部服务器IP
    /// </summary>
    [ProtoMember(5)]
    public string OuterIP { get; set; }

    /// <summary>
    /// 外部服务器端口
    /// </summary>
    [ProtoMember(6)]
    public ushort OuterPort { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(7)]
    public long ServerID { get; set; }
}