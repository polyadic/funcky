using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers.AlternativeMonad;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class AlternativeMonadAnalyzer : DiagnosticAnalyzer
{
    public const string PreservedArgumentIndexProperty = nameof(PreservedArgumentIndexProperty);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PreferGetOrElse, PreferOrElse, PreferSelectMany, PreferToNullable);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var alternativeMonadTypes = AlternativeMonadTypeCollection.FromCompilation(context.Compilation);
        if (alternativeMonadTypes.Value.Any())
        {
            context.RegisterOperationAction(context => AnalyzeInvocation(context, alternativeMonadTypes), OperationKind.Invocation);
        }
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, AlternativeMonadTypeCollection alternativeMonadTypes)
    {
        var operation = (IInvocationOperation)context.Operation;

        if (MatchMatchInvocation(operation, alternativeMonadTypes) is [var matchInvocation]
            && AnalyzeMatchInvocation(operation, matchInvocation) is { } diagnostic)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static Option<MatchInvocation> MatchMatchInvocation(IInvocationOperation invocation, AlternativeMonadTypeCollection alternativeMonadTypes)
        => invocation is { TargetMethod: { ReceiverType: INamedTypeSymbol receiverType, Name: MatchMethodName } }
            && alternativeMonadTypes.Value.TryGetValue(receiverType.ConstructedFrom, out var alternativeMonadType)
            && invocation.GetArgumentsInParameterOrder() is { Length: 2 } arguments
                ? [new MatchInvocation(
                    receiverType,
                    alternativeMonadType,
                    arguments[alternativeMonadType.ErrorStateArgumentIndex],
                    arguments[alternativeMonadType.SuccessStateArgumentIndex])]
                : [];

    private static Diagnostic? AnalyzeMatchInvocation(
        IInvocationOperation operation,
        MatchInvocation match)
    {
        var alternativeMonadType = match.AlternativeMonadType;

        if (alternativeMonadType.HasToNullable && IsToNullableEquivalent(operation, match.Receiver, match.ErrorState, match.SuccessState))
        {
            return Diagnostic.Create(PreferToNullable, operation.Syntax.GetLocation());
        }

        if (alternativeMonadType.HasGetOrElse && IsGetOrElseEquivalent(match.Receiver, match.ErrorState, match.SuccessState))
        {
            var errorStateArgumentIndex = operation.Arguments.IndexOf(match.ErrorState);
            return Diagnostic.Create(
                PreferGetOrElse,
                operation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, errorStateArgumentIndex.ToString()));
        }

        if (alternativeMonadType.HasOrElse && IsOrElseEquivalent(alternativeMonadType, operation, match.Receiver, match.SuccessState))
        {
            var errorStateArgumentIndex = operation.Arguments.IndexOf(match.ErrorState);
            return Diagnostic.Create(
                PreferOrElse,
                operation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, errorStateArgumentIndex.ToString()));
        }

        if (IsSelectManyEquivalent(alternativeMonadType, operation, match.Receiver, match.ErrorState))
        {
            var successStateArgumentIndex = operation.Arguments.IndexOf(match.SuccessState);
            return Diagnostic.Create(
                PreferSelectMany,
                operation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, successStateArgumentIndex.ToString()));
        }

        return null;
    }

    private sealed record MatchInvocation(
        INamedTypeSymbol Receiver,
        AlternativeMonadType AlternativeMonadType,
        IArgumentOperation ErrorState,
        IArgumentOperation SuccessState);
}
