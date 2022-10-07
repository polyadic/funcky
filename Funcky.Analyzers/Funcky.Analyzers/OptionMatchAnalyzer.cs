using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.IdentityFunctionMatching;
using static Funcky.Analyzers.OptionReturnMatching;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptionMatchAnalyzer : DiagnosticAnalyzer
{
    public const string NoneArgumentIndexProperty = nameof(NoneArgumentIndexProperty);

    public static readonly DiagnosticDescriptor PreferGetOrElse = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}05",
        title: "Prefer GetOrElse over Match",
        messageFormat: "Prefer GetOrElse over Match",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    public static readonly DiagnosticDescriptor PreferOrElse = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}06",
        title: "Prefer OrElse over Match",
        messageFormat: "Prefer OrElse over Match",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PreferGetOrElse, PreferOrElse);

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
        if (IsGetOrElseEquivalent(matchInvocation, receiverType, someArgument))
        {
            var noneArgumentIndex = matchInvocation.Arguments.IndexOf(noneArgument);
            return Diagnostic.Create(
                PreferGetOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(NoneArgumentIndexProperty, noneArgumentIndex.ToString()));
        }

        if (IsOrElseEquivalent(matchInvocation, receiverType, someArgument))
        {
            var noneArgumentIndex = matchInvocation.Arguments.IndexOf(noneArgument);
            return Diagnostic.Create(
                PreferOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(NoneArgumentIndexProperty, noneArgumentIndex.ToString()));
        }

        return null;
    }

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: A, some: Identity)</c>.</summary>
    private static bool IsGetOrElseEquivalent(IInvocationOperation matchInvocation, INamedTypeSymbol receiverType, IArgumentOperation someArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType.TypeArguments.Single(), matchInvocation.Type)
            && IsIdentityFunction(someArgument.Value);

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: A, some: Option.Return)</c>.</summary>
    private static bool IsOrElseEquivalent(IInvocationOperation matchInvocation, INamedTypeSymbol receiverType, IArgumentOperation someArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType, matchInvocation.Type)
            && IsOptionReturnFunction(someArgument.Value);
}
