#if ROSLYN_4_4_0
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.BuiltinAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptionListPatternAnalyzer : DiagnosticAnalyzer
{
    // λ1010 was written for Funcky.Analyzers and moved into the built-in analyzers before it shipped. It keeps its
    // number: the λ0xxx/λ1xxx ranges record where a rule was born, the rules table in the documentation says where it lives.
    public const string OptionHasZeroOrOneElementsId = $"{DiagnosticName.Prefix}1010";

    public static readonly DiagnosticDescriptor OptionHasZeroOrOneElements = new(
        id: OptionHasZeroOrOneElementsId,
        title: "An option has either zero or one elements",
        messageFormat: "An option has either zero or one elements, testing for more elements will never match",
        category: DiagnosticName.Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: DiagnosticName.HelpLink(OptionHasZeroOrOneElementsId));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(OptionHasZeroOrOneElements);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetOptionOfTType() is { } optionOfTType)
        {
            context.RegisterOperationAction(AnalyzeListPattern(optionOfTType), OperationKind.ListPattern);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeListPattern(INamedTypeSymbol optionOfTType)
        => context
            =>
            {
                var operation = (IListPatternOperation)context.Operation;
                if (SymbolEqualityComparer.Default.Equals(operation.InputType.OriginalDefinition, optionOfTType)
                    && operation.Patterns.Length > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(OptionHasZeroOrOneElements, operation.Syntax.GetLocation()));
                }
            };
}
#endif
