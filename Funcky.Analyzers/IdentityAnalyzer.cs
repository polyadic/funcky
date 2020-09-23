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

        protected override bool CanBeReplacedWithMethodGroup(
            SyntaxNodeAnalysisContext context,
            AnonymousFunctionExpressionSyntax expression,
            ParameterSyntax parameter)
            => IsParameterReferenceWithMatchingType(context, expression)
               && ExpressionBodyReferencesMatchingParameter(expression, parameter);

        protected override Diagnostic GenerateDiagnostic(SyntaxNodeAnalysisContext context, AnonymousFunctionExpressionSyntax expression)
            => Diagnostic.Create(
                UseFunctionalIdentity,
                expression.GetLocation());

        private static bool ExpressionBodyReferencesMatchingParameter(
            AnonymousFunctionExpressionSyntax expression,
            ParameterSyntax parameter)
            => expression.Body is IdentifierNameSyntax body
               && body.Identifier.Text == parameter.Identifier.Text;

        private static bool IsParameterReferenceWithMatchingType(
            SyntaxNodeAnalysisContext context,
            AnonymousFunctionExpressionSyntax expression)
            => context.SemanticModel.GetOperation(expression.Body) is IParameterReferenceOperation operation
               && expression.GetReturnType(context) is { } returnType
               && SymbolEqualityComparer.IncludeNullability.Equals(returnType, operation.Type);
    }
}
