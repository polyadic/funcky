using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.Analyzers.FunctionalAssert.FunctionalAssertAnalyzer>;

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
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + AttributeSource + Stubs);
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
