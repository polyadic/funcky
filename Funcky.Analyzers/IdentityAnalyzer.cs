using System.Collections.Immutable;
using Funcky.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.DiagnosticDescriptors;

namespace Funcky.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class IdentityAnalyzer : UnaryLambdaReplaceAnalyzerBase
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(UseFunctionalIdentity);

        protected override bool CanBeReplacedWithMethodGroup(SyntaxNodeAnalysisContext context, AnonymousFunctionExpressionSyntax expression)
            => AnonymousFunctionContainsParameterReferenceThatMatchesReturnTypeOnly(context, expression);

        protected override Diagnostic GenerateDiagnostic(SyntaxNodeAnalysisContext context, AnonymousFunctionExpressionSyntax expression)
            => Diagnostic.Create(
                UseFunctionalIdentity,
                expression.GetLocation());

        private static bool AnonymousFunctionContainsParameterReferenceThatMatchesReturnTypeOnly(
            SyntaxNodeAnalysisContext context,
            AnonymousFunctionExpressionSyntax expression)
            => expression.GetReturnType(context) is { } returnType &&
               GetParameterReferenceOperationType(context, expression) is { } parameterType &&
               SymbolEqualityComparer.IncludeNullability.Equals(returnType, parameterType);

        private static ITypeSymbol? GetParameterReferenceOperationType(
            SyntaxNodeAnalysisContext context,
            AnonymousFunctionExpressionSyntax expression)
            => context.SemanticModel.GetOperation(expression.Body) is IParameterReferenceOperation parameterReference
                ? parameterReference.Parameter.Type
                : null;
    }
}
