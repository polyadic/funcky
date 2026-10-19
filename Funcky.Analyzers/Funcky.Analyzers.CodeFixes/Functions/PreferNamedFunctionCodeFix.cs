using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.Functions;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferNamedFunctionCodeFix))]
public sealed class PreferNamedFunctionCodeFix : CodeFixProvider
{
    private const string FunctionalNamespace = "Funcky";

    private const string FunctionalTypeName = "Functional";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferNamedFunctionAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root?.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is AnonymousFunctionExpressionSyntax anonymousFunction
                && diagnostic.Properties.TryGetValue(PreferNamedFunctionAnalyzer.FunctionNameProperty, out var functionName)
                && functionName is not null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        $"Replace with '{functionName}'",
                        cancellationToken => ReplaceWithNamedFunctionAsync(context.Document, anonymousFunction, functionName, cancellationToken),
                        nameof(PreferNamedFunctionCodeFix)),
                    diagnostic);
            }
        }
    }

    private static async Task<Document> ReplaceWithNamedFunctionAsync(Document document, AnonymousFunctionExpressionSyntax anonymousFunction, string functionName, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not { } root
            || await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is not { } semanticModel)
        {
            return document;
        }

        var (replacement, addUsingStatic) = ReplacementExpression(semanticModel, anonymousFunction, functionName);
        return document.WithSyntaxRoot(Replace(root, anonymousFunction, replacement.WithTriviaFrom(anonymousFunction), addUsingStatic));
    }

    /// <summary>The bare name is used when it already binds to <c>Funcky.Functional</c> or to nothing at all (in which case a <c>using static</c> is added).
    /// When another member of the same name is in scope, the qualified name is used instead.</summary>
    private static (ExpressionSyntax Replacement, bool AddUsingStatic) ReplacementExpression(SemanticModel semanticModel, SyntaxNode anonymousFunction, string functionName)
    {
        var symbolInfo = semanticModel.GetSpeculativeSymbolInfo(anonymousFunction.SpanStart, IdentifierName(functionName), SpeculativeBindingOption.BindAsExpression);
        var symbols = symbolInfo.Symbol is { } symbol ? ImmutableArray.Create(symbol) : symbolInfo.CandidateSymbols;

        return symbols switch
        {
            { IsEmpty: true } => (IdentifierName(functionName), AddUsingStatic: true),
            _ when symbols.All(IsFunctionalMember) => (IdentifierName(functionName), AddUsingStatic: false),
            _ => (ParseExpression($"global::{FunctionalNamespace}.{FunctionalTypeName}.{functionName}").WithAdditionalAnnotations(Simplifier.Annotation), AddUsingStatic: false),
        };
    }

    private static bool IsFunctionalMember(ISymbol symbol)
        => symbol.ContainingType is { Name: FunctionalTypeName, ContainingNamespace: { Name: FunctionalNamespace, ContainingNamespace.IsGlobalNamespace: true } };

    /// <summary>The <c>using static</c> goes next to the existing usings: into the enclosing namespace when the usings are there, otherwise at the top of the file.</summary>
    private static SyntaxNode Replace(SyntaxNode root, AnonymousFunctionExpressionSyntax anonymousFunction, ExpressionSyntax replacement, bool addUsingStatic)
        => (addUsingStatic, anonymousFunction.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>(namespaceDeclaration => namespaceDeclaration.Usings.Count > 0)) switch
        {
            (false, _) => root.ReplaceNode(anonymousFunction, replacement),
            (true, { } namespaceDeclaration) => root.ReplaceNodes(
                new SyntaxNode[] { anonymousFunction, namespaceDeclaration },
                (original, rewritten) => original == anonymousFunction
                    ? replacement
                    : ((BaseNamespaceDeclarationSyntax)rewritten).AddUsings(UsingStaticDirective(rewritten))),
            (true, null) => ((CompilationUnitSyntax)root.ReplaceNode(anonymousFunction, replacement)).AddUsings(UsingStaticDirective(root)),
        };

    /// <summary>The directive is parsed rather than built from tokens so that it carries no elastic trivia, which the formatter would otherwise
    /// normalize together with the surrounding line endings. It takes its indentation and line ending from the last existing using.</summary>
    private static UsingDirectiveSyntax UsingStaticDirective(SyntaxNode container)
    {
        var usings = container switch
        {
            CompilationUnitSyntax compilationUnit => compilationUnit.Usings,
            BaseNamespaceDeclarationSyntax namespaceDeclaration => namespaceDeclaration.Usings,
            _ => default,
        };
        var usingDirective = ParseCompilationUnit($"using static {FunctionalNamespace}.{FunctionalTypeName};").Usings.Single();
        return usings.LastOrDefault() is { } lastUsing
            ? usingDirective.WithLeadingTrivia(lastUsing.GetLeadingTrivia()).WithTrailingTrivia(lastUsing.GetTrailingTrivia())
            : usingDirective.WithTrailingTrivia(CarriageReturnLineFeed, CarriageReturnLineFeed);
    }
}
