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

        context.RegisterCompilationStartAction(static context =>
        {
            if (context.Compilation.GetOptionOfTType() is { } optionOfTType)
            {
                context.RegisterOperationAction(context => AnalyzeInvocation(context, optionOfTType), OperationKind.Invocation);
            }
        });
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, INamedTypeSymbol optionOfTType)
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
    }

    private static bool IsMatchInvocation(
        IInvocationOperation invocation,
        INamedTypeSymbol optionOfTType,
        [NotNullWhen(true)] out INamedTypeSymbol? receiverType)
    {
        receiverType = null;
        return invocation.TargetMethod.ReceiverType is INamedTypeSymbol receiverType_
           && SymbolEqualityComparer.Default.Equals(receiverType_.ConstructedFrom, optionOfTType)
           && invocation.TargetMethod.Name == MatchMethodName
           && invocation.Arguments.Length == 2
           && (receiverType = receiverType_) is var _;
    }

    private static Diagnostic? AnalyzeMatchInvocation(
        IInvocationOperation matchInvocation,
        INamedTypeSymbol receiverType,
        IArgumentOperation noneArgument,
        IArgumentOperation someArgument)
    {
        if (IsToNullableEquivalent(matchInvocation, receiverType, noneArgument, someArgument))
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
}
