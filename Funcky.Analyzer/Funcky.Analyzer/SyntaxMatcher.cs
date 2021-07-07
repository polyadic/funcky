using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Funcky.Analyzer
{
    public class SyntaxMatcher
    {
        private const string Unknown = "<UNKNOWN>";
        private readonly SyntaxNodeAnalysisContext _analysisContext;

        public SyntaxMatcher(SyntaxNodeAnalysisContext analysisContext)
            => _analysisContext = analysisContext;

        private InvocationExpressionSyntax InvocationExpr => (InvocationExpressionSyntax)_analysisContext.Node;

        public bool MatchStaticCall(string className, string methodName)
            => InvocationExpr.Expression is MemberAccessExpressionSyntax memberAccessExpr
                && memberAccessExpr.Name.Identifier.ValueText == methodName
                && memberAccessExpr.Expression is IdentifierNameSyntax identifier
                && identifier.Identifier.ValueText == className;

        public bool MatchArgument<TArgument>(int argumentPosition, TArgument argument)
            => InvocationExpr.ArgumentList is ArgumentListSyntax argumentList
                && argumentList.Arguments.Count > argumentPosition
                && argumentList.Arguments[argumentPosition] is ArgumentSyntax repeatArgument
                && repeatArgument.Expression is LiteralExpressionSyntax literal
                && literal.Token.Value is TArgument value
                && value.Equals(argument);

        public string GetArgumentAsString(int argumentPosition)
            => InvocationExpr.ArgumentList is ArgumentListSyntax argumentList
                && argumentList.Arguments.Count > argumentPosition
                && argumentList.Arguments[argumentPosition] is ArgumentSyntax argument
                    ? argument.ToString()
                    : Unknown;
    }
}
