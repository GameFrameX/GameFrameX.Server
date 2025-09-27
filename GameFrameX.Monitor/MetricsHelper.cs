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

using System.Diagnostics;
using Prometheus;

namespace GameFrameX.Monitor;

/// <summary>
/// 监控帮助类
/// </summary>
public static class MetricsHelper
{
    private static KestrelMetricServer _server;

    /// <summary>
    /// 停止监控
    /// </summary>
    public static void Stop()
    {
        _server.Stop();
    }

    /// <summary>
    /// 启动监控
    /// </summary>
    /// <param name="port">对外访问的端口</param>
    public static void Start(int port = 0)
    {
        if (port <= 0)
        {
            return;
        }

        Metrics.SuppressDefaultMetrics(new SuppressDefaultMetricOptions
        {
            SuppressEventCounters = true,
            SuppressMeters = true,
            SuppressProcessMetrics = true,
        });
        _server = new KestrelMetricServer(port);
        _server.Start();

        var totalSleepTime = Metrics.CreateCounter("sample_sleep_seconds_total", "Total amount of time spent sleeping.");

        _ = Task.Run(async delegate
        {
            while (true)
            {
                // DEFAULT EXEMPLAR: We expose the trace_id and span_id for distributed tracing, based on Activity.Current.
                // Activity.Current is often automatically inherited from incoming HTTP requests if using OpenTelemetry tracing with ASP.NET Core.
                // Here, we manually create and start an activity for sample purposes, without relying on the platform managing the activity context.
                // See https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts
                using var activity = new Activity("Pausing before record processing").Start();
                var sleepStopwatch = Stopwatch.StartNew();
                await Task.Delay(TimeSpan.FromSeconds(1));

                // The trace_id and span_id from the current Activity are exposed as the exemplar by default.
                totalSleepTime.Inc(sleepStopwatch.Elapsed.TotalSeconds);
            }
        });
        Console.WriteLine($"Open http://localhost:{port}/metrics?accept=application/openmetrics-text in a web browser.");
    }
}