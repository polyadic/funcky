namespace Funcky.Analyzers.Test;

public sealed partial class FunctionalAssertAnalyzerTest
{
    // language=csharp
    private const string AttributeSource =
        """
        namespace Funcky.CodeAnalysis
        {
            internal sealed class AssertMethodHasOverloadWithExpectedValueAttribute : System.Attribute { }
        }
        """;

    // language=csharp
    private const string Stubs =
        """
        namespace Xunit
        {
            public static class Assert
            {
                public static void Equal<T>(T expected, T actual) { }

                public static void Equal<T>(T expected, T actual, System.Func<T, T, bool> compare) { }

                public static void Equal(System.DateTime expected, System.DateTime actual) { }
            }
        }

        namespace Funcky.Monads
        {
            public readonly struct Option<T> { }
        }

        namespace Funcky
        {
            public static class FunctionalAssert
            {
                [Funcky.CodeAnalysis.AssertMethodHasOverloadWithExpectedValueAttribute]
                public static T Some<T>(Option<T> option) => throw null!;

                public static T Some<T>(T expected, Option<T> option) => throw null!;
            }
        }
        """;
}
