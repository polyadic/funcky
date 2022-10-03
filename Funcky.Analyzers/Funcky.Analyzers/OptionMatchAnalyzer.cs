using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.IdentityFunctionMatching;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptionMatchAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor PreferGetOrElse = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}05",
        title: "Prefer GetOrElse over Match",
        messageFormat: "Prefer GetOrElse over Match",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PreferGetOrElse);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            if (context.Compilation.GetOptionOfTType() is { } optionOfTType)
            {
                context.RegisterOperationAction(AnalyzeInvocation(optionOfTType), OperationKind.Invocation);
            }
        });
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol optionOfTType)
        => context
            =>
            {
                var operation = (IInvocationOperation)context.Operation;

                if (IsMatchInvocation(operation, optionOfTType, out var receiverType)
                    && AnalyzeMatchInvocation(
                        operation,
                        receiverType,
                        noneArgument: operation.GetArgumentForParameterAtIndex(0),
                        someArgument: operation.GetArgumentForParameterAtIndex(1)) is { } diagnostic)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            };

    private static bool IsMatchInvocation(
        IInvocationOperation invocation,
        INamedTypeSymbol optionOfTType,
        [NotNullWhen(true)] out INamedTypeSymbol? receiverType)
    {
        receiverType = null;
        return invocation.TargetMethod.ReceiverType is INamedTypeSymbol receiverTypeTemp
           && SymbolEqualityComparer.Default.Equals(receiverTypeTemp.ConstructedFrom, optionOfTType)
           && invocation.TargetMethod.Name == "Match"
           && invocation.Arguments.Length == 2
           && (receiverType = receiverTypeTemp) is var _;
    }

    private static Diagnostic? AnalyzeMatchInvocation(
        IInvocationOperation matchInvocation,
        INamedTypeSymbol receiverType,
        IArgumentOperation noneArgument,
        IArgumentOperation someArgument)
    {
        if (SymbolEqualityComparer.IncludeNullability.Equals(receiverType.TypeArguments.Single(), matchInvocation.Type)
            && IsIdentityFunction(someArgument.Value))
        {
            return Diagnostic.Create(PreferGetOrElse, matchInvocation.Syntax.GetLocation());
        }

        return null;
    }
}
