using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Funcky.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class OptionNoneAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new(
            id: "ƛ1001",
            title: "Replace method call to Option<TItem>.None() with property access",
            messageFormat: "Replace method call to Option<TItem>.None() with property access",
            category: "Funcky.Migration",
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeInvocations, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocations(SyntaxNodeAnalysisContext context)
        {
            var optionSymbol = context.Compilation.GetTypeByMetadataName("Funcky.Monads.Option`1")!; // TODO: null check
            var node = (InvocationExpressionSyntax)context.Node;

            if (node.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax
                && context.SemanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Expression) is { Symbol: INamedTypeSymbol { IsGenericType: true } symbol }
                && SymbolEqualityComparer.Default.Equals(optionSymbol, symbol.ConstructedFrom)
                && memberAccessExpressionSyntax.Name.Identifier.Text == "None")
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
            }
        }
    }
}
