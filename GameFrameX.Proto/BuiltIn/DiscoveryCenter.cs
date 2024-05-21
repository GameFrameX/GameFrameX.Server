using GameFrameX.NetWork.Messages;
using GameFrameX.Setting;
using ProtoBuf;

namespace GameFrameX.Proto.BuiltIn;

/// <summary>
/// 请求注册服务
/// </summary>
[MessageTypeHandler(9000001)]
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
    /// 服务器IP
    /// </summary>
    [ProtoMember(3)]
    public string ServerIP { get; set; }

    /// <summary>
    /// 服务器端口
    /// </summary>
    [ProtoMember(4)]
    public int ServerPort { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(5)]
    public int ServerID { get; set; }
}