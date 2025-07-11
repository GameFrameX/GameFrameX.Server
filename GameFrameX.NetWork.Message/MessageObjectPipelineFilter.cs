using System.Buffers;
using GameFrameX.Foundation.Extensions;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.ProtoBase;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 消息对象流水线过滤处理器
/// </summary>
public sealed class MessageObjectPipelineFilter : PipelineFilterBase<IMessage>
{
    /// <summary>
    /// 解析函数
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public override IMessage Filter(ref SequenceReader<byte> reader)
    {
        var pack = reader.Sequence;
        reader.TryPeekBigEndianValue(out uint totalLength);
        if (totalLength <= 0)
        {
            reader.AdvanceToEnd();
            return default;
        }

        var readBuffer = pack.Slice(pack.Start, totalLength);
        if (reader.Remaining < totalLength)
        {
            reader.AdvanceToEnd();
        }
        else
        {
            reader.Advance(totalLength);
        }

        return MessageHelper.DecoderHandler.Handler(ref readBuffer);
    }
}