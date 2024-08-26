using System.Collections.Generic;
using System.Text;

namespace GameFrameX.CodeGenerator.Agent;

public class MthInfo
{
    /// <summary>
    ///     方法名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     返回类型
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
    ///     是否是Api
    /// </summary>
    public bool IsApi { get; set; }

    /// <summary>
    ///     是否修改
    /// </summary>
    public string Modify { get; set; }

    /// <summary>
    ///     是否公开
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    ///     是否静态
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    ///     是否虚方法
    /// </summary>
    public bool IsVirtual { get; set; }

    /// <summary>
    ///     是否异步
    /// </summary>
    public bool IsAsync { get; set; }

    public List<string> Params { get; } = new();

    /// <summary>
    /// 属性标记列表
    /// </summary>
    public List<string> AttributeList { get; private set; } = new();

    public bool Discard { get; set; }

    /// <summary>
    ///     是否有超时
    /// </summary>
    public bool HasTimeout { get; set; }

    /// <summary>
    ///     超时时间
    /// </summary>
    public int TimeOut { get; set; } = int.MaxValue;

    /// <summary>
    ///     是否线程安全
    /// </summary>
    public bool IsThreadSafe { get; set; }

    /// <summary>
    ///     约束
    /// </summary>
    public string Constraint { get; set; }

    /// <summary>
    ///     泛型参数
    /// </summary>
    public string Typeparams { get; set; }

    /// <summary>
    ///     参数声明
    /// </summary>
    public string ParamDeclare { get; set; }

    /// <summary>
    ///     参数字符串
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
    ///     命名空间
    /// </summary>
    public string Space { get; set; }

    /// <summary>
    /// 分部类
    /// </summary>
    public string Partial { get; set; } = "";

    /// <summary>
    ///     类名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     父类
    /// </summary>
    public string Super { get; set; }

    /// <summary>
    ///     方法列表
    /// </summary>
    public List<MthInfo> Methods { get; set; } = new();

    /// <summary>
    ///     用到的命名空间
    /// </summary>
    public List<string> UsingSpaces { get; set; } = new();
}