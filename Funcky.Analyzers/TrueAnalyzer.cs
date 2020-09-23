using System.Collections.Immutable;
using Funcky.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Funcky.Analyzers.DiagnosticDescriptors;

namespace Funcky.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class TrueAnalyzer : UnaryLambdaReplaceAnalyzerBase
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(UseFunctionalTrue);

        protected override Diagnostic GenerateDiagnostic(SyntaxNodeAnalysisContext context, AnonymousFunctionExpressionSyntax expression)
            => Diagnostic.Create(UseFunctionalTrue, expression.GetLocation());

        protected override bool CanBeReplacedWithMethodGroup(
            SyntaxNodeAnalysisContext context,
            AnonymousFunctionExpressionSyntax expression,
            ParameterSyntax parameter)
            => expression.HasBooleanAsReturnType(context) && expression.Body.IsTrueLiteral();
    }
}
