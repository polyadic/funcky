using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

/// <summary>
/// Offers a fix for the obsoletion warning CS0618 on the <c>…Await</c> and <c>…AwaitWithCancellation</c> operators of
/// <c>AsyncEnumerableExtensions</c>, which the single-name overloads with a <c>(…, cancellationToken)</c> delegate replace:
/// <c>xs.FooAwaitWithCancellation((x, ct) => …)</c> becomes <c>xs.Foo((x, ct) => …)</c>, and <c>xs.FooAwait(x => …)</c>
/// becomes <c>xs.Foo((x, _) => …)</c>, with the ignored token parameter added to every delegate argument.
/// The terminal <c>…AwaitAsync</c> and <c>…AwaitWithCancellationAsync</c> aggregations map to <c>…Async</c> the same way.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncEnumerableAwaitOperatorCodeFix))]
public sealed class AsyncEnumerableAwaitOperatorCodeFix : CodeFixProvider
{
    /// <summary>The compiler diagnostic reported on the use of a member marked <c>[Obsolete]</c> with a message.</summary>
    private const string ObsoleteMemberDiagnosticId = "CS0618";

    private const string AwaitSuffix = "Await";

    private const string AwaitWithCancellationSuffix = "AwaitWithCancellation";

    private const string AsyncSuffix = "Async";

    private const string DiscardName = "_";

    private static readonly string[] CandidateParameterNames = ["x", "y", "z", "w"];

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ObsoleteMemberDiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null || semanticModel is null || semanticModel.Compilation.GetAsyncEnumerableExtensionsType() is not { } extensionsType)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            // The warning spans the whole call; one with a smaller span belongs to an obsolete member inside an argument.
            if (root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
                && invocation.Span == diagnostic.Location.SourceSpan
                && GetMethodName(invocation) is { } name
                && semanticModel.GetOperation(invocation, context.CancellationToken) is IInvocationOperation { TargetMethod: var method } operation
                && IsDeclaredIn(method, extensionsType)
                && Replacement.TryCreate(method.Name) is { } replacement
                && HasMember(extensionsType, replacement.Name)
                && DelegateArguments(operation, invocation) is { } delegateArguments
                && (!replacement.AddsTokenParameter || delegateArguments.All(argument => IsRewritable(argument.Syntax))))
            {
                context.RegisterCodeFix(new RenameOperatorCodeFixAction(context.Document, invocation, name, replacement, delegateArguments), diagnostic);
            }
        }
    }

    private static SimpleNameSyntax? GetMethodName(InvocationExpressionSyntax invocation)
        => invocation.Expression switch
        {
            SimpleNameSyntax simpleName => simpleName,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name,
            _ => null,
        };

    // An extension member lives in an unnamed extension type nested in the static class; a classic extension method directly in it.
    private static bool IsDeclaredIn(IMethodSymbol method, INamedTypeSymbol type)
        => (method.ReducedFrom ?? method) is { ContainingType: { } containingType }
           && (SymbolEqualityComparer.Default.Equals(containingType, type)
               || SymbolEqualityComparer.Default.Equals(containingType.ContainingType, type));

    private static bool HasMember(INamedTypeSymbol type, string name)
        => type.GetMembers(name).Any()
           || type.GetTypeMembers().Any(extension => extension.GetMembers(name).Any());

    /// <summary>The arguments bound to a delegate parameter, with the delegate's arity, in parameter order.</summary>
    private static ImmutableArray<DelegateArgument>? DelegateArguments(IInvocationOperation operation, InvocationExpressionSyntax invocation)
    {
        var builder = ImmutableArray.CreateBuilder<DelegateArgument>();

        foreach (var argument in operation.Arguments.OrderBy(argument => argument.Parameter?.Ordinal ?? int.MaxValue))
        {
            if (argument.Parameter?.Type is INamedTypeSymbol { TypeKind: TypeKind.Delegate, DelegateInvokeMethod: { } invoke })
            {
                if (ArgumentSyntax(argument, invocation) is not { } syntax)
                {
                    return null;
                }

                builder.Add(new DelegateArgument(syntax, invoke.Parameters.Length));
            }
        }

        return builder.ToImmutable();
    }

    // The operation's syntax is the argument, or the bare expression when parentheses surround it, which the operation tree skips.
    private static ArgumentSyntax? ArgumentSyntax(IArgumentOperation argument, InvocationExpressionSyntax invocation)
        => argument.Syntax.FirstAncestorOrSelf<ArgumentSyntax>() is { } syntax && syntax.Parent == invocation.ArgumentList
            ? syntax
            : null;

    // A lambda gets the token parameter appended; a method group or a delegate value is wrapped in a lambda that forwards its parameters.
    private static bool IsRewritable(ArgumentSyntax argument)
        => Unparenthesize(argument.Expression) switch
        {
            AnonymousMethodExpressionSyntax => false,
            LambdaExpressionSyntax => true,
            var callable => IsReferentiallyStable(callable),
        };

    // The forwarding lambda evaluates the callable per element instead of once at the call: only names, member paths and
    // casts of those keep the same meaning. An invocation, an element access or a `new` would run once per element.
    private static bool IsReferentiallyStable(ExpressionSyntax expression)
        => expression switch
        {
            SimpleNameSyntax or AliasQualifiedNameSyntax or ThisExpressionSyntax or BaseExpressionSyntax or PredefinedTypeSyntax => true,
            MemberAccessExpressionSyntax memberAccess => IsReferentiallyStable(memberAccess.Expression),
            CastExpressionSyntax cast => IsReferentiallyStable(cast.Expression),
            ParenthesizedExpressionSyntax parenthesized => IsReferentiallyStable(parenthesized.Expression),
            _ => false,
        };

    private static ExpressionSyntax Unparenthesize(ExpressionSyntax expression)
        => expression is ParenthesizedExpressionSyntax parenthesized
            ? Unparenthesize(parenthesized.Expression)
            : expression;

    private static ImmutableHashSet<string> IdentifiersIn(SyntaxNode node)
        => node.DescendantTokens().Where(token => token.IsKind(SyntaxKind.IdentifierToken)).Select(token => token.ValueText).ToImmutableHashSet();

    private sealed record DelegateArgument(ArgumentSyntax Syntax, int Arity);

    private sealed record Replacement(string Name, bool AddsTokenParameter)
    {
        // Lazy operators: FooAwait → Foo, FooAwaitWithCancellation → Foo. Terminal aggregations: FooAwaitAsync → FooAsync,
        // FooAwaitWithCancellationAsync → FooAsync.
        public static Replacement? TryCreate(string obsoleteName)
            => obsoleteName switch
            {
                _ when EndsWith(obsoleteName, AwaitWithCancellationSuffix + AsyncSuffix) => new Replacement(obsoleteName[..^(AwaitWithCancellationSuffix.Length + AsyncSuffix.Length)] + AsyncSuffix, AddsTokenParameter: false),
                _ when EndsWith(obsoleteName, AwaitSuffix + AsyncSuffix) => new Replacement(obsoleteName[..^(AwaitSuffix.Length + AsyncSuffix.Length)] + AsyncSuffix, AddsTokenParameter: true),
                _ when EndsWith(obsoleteName, AwaitWithCancellationSuffix) => new Replacement(obsoleteName[..^AwaitWithCancellationSuffix.Length], AddsTokenParameter: false),
                _ when EndsWith(obsoleteName, AwaitSuffix) => new Replacement(obsoleteName[..^AwaitSuffix.Length], AddsTokenParameter: true),
                _ => null,
            };

        private static bool EndsWith(string name, string suffix)
            => name.Length > suffix.Length && name.EndsWith(suffix, StringComparison.Ordinal);
    }

    private sealed class RenameOperatorCodeFixAction(
        Document document,
        InvocationExpressionSyntax invocation,
        SimpleNameSyntax name,
        Replacement replacement,
        ImmutableArray<DelegateArgument> delegateArguments) : CodeAction
    {
        public override string Title => $"Replace {name.Identifier.ValueText} with {replacement.Name}";

        public override string EquivalenceKey => nameof(AsyncEnumerableAwaitOperatorCodeFix);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (root is null || semanticModel is null)
            {
                return document;
            }

            var replacements = new Dictionary<SyntaxNode, SyntaxNode> { [name] = RenamedName() };

            if (replacement.AddsTokenParameter)
            {
                foreach (var argument in delegateArguments)
                {
                    replacements[argument.Syntax.Expression] = WithTokenParameter(argument, semanticModel);
                }
            }

            return document.WithSyntaxRoot(root.ReplaceNode(invocation, invocation.ReplaceNodes(replacements.Keys, (original, _) => replacements[original])));
        }

        private SimpleNameSyntax RenamedName()
            => (name is GenericNameSyntax genericName
                ? (SimpleNameSyntax)genericName.WithIdentifier(Identifier(replacement.Name))
                : IdentifierName(replacement.Name)).WithTriviaFrom(name);

        // The token parameter is appended to the lambda found inside any parentheses; a callable replaces the whole argument.
        private static ExpressionSyntax WithTokenParameter(DelegateArgument argument, SemanticModel semanticModel)
        {
            var expression = argument.Syntax.Expression;
            var names = new NameSource(IdentifiersIn(expression), semanticModel, expression.SpanStart);

            return Unparenthesize(expression) switch
            {
                SimpleLambdaExpressionSyntax lambda => Replace(expression, lambda, AppendParameter(lambda, TokenParameter(names, typed: lambda.Parameter.Type is not null))),
                ParenthesizedLambdaExpressionSyntax lambda => Replace(expression, lambda, lambda.WithParameterList(lambda.ParameterList.AddParameters(TokenParameter(names, typed: lambda.ParameterList.Parameters.All(parameter => parameter.Type is not null))))),
                var callable => ForwardingLambda(callable, argument.Arity, names).WithTriviaFrom(expression),
            };
        }

        private static ExpressionSyntax Replace(ExpressionSyntax expression, ExpressionSyntax original, ExpressionSyntax replacement)
            => expression == original
                ? replacement.WithTriviaFrom(original)
                : expression.ReplaceNode(original, replacement.WithTriviaFrom(original));

        private static ParenthesizedLambdaExpressionSyntax AppendParameter(SimpleLambdaExpressionSyntax lambda, ParameterSyntax parameter)
            => ParenthesizedLambdaExpression((CSharpSyntaxNode?)lambda.Block ?? lambda.ExpressionBody!)
                .WithAttributeLists(lambda.AttributeLists)
                .WithModifiers(lambda.Modifiers)
                .WithParameterList(ParameterList(SeparatedList([lambda.Parameter.WithoutTrivia(), parameter])))
                .WithArrowToken(lambda.ArrowToken);

        // `xs.FooAwait(Callable)` becomes `xs.Foo((x, _) => Callable(x))`; the names avoid anything visible at the call site.
        private static ParenthesizedLambdaExpressionSyntax ForwardingLambda(ExpressionSyntax callable, int arity, NameSource names)
        {
            var parameterNames = CandidateParameterNames.Take(arity).Select(names.Unused).ToList();
            var parameters = parameterNames.Select(name => Parameter(Identifier(name))).Append(TokenParameter(names, typed: false));
            var arguments = parameterNames.Select(name => Argument(IdentifierName(name)));

            return ParenthesizedLambdaExpression(InvocationExpression(InvocationTarget(callable.WithoutTrivia()), ArgumentList(SeparatedList(arguments))))
                .WithParameterList(ParameterList(SeparatedList(parameters)));
        }

        // Only a primary expression can be invoked as is: `(Func<int, Task>)Log(x)` would cast the result of `Log(x)`.
        private static ExpressionSyntax InvocationTarget(ExpressionSyntax callable)
            => callable is SimpleNameSyntax or MemberAccessExpressionSyntax or AliasQualifiedNameSyntax or ThisExpressionSyntax or BaseExpressionSyntax
                ? callable
                : ParenthesizedExpression(callable);

        private static ParameterSyntax TokenParameter(NameSource names, bool typed)
            => typed
                ? Parameter(Identifier(names.Discard())).WithType(ParseTypeName("global::System.Threading.CancellationToken").WithAdditionalAnnotations(Simplifier.Annotation))
                : Parameter(Identifier(names.Discard()));

        /// <summary>Picks parameter names that neither shadow a symbol visible at the argument nor appear in the argument
        /// itself: a second <c>_</c> parameter would turn the lambda's own <c>_</c> parameter into a discard and a lone
        /// <c>_</c> parameter would capture the <c>out _</c> and <c>_ = …</c> discards in the body.</summary>
        private sealed class NameSource(ImmutableHashSet<string> identifiersInArgument, SemanticModel semanticModel, int position)
        {
            public string Discard()
                => Enumerable.Range(1, int.MaxValue)
                    .Select(length => new string(DiscardName[0], length))
                    .First(IsUnused);

            public string Unused(string candidate)
                => Enumerable.Repeat(candidate, 1)
                    .Concat(Enumerable.Range(1, int.MaxValue).Select(suffix => candidate + suffix))
                    .First(IsUnused);

            private bool IsUnused(string name)
                => !identifiersInArgument.Contains(name) && semanticModel.LookupSymbols(position, name: name).IsEmpty;
        }
    }
}
