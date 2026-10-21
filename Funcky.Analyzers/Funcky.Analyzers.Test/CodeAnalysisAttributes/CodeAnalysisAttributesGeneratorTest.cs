using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Funcky.Analyzers.Test.CodeAnalysisAttributes;

public sealed class CodeAnalysisAttributesGeneratorTest
{
    // language=csharp
    private const string ConsumerCode =
        """
        using Funcky.CodeAnalysis;

        [NonDefaultable]
        public struct Money
        {
            [UseWithArgumentNames]
            public static Money Create(decimal amount, string currency) => throw null!;
        }
        """;

    [Fact]
    public void NothingIsGeneratedWhenThePropertyIsNotSet()
    {
        var (compilation, diagnostics) = Generate("class C { }", property: null);

        Assert.Empty(diagnostics);
        Assert.Single(compilation.SyntaxTrees);
    }

    [Fact]
    public void TheRequestedAttributesAreGeneratedAndCompile()
    {
        var (compilation, diagnostics) = Generate(ConsumerCode, property: "UseWithArgumentNames; NonDefaultable");

        Assert.Empty(diagnostics);
        Assert.Equal(3, compilation.SyntaxTrees.Count());
        Assert.Empty(compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
        Assert.NotNull(compilation.GetTypeByMetadataName("Funcky.CodeAnalysis.UseWithArgumentNamesAttribute"));
        Assert.NotNull(compilation.GetTypeByMetadataName("Funcky.CodeAnalysis.NonDefaultableAttribute"));
    }

    [Fact]
    public void TheGeneratedAttributesCompileInCSharp73Projects()
    {
        var (compilation, diagnostics) = Generate(ConsumerCode.Replace("throw null!", "throw null"), property: "UseWithArgumentNames; NonDefaultable", LanguageVersion.CSharp7_3);

        Assert.Empty(diagnostics);
        Assert.Empty(compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
        Assert.NotNull(compilation.GetTypeByMetadataName("Funcky.CodeAnalysis.UseWithArgumentNamesAttribute"));
        Assert.NotNull(compilation.GetTypeByMetadataName("Funcky.CodeAnalysis.NonDefaultableAttribute"));
    }

    [Fact]
    public void OnlyTheRequestedAttributeIsGenerated()
    {
        var (compilation, diagnostics) = Generate("class C { }", property: "NonDefaultable");

        Assert.Empty(diagnostics);
        Assert.Null(compilation.GetTypeByMetadataName("Funcky.CodeAnalysis.UseWithArgumentNamesAttribute"));
        Assert.NotNull(compilation.GetTypeByMetadataName("Funcky.CodeAnalysis.NonDefaultableAttribute"));
    }

    [Fact]
    public void UnknownNamesAreIgnored()
    {
        var (compilation, diagnostics) = Generate("class C { }", property: "Unknown,NonDefaultable");

        Assert.Empty(diagnostics);
        Assert.Equal(2, compilation.SyntaxTrees.Count());
    }

    [Theory]
    [InlineData("", new string[0])]
    [InlineData("UseWithArgumentNames", new[] { "UseWithArgumentNames" })]
    [InlineData("UseWithArgumentNames;NonDefaultable", new[] { "UseWithArgumentNames", "NonDefaultable" })]
    [InlineData(" UseWithArgumentNames , NonDefaultable ;; NonDefaultable", new[] { "UseWithArgumentNames", "NonDefaultable" })]
    public void ThePropertyIsSplitTrimmedAndDeduplicated(string value, string[] expected)
        => Assert.Equal(expected, CodeAnalysisAttributesGenerator.ParseAttributeNames(value));

    private static (Compilation Compilation, ImmutableArray<Diagnostic> Diagnostics) Generate(string source, string? property, LanguageVersion languageVersion = LanguageVersion.Latest)
    {
        var parseOptions = new CSharpParseOptions(languageVersion);
        var compilation = CSharpCompilation.Create(
            "Consumer",
            [CSharpSyntaxTree.ParseText(source, parseOptions)],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver
            .Create([new CodeAnalysisAttributesGenerator().AsSourceGenerator()], parseOptions: parseOptions)
            .WithUpdatedAnalyzerConfigOptions(new BuildPropertyOptionsProvider(property))
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        _ = driver;
        return (outputCompilation, diagnostics);
    }

    private sealed class BuildPropertyOptionsProvider(string? value) : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GlobalOptions { get; } = new BuildPropertyOptions(value);

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => GlobalOptions;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => GlobalOptions;
    }

    private sealed class BuildPropertyOptions(string? propertyValue) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value)
        {
            if (key == CodeAnalysisAttributesGenerator.BuildPropertyKey && propertyValue is not null)
            {
                value = propertyValue;
                return true;
            }

            value = null!;
            return false;
        }
    }
}
