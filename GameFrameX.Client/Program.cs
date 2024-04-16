// See https://aka.ms/new-console-template for more information


using GameFrameX.Extension;
using GameFrameX.Proto.Proto;
using GameFrameX.Serialize.Serialize;
using GameFrameX.Utility;
using TouchSocket.Core;
using TouchSocket.Sockets;
using ResultCode = TouchSocket.Core.ResultCode;

var tcpClient = new TcpClient();
tcpClient.Connecting = (client, e) => { return EasyTask.CompletedTask; }; //即将连接到服务器，此时已经创建socket，但是还未建立tcp
tcpClient.Connected = (client, e) => { return EasyTask.CompletedTask; }; //成功连接到服务器
tcpClient.Disconnecting = (client, e) => { return EasyTask.CompletedTask; }; //即将从服务器断开连接。此处仅主动断开才有效。
tcpClient.Disconnected = (client, e) => { return EasyTask.CompletedTask; }; //从服务器断开连接，当连接不成功时不会触发。
tcpClient.Received = (client, e) =>
{
    //从服务器收到信息。但是一般byteBlock和requestInfo会根据适配器呈现不同的值。
    // var mes = Encoding.UTF8.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
    tcpClient.Logger.Info($"客户端接收到信息：");
    return EasyTask.CompletedTask;
};

//载入配置
tcpClient.Setup(new TouchSocketConfig()
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

    ReqHeartBeat req = new ReqHeartBeat
    {
        Timestamp = TimeHelper.UnixTimeSeconds()
    };

    var bytes = SerializerHelper.Serialize(req);
    var buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(bytes.Length + 20);
    int offset = 0;
    buffer.WriteInt(bytes.Length, ref offset);
    buffer.WriteLong(TimeHelper.UnixTimeSeconds(), ref offset);
    buffer.WriteInt(1, ref offset);
    buffer.WriteBytes(bytes, ref offset);
    tcpClient.Send(buffer);
    System.Buffers.ArrayPool<byte>.Shared.Return(buffer);
}