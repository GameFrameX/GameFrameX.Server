using System.Net;
using GameFrameX.Extension;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto;
using GameFrameX.Proto.Proto;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.SuperSocket.ClientEngine;
using GameFrameX.Utility;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.Client;

public class UnityTcpClient
{
    private AsyncTcpSession _tcpClient;

    public void Entry(string[] args)
    {
        _tcpClient = new AsyncTcpSession();
        _tcpClient.Connected += OnTcpClientOnConnected; //成功连接到服务器
        _tcpClient.Closed += OnTcpClientOnClosed; //从服务器断开连接，当连接不成功时不会触发。
        _tcpClient.DataReceived += OnTcpClientOnDataReceived;
        _tcpClient.Error += OnTcpClientOnError;

        while (true)
        {
            Thread.Sleep(5000);

            if (!_tcpClient.IsConnected)
            {
                Console.WriteLine("未链接到服务器,开启重连");
                _tcpClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 29100));
                continue;
                // Console.WriteLine("链接到服务器结果：" + result);
                // if (result.ResultCode != ResultCode.Success)
                // {
                //     continue;
                // }
            }

            Console.WriteLine("--------------------------------");
            // for (int i = 0; i < 10; i++)
            {
                ReqHeartBeat req = new ReqHeartBeat
                {
                    Timestamp = TimeHelper.UnixTimeSeconds(),
                };
                SendToServer(req);

                if (_count % 2 == 0)
                {
                    ReqLogin reqLogin = new ReqLogin
                    {
                        UserName = "admin",
                        Password = "123456",
                        UniqueId = _count
                    };
                    SendToServer(reqLogin);
                }
            }
        }
    }

    private void SendToServer(MessageObject messageObject)
    {
        var buffer = Handler(messageObject);
        _tcpClient.Send(buffer);
    }

    private static void OnTcpClientOnError(object client, ErrorEventArgs e)
    {
        Console.WriteLine("客户端发生错误:" + e.Exception.Message);
    }

    private static void OnTcpClientOnClosed(object client, EventArgs e)
    {
        Console.WriteLine("客户端断开连接");
    }

    private static void OnTcpClientOnConnected(object client, EventArgs e)
    {
        Console.WriteLine("客户端成功连接到服务器");
    }

    private static void OnTcpClientOnDataReceived(object client, DataEventArgs e)
    {
        DecodeMessage(e.Data.ReadBytes(e.Offset, e.Length));
    }

    private static void DecodeMessage(byte[] data)
    {
        int offset = 0;
        var length = data.ReadUShort(ref offset);
        var operationType = data.ReadByte(ref offset);
        var zipFlag = data.ReadByte(ref offset);
        var uniqueId = data.ReadInt(ref offset);
        int messageId = data.ReadInt(ref offset);
        var messageData = data.ReadBytes(offset, length - offset);
        var messageType = MessageProtoHelper.GetMessageTypeById(messageId);
        if (messageType != null)
        {
            var messageObject = (MessageObject)ProtoBufSerializerHelper.Deserialize(messageData, messageType);
            messageObject.SetMessageId(messageId);
            messageObject.SetOperationType((MessageOperationType)operationType);
            messageObject.SetUniqueId(uniqueId);
            Console.WriteLine($"客户端接收到信息：{messageObject.ToMessageString()}");
        }
    }

    private static int _count;

    private static byte[] Handler(MessageObject message)
    {
        _count++;
        var bytes = ProtoBufSerializerHelper.Serialize(message);
        ushort len = (ushort)(2 + 1 + 1 + 4 + 4 + bytes.Length);
        var buffer = new byte[len];
        int offset = 0;
        buffer.WriteUShort(len, ref offset);
        buffer.WriteByte((byte)(message is ReqHeartBeat ? MessageOperationType.HeartBeat : MessageOperationType.Game), ref offset);
        buffer.WriteByte(0, ref offset);
        buffer.WriteInt(message.UniqueId, ref offset);
        var messageId = MessageProtoHelper.GetMessageIdByType(message.GetType());
        message.SetMessageId(messageId);
        buffer.WriteInt(messageId, ref offset);
        buffer.WriteBytesWithoutLength(bytes, ref offset);
        Console.WriteLine($"客户端接发送信息：{message.ToMessageString()}");
        return buffer;
    }
}