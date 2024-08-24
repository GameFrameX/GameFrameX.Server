using Microsoft.CodeAnalysis;

namespace GameFrameX.CodeGenerator.Utils;

public static class Logger
{
    public static void LogError(this GeneratorExecutionContext context, string msg)
    {
        var invalidXmlWarning = new DiagnosticDescriptor("Error",
                                                         "Code Generator Error",
                                                         "{0}",
                                                         "CodeGenerator",
                                                         DiagnosticSeverity.Error,
                                                         true);
        context.ReportDiagnostic(Diagnostic.Create(invalidXmlWarning, Location.None, msg));
    }
}