using System.Text;

namespace GameFrameX.CodeGenerator.Agent;

public static class AgentTemplate
{
    private static readonly StringBuilder TemplateStringBuilder = new StringBuilder();

    public static string Run(AgentInfo info)
    {
        TemplateStringBuilder.Clear();

        foreach (var value in info.UsingSpaces)
        {
            TemplateStringBuilder.AppendLine("using " + value + ";");
        }

        TemplateStringBuilder.AppendLine();

        TemplateStringBuilder.AppendLine("namespace " + info.Space);
        TemplateStringBuilder.AppendLine("{");
        TemplateStringBuilder.AppendLine("\tpublic partial class " + info.Name + " : " + info.Super);
        TemplateStringBuilder.AppendLine("\t{");
        foreach (var infoMethod in info.Methods)
        {
            TemplateStringBuilder.AppendLine("\t\t" + infoMethod.Declare);
            TemplateStringBuilder.AppendLine("\t\t{");

            if (infoMethod.Discard)
            {
                if (infoMethod.IsThreadSafe)
                {
                    TemplateStringBuilder.AppendLine($"\t\t\t_ = base.{infoMethod.Name}{infoMethod.Typeparams}({infoMethod.ParamString});");
                }
                else
                {
                    TemplateStringBuilder.AppendLine("\t\t\tlong callChainId = GameFrameX.Core.Actors.Impl.WorkerActor.NextChainId();");
                    TemplateStringBuilder.AppendLine($"\t\t\t_ = base.Actor.WorkerActor.Enqueue(()=>base.{infoMethod.Name}{infoMethod.Typeparams}({infoMethod.ParamString}), callChainId, true, {infoMethod.TimeOut});");
                }

                TemplateStringBuilder.AppendLine($"\t\t\treturn {infoMethod.ReturnType}.CompletedTask;");
            }
            else
            {
                TemplateStringBuilder.AppendLine("\t\t\t(bool needEnqueue, long chainId)= base.Actor.WorkerActor.IsNeedEnqueue();");
                TemplateStringBuilder.AppendLine($"\t\t\tif (!needEnqueue)");
                TemplateStringBuilder.AppendLine("\t\t\t{");
                TemplateStringBuilder.AppendLine($"\t\t\t\treturn {(infoMethod.IsAsync ? "await " : string.Empty)} base.{infoMethod.Name}{infoMethod.Typeparams}({infoMethod.ParamString});");
                TemplateStringBuilder.AppendLine("\t\t\t}");
                TemplateStringBuilder.AppendLine($"\t\t\treturn {(infoMethod.IsAsync ? "await " : string.Empty)} base.Actor.WorkerActor.Enqueue(()=>base.{infoMethod.Name}{infoMethod.Typeparams}({infoMethod.ParamString}), chainId, {(infoMethod.Discard ? "true" : "false")}, {infoMethod.TimeOut});");
            }

            TemplateStringBuilder.AppendLine("\t\t}");
            TemplateStringBuilder.AppendLine();
        }

        TemplateStringBuilder.AppendLine("\t}");
        TemplateStringBuilder.AppendLine("}");

        return TemplateStringBuilder.ToString();
    }
}