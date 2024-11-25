@REM 网关服务器
start dotnet GameFrameX.Launcher.dll --ServerType=Gateway --ServerId=22000 --InnerIp=127.0.0.1 --InnerPort=22001 --OuterIp=127.0.0.1 --OuterPort=22001 --DiscoveryCenterIp=127.0.0.1 --DiscoveryCenterPort=21001 --IsDebug=true --IsDebugReceive=true --IsDebugSend=true

@REM 网关服务器
@REM start dotnet GameFrameX.Launcher.dll --ServerType=Gateway --ServerId=22001 --InnerIp=127.0.0.1 --InnerPort=22002 --OuterIp=127.0.0.1 --OuterPort=22002 --DiscoveryCenterIp=127.0.0.1 --DiscoveryCenterPort=21001 --IsDebug=true --IsDebugReceive=true --IsDebugSend=true

@REM 网关服务器
@REM start dotnet GameFrameX.Launcher.dll --ServerType=Gateway --ServerId=22002 --InnerIp=127.0.0.1 --InnerPort=22003 --OuterIp=127.0.0.1 --OuterPort=22003 --DiscoveryCenterIp=127.0.0.1 --DiscoveryCenterPort=21001 --IsDebug=true --IsDebugReceive=true --IsDebugSend=true
