@REM 对外服务器
start dotnet GameFrameX.Launcher.dll --ServerType=Router --ServerId=23000 --InnerIp=127.0.0.1 --InnerPort=23110 --WsPort=23111 --OuterIp=127.0.0.1 --OuterPort=23110 --DiscoveryCenterIp=127.0.0.1 --DiscoveryCenterPort=21001 --IsDebug=true --IsDebugReceive=true --IsDebugSend=true
