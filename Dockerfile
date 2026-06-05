FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["GameFrameX.Launcher/GameFrameX.Launcher.csproj", "GameFrameX.Launcher/"]
COPY ["GameFrameX.Config/GameFrameX.Config.csproj", "GameFrameX.Config/"]
COPY ["GameFrameX.Apps/GameFrameX.Apps.csproj", "GameFrameX.Apps/"]
COPY ["GameFrameX.Proto/GameFrameX.Proto.csproj", "GameFrameX.Proto/"]
COPY ["GameFrameX.Hotfix/GameFrameX.Hotfix.csproj", "GameFrameX.Hotfix/"]
COPY ["GameFrameX.CodeGenerator/GameFrameX.CodeGenerator.csproj", "GameFrameX.CodeGenerator/"]

RUN dotnet restore "GameFrameX.Launcher/GameFrameX.Launcher.csproj"
COPY . .
WORKDIR "/src/GameFrameX.Launcher"
RUN dotnet build "GameFrameX.Launcher.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GameFrameX.Launcher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish "../GameFrameX.Hotfix/GameFrameX.Hotfix.csproj" -c $BUILD_CONFIGURATION -o /app/hotfix /p:UseAppHost=false

FROM base AS final
USER root
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/hotfix /app/hotfix-default

RUN mkdir -p /app/hotfix && \
    printf '#!/bin/sh\n\
if [ -z "$(ls -A /app/hotfix 2>/dev/null)" ]; then\n\
    echo "hotfix directory is empty, copying defaults..."\n\
    cp -r /app/hotfix-default/* /app/hotfix/\n\
fi\n\
exec "$@"' > /app/entrypoint.sh && \
    chmod +x /app/entrypoint.sh

USER $APP_UID
ENTRYPOINT ["/app/entrypoint.sh", "dotnet", "GameFrameX.Launcher.dll"]
