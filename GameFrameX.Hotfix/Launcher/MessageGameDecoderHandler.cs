using System.Buffers;
using GameFrameX.Launcher.StartUp;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Messages;
using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Hotfix.Launcher;

/// <summary>
/// 消息结构 4 + 4 + 4 + bytes.length
/// </summary>
public class MessageGameDecoderHandler : BaseMessageDecoderHandler
{
}