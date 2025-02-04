using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.FunctionalAssert.FunctionalAssertAnalyzer, Funcky.Analyzers.FunctionalAssertFix>;

namespace Funcky.Analyzers.Test;

public sealed partial class FunctionalAssertAnalyzerTest
{
    [Fact]
    public async Task WarnsWhenCombiningAssertEqualWithOurMethod()
    {
        // language=csharp
        const string inputCode =
            """
            using Funcky;
            using Funcky.Monads;
            using Xunit;

            class C
            {
                private void M()
                {
                    Assert.Equal(42, FunctionalAssert.Some(default(Option<int>)));
                    Assert.Equal(
                        42,
                        FunctionalAssert.Some(default(Option<int>)));
                    Assert.Equal(42, FunctionalAssert.Some(
                        default(Option<int>)
                    ));
                    Assert.Equal(actual: FunctionalAssert.Some(default(Option<int>)), expected: 42);
                }
            }
            """;

        // language=csharp
        const string fixedCode =
            """
            using Funcky;
            using Funcky.Monads;
            using Xunit;

            class C
            {
                private void M()
                {
                    FunctionalAssert.Some(42, default(Option<int>));
                    FunctionalAssert.Some(
                        42,
                        default(Option<int>));
                    FunctionalAssert.Some(42, default(Option<int>));
                    FunctionalAssert.Some(42, default(Option<int>));
                }
            }
            """;
        DiagnosticResult[] expectedDiagnostics = [
            VerifyCS.Diagnostic().WithSpan(9, 9, 9, 70).WithArguments("FunctionalAssert", "Some"),
            VerifyCS.Diagnostic().WithSpan(10, 9, 12, 57).WithArguments("FunctionalAssert", "Some"),
            VerifyCS.Diagnostic().WithSpan(13, 9, 15, 11).WithArguments("FunctionalAssert", "Some"),
            VerifyCS.Diagnostic().WithSpan(16, 9, 16, 88).WithArguments("FunctionalAssert", "Some"),
        ];

        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + AttributeSource + Stubs,
            expectedDiagnostics,
            fixedCode + Environment.NewLine + AttributeSource + Stubs);
    }

    [Fact]
    public async Task DoesNotWarnForSpecializedOverloadsOfAssertEqual()
    {
        // language=csharp
        const string inputCode =
            """
            using System;
            using Funcky;
            using Funcky.Monads;
            using Xunit;

            class C
            {
                private void M()
                {
                    Assert.Equal(DateTime.UnixEpoch, FunctionalAssert.Some(default(Option<DateTime>)));
                    Assert.Equal(42, FunctionalAssert.Some(default(Option<int>)), (a, b) => throw null!);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + AttributeSource + Stubs);
    }
}
