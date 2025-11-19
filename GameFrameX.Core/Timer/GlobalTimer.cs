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

using GameFrameX.Core.Actors;
using GameFrameX.Core.Components;
using GameFrameX.DataBase;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Utility;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.Timer;

/// <summary>
/// 全局定时器
/// </summary>
public static class GlobalTimer
{
    /// <summary>
    /// 循环任务
    /// </summary>
    private static Task _loopTask;

    /// <summary>
    /// 是否正在工作
    /// </summary>
    public static volatile bool IsWorking;

    /// <summary>
    /// 开始全局定时
    /// </summary>
    public static void Start()
    {
        LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.Timer.GlobalTimerInitializationStart));
        IsWorking = true;
        _loopTask = Task.Run(Loop);
        LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.Timer.GlobalTimerInitializationComplete));
    }

    /// <summary>
    /// 循环执行的方法
    /// </summary>
    private static async Task Loop()
    {
        var nextSaveTime = NextSaveTime();
        var onceDelay = TimeSpan.FromSeconds(5);

        while (IsWorking)
        {
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.Timer.NextSaveTime, nextSaveTime));
            var currentTime = TimerHelper.UnixTimeMilliseconds();
            while (currentTime < nextSaveTime && IsWorking)
            {
                await Task.Delay(onceDelay);
                currentTime = TimerHelper.UnixTimeMilliseconds();
            }

            if (!IsWorking)
            {
                break;
            }

            var startTime = TimerHelper.UnixTimeMilliseconds();
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.Timer.SaveStart, startTime));
            await StateComponent.TimerSave();
            var endTime = TimerHelper.UnixTimeMilliseconds();
            var cost = endTime - startTime;
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.Timer.SaveEnd, endTime, cost));
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.Timer.ActorRecycleStart, startTime));
            await ActorManager.CheckIdle();
            currentTime = TimerHelper.UnixTimeMilliseconds();
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.Timer.ActorRecycleEnd, currentTime));
            do
            {
                nextSaveTime = NextSaveTime();
            } while (currentTime > nextSaveTime);
        }
    }

    /// <summary>
    /// 计算下次回存时间
    /// </summary>
    /// <returns>下次回存时间</returns>
    private static long NextSaveTime()
    {
        return TimerHelper.UnixTimeMilliseconds() + GlobalSettings.CurrentSetting.SaveDataInterval;
    }

    /// <summary>
    /// 停止全局定时
    /// </summary>
    public static async Task Stop()
    {
        LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.Timer.GlobalTimerStopStart));
        IsWorking = false;
        if (_loopTask != null)
        {
            await _loopTask;
        }
        await StateComponent.SaveAll(true);
        GameDb.Close();
        LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Core.Timer.GlobalTimerStopComplete));
    }
}