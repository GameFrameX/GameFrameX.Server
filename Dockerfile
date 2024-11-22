FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["README.md", "README.md"]
COPY ["GameFrameX.Launcher/GameFrameX.Launcher.csproj", "GameFrameX.Launcher/"]
COPY ["GameFrameX.Apps/GameFrameX.Apps.csproj", "GameFrameX.Apps/"]
COPY ["GameFrameX.Proto/GameFrameX.Proto.csproj", "GameFrameX.Proto/"]
COPY ["GameFrameX.Core/GameFrameX.Core.csproj", "GameFrameX.Core/"]
COPY ["DataBaseServer/GameFrameX.DataBase.Mongo/GameFrameX.DataBase.Mongo.csproj", "DataBaseServer/GameFrameX.DataBase.Mongo/"]
COPY ["DataBaseServer/GameFrameX.DataBase/GameFrameX.DataBase.csproj", "DataBaseServer/GameFrameX.DataBase/"]
COPY ["GameFrameX.Setting/GameFrameX.Setting.csproj", "GameFrameX.Setting/"]
COPY ["GameFrameX.Utility/GameFrameX.Utility.csproj", "GameFrameX.Utility/"]
COPY ["GameFrameX.Extension/GameFrameX.Extension.csproj", "GameFrameX.Extension/"]
COPY ["GameFrameX.Log/GameFrameX.Log.csproj", "GameFrameX.Log/"]
COPY ["DataBaseServer/GameFrameX.DataBase.Abstractions/GameFrameX.DataBase.Abstractions.csproj", "DataBaseServer/GameFrameX.DataBase.Abstractions/"]
COPY ["GameFrameX.Core.Abstractions/GameFrameX.Core.Abstractions.csproj", "GameFrameX.Core.Abstractions/"]
COPY ["GameFrameX.NetWork.HTTP/GameFrameX.NetWork.HTTP.csproj", "GameFrameX.NetWork.HTTP/"]
COPY ["GameFrameX.NetWork/GameFrameX.NetWork.csproj", "GameFrameX.NetWork/"]
COPY ["GameFrameX.NetWork.Abstractions/GameFrameX.NetWork.Abstractions.csproj", "GameFrameX.NetWork.Abstractions/"]
COPY ["GameFrameX.ProtoBuf.Net/GameFrameX.ProtoBuf.Net.csproj", "GameFrameX.ProtoBuf.Net/"]
COPY ["GameFrameX.Monitor/GameFrameX.Monitor.csproj", "GameFrameX.Monitor/"]
COPY ["GameFrameX.Config/GameFrameX.Config.csproj", "GameFrameX.Config/"]
COPY ["GameFrameX.Core.Config/GameFrameX.Core.Config.csproj", "GameFrameX.Core.Config/"]
COPY ["GameFrameX.GameAnalytics/GameFrameX.GameAnalytics.csproj", "GameFrameX.GameAnalytics/"]
COPY ["GameFrameX.NetWork.ChannelBase/GameFrameX.NetWork.ChannelBase.csproj", "GameFrameX.NetWork.ChannelBase/"]
COPY ["GameFrameX.NetWork.Message/GameFrameX.NetWork.Message.csproj", "GameFrameX.NetWork.Message/"]
COPY ["GameFrameX.Proto.BuiltIn/GameFrameX.Proto.BuiltIn.csproj", "GameFrameX.Proto.BuiltIn/"]
COPY ["GameFrameX.ServerManager/GameFrameX.ServerManager.csproj", "GameFrameX.ServerManager/"]
COPY ["GameFrameX.StartUp/GameFrameX.StartUp.csproj", "GameFrameX.StartUp/"]
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
