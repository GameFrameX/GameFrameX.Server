using System.Text;
using GameFrameX.Core.Actors.Impl;
using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;

namespace GameFrameX.Core.Utility;

/// <summary>
/// 统计工具
/// </summary>
public sealed class StatisticsTool
{
    private readonly Dictionary<string, Dictionary<string, int>> _countDic = new();

    private readonly WorkerActor _workerActor = new();

    /// <summary>
    /// 统计
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<string> CountRecord(int limit = 10)
    {
        return await _workerActor.SendAsync(() =>
        {
            var sb = new StringBuilder();
            foreach (var kv in _countDic)
            {
                var time = kv.Key;

                foreach (var item in kv.Value)
                {
                    var type = item.Key;
                    var count = item.Value;
                    if (count >= limit)
                    {
                        sb.Append('\t').Append(time).Append('\t').Append(count).Append('\t').Append(type).Append('\n');
                    }
                }
            }

            return sb.ToString();
        });
    }

    /// <summary>
    /// 清理统计
    /// </summary>
    public void ClearCount()
    {
        _workerActor.Tell(_countDic.Clear);
    }

    /// <summary>
    /// 清理统计
    /// </summary>
    /// <param name="time"></param>
    public void ClearCount(DateTime time)
    {
        _workerActor.Tell(() =>
        {
            var timeStr = TimeHelper.CurrentDateTimeWithUtcFormat();
            _countDic.RemoveIf((k, v) => string.Compare(k, timeStr, StringComparison.Ordinal) < 0);
        });
    }

    /// <summary>
    /// 记录统计
    /// </summary>
    /// <param name="key"></param>
    /// <param name="num"></param>
    public void Count(string key, int num = 1)
    {
        if (num <= 0)
        {
            return;
        }

        _workerActor.Tell(() =>
        {
            var timeStr = TimeHelper.CurrentDateTimeWithUtcFullString();
            if (!_countDic.TryGetValue(timeStr, out var cd))
            {
                cd = new Dictionary<string, int>();
                _countDic[timeStr] = cd;
            }

            var old = cd.GetValueOrDefault(key, 0);
            cd[key] = old + num;
        });
    }
}