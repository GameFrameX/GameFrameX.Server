using System.Collections.Generic;
using System.Text;

namespace GameFrameX.CodeGenerator.Agent;

public class MthInfo
{
    public string Name { get; set; }

    public string ReturnType { get; set; }

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

    public bool IsApi { get; set; }

    public string Modify { get; set; }

    public bool IsPublic { get; set; }

    public bool IsStatic { get; set; }

    public bool IsVirtual { get; set; }

    public bool Isasync { get; set; }

    public List<string> Params { get; } = new();

    public List<string> AttributeList { get; private set; } = new();

    public bool Discard { get; set; }

    public bool HasTimeout { get; set; }
    public int Timeout { get; set; } = int.MaxValue;

    public bool Threadsafe { get; set; }

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

    public string Paramstr
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
    public string Space { get; set; }
    public string Partial { get; set; } = "";
    public string Name { get; set; }
    public string Super { get; set; }
    public List<MthInfo> Methods { get; set; } = new();
    public List<string> Usingspaces { get; set; } = new();
}