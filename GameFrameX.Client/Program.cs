// See https://aka.ms/new-console-template for more information


using GameFrameX.Client;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Proto;

Console.WriteLine("模拟客户端启动成功!!!");
MessageProtoHelper.Init(typeof(MessageProtoHandler).Assembly);

new UnityTcpClient().Entry(args);
// for (int i = 0; i < 20; i++)
// {
//     Task.Run(() => { new UnityTcpClient().Entry(args); });
// }

Console.ReadKey();