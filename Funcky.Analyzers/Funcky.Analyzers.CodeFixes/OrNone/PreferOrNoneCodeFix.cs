using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Funcky.Analyzers.Fallback;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.OrNone;

/// <summary>Rewrites <c>xs.FirstOrDefault() ?? fallback</c> to <c>xs.FirstOrNone().GetOrElse(fallback)</c>.
/// The other null checks (<c>?.</c>, <c>is null</c>, …) have no single idiomatic shape, so they are reported without a fix.</summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferOrNoneCodeFix))]
public sealed class PreferOrNoneCodeFix : ExtensionMemberCodeFix
{
    private const string GetOrElseMethodName = "GetOrElse";

    private enum FallbackKind
    {
        Eager,
        Lazy,
    }

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferOrNoneAnalyzer.DiagnosticId);

    // The replacement is `xs.FirstOrNone().GetOrElse(fallback)`.
    protected override string GetTitle(ExtensionMemberRewrite rewrite)
        => $"Replace with '{OrNoneMemberAccess(rewrite).Name.Identifier.Text}().{GetOrElseMethodName}(…)'";

    protected override ExtensionMemberRewrite? TryCreateRewrite(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, CancellationToken cancellationToken)
        => root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { Expression: MemberAccessExpressionSyntax memberAccess } invocation
           && diagnostic.Properties.TryGetValue(PreferOrNoneAnalyzer.OrNoneMethodNameProperty, out var orNoneMethodName)
           && orNoneMethodName is not null
           && FindCoalesceExpression(invocation) is { } coalesce
           && KeepsTheResultType(semanticModel, invocation, coalesce, cancellationToken)
           && ClassifyFallback(semanticModel, Unparenthesized(coalesce.Right), cancellationToken) is { } fallbackKind
            ? new ExtensionMemberRewrite(coalesce, GetOrElseInvocation(coalesce, invocation, memberAccess, orNoneMethodName, fallbackKind))
            : null;

    /// <summary>The replacement binds to <c>Option.GetOrElse</c>, a member of <c>Funcky.Monads</c>, when its <c>…OrNone</c> receiver binds,
    /// which it does only with the <c>using</c> for <c>Funcky.Extensions</c> in place. Any bound method therefore shows the directive is there.</summary>
    protected override Task<Document?> TryApplyAsync(Document document, SyntaxNode root, SemanticModel semanticModel, ImmutableArray<ExtensionMemberRewrite> rewrites, CancellationToken cancellationToken)
        => Task.FromResult<Document?>(FunckyExtensionsImport.ReplaceWithExtensionMemberCalls(document, root, semanticModel, rewrites, static symbol => symbol is IMethodSymbol));

    private static MemberAccessExpressionSyntax OrNoneMemberAccess(ExtensionMemberRewrite rewrite)
        => (MemberAccessExpressionSyntax)((InvocationExpressionSyntax)((MemberAccessExpressionSyntax)((InvocationExpressionSyntax)rewrite.Replacement).Expression).Expression).Expression;

    private static BinaryExpressionSyntax? FindCoalesceExpression(InvocationExpressionSyntax invocation)
        => SkipParentheses(invocation) is { Parent: BinaryExpressionSyntax { RawKind: (int)SyntaxKind.CoalesceExpression } coalesce } left && coalesce.Left == left
            ? coalesce
            : null;

    private static SyntaxNode SkipParentheses(SyntaxNode node)
        => node.Parent is ParenthesizedExpressionSyntax parenthesized ? SkipParentheses(parenthesized) : node;

    // The operation tree has no node for parentheses in C#, and as an argument the fallback needs none.
    private static ExpressionSyntax Unparenthesized(ExpressionSyntax expression)
        => expression is ParenthesizedExpressionSyntax parenthesized ? Unparenthesized(parenthesized.Expression) : expression;

    /// <summary>A <c>??</c> may widen the type: <c>values.FirstOrDefault() ?? new object()</c> is an <c>object</c>, but
    /// <c>Option&lt;string&gt;.GetOrElse</c> only takes a <c>string</c>. The fix is offered when the coalesce has the element type.</summary>
    private static bool KeepsTheResultType(SemanticModel semanticModel, InvocationExpressionSyntax invocation, BinaryExpressionSyntax coalesce, CancellationToken cancellationToken)
        => SymbolEquals(semanticModel.GetTypeInfo(coalesce, cancellationToken).Type, semanticModel.GetTypeInfo(invocation, cancellationToken).Type);

    /// <summary>A <c>??</c> evaluates its fallback only when the left side is <c>null</c>. A fallback that does work (a call, an object
    /// creation, an interpolated string, a <c>throw</c>) therefore goes to the lazy overload <c>GetOrElse(() => fallback)</c>, a plain
    /// value to the eager one, so that the rewritten code is what λ1014, λ1015 and λ1016 ask for. There is no fix when the
    /// work cannot move into a lambda (an <c>await</c>, a <c>ref</c> parameter, a variable declared in the expression).</summary>
    private static FallbackKind? ClassifyFallback(SemanticModel semanticModel, ExpressionSyntax fallback, CancellationToken cancellationToken)
        => (DoesWork(semanticModel, fallback, cancellationToken), LambdaWrapping.CanWrapInLambda(semanticModel, fallback, cancellationToken)) switch
        {
            (false, _) => FallbackKind.Eager,
            (true, true) => FallbackKind.Lazy,
            (true, false) => null,
        };

    private static bool DoesWork(SemanticModel semanticModel, ExpressionSyntax fallback, CancellationToken cancellationToken)
        => semanticModel.GetOperation(fallback, cancellationToken) is { } operation
           && DescendantsAndSelfOutsideFunctions(operation).Any(descendant => descendant
               is IInvocationOperation
               or IObjectCreationOperation or IAnonymousObjectCreationOperation or IArrayCreationOperation
               or IDynamicInvocationOperation or IDynamicObjectCreationOperation
               or IAwaitOperation or IInterpolatedStringOperation or IThrowOperation);

    // What happens inside a nested function is deferred already.
    private static IEnumerable<IOperation> DescendantsAndSelfOutsideFunctions(IOperation operation)
        => operation is IAnonymousFunctionOperation or ILocalFunctionOperation
            ? []
            : [operation, .. operation.ChildOperations.SelectMany(DescendantsAndSelfOutsideFunctions)];

    private static InvocationExpressionSyntax GetOrElseInvocation(BinaryExpressionSyntax coalesce, InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess, string orNoneMethodName, FallbackKind fallbackKind)
    {
        var orNoneInvocation = invocation.WithExpression(memberAccess.WithName(RenamedTo(memberAccess.Name, orNoneMethodName)));

        return InvocationExpression(
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, orNoneInvocation.WithoutTrailingTrivia(), IdentifierName(GetOrElseMethodName)),
                ArgumentList(SingletonSeparatedList(Argument(Fallback(Unparenthesized(coalesce.Right), fallbackKind)))))
            .WithTriviaFrom(coalesce);
    }

    /// <summary>Only the identifier changes: explicit type arguments (<c>FirstOrDefault&lt;object&gt;()</c>) select the element type and stay.</summary>
    private static SimpleNameSyntax RenamedTo(SimpleNameSyntax name, string newName)
        => name.WithIdentifier(Identifier(newName).WithTriviaFrom(name.Identifier));

    private static ExpressionSyntax Fallback(ExpressionSyntax fallback, FallbackKind fallbackKind)
        => fallbackKind == FallbackKind.Lazy
            ? ParenthesizedLambdaExpression(fallback.WithoutTrivia())
            : fallback.WithoutTrivia();
}
