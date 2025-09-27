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



using GameFrameX.ServerManager;

namespace GameFrameX.Hotfix.Logic.Http.DiscoveryCenter;

/// <summary>
/// 根据服务器类型获取节点列表的HTTP处理器
/// </summary>
[HttpMessageMapping(typeof(GetNodesByTypeHttpHandler))]
[HttpMessageResponse(typeof(GetNodesByTypeResponse))]
[HttpMessageRequest(typeof(GetNodesByTypeRequest))]
public sealed class GetNodesByTypeHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 处理根据服务器类型获取节点列表的HTTP请求
    /// 该方法通过 <see cref="NamingServiceManager.GetNodesByType(ServerType)"/> 方法获取指定类型的所有服务节点。
    /// </summary>
    /// <param name="ip">客户端IP地址</param>
    /// <param name="url">请求的URL地址</param>
    /// <param name="request">HTTP请求对象</param>
    /// <returns>包含节点列表的JSON响应字符串</returns>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        // 将请求对象转换为具体的请求类型
        if (request is not GetNodesByTypeRequest typeRequest)
        {
            var errorResponse = new GetNodesByTypeResponse
            {
                Success = false,
                Message = "无效的请求类型",
                ServerType = ServerType.None,
                Nodes = new List<IServiceInfo>(),
                Count = 0
            };
            return Task.FromResult(HttpJsonResult.SuccessString(errorResponse));
        }
        
        var serverType = typeRequest.ServerType;
        
        // 验证ServerType参数
        if (serverType == ServerType.None)
        {
            var errorResponse = new GetNodesByTypeResponse
            {
                Success = false,
                Message = "无效的ServerType参数",
                ServerType = ServerType.None,
                Nodes = new List<IServiceInfo>(),
                Count = 0
            };
            return Task.FromResult(HttpJsonResult.SuccessString(errorResponse));
        }
        
        var namingServiceManager = NamingServiceManager.Instance;
        var nodes = namingServiceManager.GetNodesByType(serverType);

        var response = new GetNodesByTypeResponse
        {
            Success = true,
            Message = $"成功获取服务器类型 {serverType} 的节点列表",
            ServerType = serverType,
            Nodes = nodes,
            Count = nodes.Count
        };
        
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

/// <summary>
/// 根据服务器类型获取节点列表的请求对象
/// </summary>
public class GetNodesByTypeRequest : HttpMessageRequestBase
{
    /// <summary>
    /// 服务器类型
    /// </summary>
    /// <value>要查询的服务器类型枚举值</value>
    public ServerType ServerType { get; set; }
}

/// <summary>
/// 根据服务器类型获取节点列表的响应对象
/// </summary>
public class GetNodesByTypeResponse : HttpMessageResponseBase
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    /// <value>表示操作是否成功执行的布尔值</value>
    public bool Success { get; set; }
    
    /// <summary>
    /// 响应消息
    /// </summary>
    /// <value>描述操作结果的消息字符串</value>
    public string Message { get; set; }
    
    /// <summary>
    /// 查询的服务器类型
    /// </summary>
    /// <value>请求中指定的服务器类型</value>
    public ServerType ServerType { get; set; }
    
    /// <summary>
    /// 指定类型的节点列表
    /// </summary>
    /// <value>包含所有指定类型服务节点信息的 <see cref="List{T}"/> 其中 T 为 <see cref="IServiceInfo"/></value>
    public List<IServiceInfo> Nodes { get; set; } = new();
    
    /// <summary>
    /// 节点总数
    /// </summary>
    /// <value>指定类型节点的总数量</value>
    public int Count { get; set; }
}