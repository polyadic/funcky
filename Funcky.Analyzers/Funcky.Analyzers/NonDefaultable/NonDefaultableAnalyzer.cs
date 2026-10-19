using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.NonDefaultable;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NonDefaultableAnalyzer : DiagnosticAnalyzer
{
    public const string DoNotUseDefaultId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}09";

    public static readonly DiagnosticDescriptor DoNotUseDefault = new(
        id: DoNotUseDefaultId,
        title: "Do not use default to instantiate this type",
        messageFormat: "Do not use default(...) to instantiate '{0}'",
        category: nameof(Funcky),
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Values instantiated with default are in an invalid state; any member may throw an exception.",
        helpLinkUri: DiagnosticName.HelpLink(DoNotUseDefaultId));

    /// <summary>The attribute is matched by name, so any assembly may define it: Funcky, the user's own project or the generator.</summary>
    internal const string AttributeFullName = "Funcky.CodeAnalysis.NonDefaultableAttribute";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DoNotUseDefault);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeDefaultValueOperation, OperationKind.DefaultValue);
        context.RegisterOperationAction(AnalyzeObjectCreationOperation, OperationKind.ObjectCreation);
    }

    internal static bool IsParameterlessObjectCreationOfNonDefaultableStruct(IObjectCreationOperation operation)
        => operation is { Type: { } type, Arguments.Length: 0, Initializer: null }
            && type.HasAttribute(AttributeFullName);

    private static void AnalyzeDefaultValueOperation(OperationAnalysisContext context)
    {
        var operation = (IDefaultValueOperation)context.Operation;
        if (operation.Type is { } type && type.HasAttribute(AttributeFullName))
        {
            ReportDiagnostic(context);
        }
    }

    private static void AnalyzeObjectCreationOperation(OperationAnalysisContext context)
    {
        var operation = (IObjectCreationOperation)context.Operation;
        if (IsParameterlessObjectCreationOfNonDefaultableStruct(operation))
        {
            ReportDiagnostic(context);
        }
    }

    private static void ReportDiagnostic(OperationAnalysisContext context)
        => context.ReportDiagnostic(Diagnostic.Create(
            DoNotUseDefault,
            context.Operation.Syntax.GetLocation(),
            messageArgs: context.Operation.Type?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
}
