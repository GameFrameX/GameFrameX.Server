FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .
RUN dotnet restore "GameFrameX.Launcher/GameFrameX.Launcher.csproj"
RUN dotnet restore "GameFrameX.Hotfix/GameFrameX.Hotfix.csproj"
WORKDIR "/src/GameFrameX.Launcher"
RUN dotnet build "GameFrameX.Launcher.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GameFrameX.Launcher.csproj" -f net10.0 -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 编译Hotfix程序集
WORKDIR "/src/GameFrameX.Hotfix"
RUN mkdir -p /app/build/hotfix
RUN dotnet build "GameFrameX.Hotfix.csproj" -c $BUILD_CONFIGURATION -o /app/build/hotfix /p:UseSharedCompilation=false

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
