// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace GameFrameX.Apps;

/// <summary>
/// 每个服存在多个实例的（如玩家和公会）需要小于Separator
/// 最大id应当小于999
/// Id一旦定义了不应该修改
/// </summary>
public enum ActorType : ushort
{
    /// <summary>
    /// 空将会被判断为无效值
    /// </summary>
    None,

    /// <summary>
    /// 分割线(勿调整,勿用于业务逻辑)
    /// </summary>
    Separator = GlobalConst.ActorTypeSeparator,

    /// <summary>
    /// 账号
    /// 管理玩家账号信息，如注册、密码找回等。
    /// </summary>
    Account = 130,

    /// <summary>
    /// 最大值
    /// </summary>
    Max = GlobalConst.ActorTypeMax,
}