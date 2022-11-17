using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class OptionMatchAnalyzer : DiagnosticAnalyzer
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
        if (context.Compilation.GetOptionOfTType() is { } optionOfTType)
        {
            var toNullableExists = ToNullableExtensionIsAvailable(context.Compilation);
            var symbols = new CompilationSymbols(optionOfTType, toNullableExists);
            context.RegisterOperationAction(context => AnalyzeInvocation(context, symbols), OperationKind.Invocation);
        }
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, CompilationSymbols symbols)
    {
        var operation = (IInvocationOperation)context.Operation;

        if (IsMatchInvocation(operation, symbols, out var receiverType, out var noneArgument, out var someArgument)
            && AnalyzeMatchInvocation(operation, symbols, receiverType, noneArgument, someArgument) is { } diagnostic)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsMatchInvocation(
        IInvocationOperation invocation,
        CompilationSymbols symbols,
        [NotNullWhen(true)] out INamedTypeSymbol? matchReceiverType,
        [NotNullWhen(true)] out IArgumentOperation? noneArgument,
        [NotNullWhen(true)] out IArgumentOperation? someArgument)
    {
        matchReceiverType = null;
        noneArgument = null;
        someArgument = null;
        return invocation.TargetMethod.ReceiverType is INamedTypeSymbol receiverType
           && SymbolEqualityComparer.Default.Equals(receiverType.ConstructedFrom, symbols.OptionOfTType)
           && invocation.TargetMethod.Name == MatchMethodName
           && invocation.Arguments.InDeclarationOrder() is /*🎨*/[var none, var some]
           && (matchReceiverType = receiverType) is var _
           && (noneArgument = none) is var _
           && (someArgument = some) is var _;
    }

    private static Diagnostic? AnalyzeMatchInvocation(
        IInvocationOperation matchInvocation,
        CompilationSymbols symbols,
        INamedTypeSymbol receiverType,
        IArgumentOperation noneArgument,
        IArgumentOperation someArgument)
    {
        if (symbols.ToNullableExtensionIsAvailable && IsToNullableEquivalent(matchInvocation, receiverType, noneArgument, someArgument))
        {
            return Diagnostic.Create(PreferToNullable, matchInvocation.Syntax.GetLocation());
        }

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

    private sealed record CompilationSymbols(INamedTypeSymbol OptionOfTType, bool ToNullableExtensionIsAvailable);
}
