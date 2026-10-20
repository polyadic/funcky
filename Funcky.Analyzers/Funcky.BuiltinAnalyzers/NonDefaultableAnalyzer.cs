using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.BuiltinAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NonDefaultableAnalyzer : DiagnosticAnalyzer
{
    // λ1009 was introduced in Funcky.Analyzers 1.4.0 and moved into the built-in analyzers in Funcky 3.7.0.
    // Diagnostic IDs are stable identifiers (consumers configure and suppress them by ID), so it keeps its number.
    public const string DoNotUseDefaultId = $"{DiagnosticName.Prefix}1009";

    public static readonly DiagnosticDescriptor DoNotUseDefault = new(
        id: DoNotUseDefaultId,
        title: "Do not use default to instantiate this type",
        messageFormat: "Do not use default(...) to instantiate '{0}'",
        category: DiagnosticName.Category,
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
