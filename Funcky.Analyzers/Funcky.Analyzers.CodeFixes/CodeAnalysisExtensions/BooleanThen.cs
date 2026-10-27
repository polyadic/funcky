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

    private static readonly SyntaxAnnotation ReplacementAnnotation = new();

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
    public static async Task<Document?> TryReplaceAsync(Document document, SyntaxNode root, SemanticModel semanticModel, ExpressionSyntax target, InvocationExpressionSyntax replacement, INamedTypeSymbol booleanExtensionsType, CancellationToken cancellationToken)
    {
        var rootWithReplacement = root.ReplaceNode(target, replacement.WithAdditionalAnnotations(ReplacementAnnotation));

        if (IsThen(semanticModel.GetSpeculativeSymbolInfo(target.SpanStart, replacement, SpeculativeBindingOption.BindAsExpression).Symbol, booleanExtensionsType))
        {
            return document.WithSyntaxRoot(rootWithReplacement);
        }

        var documentWithUsing = document.WithSyntaxRoot(UsingDirectives.AddUsing(rootWithReplacement, rootWithReplacement.GetAnnotatedNodes(ReplacementAnnotation).Single(), BooleanExtensionsNamespace));

        return await BindsToThenAsync(documentWithUsing, cancellationToken).ConfigureAwait(false)
            ? documentWithUsing
            : null;
    }

    private static async Task<bool> BindsToThenAsync(Document document, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        return root is not null
               && semanticModel is not null
               && semanticModel.Compilation.GetBooleanExtensionsType() is { } booleanExtensionsType
               && IsThen(semanticModel.GetSymbolInfo(root.GetAnnotatedNodes(ReplacementAnnotation).Single(), cancellationToken).Symbol, booleanExtensionsType);
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
