namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 组件接口
/// </summary>
public interface IComponent
{
    /// <summary>
    /// Actor对象
    /// </summary>
    IActor Actor { get; set; }
}