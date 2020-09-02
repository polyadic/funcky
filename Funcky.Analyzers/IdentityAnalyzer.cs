using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.DiagnosticDescriptors;

namespace Funcky.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class IdentityAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(UseFunctionalIdentity);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleLambdaExpression, SyntaxKind.SimpleLambdaExpression);
        }

        private static void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (SimpleLambdaExpressionSyntax)context.Node;
            if (CanBeReplacedWithIdentityFunction(context, expression))
            {
                ReportDiagnostic(context, expression);
            }
        }

        private static void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (ParenthesizedLambdaExpressionSyntax)context.Node;
            if (IsUnaryLambdaExpression(expression) && CanBeReplacedWithIdentityFunction(context, expression))
            {
                ReportDiagnostic(context, expression);
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, AnonymousFunctionExpressionSyntax expression)
            => context.ReportDiagnostic(Diagnostic.Create(
                UseFunctionalIdentity,
                expression.GetLocation()));

        private static bool IsUnaryLambdaExpression(ParenthesizedLambdaExpressionSyntax expression)
            => expression.ParameterList.Parameters.Count == 1;

        private static bool CanBeReplacedWithIdentityFunction(SyntaxNodeAnalysisContext context, AnonymousFunctionExpressionSyntax expression)
            => !IsLinqExpression(context, expression) &&
               AnonymousFunctionContainsParameterReferenceThatMatchesReturnTypeOnly(context, expression);

        private static bool AnonymousFunctionContainsParameterReferenceThatMatchesReturnTypeOnly(
            SyntaxNodeAnalysisContext context,
            AnonymousFunctionExpressionSyntax expression)
            => GetReturnType(context, expression) is { } returnType &&
               GetParameterReferenceOperationType(context, expression) is { } parameterType &&
               SymbolEqualityComparer.IncludeNullability.Equals(returnType, parameterType);

        private static ITypeSymbol? GetParameterReferenceOperationType(
            SyntaxNodeAnalysisContext context,
            AnonymousFunctionExpressionSyntax expression)
            => context.SemanticModel.GetOperation(expression.Body) is IParameterReferenceOperation parameterReference
                ? parameterReference.Parameter.Type
                : null;

        private static ITypeSymbol? GetReturnType(
            SyntaxNodeAnalysisContext context,
            AnonymousFunctionExpressionSyntax expression)
            => context.SemanticModel.GetOperation(expression) is IAnonymousFunctionOperation functionOperation
                ? functionOperation.Symbol.ReturnType
                : null;

        private static bool IsLinqExpression(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            var lambdaType = context.SemanticModel.GetTypeInfo(expression).ConvertedType;
            var typeWithoutGenericParameters = lambdaType?.OriginalDefinition ?? lambdaType;
            return SymbolEqualityComparer.Default.Equals(typeWithoutGenericParameters, GetLinqExpressionType(context));
        }

        private static ITypeSymbol GetLinqExpressionType(SyntaxNodeAnalysisContext context)
            => context.Compilation?.GetTypeByMetadataName("System.Linq.Expressions.Expression`1") ?? throw new NullReferenceException();
    }
}
