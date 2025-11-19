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

namespace GameFrameX.Localization;

/// <summary>
/// 本地化键常量定义 - CodeGenerator 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// 代码生成器相关错误消息资源键
    /// </summary>
    public static class CodeGenerator
    {
        /// <summary>
        /// Agent生成器相关错误消息
        /// </summary>
        public static class AgentGenerator
        {
            /// <summary>
            /// 无法为标记【{0}】的函数指定超时时间
            /// </summary>
            /// <remarks>
            /// 键名: CodeGenerator.AgentGenerator.CannotSetTimeoutForThreadSafeAttribute
            /// 用途: 当尝试为线程安全函数指定超时时间时记录错误
            /// 参数: {0} - 线程安全属性名称
            /// </remarks>
            public const string CannotSetTimeoutForThreadSafeAttribute = "CodeGenerator.AgentGenerator.CannotSetTimeoutForThreadSafeAttribute";

            /// <summary>
            /// 【{0}】注解只能配合【{1}】或【{2}】使用
            /// </summary>
            /// <remarks>
            /// 键名: CodeGenerator.AgentGenerator.TimeoutAttributeMustBeUsedWithApiOrDiscard
            /// 用途: 当超时注解没有配合正确的注解使用时记录错误
            /// 参数: {0} - 超时属性名称, {1} - API属性名称, {2} - 丢弃属性名称
            /// </remarks>
            public const string TimeoutAttributeMustBeUsedWithApiOrDiscard = "CodeGenerator.AgentGenerator.TimeoutAttributeMustBeUsedWithApiOrDiscard";

            /// <summary>
            /// 非【{0}】的【{1}】接口只能是异步函数
            /// </summary>
            /// <remarks>
            /// 键名: CodeGenerator.AgentGenerator.NonThreadSafeApiMustBeAsync
            /// 用途: 当非线程安全的API接口不是异步函数时记录错误
            /// 参数: {0} - 线程安全属性名称, {1} - API属性名称
            /// </remarks>
            public const string NonThreadSafeApiMustBeAsync = "CodeGenerator.AgentGenerator.NonThreadSafeApiMustBeAsync";

            /// <summary>
            /// 标记了【{0}】【{1}】【{2}】注解的函数必须申明为virtual
            /// </summary>
            /// <remarks>
            /// 键名: CodeGenerator.AgentGenerator.MarkedFunctionMustBeVirtual
            /// 用途: 当标记了特定注解的函数不是virtual时记录错误
            /// 参数: {0} - 异步API属性名称, {1} - 线程安全属性名称, {2} - 丢弃属性名称
            /// </remarks>
            public const string MarkedFunctionMustBeVirtual = "CodeGenerator.AgentGenerator.MarkedFunctionMustBeVirtual";

            /// <summary>
            /// 只有返回值为Task类型或ValueTask类型才能添加【{0}】注解
            /// </summary>
            /// <remarks>
            /// 键名: CodeGenerator.AgentGenerator.DiscardAttributeRequiresTaskOrValueTask
            /// 用途: 当非Task/ValueTask返回类型的方法使用Discard注解时记录错误
            /// 参数: {0} - 丢弃属性名称
            /// </remarks>
            public const string DiscardAttributeRequiresTaskOrValueTask = "CodeGenerator.AgentGenerator.DiscardAttributeRequiresTaskOrValueTask";
        }
    }
}