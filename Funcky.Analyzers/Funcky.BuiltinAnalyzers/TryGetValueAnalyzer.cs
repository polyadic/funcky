using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.BuiltinAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TryGetValueAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Descriptor = new(
        id: "λ0001",
        title: "Disallowed use of TryGetValue",
        messageFormat: "Disallowed use of TryGetValue",
        category: "Funcky",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "TryGetValue should be used carefully.",
        customTags: WellKnownDiagnosticTags.NotConfigurable,
        helpLinkUri: "https://polyadic.github.io/funcky/analyzer-rules/λ0001.html");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterCompilationStartAction(OnCompilationStarted);
    }

    private static void OnCompilationStarted(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetOptionOfTType() is { } optionOfTType)
        {
            context.RegisterOperationAction(AnalyzeInvocation(optionOfTType), OperationKind.Invocation);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol optionOfTType)
        => context
            =>
            {
                var operation = (IInvocationOperation)context.Operation;

                if (IsTryGetValueMethod(operation, optionOfTType)
                    && !IsAllowedUsageOfTryGetValue(operation.Syntax))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, operation.Syntax.GetLocation()));
                }
            };

    private static bool IsTryGetValueMethod(IInvocationOperation operation, INamedTypeSymbol optionOfTType)
        => operation.TargetMethod.MetadataName == WellKnownMemberNames.OptionOfT.TryGetValue
           && SymbolEqualityComparer.Default.Equals(operation.TargetMethod.ContainingType.OriginalDefinition, optionOfTType);

    private static bool IsAllowedUsageOfTryGetValue(SyntaxNode node)
        => IsPartOfCatchFilterClause(node)
           || (IsInLoopCondition(node) && node.IsWithinIterator())
           || (IsInIfConditionWithYield(node) && node.IsWithinIterator());

    private static bool IsPartOfCatchFilterClause(SyntaxNode node)
        => node.Parent is CatchFilterClauseSyntax;

    private static bool IsInLoopCondition(SyntaxNode node)
        => node.AncestorsAndSelf().Any(n => IsConditionOfWhileStatement(n) || IsConditionOfDoStatement(n));

    private static bool IsInIfConditionWithYield(SyntaxNode node)
        => node.AncestorsAndSelf().Any(n => n.Parent is IfStatementSyntax ifStatement
            && ifStatement.Condition == n
            && ifStatement.ContainsYield());

    private static bool IsConditionOfWhileStatement(SyntaxNode node)
        => node.Parent is WhileStatementSyntax whileStatementSyntax && whileStatementSyntax.Condition == node;

    private static bool IsConditionOfDoStatement(SyntaxNode node)
        => node.Parent is DoStatementSyntax whileStatementSyntax && whileStatementSyntax.Condition == node;
}
