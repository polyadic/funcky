using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
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

    // The analyzer only reports a lambda when the call also binds identically with every other reported lambda of the
    // same call replaced, so the batch fixer cannot combine individually safe replacements into a broken call.
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

        var (replacement, addUsingStatic) = ReplacementExpression(semanticModel, root, anonymousFunction, functionName);
        return document.WithSyntaxRoot(Replace(semanticModel, root, anonymousFunction, replacement.WithTriviaFrom(anonymousFunction), addUsingStatic));
    }

    /// <summary>The bare name is used when it already binds to <c>Funcky.Functional</c>, or when it binds to nothing and a <c>using static</c>
    /// can be added without disturbing the rest of the file. Otherwise the qualified name is used.</summary>
    private static (ExpressionSyntax Replacement, bool AddUsingStatic) ReplacementExpression(SemanticModel semanticModel, SyntaxNode root, AnonymousFunctionExpressionSyntax anonymousFunction, string functionName)
    {
        var symbolInfo = semanticModel.GetSpeculativeSymbolInfo(anonymousFunction.SpanStart, IdentifierName(functionName), SpeculativeBindingOption.BindAsExpression);
        var symbols = symbolInfo.Symbol is { } symbol ? ImmutableArray.Create(symbol) : symbolInfo.CandidateSymbols;

        return symbols switch
        {
            { IsEmpty: true } when !ImportWouldClash(semanticModel, root, anonymousFunction) => (IdentifierName(functionName), AddUsingStatic: true),
            { IsEmpty: false } when symbols.All(IsFunctionalMember) => (IdentifierName(functionName), AddUsingStatic: false),
            _ => (ParseExpression($"global::{FunctionalNamespace}.{FunctionalTypeName}.{functionName}").WithAdditionalAnnotations(Simplifier.Annotation), AddUsingStatic: false),
        };
    }

    // `using static Funcky.Functional` imports every member of the type. A bare name elsewhere in the file that matches one of
    // them, say a `True()` from another static import, would become ambiguous, so the file is scanned for such names first.
    private static bool ImportWouldClash(SemanticModel semanticModel, SyntaxNode root, AnonymousFunctionExpressionSyntax anonymousFunction)
    {
        if (semanticModel.Compilation.GetFunctionalType() is not { } functionalType)
        {
            return true;
        }

        var importedNames = functionalType.GetMembers().Where(member => member.IsStatic && member.DeclaredAccessibility == Accessibility.Public).Select(member => member.Name).ToImmutableHashSet();

        return root
            .DescendantNodes(node => node != anonymousFunction)
            .OfType<IdentifierNameSyntax>()
            .Any(identifier => importedNames.Contains(identifier.Identifier.ValueText) && !HasExplicitReceiver(identifier));
    }

    private static bool HasExplicitReceiver(IdentifierNameSyntax identifier)
        => (identifier.Parent is MemberAccessExpressionSyntax { Name: var name } && name == identifier)
           || (identifier.Parent is QualifiedNameSyntax { Right: var right } && right == identifier)
           || identifier.Parent is MemberBindingExpressionSyntax;

    private static bool IsFunctionalMember(ISymbol symbol)
        => symbol.ContainingType is { Name: FunctionalTypeName, ContainingNamespace: { Name: FunctionalNamespace, ContainingNamespace.IsGlobalNamespace: true } };

    /// <summary>The <c>using static</c> goes next to the existing usings: into the enclosing namespace when the usings are there, otherwise at the top of the file.</summary>
    private static SyntaxNode Replace(SemanticModel semanticModel, SyntaxNode root, AnonymousFunctionExpressionSyntax anonymousFunction, ExpressionSyntax replacement, bool addUsingStatic)
        => (addUsingStatic, anonymousFunction.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>(namespaceDeclaration => namespaceDeclaration.Usings.Count > 0)) switch
        {
            (false, _) => root.ReplaceNode(anonymousFunction, replacement),
            (true, { } namespaceDeclaration) => root.ReplaceNodes(
                new SyntaxNode[] { anonymousFunction, namespaceDeclaration },
                (original, rewritten) => original == anonymousFunction
                    ? replacement
                    : WithUsingStatic((BaseNamespaceDeclarationSyntax)rewritten, semanticModel, namespaceDeclaration.Usings[0].SpanStart)),
            (true, null) => WithUsingStatic((CompilationUnitSyntax)root.ReplaceNode(anonymousFunction, replacement), semanticModel, position: 0),
        };

    /// <summary>The <c>using static</c> goes after the last using outside a <c>#if</c> region, see <see cref="UsingDirectives.Insert{TContainer}"/>.</summary>
    private static TContainer WithUsingStatic<TContainer>(TContainer container, SemanticModel semanticModel, int position)
        where TContainer : SyntaxNode
        => UsingDirectives.Insert(container, UsingDirectives.ParseUsingDirective($"using static {QualifiedFunctionalName(semanticModel, position)};"));

    // Inside a namespace that contains its own `Funcky` namespace a relative `Funcky.Functional` would bind to that one.
    private static string QualifiedFunctionalName(SemanticModel semanticModel, int position)
        => semanticModel.LookupNamespacesAndTypes(position, name: FunctionalNamespace).Any(symbol => symbol is not INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: true })
            ? $"global::{FunctionalNamespace}.{FunctionalTypeName}"
            : $"{FunctionalNamespace}.{FunctionalTypeName}";
}
