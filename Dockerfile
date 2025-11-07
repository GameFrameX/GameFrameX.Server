FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["README.md", "README.md"]
COPY ["GameFrameX.Launcher/GameFrameX.Launcher.csproj", "GameFrameX.Launcher/"]
COPY ["GameFrameX.Apps/GameFrameX.Apps.csproj", "GameFrameX.Apps/"]
COPY ["GameFrameX.Core/GameFrameX.Core.csproj", "GameFrameX.Core/"]
COPY ["GameFrameX.DataBase/GameFrameX.DataBase.csproj", "GameFrameX.DataBase/"]
COPY ["GameFrameX.DataBase.Mongo/GameFrameX.DataBase.Mongo.csproj", "GameFrameX.DataBase.Mongo/"]
COPY ["GameFrameX.Utility/GameFrameX.Utility.csproj", "GameFrameX.Utility/"]
COPY ["GameFrameX.NetWork.HTTP/GameFrameX.NetWork.HTTP.csproj", "GameFrameX.NetWork.HTTP/"]
COPY ["GameFrameX.NetWork.Abstractions/GameFrameX.NetWork.Abstractions.csproj", "GameFrameX.NetWork.Abstractions/"]
COPY ["GameFrameX.NetWork/GameFrameX.NetWork.csproj", "GameFrameX.NetWork/"]
COPY ["GameFrameX.ProtoBuf.Net/GameFrameX.ProtoBuf.Net.csproj", "GameFrameX.ProtoBuf.Net/"]
COPY ["GameFrameX.Monitor/GameFrameX.Monitor.csproj", "GameFrameX.Monitor/"]
COPY ["GameFrameX.Proto/GameFrameX.Proto.csproj", "GameFrameX.Proto/"]
COPY ["GameFrameX.Config/GameFrameX.Config.csproj", "GameFrameX.Config/"]
COPY ["GameFrameX.Core.Config/GameFrameX.Core.Config.csproj", "GameFrameX.Core.Config/"]
COPY ["GameFrameX.NetWork.Message/GameFrameX.NetWork.Message.csproj", "GameFrameX.NetWork.Message/"]
COPY ["GameFrameX.DiscoveryCenterManager/GameFrameX.DiscoveryCenterManager.csproj", "GameFrameX.DiscoveryCenterManager/"]
COPY ["GameFrameX.StartUp/GameFrameX.StartUp.csproj", "GameFrameX.StartUp/"]
COPY ["GameFrameX.Hotfix/GameFrameX.Hotfix.csproj", "GameFrameX.Hotfix/"]

RUN dotnet restore "GameFrameX.Launcher/GameFrameX.Launcher.csproj"
RUN dotnet restore "GameFrameX.Hotfix/GameFrameX.Hotfix.csproj"

COPY . .
WORKDIR "/src/GameFrameX.Launcher"
RUN dotnet build "GameFrameX.Launcher.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GameFrameX.Launcher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 编译Hotfix程序集
WORKDIR "/src/GameFrameX.Hotfix"
RUN mkdir -p /app/build/hotfix
RUN dotnet build "GameFrameX.Hotfix.csproj" -c $BUILD_CONFIGURATION -o /app/build/hotfix

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 复制json配置文件
COPY GameFrameX.Config/json ./json

# 复制Hotfix编译结果到发布目录
RUN mkdir -p /app/hotfix
COPY --from=publish /app/build/hotfix/GameFrameX.Hotfix.dll /app/hotfix/

# 切换到root用户创建数据目录并设置权限
USER root
RUN mkdir -p /app/data && chmod 755 /app/data

# 复制发布文件
COPY --from=publish /app/publish .

# 声明数据卷以实现数据持久化
VOLUME ["/app/data"]

# 切换回非root用户运行应用
USER $APP_UID


ENTRYPOINT ["dotnet", "GameFrameX.Launcher.dll"]
