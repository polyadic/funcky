using Microsoft.CodeAnalysis.Testing;
using Xunit;
using NonDefaultableVerifier = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.BuiltinAnalyzers.NonDefaultableAnalyzer>;
using UseWithArgumentNamesVerifier = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.Analyzers.UseWithArgumentNamesAnalyzer>;

namespace Funcky.Analyzers.Test.CodeAnalysisAttributes;

/// <summary>Users may define their own copy of the attributes (by hand or through the generator) in a library next to the
/// one in Funcky. A project referencing both must still get the diagnostics, which rules out a symbol lookup by metadata
/// name: that lookup returns nothing as soon as two referenced assemblies define the same name.</summary>
public sealed class AttributeInSeveralAssembliesTest
{
    private const string LibraryProjectName = "Library";

    private const string OtherLibraryProjectName = "OtherLibrary";

    // language=csharp
    private const string AttributeSource =
        """
        namespace Funcky.CodeAnalysis
        {
            [System.AttributeUsage(System.AttributeTargets.Method)]
            internal sealed class UseWithArgumentNamesAttribute : System.Attribute { }

            [System.AttributeUsage(System.AttributeTargets.Struct)]
            internal sealed class NonDefaultableAttribute : System.Attribute { }
        }
        """;

    // language=csharp
    private const string LibrarySource =
        """
        using Funcky.CodeAnalysis;

        namespace Library
        {
            public static class Api
            {
                [UseWithArgumentNames]
                public static void Method(int x, int y) { }
            }

            [NonDefaultable]
            public struct Foo { }
        }
        """;

    [Fact]
    public async Task UseWithArgumentNamesIsReportedWhenTheAttributeIsDefinedInTwoReferencedAssemblies()
    {
        // language=csharp
        const string inputCode =
            """
            class Test
            {
                private void Syntax()
                {
                    Library.Api.Method({|λ1003:10|}, {|λ1003:20|});
                }
            }
            """;

        await RunAsync(new UseWithArgumentNamesVerifier.Test(), inputCode);
    }

    [Fact]
    public async Task NonDefaultableIsReportedWhenTheAttributeIsDefinedInTwoReferencedAssemblies()
    {
        // language=csharp
        const string inputCode =
            """
            class Test
            {
                private void Usage()
                {
                    _ = {|λ1009:default(Library.Foo)|};
                }
            }
            """;

        await RunAsync(new NonDefaultableVerifier.Test(), inputCode);
    }

    private static async Task RunAsync(AnalyzerTest<DefaultVerifier> test, string inputCode)
    {
        test.TestState.Sources.Add(inputCode);
        test.TestState.AdditionalProjects[LibraryProjectName].Sources.Add(LibrarySource + Environment.NewLine + AttributeSource);
        test.TestState.AdditionalProjects[OtherLibraryProjectName].Sources.Add(AttributeSource);
        test.TestState.AdditionalProjectReferences.Add(LibraryProjectName);
        test.TestState.AdditionalProjectReferences.Add(OtherLibraryProjectName);
        await test.RunAsync();
    }
}
