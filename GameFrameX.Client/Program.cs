﻿// See https://aka.ms/new-console-template for more information


using GameFrameX.Client;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto;

MessageProtoHelper.Init(typeof(MessageProtoHandler).Assembly);

new UnityTcpClient().Entry(args);
// for (int i = 0; i < 20; i++)
// {
//     Task.Run(() => { new UnityTcpClient().Entry(args); });
// }

Console.ReadKey();