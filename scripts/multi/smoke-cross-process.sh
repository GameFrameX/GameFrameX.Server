#!/usr/bin/env bash

set -euo pipefail

COMPOSE_FILE="${COMPOSE_FILE:-docker-compose.multi.yml}"
PROJECT_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"

cd "$PROJECT_ROOT"

echo "[1/4] 检查容器状态..."
docker compose -f "$COMPOSE_FILE" ps

call_test() {
  local url="$1"
  local label="$2"

  local response
  response="$(curl -sS -X POST "${url}" \
    -H 'Content-Type: application/json' \
    -d '{"mode":"cross-process-friend"}')"

  python3 - "$label" "$response" <<'PY'
import json
import sys

label = sys.argv[1]
raw = sys.argv[2]

try:
    outer = json.loads(raw)
except Exception as exc:
    print(f"[FAIL] {label}: 返回不是合法 JSON: {exc}")
    print(raw)
    sys.exit(1)

if outer.get("code") != 0:
    print(f"[FAIL] {label}: code != 0, body={raw}")
    sys.exit(1)

data = outer.get("data")
if isinstance(data, str):
    try:
        inner = json.loads(data)
    except Exception as exc:
        print(f"[FAIL] {label}: data 字段不是可解析 JSON 字符串: {exc}")
        print(raw)
        sys.exit(1)
else:
    inner = data or {}

friend_count = inner.get("FriendCount")
error_code = inner.get("ErrorCode")
mode = inner.get("Mode")

if mode != "cross-process-friend":
    print(f"[FAIL] {label}: Mode 不匹配, mode={mode}")
    sys.exit(1)

if error_code != 0:
    print(f"[FAIL] {label}: ErrorCode != 0, error_code={error_code}")
    sys.exit(1)

if not isinstance(friend_count, int) or friend_count < 1:
    print(f"[FAIL] {label}: FriendCount 非法, friend_count={friend_count}")
    sys.exit(1)

print(f"[OK] {label}: mode={mode}, error_code={error_code}, friend_count={friend_count}")
PY
}

echo "[2/4] 验证 game-1 -> social 跨进程调用..."
call_test "http://localhost:48080/game/api/Test" "game-1"

echo "[3/4] 验证 game-2 -> social 跨进程调用..."
call_test "http://localhost:48082/game/api/Test" "game-2"

echo "[4/4] 跨进程 smoke 测试通过。"
