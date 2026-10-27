using System.Collections.Immutable;
using Funcky.Analyzers.ExtensionForms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Funcky.Analyzers.Test;

/// <summary>Applies the fixes to the changed solution directly, the way a host that keeps the syntax trees does, where the test
/// harness re-parses every document between two fixes.</summary>
public sealed class ExtensionMemberCodeFixTest
{
    private const string ObsoleteMember = "CS0618";

    // language=csharp
    private const string InputCode =
        """
        #nullable enable
        using Funcky.Monads;

        public static class C
        {
            public static Option<int> Parse(string text) => Funcky.Extensions.ParseExtensions.ParseInt32OrNone(text);

            public static Option<int> Then(bool flag, int number) => Option.FromBoolean(flag, number);
        }

        namespace Funcky
        {
            public readonly struct Unit
            {
                public static Unit Value => default;
            }
        }

        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
                where TItem : notnull
            {
            }

            public static class Option
            {
                [System.Obsolete("Use condition.Then(item) instead.")]
                public static Option<TItem> FromBoolean<TItem>(bool boolean, TItem item) where TItem : notnull => default;
            }
        }

        namespace Funcky.Extensions
        {
            using Funcky.Monads;

            public static class ParseExtensions
            {
                public static Option<int> ParseInt32OrNone(this string candidate) => default;
            }

            public static class Int32ParseExtensions
            {
                extension(int)
                {
                    public static Option<int> ParseOrNone(string candidate) => default;
                }
            }

            public static class BooleanExtensions
            {
                public static Option<TItem> Then<TItem>(this bool condition, TItem value) where TItem : notnull => default;
            }
        }
        """;

    [Fact]
    public async Task AFixLeavesNoTraceThatConfusesALaterFixOnTheSameDocument()
    {
        using var workspace = new AdhocWorkspace();
        var document = await CreateDocumentAsync(workspace);

        var parseDiagnostic = Assert.Single(await GetAnalyzerDiagnosticsAsync(document, new PreferStaticParseOrNoneAnalyzer()));
        var documentAfterParseFix = await ApplyFixAsync(document, new PreferStaticParseOrNoneCodeFix(), parseDiagnostic);

        var thenDiagnostic = Assert.Single(await GetCompilerDiagnosticsAsync(documentAfterParseFix, ObsoleteMember));
        var documentAfterThenFix = await ApplyFixAsync(documentAfterParseFix, new OptionFromBooleanCodeFix(), thenDiagnostic);

        var text = (await documentAfterThenFix.GetTextAsync()).ToString();
        Assert.Contains("int.ParseOrNone(text)", text, StringComparison.Ordinal);
        Assert.Contains("flag.Then(number)", text, StringComparison.Ordinal);
        Assert.Equal(1, text.Split("using Funcky.Extensions;").Length - 1);
    }

    private static async Task<Document> CreateDocumentAsync(AdhocWorkspace workspace)
    {
        var references = await ReferenceAssemblies.Default.ResolveAsync(LanguageNames.CSharp, CancellationToken.None);
        var project = workspace
            .AddProject("Test", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
            .AddMetadataReferences(references);

        return project.AddDocument("Test.cs", InputCode);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(Document document, DiagnosticAnalyzer analyzer)
    {
        var compilation = await document.Project.GetCompilationAsync() ?? throw new InvalidOperationException("Missing compilation");
        var syntaxTree = await document.GetSyntaxTreeAsync();
        var diagnostics = await compilation.WithAnalyzers([analyzer]).GetAnalyzerDiagnosticsAsync();
        return diagnostics.Where(diagnostic => diagnostic.Location.SourceTree == syntaxTree).ToImmutableArray();
    }

    private static async Task<ImmutableArray<Diagnostic>> GetCompilerDiagnosticsAsync(Document document, string id)
    {
        var semanticModel = await document.GetSemanticModelAsync() ?? throw new InvalidOperationException("Missing semantic model");
        return semanticModel.GetDiagnostics().Where(diagnostic => diagnostic.Id == id).ToImmutableArray();
    }

    private static async Task<Document> ApplyFixAsync(Document document, CodeFixProvider codeFix, Diagnostic diagnostic)
    {
        var actions = new List<CodeAction>();
        await codeFix.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None));
        var action = Assert.Single(actions);
        var operations = await action.GetOperationsAsync(CancellationToken.None);
        var changedSolution = Assert.Single(operations.OfType<ApplyChangesOperation>()).ChangedSolution;

        return changedSolution.GetDocument(document.Id) ?? throw new InvalidOperationException("Missing document");
    }
}
