using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.ChannelBase;
using GameFrameX.NetWork.Message;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.ProtoBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GameFrameX.Launcher.StartUp;

public abstract class AppStartUpService : AppStartUpBase
{
}