using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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

        if (IsMatchInvocation(operation, alternativeMonadTypes, out var receiverType, out var alternativeMonad)
            && AnalyzeMatchInvocation(
                operation,
                alternativeMonad,
                receiverType,
                errorStateArgument: operation.GetArgumentForParameterAtIndex(alternativeMonad.ErrorStateArgumentIndex),
                successStateArgument: operation.GetArgumentForParameterAtIndex(alternativeMonad.SuccessStateArgumentIndex)) is { } diagnostic)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsMatchInvocation(
        IInvocationOperation invocation,
        AlternativeMonadTypeCollection alternativeMonadTypes,
        [NotNullWhen(true)] out INamedTypeSymbol? matchReceiverType,
        [NotNullWhen(true)] out AlternativeMonadType? alternativeMonadType)
    {
        matchReceiverType = null;
        alternativeMonadType = null;
        return invocation is { TargetMethod: { ReceiverType: INamedTypeSymbol receiverType, Name: MatchMethodName }, Arguments: [_, _] }
               && alternativeMonadTypes.Value.TryGetValue(receiverType.ConstructedFrom, out alternativeMonadType)
               && (matchReceiverType = receiverType) is var _;
    }

    private static Diagnostic? AnalyzeMatchInvocation(
        IInvocationOperation matchInvocation,
        AlternativeMonadType alternativeMonadType,
        INamedTypeSymbol receiverType,
        IArgumentOperation errorStateArgument,
        IArgumentOperation successStateArgument)
    {
        if (alternativeMonadType.HasToNullable && IsToNullableEquivalent(matchInvocation, receiverType, errorStateArgument, successStateArgument))
        {
            return Diagnostic.Create(PreferToNullable, matchInvocation.Syntax.GetLocation());
        }

        if (alternativeMonadType.HasGetOrElse && IsGetOrElseEquivalent(receiverType, errorStateArgument, successStateArgument))
        {
            var errorStateArgumentIndex = matchInvocation.Arguments.IndexOf(errorStateArgument);
            return Diagnostic.Create(
                PreferGetOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, errorStateArgumentIndex.ToString()));
        }

        if (alternativeMonadType.HasOrElse && IsOrElseEquivalent(alternativeMonadType, matchInvocation, receiverType, successStateArgument))
        {
            var errorStateArgumentIndex = matchInvocation.Arguments.IndexOf(errorStateArgument);
            return Diagnostic.Create(
                PreferOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, errorStateArgumentIndex.ToString()));
        }

        if (IsSelectManyEquivalent(alternativeMonadType, matchInvocation, receiverType, errorStateArgument))
        {
            var successStateArgumentIndex = matchInvocation.Arguments.IndexOf(successStateArgument);
            return Diagnostic.Create(
                PreferSelectMany,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, successStateArgumentIndex.ToString()));
        }

        return null;
    }
}
