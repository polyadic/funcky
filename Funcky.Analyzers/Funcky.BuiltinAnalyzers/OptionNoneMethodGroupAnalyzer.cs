using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.BuiltinAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptionNoneMethodGroupAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
        id: "λ0002",
        title: "Prefer Option.None<T>() over Option<T>.None() when used as a method group",
        messageFormat: "Use Option.None<{0}> instead of Option<{0}>.None",
        category: "Funcky.Deprecation",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Funcky 3 changes Option<T>.None to a property, which will break method groups.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetOptionType() is not null && context.Compilation.GetOptionOfTType() is { } optionOfTType)
        {
            context.RegisterOperationAction(AnalyzeInvocation(optionOfTType), OperationKind.MethodReference);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol optionOfTType)
        => context
            =>
            {
                var operation = (IMethodReferenceOperation)context.Operation;
                if (IsOptionOfTNoneMethodGroup(operation, optionOfTType))
                {
                    var itemType = operation.Method.ContainingType.TypeArguments.Single();
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, operation.Syntax.GetLocation(), itemType.ToDisplayString()));
                }
            };

    private static bool IsOptionOfTNoneMethodGroup(IMethodReferenceOperation operation, INamedTypeSymbol genericOptionType)
        => operation.Method.Name is WellKnownMemberNames.OptionOfT.None
           && SymbolEqualityComparer.Default.Equals(genericOptionType, operation.Method.ContainingType.ConstructedFrom);
}
