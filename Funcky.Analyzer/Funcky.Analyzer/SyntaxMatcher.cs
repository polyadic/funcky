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

        internal bool MatchArgument<TArgument>(int argumentPosition, TArgument argument)
        {
            var invocationExpr = (InvocationExpressionSyntax)_analysisContext.Node;

            if (invocationExpr.ArgumentList is not ArgumentListSyntax argumentList)
            {
                return false;
            }

            if (argumentList.Arguments.Count <= argumentPosition)
            {
                return false;
            }

            if (argumentList.Arguments[argumentPosition] is not ArgumentSyntax repeatArgument)
            {
                return false;
            }

            if (repeatArgument.Expression is not LiteralExpressionSyntax literal)
            {
                return false;
            }

            if (literal.Token.Value is TArgument value)
            {
                return value.Equals(argument);
            }

            return true;
        }
    }
}
