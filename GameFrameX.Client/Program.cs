// See https://aka.ms/new-console-template for more information


using GameFrameX.Client;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto;

MessageProtoHelper.Init(typeof(MessageProtoHandler).Assembly);

UnityTcpClient.Entry(args);