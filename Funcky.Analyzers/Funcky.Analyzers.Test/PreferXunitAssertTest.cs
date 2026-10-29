using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.ExtensionForms.PreferXunitAssertAnalyzer, Funcky.Analyzers.PreferXunitAssertCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class PreferXunitAssertTest
{
    // language=csharp
    private const string StubCode =
        """
        namespace Xunit
        {
            public partial class Assert
            {
                public static void Equal<T>(T expected, T actual)
                {
                }
            }
        }

        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
                where TItem : notnull
            {
            }
        }

        namespace Funcky
        {
            using Funcky.Monads;

            public static class FunctionalAssert
            {
                public static void Some<TItem>(TItem expectedValue, Option<TItem> option) where TItem : notnull { }

                public static TItem Some<TItem>(Option<TItem> option) where TItem : notnull => default!;

                public static void None<TItem>(Option<TItem> option) where TItem : notnull { }

                public static void Legacy<TItem>(Option<TItem> option) where TItem : notnull { }
            }
        }

        namespace Funcky.Extensions
        {
            using Funcky.Monads;
            using Xunit;

            public static class AssertExtensions
            {
                extension(Assert)
                {
                    public static void Some<TItem>(TItem expectedValue, Option<TItem> option) where TItem : notnull { }

                    public static TItem Some<TItem>(Option<TItem> option) where TItem : notnull => default!;

                    public static void None<TItem>(Option<TItem> option) where TItem : notnull { }
                }
            }
        }
        """;

    [Fact]
    public async Task ReportsFunctionalAssertAndRewritesItToAssert()
    {
        const string inputCode =
            """
            using Funcky;
            using Funcky.Extensions;
            using Funcky.Monads;
            using Xunit;
            using static Funcky.FunctionalAssert;

            public static class C
            {
                public static void M(Option<int> option)
                {
                    {|λ1203:FunctionalAssert.Some(10, option)|};
                    var value = {|λ1203:FunctionalAssert.Some(option)|};
                    {|λ1203:Funcky.FunctionalAssert.None<int>(option)|};
                    {|λ1203:None(option)|};
                    FunctionalAssert.Legacy(option);
                    Assert.Equal(10, value);
                }
            }
            """;
        const string fixedCode =
            """
            using Funcky;
            using Funcky.Extensions;
            using Funcky.Monads;
            using Xunit;
            using static Funcky.FunctionalAssert;

            public static class C
            {
                public static void M(Option<int> option)
                {
                    Assert.Some(10, option);
                    var value = Assert.Some(option);
                    Assert.None<int>(option);
                    Assert.None(option);
                    FunctionalAssert.Legacy(option);
                    Assert.Equal(10, value);
                }
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(inputCode + Environment.NewLine + StubCode, fixedCode + Environment.NewLine + StubCode);
    }

    // Three or more diagnostics: Fix-All has to rewrite every call in one pass although each fix on its own adds the same using (#1009).
    [Fact]
    public async Task AddsTheUsingForTheExtensionWhenMissing()
    {
        const string inputCode =
            """
            using Funcky;
            using Funcky.Monads;
            using Xunit;

            public static class C
            {
                public static void M(Option<int> option)
                {
                    {|λ1203:FunctionalAssert.None(option)|};
                    {|λ1203:FunctionalAssert.Some(10, option)|};
                    var value = {|λ1203:FunctionalAssert.Some(option)|};
                    Assert.Equal(10, value);
                }
            }
            """;
        const string fixedCode =
            """
            using Funcky;
            using Funcky.Monads;
            using Xunit;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(Option<int> option)
                {
                    Assert.None(option);
                    Assert.Some(10, option);
                    var value = Assert.Some(option);
                    Assert.Equal(10, value);
                }
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(inputCode + Environment.NewLine + StubCode, fixedCode + Environment.NewLine + StubCode);
    }

    [Fact]
    public async Task DoesNotReportInsideExpressionTrees()
    {
        const string inputCode =
            """
            using System;
            using System.Linq.Expressions;
            using Funcky;
            using Funcky.Extensions;
            using Funcky.Monads;
            using Xunit;

            public static class C
            {
                public static void M(Option<int> option)
                {
                    Expression<Func<Option<int>, int>> e = option => FunctionalAssert.Some(option);
                    Func<Option<int>, int> f = option => {|λ1203:FunctionalAssert.Some(option)|};
                }
            }
            """;
        const string fixedCode =
            """
            using System;
            using System.Linq.Expressions;
            using Funcky;
            using Funcky.Extensions;
            using Funcky.Monads;
            using Xunit;

            public static class C
            {
                public static void M(Option<int> option)
                {
                    Expression<Func<Option<int>, int>> e = option => FunctionalAssert.Some(option);
                    Func<Option<int>, int> f = option => Assert.Some(option);
                }
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(inputCode + Environment.NewLine + StubCode, fixedCode + Environment.NewLine + StubCode);
    }
}
