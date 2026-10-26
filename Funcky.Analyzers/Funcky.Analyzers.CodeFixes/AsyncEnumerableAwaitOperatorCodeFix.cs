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
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncEnumerableAwaitOperatorCodeFix))]
public sealed class AsyncEnumerableAwaitOperatorCodeFix : CodeFixProvider
{
    /// <summary>The compiler diagnostic reported on the use of a member marked <c>[Obsolete]</c> with a message.</summary>
    private const string ObsoleteMemberDiagnosticId = "CS0618";

    private const string AwaitSuffix = "Await";

    private const string AwaitWithCancellationSuffix = "AwaitWithCancellation";

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
                && DelegateArguments(operation) is { } delegateArguments
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
    private static ImmutableArray<DelegateArgument>? DelegateArguments(IInvocationOperation operation)
    {
        var builder = ImmutableArray.CreateBuilder<DelegateArgument>();

        foreach (var argument in operation.Arguments.OrderBy(argument => argument.Parameter?.Ordinal ?? int.MaxValue))
        {
            if (argument.Parameter?.Type is INamedTypeSymbol { TypeKind: TypeKind.Delegate, DelegateInvokeMethod: { } invoke })
            {
                if (argument.Syntax is not ArgumentSyntax syntax)
                {
                    return null;
                }

                builder.Add(new DelegateArgument(syntax, invoke.Parameters.Length));
            }
        }

        return builder.ToImmutable();
    }

    // A lambda gets the discard appended; a method group or a delegate value is wrapped in a lambda that forwards its parameters.
    private static bool IsRewritable(ArgumentSyntax argument)
        => argument.Expression is not AnonymousMethodExpressionSyntax;

    private sealed record DelegateArgument(ArgumentSyntax Syntax, int Arity);

    private sealed record Replacement(string Name, bool AddsTokenParameter)
    {
        public static Replacement? TryCreate(string obsoleteName)
            => obsoleteName switch
            {
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

        private static ExpressionSyntax WithTokenParameter(DelegateArgument argument, SemanticModel semanticModel)
            => (argument.Syntax.Expression switch
            {
                SimpleLambdaExpressionSyntax lambda => AppendParameter(lambda, Discard(typed: lambda.Parameter.Type is not null)),
                ParenthesizedLambdaExpressionSyntax lambda => lambda.WithParameterList(lambda.ParameterList.AddParameters(Discard(typed: lambda.ParameterList.Parameters.All(parameter => parameter.Type is not null)))),
                var callable => ForwardingLambda(callable, argument, semanticModel),
            }).WithTriviaFrom(argument.Syntax.Expression);

        private static ParenthesizedLambdaExpressionSyntax AppendParameter(SimpleLambdaExpressionSyntax lambda, ParameterSyntax parameter)
            => ParenthesizedLambdaExpression((CSharpSyntaxNode?)lambda.Block ?? lambda.ExpressionBody!)
                .WithAttributeLists(lambda.AttributeLists)
                .WithModifiers(lambda.Modifiers)
                .WithParameterList(ParameterList(SeparatedList([lambda.Parameter.WithoutTrivia(), parameter])))
                .WithArrowToken(lambda.ArrowToken);

        // `xs.FooAwait(Callable)` becomes `xs.Foo((x, _) => Callable(x))`; the names avoid anything visible at the call site.
        private static ParenthesizedLambdaExpressionSyntax ForwardingLambda(ExpressionSyntax callable, DelegateArgument argument, SemanticModel semanticModel)
        {
            var names = ParameterNames(argument.Arity, semanticModel, argument.Syntax.SpanStart);
            var parameters = names.Select(name => Parameter(Identifier(name))).Append(Discard(typed: false));
            var arguments = names.Select(name => Argument(IdentifierName(name)));

            return ParenthesizedLambdaExpression(InvocationExpression(callable.WithoutTrivia(), ArgumentList(SeparatedList(arguments))))
                .WithParameterList(ParameterList(SeparatedList(parameters)));
        }

        private static IReadOnlyList<string> ParameterNames(int arity, SemanticModel semanticModel, int position)
            => CandidateParameterNames
                .Take(arity)
                .Select(candidate => semanticModel.LookupSymbols(position, name: candidate).IsEmpty ? candidate : candidate + arity)
                .ToList();

        private static ParameterSyntax Discard(bool typed)
            => typed
                ? Parameter(Identifier(DiscardName)).WithType(ParseTypeName("global::System.Threading.CancellationToken").WithAdditionalAnnotations(Simplifier.Annotation))
                : Parameter(Identifier(DiscardName));
    }
}
