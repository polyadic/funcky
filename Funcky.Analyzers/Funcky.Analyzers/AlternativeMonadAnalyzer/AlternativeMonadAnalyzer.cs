using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

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
        if (alternativeMonadTypes.Any())
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
                noneArgument: operation.GetArgumentForParameterAtIndex(alternativeMonad.ErrorStateArgumentIndex),
                someArgument: operation.GetArgumentForParameterAtIndex(alternativeMonad.SuccessStateArgumentIndex)) is { } diagnostic)
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
        return invocation.TargetMethod.ReceiverType is INamedTypeSymbol receiverType
           && invocation.TargetMethod.Name == MatchMethodName
           && invocation.Arguments.Length == 2
           && alternativeMonadTypes.TryGetValue(receiverType.ConstructedFrom, out alternativeMonadType)
           && (matchReceiverType = receiverType) is var _;
    }

    private static Diagnostic? AnalyzeMatchInvocation(
        IInvocationOperation matchInvocation,
        AlternativeMonadType alternativeMonadType,
        INamedTypeSymbol receiverType,
        IArgumentOperation noneArgument,
        IArgumentOperation someArgument)
    {
        if (alternativeMonadType.HasToNullable && IsToNullableEquivalent(matchInvocation, receiverType, noneArgument, someArgument))
        {
            return Diagnostic.Create(PreferToNullable, matchInvocation.Syntax.GetLocation());
        }

        if (alternativeMonadType.HasGetOrElse && IsGetOrElseEquivalent(receiverType, noneArgument, someArgument))
        {
            var noneArgumentIndex = matchInvocation.Arguments.IndexOf(noneArgument);
            return Diagnostic.Create(
                PreferGetOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, noneArgumentIndex.ToString()));
        }

        if (alternativeMonadType.HasOrElse && IsOrElseEquivalent(alternativeMonadType, matchInvocation, receiverType, someArgument))
        {
            var noneArgumentIndex = matchInvocation.Arguments.IndexOf(noneArgument);
            return Diagnostic.Create(
                PreferOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, noneArgumentIndex.ToString()));
        }

        if (IsSelectManyEquivalent(alternativeMonadType, matchInvocation, receiverType, noneArgument))
        {
            var someArgumentIndex = matchInvocation.Arguments.IndexOf(someArgument);
            return Diagnostic.Create(
                PreferSelectMany,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, someArgumentIndex.ToString()));
        }

        return null;
    }
}
