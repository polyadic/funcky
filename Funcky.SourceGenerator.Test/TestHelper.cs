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
        => Verifier.Verify(CreateGeneratorDriver(source));

    /// <summary>
    /// The GeneratorDriver is used to run our generator against a compilation.
    /// </summary>
    private static GeneratorDriver CreateGeneratorDriver(string source)
    {
        var driver = CSharpGeneratorDriver
            .Create(new OrNoneFromTryPatternGenerator())
            .RunGeneratorsAndUpdateCompilation(ToCompilation(source), out var outputCompilation, out _);

        if (outputCompilation.GetDiagnostics().Any(d => d.Severity >= DiagnosticSeverity.Warning))
        {
            throw new InvalidOperationException($"Compilation has warnings or errors:{Environment.NewLine}{string.Join(Environment.NewLine, outputCompilation.GetDiagnostics())}");
        }

        return driver;
    }

    /// <summary>
    /// Create a Roslyn compilation for the syntax tree.
    /// </summary>
    private static CSharpCompilation ToCompilation(string source)
        => CSharpCompilation.Create(TestAssemblyName, ToSyntaxTrees(source), GetDependencyReferences(), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

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
    private static IEnumerable<SyntaxTree> ToSyntaxTrees(string source)
        => Return(CSharpSyntaxTree.ParseText(source));

    private static IEnumerable<TItem> Return<TItem>(params TItem[] items)
        => items;
}
