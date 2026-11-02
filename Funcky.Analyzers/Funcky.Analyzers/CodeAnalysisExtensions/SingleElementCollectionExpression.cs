using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

/// <summary>Builds the collection expression <c>[element]</c> that replaces <c>Enumerable.Return(element)</c>. Shared between the
/// analyzer, which binds it speculatively, and the code fix, which inserts it.</summary>
internal static class SingleElementCollectionExpression
{
    // SyntaxFactory.CollectionExpression is only available in Roslyn 4.7+, but the analyzers are also compiled against
    // older Roslyn versions. Parsing the text keeps it portable: at runtime the host compiler supports C# 12, otherwise
    // the diagnostic would not fire.
    public static ExpressionSyntax Create(ExpressionSyntax element, string leadingTrivia = "", string trailingTrivia = "")
        => SyntaxFactory.ParseExpression($"[{leadingTrivia}{ElementText(element)}{trailingTrivia}]");

    // An element starting with `..` would be reparsed as a spread element, so a range without a left operand is parenthesized.
    private static string ElementText(ExpressionSyntax element)
        => element is RangeExpressionSyntax { LeftOperand: null }
            ? $"({element})"
            : element.ToString();
}
