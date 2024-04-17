using GameFrameX.Extension;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto;
using GameFrameX.Proto.Proto;
using GameFrameX.Serialize.Serialize;
using GameFrameX.Utility;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace GameFrameX.Client;

using ResultCode = TouchSocket.Core.ResultCode;

public static class UnityTcpClient
{
    public static async void Entry(string[] args)
    {
        var tcpClient = new TcpClient();
        tcpClient.Connected = (client, e) =>
        {
            Console.WriteLine("客户端成功连接到服务器");
            return EasyTask.CompletedTask;
        }; //成功连接到服务器
        tcpClient.Disconnected = (client, e) =>
        {
            Console.WriteLine("客户端断开连接");
            return EasyTask.CompletedTask;
        }; //从服务器断开连接，当连接不成功时不会触发。
        tcpClient.Received = (client, e) =>
        {
            //从服务器收到信息。但是一般byteBlock和requestInfo会根据适配器呈现不同的值。
            // var mes = Encoding.UTF8.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
            int length = e.ByteBlock.ReadInt32(EndianType.Big);
            long timestamp = e.ByteBlock.ReadInt64(EndianType.Big);
            int messageId = e.ByteBlock.ReadInt32(EndianType.Big);
            int offset = e.ByteBlock.Pos;
            var messageData = e.ByteBlock.Buffer.ReadBytes(ref offset);
            var messageType = ProtoMessageIdHandler.GetRespTypeById(messageId);
            if (messageType != null)
            {
                var messageObject = (MessageObject)SerializerHelper.Deserialize(messageData, messageType);
                messageObject.MessageId = messageId;
                tcpClient.Logger.Info($"客户端接收到信息：{messageObject}");
            }

            return EasyTask.CompletedTask;
        };

//载入配置
        await tcpClient.SetupAsync(new TouchSocketConfig()
            .SetRemoteIPHost("127.0.0.1:21000")
            .ConfigureContainer(a =>
            {
                a.AddConsoleLogger(); //添加一个日志注入
            }));

        while (true)
        {
            Thread.Sleep(5000);

            if (!tcpClient.Online)
            {
                Console.WriteLine("未链接到服务器,开启重连");
                var result = tcpClient.TryConnect();
                Console.WriteLine("链接到服务器结果：" + result);
                if (result.ResultCode != ResultCode.Success)
                {
                    continue;
                }
            }

            var buffer = GetBuffer();
            tcpClient.Send(buffer);
        }
    }

    public static byte[] GetBuffer()
    {
        ReqLogin req = new ReqLogin()
        {
            Password = "123456",
            UserName = "admin",
        };

        var bytes = SerializerHelper.Serialize(req);
        var buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(bytes.Length + 20);
        int offset = 0;
        buffer.WriteInt(bytes.Length, ref offset);
        buffer.WriteLong(TimeHelper.UnixTimeSeconds(), ref offset);
        buffer.WriteInt(1, ref offset);
        buffer.WriteBytes(bytes, ref offset);
        System.Buffers.ArrayPool<byte>.Shared.Return(buffer);
        return buffer;
    }
}