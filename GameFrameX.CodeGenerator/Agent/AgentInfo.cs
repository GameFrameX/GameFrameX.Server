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

using System.Collections.Generic;
using System.Text;

namespace GameFrameX.CodeGenerator.Agent;

public class MthInfo
{
    /// <summary>
    /// 方法名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 返回类型
    /// </summary>
    public string ReturnType { get; set; }


    /// <summary>
    /// 函数签名
    /// </summary>
    public string Declare
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(Modify);
            sb.Append(ReturnType);
            sb.Append(" ");
            sb.Append(Name);
            sb.Append(Typeparams);
            sb.Append(ParamDeclare);
            //sb.Append(" ");
            //sb.Append(Constraint);
            return sb.ToString();
        }
    }

    /// <summary>
    /// 是否是Api
    /// </summary>
    public bool IsApi { get; set; }

    /// <summary>
    /// 是否修改
    /// </summary>
    public string Modify { get; set; }

    /// <summary>
    /// 是否公开
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 是否静态
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    /// 是否虚方法
    /// </summary>
    public bool IsVirtual { get; set; }

    /// <summary>
    /// 是否异步
    /// </summary>
    public bool IsAsync { get; set; }

    public List<string> Params { get; } = new();

    /// <summary>
    /// 属性标记列表
    /// </summary>
    public List<string> AttributeList { get; private set; } = new();

    public bool Discard { get; set; }

    /// <summary>
    /// 是否有超时
    /// </summary>
    public bool HasTimeout { get; set; }

    /// <summary>
    /// 超时时间
    /// </summary>
    public int TimeOut { get; set; } = int.MaxValue;

    /// <summary>
    /// 是否线程安全
    /// </summary>
    public bool IsThreadSafe { get; set; }

    /// <summary>
    /// 约束
    /// </summary>
    public string Constraint { get; set; }

    /// <summary>
    /// 泛型参数
    /// </summary>
    public string Typeparams { get; set; }

    /// <summary>
    /// 参数声明
    /// </summary>
    public string ParamDeclare { get; set; }

    /// <summary>
    /// 参数字符串
    /// </summary>
    public string ParamString
    {
        get
        {
            if (Params.Count > 0)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < Params.Count; i++)
                {
                    sb.Append(Params[i]);
                    if (i != Params.Count - 1)
                    {
                        sb.Append(",");
                    }
                }

                return sb.ToString();
            }

            return "";
        }
    }
}

public class AgentInfo
{
    /// <summary>
    /// 命名空间
    /// </summary>
    public string Space { get; set; }

    /// <summary>
    /// 分部类
    /// </summary>
    public string Partial { get; set; } = "";

    /// <summary>
    /// 类名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 父类
    /// </summary>
    public string Super { get; set; }

    /// <summary>
    /// 方法列表
    /// </summary>
    public List<MthInfo> Methods { get; set; } = new();

    /// <summary>
    /// 用到的命名空间
    /// </summary>
    public List<string> UsingSpaces { get; set; } = new();
}