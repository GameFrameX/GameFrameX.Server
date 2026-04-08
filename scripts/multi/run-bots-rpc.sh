#!/usr/bin/env bash

set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$PROJECT_ROOT"

BOT_COUNT="${BOT_COUNT:-100}"
TCP_HOST="${TCP_HOST:-127.0.0.1}"
TCP_PORT="${TCP_PORT:-49100}"
LOGIN_URL="${LOGIN_URL:-http://127.0.0.1:48080/game/api/}"
DISCONNECT_LOOP="${DISCONNECT_LOOP:-true}"
DISCONNECT_AFTER_LOGIN_SECONDS="${DISCONNECT_AFTER_LOGIN_SECONDS:-15}"
RUN_SECONDS="${RUN_SECONDS:-180}"
CONNECT_STAGGER_MS="${CONNECT_STAGGER_MS:-10}"

echo "启动机器人压测:"
echo "  BOT_COUNT=$BOT_COUNT"
echo "  TCP=$TCP_HOST:$TCP_PORT"
echo "  LOGIN_URL=$LOGIN_URL"
echo "  DISCONNECT_LOOP=$DISCONNECT_LOOP"
echo "  DISCONNECT_AFTER_LOGIN_SECONDS=$DISCONNECT_AFTER_LOGIN_SECONDS"
echo "  RUN_SECONDS=$RUN_SECONDS"
echo "  CONNECT_STAGGER_MS=$CONNECT_STAGGER_MS"

dotnet run --framework net10.0 --project GameFrameX.Client/GameFrameX.Client.csproj -- \
  --bot-count="$BOT_COUNT" \
  --tcp-host="$TCP_HOST" \
  --tcp-port="$TCP_PORT" \
  --login-url="$LOGIN_URL" \
  --disconnect-loop="$DISCONNECT_LOOP" \
  --disconnect-after-login-seconds="$DISCONNECT_AFTER_LOGIN_SECONDS" \
  --run-seconds="$RUN_SECONDS" \
  --connect-stagger-ms="$CONNECT_STAGGER_MS"
