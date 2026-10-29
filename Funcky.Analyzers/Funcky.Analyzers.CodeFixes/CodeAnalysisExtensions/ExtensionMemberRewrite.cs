using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

/// <summary>An expression to replace with a call of a static extension member of a class in <c>Funcky.Extensions</c>.</summary>
public sealed record ExtensionMemberRewrite(ExpressionSyntax Original, ExpressionSyntax Replacement);
