// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Text;
using GameFrameX.Core.Actors.Impl;
using GameFrameX.Utility;
using GameFrameX.Foundation.Extensions;

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