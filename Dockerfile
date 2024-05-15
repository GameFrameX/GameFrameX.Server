FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SuperSocket/SuperSocket.Server.Abstractions/SuperSocket.Server.Abstractions.csproj", "SuperSocket/SuperSocket.Server.Abstractions/"]
COPY ["SuperSocket/SuperSocket.Primitives/SuperSocket.Primitives.csproj", "SuperSocket/SuperSocket.Primitives/"]
COPY ["SuperSocket/SuperSocket.ProtoBase/SuperSocket.ProtoBase.csproj", "SuperSocket/SuperSocket.ProtoBase/"]
COPY ["SuperSocket/SuperSocket.Connection/SuperSocket.Connection.csproj", "SuperSocket/SuperSocket.Connection/"]
COPY ["SuperSocket/SuperSocket.WebSocket.Server/SuperSocket.WebSocket.Server.csproj", "SuperSocket/SuperSocket.WebSocket.Server/"]
COPY ["SuperSocket/SuperSocket.WebSocket/SuperSocket.WebSocket.csproj", "SuperSocket/SuperSocket.WebSocket/"]
COPY ["SuperSocket/SuperSocket.Server/SuperSocket.Server.csproj", "SuperSocket/SuperSocket.Server/"]
COPY ["SuperSocket/SuperSocket.Command/SuperSocket.Command.csproj", "SuperSocket/SuperSocket.Command/"]
COPY ["SuperSocket/SuperSocket.ClientEngine/SuperSocket.ClientEngine.csproj", "SuperSocket/SuperSocket.ClientEngine/"]
COPY ["SuperSocket/SuperSocket.Client/SuperSocket.Client.csproj", "SuperSocket/SuperSocket.Client/"]

COPY ["GameFrameX.Launcher/GameFrameX.Launcher.csproj", "GameFrameX.Launcher/"]
COPY ["GameFrameX.Apps/GameFrameX.Apps.csproj", "GameFrameX.Apps/"]
COPY ["GameFrameX.Core/GameFrameX.Core.csproj", "GameFrameX.Core/"]
COPY ["GameFrameX.DBServer/GameFrameX.DBServer.csproj", "GameFrameX.DBServer/"]
COPY ["GameFrameX.Serialize/GameFrameX.Serialize.csproj", "GameFrameX.Serialize/"]
COPY ["GameFrameX.ProtoBuf.Net/GameFrameX.ProtoBuf.Net.csproj", "GameFrameX.ProtoBuf.Net/"]
COPY ["GameFrameX.Utility/GameFrameX.Utility.csproj", "GameFrameX.Utility/"]
COPY ["GameFrameX.Extension/GameFrameX.Extension.csproj", "GameFrameX.Extension/"]
COPY ["GameFrameX.Log/GameFrameX.Log.csproj", "GameFrameX.Log/"]
COPY ["GameFrameX.Setting/GameFrameX.Setting.csproj", "GameFrameX.Setting/"]
COPY ["GameFrameX.NetWork.HTTP/GameFrameX.NetWork.HTTP.csproj", "GameFrameX.NetWork.HTTP/"]
COPY ["GameFrameX.NetWork/GameFrameX.NetWork.csproj", "GameFrameX.NetWork/"]
COPY ["GameFrameX.Monitor/GameFrameX.Monitor.csproj", "GameFrameX.Monitor/"]
COPY ["GameFrameX.Proto/GameFrameX.Proto.csproj", "GameFrameX.Proto/"]
COPY ["GameFrameX.Cache.Memory/GameFrameX.Cache.Memory.csproj", "GameFrameX.Cache.Memory/"]
COPY ["GameFrameX.Cache/GameFrameX.Cache.csproj", "GameFrameX.Cache/"]
COPY ["GameFrameX.Config/GameFrameX.Config.csproj", "GameFrameX.Config/"]
COPY ["GameFrameX.ServerManager/GameFrameX.ServerManager.csproj", "GameFrameX.ServerManager/"]

RUN dotnet restore "GameFrameX.Launcher/GameFrameX.Launcher.csproj"
COPY . .
WORKDIR "/src/GameFrameX.Launcher"
RUN dotnet build "GameFrameX.Launcher.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GameFrameX.Launcher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameFrameX.Launcher.dll"]
