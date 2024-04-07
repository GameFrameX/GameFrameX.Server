FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
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
COPY ["GameFrameX.Proto/GameFrameX.Proto.csproj", "GameFrameX.Proto/"]
COPY ["GameFrameX.Cache.Memory/GameFrameX.Cache.Memory.csproj", "GameFrameX.Cache.Memory/"]
COPY ["GameFrameX.Cache/GameFrameX.Cache.csproj", "GameFrameX.Cache/"]
COPY ["GameFrameX.Config/GameFrameX.Config.csproj", "GameFrameX.Config/"]
COPY ["GameFrameX.EntryUtility/GameFrameX.EntryUtility.csproj", "GameFrameX.EntryUtility/"]
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
