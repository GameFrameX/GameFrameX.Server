#!/usr/bin/env bash

set -u

ROOT_DIR="/Users/blank/Documents/GithubWorks/GameFrameX/Server"
TEST_PROJECT="Tests/GameFrameX.Tests/GameFrameX.Tests.csproj"
SOLUTION="Server.sln"
REMOTE_FILTER="FullyQualifiedName~RemoteMessaging"

MODE="fast"
if [[ "${1:-}" == "--fast" ]]; then
  MODE="fast"
fi
if [[ "${1:-}" == "--full" ]]; then
  MODE="full"
fi

if [[ "${1:-}" == "--help" || "${1:-}" == "-h" ]]; then
  cat <<'EOF'
用法:
  scripts/acceptance.sh [--fast|--full]

说明:
  默认    等同 --fast
  --fast  只跑必过门禁（build/test/socket扫描/Friend语义/回滚）
  --full  跑完整验收（在 fast 基础上增加重试开关与故障注入演练）
EOF
  exit 0
fi

if [[ ! -d "$ROOT_DIR" ]]; then
  echo "ERROR: 根目录不存在: $ROOT_DIR"
  exit 1
fi

cd "$ROOT_DIR" || exit 1

declare -a PASSED=()
declare -a FAILED=()

record_pass() {
  PASSED+=("$1")
  echo "PASS: $1"
}

record_fail() {
  FAILED+=("$1")
  echo "FAIL: $1"
}

run_cmd() {
  local name="$1"
  local cmd="$2"
  echo ""
  echo ">>> $name"
  echo "$cmd"
  if bash -lc "$cmd"; then
    record_pass "$name"
    return 0
  fi
  record_fail "$name"
  return 1
}

run_check_no_match() {
  local name="$1"
  local cmd="$2"
  echo ""
  echo ">>> $name"
  echo "$cmd"
  local output
  output="$(bash -lc "$cmd")"
  local code=$?
  if [[ $code -eq 1 && -z "$output" ]]; then
    record_pass "$name"
    return 0
  fi
  if [[ $code -eq 0 && -n "$output" ]]; then
    echo "$output"
    record_fail "$name (发现命中)"
    return 1
  fi
  if [[ $code -eq 0 && -z "$output" ]]; then
    record_pass "$name"
    return 0
  fi
  echo "$output"
  record_fail "$name (命令执行失败)"
  return 1
}

run_check_contains() {
  local name="$1"
  local cmd="$2"
  local expected1="$3"
  local expected2="$4"
  echo ""
  echo ">>> $name"
  echo "$cmd"
  local output
  output="$(bash -lc "$cmd")"
  local code=$?
  echo "$output"
  if [[ $code -ne 0 ]]; then
    record_fail "$name (命令执行失败)"
    return 1
  fi
  if [[ "$output" == *"$expected1"* && "$output" == *"$expected2"* ]]; then
    record_pass "$name"
    return 0
  fi
  record_fail "$name (未命中预期语义)"
  return 1
}

echo "========================================"
echo "GameFrameX 验收脚本"
echo "模式: $MODE"
echo "根目录: $ROOT_DIR"
echo "========================================"

run_cmd "Restore" "dotnet restore $SOLUTION"
run_cmd "Build" "dotnet build $SOLUTION"
run_cmd "Test(All)" "dotnet test $SOLUTION"

run_check_no_match \
  "Logic 禁止直连 Socket 检查" \
  "rg -n 'new\\s+TcpClient\\(|NetworkStream|Socket\\(' GameFrameX.Hotfix/Logic"

run_check_contains \
  "Friend 重试语义检查" \
  "rg -n 'AllowRetry|CallWithResultAsync' GameFrameX.Hotfix/Logic/Player/Friend/FriendComponentAgent.cs" \
  "AllowRetry = true" \
  "AllowRetry = false"

run_cmd \
  "回滚开关验证 EnableUnifiedClient=false" \
  "RemoteMessaging__EnableUnifiedClient=false dotnet test $TEST_PROJECT --filter \"$REMOTE_FILTER\""

if [[ "$MODE" == "full" ]]; then
  run_cmd \
    "重试开关验证 EnableRetry=false" \
    "RemoteMessaging__EnableRetry=false dotnet test $TEST_PROJECT --filter \"$REMOTE_FILTER\""

  run_cmd \
    "故障注入 Timeout 演练" \
    "RemoteMessaging__EnableFaultInjection=true RemoteMessaging__FaultInjection__Type=Timeout dotnet test $TEST_PROJECT --filter \"$REMOTE_FILTER\""

  run_cmd \
    "故障注入 ConnectionDrop 演练" \
    "RemoteMessaging__EnableFaultInjection=true RemoteMessaging__FaultInjection__Type=ConnectionDrop dotnet test $TEST_PROJECT --filter \"$REMOTE_FILTER\""

  run_cmd \
    "故障注入 SlowResponse 演练" \
    "RemoteMessaging__EnableFaultInjection=true RemoteMessaging__FaultInjection__Type=SlowResponse RemoteMessaging__FaultInjection__DelayMs=2000 dotnet test $TEST_PROJECT --filter \"$REMOTE_FILTER\""
fi

echo ""
echo "========================================"
echo "验收汇总"
echo "通过: ${#PASSED[@]}"
echo "失败: ${#FAILED[@]}"
echo "----------------------------------------"
for item in "${PASSED[@]-}"; do
  echo "PASS - $item"
done
if [[ ${#FAILED[@]} -gt 0 ]]; then
  for item in "${FAILED[@]-}"; do
    echo "FAIL - $item"
  done
fi
echo "========================================"

if [[ ${#FAILED[@]} -gt 0 ]]; then
  exit 1
fi

exit 0
