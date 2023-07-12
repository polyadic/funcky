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
                symbols,
                receiverType,
                noneArgument: operation.GetArgumentForParameterAtIndex(ErrorStateArgumentIndex(alternativeMonad)),
                someArgument: operation.GetArgumentForParameterAtIndex(SuccessStateArgumentIndex(alternativeMonad))) is { } diagnostic)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static int ErrorStateArgumentIndex(AlternativeMonad alternativeMonad)
        => alternativeMonad.MatchHasSuccessStateFirst ? 1 : 0;

    private static int SuccessStateArgumentIndex(AlternativeMonad alternativeMonad)
        => alternativeMonad.MatchHasSuccessStateFirst ? 0 : 1;

    private static bool IsMatchInvocation(
        IInvocationOperation invocation,
        CompilationSymbols symbols,
        [NotNullWhen(true)] out INamedTypeSymbol? matchReceiverType,
        [NotNullWhen(true)] out AlternativeMonad? matchAlternativeMonad)
    {
        matchReceiverType = null;
        matchAlternativeMonad = null;
        return invocation.TargetMethod.ReceiverType is INamedTypeSymbol receiverType
           && invocation.TargetMethod.Name == MatchMethodName
           && invocation.Arguments.Length == 2
           && ParseAlternativeMonad(receiverType, symbols.AlternativeMonadAttributeType) is { } alternativeMonad
           && (matchReceiverType = receiverType) is var _
           && (matchAlternativeMonad = alternativeMonad) is var _;
    }

    private static Diagnostic? AnalyzeMatchInvocation(
        IInvocationOperation matchInvocation,
        CompilationSymbols symbols,
        INamedTypeSymbol receiverType,
        IArgumentOperation noneArgument,
        IArgumentOperation someArgument)
    {
        // if (symbols.ToNullableExtensionIsAvailable && IsToNullableEquivalent(matchInvocation, receiverType, noneArgument, someArgument))
        // {
        //     return Diagnostic.Create(PreferToNullable, matchInvocation.Syntax.GetLocation());
        // }

        if (IsGetOrElseEquivalent(receiverType, noneArgument, someArgument))
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

    private static AlternativeMonad? ParseAlternativeMonad(INamedTypeSymbol type, INamedTypeSymbol alternativeMonadAttributeType)
        => type.GetAttributes()
            .Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, alternativeMonadAttributeType))
            .Select(ParseAlternativeMonadAttribute)
            .SingleOrDefault();

    private static AlternativeMonad ParseAlternativeMonadAttribute(AttributeData attribute)
        => new(
            MatchHasSuccessStateFirst: (bool)(attribute.NamedArguments.Single(arg => arg.Key == "MatchHasSuccessStateFirst").Value.Value ?? throw new InvalidOperationException()),
            ReturnAlias: (string)(attribute.NamedArguments.Single(arg => arg.Key == "ReturnAlias").Value.Value ?? throw new InvalidOperationException()));

    private sealed record CompilationSymbols(INamedTypeSymbol AlternativeMonadAttributeType, bool ToNullableExtensionIsAvailable);

    private sealed record AlternativeMonad(bool MatchHasSuccessStateFirst, string ReturnAlias);
}
