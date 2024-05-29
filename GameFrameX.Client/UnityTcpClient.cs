using System.Net;
using GameFrameX.Extension;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto;
using GameFrameX.Proto.Proto;
using GameFrameX.Serialize.Serialize;
using GameFrameX.Utility;
using SuperSocket.ClientEngine;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.Client;

public static class UnityTcpClient
{
    public static async void Entry(string[] args)
    {
        var tcpClient = new AsyncTcpSession();
        tcpClient.Connected += OnTcpClientOnConnected; //成功连接到服务器
        tcpClient.Closed += OnTcpClientOnClosed; //从服务器断开连接，当连接不成功时不会触发。
        tcpClient.DataReceived += OnTcpClientOnDataReceived;
        tcpClient.Error += OnTcpClientOnError;

        while (true)
        {
            Thread.Sleep(5000);

            if (!tcpClient.IsConnected)
            {
                Console.WriteLine("未链接到服务器,开启重连");
                tcpClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23001));
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
                    UniqueId = count
                };
                var buffer = Handler(req);
                tcpClient.Send(buffer);

                if (count % 2 == 0)
                {
                    ReqLogin reqLogin = new ReqLogin
                    {
                        UserName = "admin",
                        Password = "123456",
                        UniqueId = count
                    };
                    buffer = Handler(reqLogin);
                    tcpClient.Send(buffer);
                }
            }
        }
    }

    private static void OnTcpClientOnError(object? client, ErrorEventArgs e)
    {
        Console.WriteLine("客户端发生错误:" + e.Exception.Message);
    }

    private static void OnTcpClientOnClosed(object? client, EventArgs e)
    {
        Console.WriteLine("客户端断开连接");
    }

    private static void OnTcpClientOnConnected(object? client, EventArgs e)
    {
        Console.WriteLine("客户端成功连接到服务器");
    }

    private static void OnTcpClientOnDataReceived(object? client, DataEventArgs e)
    {
        //从服务器收到信息。但是一般byteBlock和requestInfo会根据适配器呈现不同的值。
        // var mes = Encoding.UTF8.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
        DecodeMessage(e.Data.ReadBytes(e.Offset, e.Length));
    }

    private static void DecodeMessage(byte[] data)
    {
        int offset = 0;
        var length = data.ReadUShort(ref offset);
        var uniqueId = data.ReadInt(ref offset);
        int messageId = data.ReadInt(ref offset);
        var messageData = data.ReadBytes(offset, length - offset);
        var messageType = ProtoMessageIdHandler.GetMessageTypeById(messageId);
        if (messageType != null)
        {
            var messageObject = (MessageObject)SerializerHelper.Deserialize(messageData, messageType);
            messageObject.MessageId = messageId;
            messageObject.SetUniqueId(uniqueId);
            Console.WriteLine($"客户端接收到信息：{messageObject.ToMessageString()}");
        }
    }

    private static int count = 0;

    private static byte[] Handler(MessageObject message)
    {
        count++;
        var bytes = SerializerHelper.Serialize(message);
        ushort len = (ushort)(2 + 4 + 4 + bytes.Length);
        var buffer = new byte[len];
        int offset = 0;
        buffer.WriteUShort(len, ref offset);
        buffer.WriteInt(message.UniqueId, ref offset);
        var messageId = ProtoMessageIdHandler.GetMessageIdByType(message.GetType());
        message.MessageId = messageId;
        buffer.WriteInt(messageId, ref offset);
        buffer.WriteBytesWithoutLength(bytes, ref offset);
        Console.WriteLine($"客户端接发送信息：{message.ToMessageString()}");
        return buffer;
    }
}