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
    private const string AttributeFullName = "Funcky.CodeAnalysis.AlternativeMonadAttribute";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PreferGetOrElse, PreferOrElse, PreferSelectMany, PreferToNullable);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetTypeByMetadataName(AttributeFullName) is { } alternativeMonadAttributeType)
        {
            var toNullableExists = ToNullableExtensionIsAvailable(context.Compilation);
            var symbols = new CompilationSymbols(alternativeMonadAttributeType, toNullableExists);
            context.RegisterOperationAction(context => AnalyzeInvocation(context, symbols), OperationKind.Invocation);
        }
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, CompilationSymbols symbols)
    {
        var operation = (IInvocationOperation)context.Operation;

        if (IsMatchInvocation(operation, symbols, out var receiverType, out var alternativeMonad)
            && AnalyzeMatchInvocation(
                operation,
                alternativeMonad,
                symbols,
                receiverType,
                noneArgument: operation.GetArgumentForParameterAtIndex(alternativeMonad.ErrorStateArgumentIndex),
                someArgument: operation.GetArgumentForParameterAtIndex(alternativeMonad.SuccessStateArgumentIndex)) is { } diagnostic)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsMatchInvocation(
        IInvocationOperation invocation,
        CompilationSymbols symbols,
        [NotNullWhen(true)] out INamedTypeSymbol? matchReceiverType,
        [NotNullWhen(true)] out AlternativeMonadType? matchAlternativeMonadType)
    {
        matchReceiverType = null;
        matchAlternativeMonadType = null;
        return invocation.TargetMethod.ReceiverType is INamedTypeSymbol receiverType
           && invocation.TargetMethod.Name == MatchMethodName
           && invocation.Arguments.Length == 2
           && AlternativeMonadType.Create(receiverType, symbols.AlternativeMonadAttributeType) is { } alternativeMonad
           && (matchReceiverType = receiverType) is var _
           && (matchAlternativeMonadType = alternativeMonad) is var _;
    }

    private static Diagnostic? AnalyzeMatchInvocation(
        IInvocationOperation matchInvocation,
        AlternativeMonadType alternativeMonadType,
        CompilationSymbols symbols,
        INamedTypeSymbol receiverType,
        IArgumentOperation noneArgument,
        IArgumentOperation someArgument)
    {
        // if (symbols.ToNullableExtensionIsAvailable && IsToNullableEquivalent(matchInvocation, receiverType, noneArgument, someArgument))
        // {
        //     return Diagnostic.Create(PreferToNullable, matchInvocation.Syntax.GetLocation());
        // }

        if (alternativeMonadType.HasGetOrElse && IsGetOrElseEquivalent(receiverType, noneArgument, someArgument))
        {
            var noneArgumentIndex = matchInvocation.Arguments.IndexOf(noneArgument);
            return Diagnostic.Create(
                PreferGetOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, noneArgumentIndex.ToString()));
        }

        if (IsOrElseEquivalent(matchInvocation, receiverType, someArgument))
        {
            var noneArgumentIndex = matchInvocation.Arguments.IndexOf(noneArgument);
            return Diagnostic.Create(
                PreferOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, noneArgumentIndex.ToString()));
        }

        if (IsSelectManyEquivalent(matchInvocation, receiverType, noneArgument))
        {
            var someArgumentIndex = matchInvocation.Arguments.IndexOf(someArgument);
            return Diagnostic.Create(
                PreferSelectMany,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, someArgumentIndex.ToString()));
        }

        return null;
    }

    private static bool ToNullableExtensionIsAvailable(Compilation compilation)
        => compilation.GetOptionExtensionsType() is { } optionExtensionsType
            && optionExtensionsType.GetMembers().Any(static member => member is IMethodSymbol { Name: OptionToNullableMethodName });

    private sealed record CompilationSymbols(INamedTypeSymbol AlternativeMonadAttributeType, bool ToNullableExtensionIsAvailable);
}
