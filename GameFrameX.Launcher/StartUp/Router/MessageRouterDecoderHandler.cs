using GameFrameX.Extension;
using GameFrameX.NetWork;
using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp.Router;

/// <summary>
/// 消息结构 4 + 4 + 4 + bytes.length
/// </summary>
public class MessageRouterDecoderHandler : BaseMessageDecoderHandler
{
}