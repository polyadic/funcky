using System.Collections.Immutable;
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
    public static InvocationExpressionSyntax Invocation(ExpressionSyntax condition, SimpleNameSyntax thenName, ArgumentSyntax argument)
        => InvocationExpression(
            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Receiver(condition), thenName),
            ArgumentList(SingletonSeparatedList(argument)));

    /// <summary>The <c>Then</c> name with the type arguments of the replaced call, so that an explicit type argument survives the rewrite.</summary>
    public static SimpleNameSyntax ThenName(SimpleNameSyntax replacedName)
        => replacedName is GenericNameSyntax genericName
            ? genericName.WithIdentifier(Identifier(BooleanThenMethodName))
            : IdentifierName(BooleanThenMethodName);

    /// <summary>The argument of the value overload. A delegate is passed by parameter name, because <c>Then(factory)</c>
    /// would pick the selector overload and invoke it instead of storing it.</summary>
    public static ArgumentSyntax ValueArgument(ExpressionSyntax value, ITypeSymbol? valueType)
        => valueType is { TypeKind: TypeKind.Delegate }
            ? Argument(NameColon(IdentifierName(BooleanThenValueParameterName)), default, value)
            : Argument(value);

    public static bool HasThen(INamedTypeSymbol booleanExtensionsType)
        => booleanExtensionsType.GetMembers(BooleanThenMethodName).OfType<IMethodSymbol>().Any()
           || booleanExtensionsType.GetTypeMembers().Any(extension => extension.GetMembers(BooleanThenMethodName).OfType<IMethodSymbol>().Any());

    public static bool IsThen(ISymbol? symbol, INamedTypeSymbol booleanExtensionsType)
        => symbol is IMethodSymbol { Name: BooleanThenMethodName } method
           && (method.ReducedFrom ?? method) is { ContainingType: { } containingType }
           && (SymbolEqualityComparer.Default.Equals(containingType, booleanExtensionsType)
               || SymbolEqualityComparer.Default.Equals(containingType.ContainingType, booleanExtensionsType));

    /// <summary>Replaces <paramref name="target"/> with the <c>Then</c> call, adding <c>using Funcky.Extensions;</c> when the call does not
    /// bind without it. Returns <see langword="null"/> when the call still does not bind to Funcky's <c>Then</c> afterwards, because
    /// another <c>Then</c> extension in scope wins or makes the call ambiguous.</summary>
    public static Task<Document?> TryReplaceAsync(Document document, SyntaxNode root, SemanticModel semanticModel, ExpressionSyntax target, InvocationExpressionSyntax replacement, INamedTypeSymbol booleanExtensionsType, CancellationToken cancellationToken)
        => TryReplaceAllAsync(document, root, semanticModel, ImmutableArray.Create(new ExtensionMemberRewrite(target, replacement)), booleanExtensionsType, cancellationToken);

    /// <summary>Applies all rewrites at once, adding <c>using Funcky.Extensions;</c> once when a <c>Then</c> call does not bind without it.
    /// A call that still does not bind to Funcky's <c>Then</c> afterwards, because another <c>Then</c> extension in scope wins or makes
    /// it ambiguous, is left as it is; the result is <see langword="null"/> when that holds for every call.</summary>
    public static async Task<Document?> TryReplaceAllAsync(Document document, SyntaxNode root, SemanticModel semanticModel, ImmutableArray<ExtensionMemberRewrite> rewrites, INamedTypeSymbol booleanExtensionsType, CancellationToken cancellationToken)
    {
        if (rewrites.IsEmpty)
        {
            return null;
        }

        var annotatedRoot = FunckyExtensionsImport.ReplaceWithAnnotatedExtensionMemberCalls(root, semanticModel, rewrites, symbol => IsThen(symbol, booleanExtensionsType));
        var unboundRewrites = await UnboundRewritesAsync(document.WithSyntaxRoot(annotatedRoot), rewrites.Length, cancellationToken).ConfigureAwait(false);

        return unboundRewrites.IsEmpty
            ? document.WithSyntaxRoot(FunckyExtensionsImport.WithoutReplacementAnnotations(annotatedRoot))
            : await TryReplaceAllAsync(document, root, semanticModel, rewrites.Where((_, index) => !unboundRewrites.Contains(index)).ToImmutableArray(), booleanExtensionsType, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>The indices of the rewrites whose <c>Then</c> call does not bind to Funcky's extension in the changed document; all of
    /// them when the document cannot be bound at all.</summary>
    private static async Task<ImmutableArray<int>> UnboundRewritesAsync(Document document, int rewriteCount, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        return root is not null && semanticModel is not null && semanticModel.Compilation.GetBooleanExtensionsType() is { } booleanExtensionsType
            ? FunckyExtensionsImport.Replacements(root)
                .Where(replacement => !IsThen(semanticModel.GetSymbolInfo(replacement.Replacement, cancellationToken).Symbol, booleanExtensionsType))
                .Select(replacement => replacement.Index)
                .ToImmutableArray()
            : ImmutableArray.CreateRange(Enumerable.Range(0, rewriteCount));
    }

    // A member access binds tighter than every operator, so anything but a primary expression needs parentheses.
    // The default literal has no type of its own, so the receiver spells it out.
    private static ExpressionSyntax Receiver(ExpressionSyntax condition)
        => condition.WithoutTrivia() switch
        {
            LiteralExpressionSyntax { RawKind: (int)SyntaxKind.DefaultLiteralExpression } => DefaultExpression(PredefinedType(Token(SyntaxKind.BoolKeyword))),
            IdentifierNameSyntax or MemberAccessExpressionSyntax or InvocationExpressionSyntax or ElementAccessExpressionSyntax
                or ParenthesizedExpressionSyntax or LiteralExpressionSyntax or ThisExpressionSyntax or ObjectCreationExpressionSyntax
                or DefaultExpressionSyntax => condition.WithoutTrivia(),
            var other => ParenthesizedExpression(other),
        };
}
