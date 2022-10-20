using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Funcky.Analyzers;

internal static partial class SyntaxNodeExtensions
{
    // getInnermostNodeForTie: true is important, because otherwise we might
    // get the ArgumentExpressionSyntax, which has the same span.
    public static InvocationExpressionSyntax? FindInvocationExpression(this SyntaxNode node, TextSpan span)
        => node.FindNode(span, getInnermostNodeForTie: true).FirstAncestorOrSelf<InvocationExpressionSyntax>();
}
