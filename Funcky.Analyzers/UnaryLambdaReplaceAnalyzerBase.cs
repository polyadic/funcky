using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Funcky.Analyzers
{
    public abstract class UnaryLambdaReplaceAnalyzerBase : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleLambdaExpression, SyntaxKind.SimpleLambdaExpression);
        }

        protected abstract Diagnostic GenerateDiagnostic(SyntaxNodeAnalysisContext context, AnonymousFunctionExpressionSyntax expression);

        protected abstract bool CanBeReplacedWithMethodGroup(SyntaxNodeAnalysisContext context, AnonymousFunctionExpressionSyntax expression);

        private void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (SimpleLambdaExpressionSyntax)context.Node;
            if (!IsLinqExpression(context, expression) && CanBeReplacedWithMethodGroup(context, expression))
            {
                ReportDiagnostic(context, expression);
            }
        }

        private void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (ParenthesizedLambdaExpressionSyntax)context.Node;
            if (!IsLinqExpression(context, expression) && IsUnaryLambdaExpression(expression) && CanBeReplacedWithMethodGroup(context, expression))
            {
                ReportDiagnostic(context, expression);
            }
        }

        private void ReportDiagnostic(SyntaxNodeAnalysisContext context, AnonymousFunctionExpressionSyntax expression)
            => context.ReportDiagnostic(GenerateDiagnostic(context, expression));

        private static bool IsUnaryLambdaExpression(ParenthesizedLambdaExpressionSyntax expression)
            => expression.ParameterList.Parameters.Count == 1;

        private static bool IsLinqExpression(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            var lambdaType = context.SemanticModel.GetTypeInfo(expression).ConvertedType;
            var typeWithoutGenericParameters = lambdaType?.OriginalDefinition ?? lambdaType;
            var linqExpressionType = GetLinqExpressionType(context);
            return linqExpressionType is { } && SymbolEqualityComparer.Default.Equals(typeWithoutGenericParameters, linqExpressionType);
        }

        private static ITypeSymbol? GetLinqExpressionType(SyntaxNodeAnalysisContext context)
            => context.Compilation?.GetTypeByMetadataName("System.Linq.Expressions.Expression`1");
    }
}
