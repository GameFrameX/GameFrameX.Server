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
/// 本地化键常量定义 - NetWorkHttp 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// NetWork.HTTP模块相关日志和错误消息资源键
    /// </summary>
    public static class NetWorkHttp
    {
        /// <summary>
        /// 参数重复了:{0}
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ParameterDuplicate
        /// 用途: 当HTTP请求中参数重复时记录错误
        /// 参数: {0} - 重复的参数名
        /// </remarks>
        public const string ParameterDuplicate = "NetWork.Http.ParameterDuplicate";

        /// <summary>
        /// 不支持的Content Type: {0}
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.UnsupportedContentType
        /// 用途: 当HTTP请求的Content-Type不支持时记录错误
        /// 参数: {0} - 不支持的Content-Type值
        /// </remarks>
        public const string UnsupportedContentType = "NetWork.Http.UnsupportedContentType";

        /// <summary>
        /// 请求参数:{0}
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.RequestParameters
        /// 用途: 记录HTTP请求的参数信息
        /// 参数: {0} - 序列化后的参数
        /// </remarks>
        public const string RequestParameters = "NetWork.Http.RequestParameters";

        /// <summary>
        /// 服务器状态错误[正在起/关服]
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ServerStatusError
        /// 用途: 当服务器状态不正常时返回错误
        /// </remarks>
        public const string ServerStatusError = "NetWork.Http.ServerStatusError";

        /// <summary>
        /// http cmd handler 不存在：{0}
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.CommandHandlerNotFound
        /// 用途: 当找不到对应的HTTP命令处理器时记录警告
        /// 参数: {0} - 命令名称
        /// </remarks>
        public const string CommandHandlerNotFound = "NetWork.Http.CommandHandlerNotFound";

        /// <summary>
        /// {0},执行时间：{1}ms, 结果: {2}
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ExecutionTime
        /// 用途: 记录HTTP请求的执行时间
        /// 参数: {0} - 日志头, {1} - 执行时间(毫秒), {2} - 结果
        /// </remarks>
        public const string ExecutionTime = "NetWork.Http.ExecutionTime";

        /// <summary>
        /// {0}, 发生异常. {1} {2}
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ExceptionOccurred
        /// 用途: 当HTTP处理过程中发生异常时记录错误
        /// 参数: {0} - 日志头, {1} - 异常消息, {2} - 堆栈跟踪
        /// </remarks>
        public const string ExceptionOccurred = "NetWork.Http.ExceptionOccurred";

        /// <summary>
        /// 消息类型 {0} 必须继承自 HttpMessageRequestBase
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.MessageTypeInheritanceError
        /// 用途: 当HTTP消息请求类型未继承自HttpMessageRequestBase时抛出异常
        /// 参数: {0} - 消息类型名称
        /// </remarks>
        public const string MessageTypeInheritanceError = "NetWork.Http.MessageTypeInheritanceError";

        /// <summary>
        /// 消息类型 {0} 必须继承自 HttpMessageResponseBase
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ResponseMessageTypeInheritanceError
        /// 用途: 当HTTP消息响应类型未继承自HttpMessageResponseBase时抛出异常
        /// 参数: {0} - 消息类型名称
        /// </remarks>
        public const string ResponseMessageTypeInheritanceError = "NetWork.Http.ResponseMessageTypeInheritanceError";

        /// <summary>
        /// {0} 必须是标记为sealed的类
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ClassMustBeSealed
        /// 用途: 当类不是sealed时抛出异常
        /// 参数: {0} - 类名
        /// </remarks>
        public const string ClassMustBeSealed = "NetWork.Http.ClassMustBeSealed";

        /// <summary>
        /// {0} 必须以{1}结尾
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ClassMustEndWithSuffix
        /// 用途: 当类名不以指定后缀结尾时抛出异常
        /// 参数: {0} - 类名, {1} - 要求的后缀
        /// </remarks>
        public const string ClassMustEndWithSuffix = "NetWork.Http.ClassMustEndWithSuffix";

        /// <summary>
        /// [HTTPServer] TraceIdentifier:[{0}], 来源[{1}], url:[{2}]
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.RequestLogHeader
        /// 用途: HTTP请求的日志头信息
        /// 参数: {0} - TraceIdentifier, {1} - IP地址, {2} - URL
        /// </remarks>
        public const string RequestLogHeader = "NetWork.Http.RequestLogHeader";

        /// <summary>
        /// {0}，请求方式:[{1}]
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.RequestMethod
        /// 用途: HTTP请求方法日志
        /// 参数: {0} - 日志头, {1} - 请求方法
        /// </remarks>
        public const string RequestMethod = "NetWork.Http.RequestMethod";

        /// <summary>
        /// 消息对象编码异常,请检查错误日志
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.MessageEncodingException
        /// 用途: 当消息对象编码异常时记录错误
        /// </remarks>
        public const string MessageEncodingException = "NetWork.Http.MessageEncodingException";

        /// <summary>
        /// http header content type is null
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.HttpHeaderContentTypeNull
        /// 用途: 当HTTP头的Content-Type为null时的错误消息
        /// </remarks>
        public const string HttpHeaderContentTypeNull = "NetWork.Http.HttpHeaderContentTypeNull";

        /// <summary>
        /// data verification failed
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.DataVerificationFailed
        /// 用途: 当数据验证失败时的错误消息
        /// </remarks>
        public const string DataVerificationFailed = "NetWork.Http.DataVerificationFailed";

        /// <summary>
        /// 请求参数
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.SwaggerRequestParameters
        /// 用途: Swagger文档中的请求参数描述
        /// </remarks>
        public const string SwaggerRequestParameters = "NetWork.Http.SwaggerRequestParameters";

        /// <summary>
        /// 响应状态码
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.SwaggerResponseStatusCode
        /// 用途: Swagger文档中的响应状态码描述
        /// </remarks>
        public const string SwaggerResponseStatusCode = "NetWork.Http.SwaggerResponseStatusCode";

        /// <summary>
        /// 响应消息
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.SwaggerResponseMessage
        /// 用途: Swagger文档中的响应消息描述
        /// </remarks>
        public const string SwaggerResponseMessage = "NetWork.Http.SwaggerResponseMessage";

        /// <summary>
        /// 成功响应
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.SwaggerSuccessResponse
        /// 用途: Swagger文档中的成功响应描述
        /// </remarks>
        public const string SwaggerSuccessResponse = "NetWork.Http.SwaggerSuccessResponse";

        /// <summary>
        /// application/json
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ApplicationJsonContentType
        /// 用途: application/json内容类型
        /// </remarks>
        public const string ApplicationJsonContentType = "NetWork.Http.ApplicationJsonContentType";

        /// <summary>
        /// application/x-protobuf
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ApplicationProtoBufContentType
        /// 用途: application/x-protobuf内容类型
        /// </remarks>
        public const string ApplicationProtoBufContentType = "NetWork.Http.ApplicationProtoBufContentType";

        /// <summary>
        /// http命令未包含验证参数{0} 和 {1}
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.HttpCommandMissingValidationParameters
        /// 用途: 当HTTP命令缺少验证参数时的错误消息
        /// 参数: {0} - 第一个参数名, {1} - 第二个参数名
        /// </remarks>
        public const string HttpCommandMissingValidationParameters = "NetWork.Http.HttpCommandMissingValidationParameters";

        /// <summary>
        /// undefined command
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.UndefinedCommand
        /// 用途: 使用未定义命令时的错误消息
        /// </remarks>
        public const string UndefinedCommand = "NetWork.Http.UndefinedCommand";

        /// <summary>
        /// not found command
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.NotFoundCommand
        /// 用途: 命令未找到时的错误消息
        /// </remarks>
        public const string NotFoundCommand = "NetWork.Http.NotFoundCommand";

        /// <summary>
        /// check failed command
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.CheckFailedCommand
        /// 用途: 命令验证失败时的错误消息
        /// </remarks>
        public const string CheckFailedCommand = "NetWork.Http.CheckFailedCommand";

        /// <summary>
        /// server error
        /// </summary>
        /// <remarks>
        /// 键名: NetWork.Http.ServerError
        /// 用途: 服务器内部错误时的错误消息
        /// </remarks>
        public const string ServerError = "NetWork.Http.ServerError";
    }
}