using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Funcky.SourceGenerator.Test;

public static class TestHelper
{
    private const string TestAssemblyName = "Tests";

    /// <summary>
    /// Use verify to snapshot test the source generator output.
    /// </summary>
    public static Task Verify(string source)
        => Verifier.Verify(CreateGeneratorDriver(Return(new SourceFile("Source.cs", source))));

    /// <summary>Snapshot test the generator output for several source files, each with its own path.</summary>
    public static Task Verify(params SourceFile[] sources)
        => Verifier.Verify(CreateGeneratorDriver(sources));

    /// <summary>
    /// The GeneratorDriver is used to run our generator against a compilation.
    /// </summary>
    private static GeneratorDriver CreateGeneratorDriver(IEnumerable<SourceFile> sources)
    {
        var driver = CSharpGeneratorDriver
            .Create(new OrNoneFromTryPatternGenerator())
            .RunGeneratorsAndUpdateCompilation(ToCompilation(sources), out var outputCompilation, out var diagnostics);

        var combinedDiagnostics = diagnostics.Concat(outputCompilation.GetDiagnostics()).ToImmutableArray();
        if (combinedDiagnostics.Any(d => d.Severity >= DiagnosticSeverity.Warning))
        {
            throw new InvalidOperationException($"Compilation has warnings or errors:{Environment.NewLine}{string.Join(Environment.NewLine, combinedDiagnostics)}");
        }

        return driver;
    }

    /// <summary>
    /// Create a Roslyn compilation for the syntax tree.
    /// </summary>
    private static CSharpCompilation ToCompilation(IEnumerable<SourceFile> sources)
        => CSharpCompilation.Create(TestAssemblyName, ToSyntaxTrees(sources), GetDependencyReferences(), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    /// <summary>
    /// Create references for assemblies we require.
    /// We could add multiple references if required.
    /// </summary>
    private static IEnumerable<PortableExecutableReference> GetDependencyReferences()
        => Return(
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location));

    /// <summary>
    /// Parse the provided string into a C# syntax tree.
    /// </summary>
    private static IEnumerable<SyntaxTree> ToSyntaxTrees(IEnumerable<SourceFile> sources)
        => sources.Select(source => CSharpSyntaxTree.ParseText(source.Content, path: source.Path));

    private static IEnumerable<TItem> Return<TItem>(params TItem[] items)
        => items;

    public sealed record SourceFile(string Path, string Content);
}
