using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

/// <summary>Builds <c>condition.Then(argument)</c>, the replacement of <c>Option.FromBoolean(condition, argument)</c>,
/// and recognizes the <c>Then</c> extension of Funcky, which is an extension member and therefore lives in an unnamed
/// extension type nested in <c>BooleanExtensions</c> (or directly in it when declared as a classic extension method).</summary>
internal static class BooleanThen
{
    public const string BooleanExtensionsNamespace = "Funcky.Extensions";

    public static InvocationExpressionSyntax Invocation(ExpressionSyntax condition, SimpleNameSyntax thenName, ArgumentSyntax argument)
        => InvocationExpression(
            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Receiver(condition), thenName),
            ArgumentList(SingletonSeparatedList(argument)));

    /// <summary>The <c>Then</c> name with the type arguments of the replaced call, so that an explicit type argument survives the rewrite.</summary>
    public static SimpleNameSyntax ThenName(SimpleNameSyntax replacedName)
        => replacedName is GenericNameSyntax genericName
            ? genericName.WithIdentifier(Identifier(BooleanThenMethodName))
            : IdentifierName(BooleanThenMethodName);

    public static bool HasThen(INamedTypeSymbol booleanExtensionsType)
        => booleanExtensionsType.GetMembers(BooleanThenMethodName).OfType<IMethodSymbol>().Any()
           || booleanExtensionsType.GetTypeMembers().Any(extension => extension.GetMembers(BooleanThenMethodName).OfType<IMethodSymbol>().Any());

    public static bool IsThen(ISymbol? symbol, INamedTypeSymbol booleanExtensionsType)
        => symbol is IMethodSymbol { Name: BooleanThenMethodName } method
           && (method.ReducedFrom ?? method) is { ContainingType: { } containingType }
           && (SymbolEqualityComparer.Default.Equals(containingType, booleanExtensionsType)
               || SymbolEqualityComparer.Default.Equals(containingType.ContainingType, booleanExtensionsType));

    // A member access binds tighter than every operator, so anything but a primary expression needs parentheses.
    private static ExpressionSyntax Receiver(ExpressionSyntax condition)
        => condition.WithoutTrivia() switch
        {
            IdentifierNameSyntax or MemberAccessExpressionSyntax or InvocationExpressionSyntax or ElementAccessExpressionSyntax
                or ParenthesizedExpressionSyntax or LiteralExpressionSyntax or ThisExpressionSyntax or ObjectCreationExpressionSyntax => condition.WithoutTrivia(),
            var other => ParenthesizedExpression(other),
        };
}
