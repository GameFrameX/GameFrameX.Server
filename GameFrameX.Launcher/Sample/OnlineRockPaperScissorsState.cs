//  ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and related international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please see the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository: https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository: https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System.Collections.Generic;

namespace GameFrameX.Capability.Online.Match;

/// <summary>
/// 石头剪刀布玩法状态（序列化后存于 <see cref="OnlineMatch.GameState"/>）。
/// <para>
/// 维护约束（红线）：<see cref="OnlineRockPaperScissorsPlayerState.Gesture"/> 持有**未结算**的出拳，
/// 因此本状态的序列化结果属于服务端内部数据——不得原样下发给客户端（否则对手可在结算前窥屏）。
/// 客户端可见的出拳只出现在已结算的 <c>RoundResolved</c> 事件载荷中（未结算前对外为 None）。
/// </para>
/// </summary>
public sealed class OnlineRockPaperScissorsState
{
    /// <summary>
    /// 获取或设置当前局数（从 1 开始）。
    /// </summary>
    public int Round { get; set; }

    /// <summary>
    /// 获取或设置上一局胜者（平局为 0）。
    /// </summary>
    public long WinnerPlayerId { get; set; }

    /// <summary>
    /// 获取或设置本局截止时刻（UTC 毫秒；0 = 未布防，由 <see cref="IOnlineMatchGame.Advance"/> 布防）。
    /// </summary>
    public long RoundDeadlineTime { get; set; }

    /// <summary>
    /// 获取或设置玩家状态集合。
    /// </summary>
    public List<OnlineRockPaperScissorsPlayerState> Players { get; set; }

    /// <summary>
    /// 单玩家玩法状态。
    /// </summary>
    public sealed class OnlineRockPaperScissorsPlayerState
    {
        /// <summary>
        /// 获取或设置玩家标识。
        /// </summary>
        public long PlayerId { get; set; }

        /// <summary>
        /// 获取或设置已胜局数。
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// 获取或设置本局出拳（<see cref="OnlineRockPaperScissorsMove.None"/> = 未出拳）。
        /// </summary>
        public OnlineRockPaperScissorsMove Gesture { get; set; }

        /// <summary>
        /// 获取或设置累计有效出拳次数（跨局累计；参与奖资格判定依据——任意玩家为 0 时结算抑制全部参与奖）。
        /// <para>唯一写入点是 <see cref="OnlineRockPaperScissorsGame.ApplyInput"/> 接受合法出拳；每局结算不清零。</para>
        /// </summary>
        public int ValidGestureCount { get; set; }
    }
}