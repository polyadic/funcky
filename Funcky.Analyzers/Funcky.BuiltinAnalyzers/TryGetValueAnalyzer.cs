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
        customTags: WellKnownDiagnosticTags.NotConfigurable);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        var operation = (IInvocationOperation)context.Operation;

        if (IsTryGetValueMethod(operation, context.Compilation)
            && !IsAllowedUsageOfTryGetValue(operation.Syntax, context))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, operation.Syntax.GetLocation()));
        }
    }

    private static bool IsTryGetValueMethod(IInvocationOperation operation, Compilation compilation)
    {
        var optionType = compilation.GetTypeByMetadataName("Funcky.Monads.Option`1");
        return operation.TargetMethod.MetadataName == "TryGetValue"
            && SymbolEqualityComparer.Default.Equals(operation.TargetMethod.ContainingType.OriginalDefinition, optionType);
    }

    private static bool IsAllowedUsageOfTryGetValue(SyntaxNode node, OperationAnalysisContext context)
        => IsPartOfCatchFilterClause(node)
           || (IsInLoopCondition(node) && context.ContainingSymbol.IsIteratorMethod());

    private static bool IsPartOfCatchFilterClause(SyntaxNode node)
        => node.Parent is CatchFilterClauseSyntax;

    private static bool IsInLoopCondition(SyntaxNode node)
        => node.AncestorsAndSelf().Any(n => IsConditionOfWhileStatement(n) || IsConditionOfDoStatement(n));

    private static bool IsConditionOfWhileStatement(SyntaxNode n)
        => n.Parent is WhileStatementSyntax whileStatementSyntax && whileStatementSyntax.Condition == n;

    private static bool IsConditionOfDoStatement(SyntaxNode n)
        => n.Parent is DoStatementSyntax whileStatementSyntax && whileStatementSyntax.Condition == n;
}
