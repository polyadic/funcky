using Xunit;
using static Funcky.Analyzers.AlternativeMonadAnalyzer;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.AlternativeMonadAnalyzer, Funcky.Analyzers.AlternativeMonad.MatchToNullableCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed partial class AlternativeMonadAnalyzerTest
{
    [Fact]
    public async Task WarnsAndFixesToNullableEquivalents()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<string> optionOfReferenceType, Option<int> optionOfValueType)
                {
                    optionOfReferenceType.Match(none: (string?)null, some: x => x);
                    optionOfReferenceType.Match(none: () => (string?)null, some: x => x);
                    optionOfReferenceType.Match<string?>(none: () => null, some: x => x);
                    optionOfReferenceType.Match(none: (string?)null, some: Identity);
                    optionOfValueType.Match(none: (int?)null, some: x => x);
                    optionOfValueType.Match(none: () => (int?)null, some: x => x);
                    optionOfValueType.Match<int?>(none: () => null, some: x => x);
                    _ = optionOfReferenceType.Match((string?)null, some: x => x)!;
                    _ = optionOfReferenceType.Match((string)null!, some: x => x);
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<string> optionOfReferenceType, Option<int> optionOfValueType)
                {
                    optionOfReferenceType.ToNullable();
                    optionOfReferenceType.ToNullable();
                    optionOfReferenceType.ToNullable();
                    optionOfReferenceType.ToNullable();
                    optionOfValueType.ToNullable();
                    optionOfValueType.ToNullable();
                    optionOfValueType.ToNullable();
                    _ = optionOfReferenceType.ToNullable()!;
                    _ = optionOfReferenceType.ToNullable();
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            new[]
            {
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(10, 9, 10, 71),
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(11, 9, 11, 77),
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(12, 9, 12, 77),
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(13, 9, 13, 73),
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(14, 9, 14, 64),
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(15, 9, 15, 70),
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(16, 9, 16, 70),
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(17, 13, 17, 69),
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(18, 13, 18, 69),
            },
            fixedCode + Environment.NewLine + OptionStubCode);
    }

    [Fact]
    public async Task FixesInvocationWhereDiagnosticSpanProducesATie()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfValueType)
                {
                    var func = (int? x) => x;
                    func(optionOfValueType.Match((int?)null, some: x => x));
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfValueType)
                {
                    var func = (int? x) => x;
                    func(optionOfValueType.ToNullable());
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            new[]
            {
                VerifyCS.Diagnostic(PreferToNullable).WithSpan(11, 14, 11, 63),
            },
            fixedCode + Environment.NewLine + OptionStubCode);
    }
}
