using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Funcky.Analyzer
{
    public class SyntaxMatcher
    {
        private readonly SyntaxNodeAnalysisContext _analysisContext;

        public SyntaxMatcher(SyntaxNodeAnalysisContext analysisContext)
            => _analysisContext = analysisContext;

        public bool MatchStaticCall(string className, string methodName)
        {
            var invocationExpr = (InvocationExpressionSyntax)_analysisContext.Node;

            return invocationExpr.Expression is MemberAccessExpressionSyntax memberAccessExpr
                && memberAccessExpr.Name.Identifier.ValueText == methodName
                && memberAccessExpr.Expression is IdentifierNameSyntax identifier
                && identifier.Identifier.ValueText == className;
        }
    }
}
