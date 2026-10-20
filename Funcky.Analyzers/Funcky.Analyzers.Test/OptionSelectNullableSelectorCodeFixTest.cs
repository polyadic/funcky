#pragma warning disable SA1118 // StyleCop support for collection expressions is missing
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Microsoft.CodeAnalysis.Testing.EmptyDiagnosticAnalyzer, Funcky.Analyzers.OptionSelectNullableSelectorCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class OptionSelectNullableSelectorCodeFixTest
{
    private const string NotNullConstraintViolation = "CS8714";

    private const string LambdaReturnTypeMismatch = "CS8621";

    // language=csharp
    private const string OptionStubCode =
        """
        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
                where TItem : notnull
            {
                public static Option<TItem> None => default;

                public Option<TResult> Select<TResult>(System.Func<TItem, TResult> selector) where TResult : notnull => default;

                public Option<TResult> SelectMany<TResult>(System.Func<TItem, Option<TResult>> selector) where TResult : notnull => default;
            }

            public static class Option
            {
                public static Option<TItem> Some<TItem>(TItem value) where TItem : notnull => default;

                public static Option<TItem> FromNullable<TItem>(TItem? item) where TItem : class => default;

                public static Option<TItem> FromNullable<TItem>(TItem? item) where TItem : struct => default;
            }

            public readonly struct Either<TLeft, TRight>
                where TLeft : notnull
                where TRight : notnull
            {
                public Either<TLeft, TResult> Select<TResult>(System.Func<TRight, TResult> selector) where TResult : notnull => default;
            }

            public readonly struct Result<TValidResult>
                where TValidResult : notnull
            {
                public Result<TResult> Select<TResult>(System.Func<TValidResult, TResult> selector) where TResult : notnull => default;
            }
        }
        """;

    [Fact]
    public async Task RewritesSelectWithNullableSelectorToSelectManyWithFromNullable()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;

            public sealed class User
            {
                public string Name { get; set; } = "";

                public string? MiddleName { get; set; }

                public int? Age { get; set; }
            }

            public static class C
            {
                public static void M(Option<User> option)
                {
                    var a = option.Select(user => user.MiddleName);
                    var b = option.Select(user => user.Age);
                    var c = option.Select((user) => user.Name.Length > 3 ? user.Name : null);
                    var d = option.Select<string?>(user => user.Name);
                    var e = option.Select(user =>
                    {
                        if (user.Name.Length > 3)
                        {
                            return user.MiddleName;
                        }

                        System.Func<string?> inner = () => null;
                        return inner();
                    });
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using Funcky.Monads;

            public sealed class User
            {
                public string Name { get; set; } = "";

                public string? MiddleName { get; set; }

                public int? Age { get; set; }
            }

            public static class C
            {
                public static void M(Option<User> option)
                {
                    var a = option.SelectMany(user => Option.FromNullable(user.MiddleName));
                    var b = option.SelectMany(user => Option.FromNullable(user.Age));
                    var c = option.SelectMany((user) => Option.FromNullable(user.Name.Length > 3 ? user.Name : null));
                    var d = option.SelectMany(user => Option.FromNullable(user.Name));
                    var e = option.SelectMany(user =>
                    {
                        if (user.Name.Length > 3)
                        {
                            return Option.FromNullable(user.MiddleName);
                        }

                        System.Func<string?> inner = () => null;
                        return Option.FromNullable(inner());
                    });
                }
            }
            """;

        // The fix also removes the CS8621 reported on the lambda, which the test framework otherwise expects to remain.
        var test = new VerifyCS.Test
        {
            TestCode = inputCode + Environment.NewLine + OptionStubCode,
            FixedCode = fixedCode + Environment.NewLine + OptionStubCode,
            FixedState = { InheritanceMode = StateInheritanceMode.Explicit },
        };
        test.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(18, 17, 18, 30),
            DiagnosticResult.CompilerError(LambdaReturnTypeMismatch).WithSpan(18, 31, 18, 38),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(19, 17, 19, 30),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(20, 17, 20, 30),
            DiagnosticResult.CompilerError(LambdaReturnTypeMismatch).WithSpan(20, 31, 20, 40),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(21, 17, 21, 39),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(22, 17, 22, 30),
            DiagnosticResult.CompilerError(LambdaReturnTypeMismatch).WithSpan(22, 31, 22, 38),
        ]);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task SuppliesTheTypeArgumentWhenTheSelectorReturnsNullOrDefault()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;

            public static class C
            {
                public static void M(Option<string> option)
                {
                    var a = option.Select<string?>(_ => null);
                    var b = option.Select<int?>(_ => default);
                    var c = option.Select<string?>(value =>
                    {
                        if (value.Length > 3)
                        {
                            return null;
                        }

                        return default;
                    });
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using Funcky.Monads;

            public static class C
            {
                public static void M(Option<string> option)
                {
                    var a = option.SelectMany(_ => Option.FromNullable<string>(null));
                    var b = option.SelectMany(_ => Option.FromNullable<int>(default));
                    var c = option.SelectMany(value =>
                    {
                        if (value.Length > 3)
                        {
                            return Option.FromNullable<string>(null);
                        }

                        return Option.FromNullable<string>(default);
                    });
                }
            }
            """;

        var test = new VerifyCS.Test
        {
            TestCode = inputCode + Environment.NewLine + OptionStubCode,
            FixedCode = fixedCode + Environment.NewLine + OptionStubCode,
            FixedState = { InheritanceMode = StateInheritanceMode.Explicit },
        };
        test.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(9, 17, 9, 39),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(10, 17, 10, 36),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(11, 17, 11, 39),
        ]);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task RewritesAnExplicitNullableReturnTypeToOption()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;

            public sealed class User
            {
                public string? MiddleName { get; set; }

                public int? Age { get; set; }
            }

            public static class C
            {
                public static void M(Option<User> option)
                {
                    var a = option.Select(string? (User user) => user.MiddleName);
                    var b = option.Select(int? (user) =>
                    {
                        return user.Age;
                    });
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using Funcky.Monads;

            public sealed class User
            {
                public string? MiddleName { get; set; }

                public int? Age { get; set; }
            }

            public static class C
            {
                public static void M(Option<User> option)
                {
                    var a = option.SelectMany(Option<string> (User user) => Option.FromNullable(user.MiddleName));
                    var b = option.SelectMany(Option<int> (user) =>
                    {
                        return Option.FromNullable(user.Age);
                    });
                }
            }
            """;

        var test = new VerifyCS.Test
        {
            TestCode = inputCode + Environment.NewLine + OptionStubCode,
            FixedCode = fixedCode + Environment.NewLine + OptionStubCode,
            FixedState = { InheritanceMode = StateInheritanceMode.Explicit },
        };
        test.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(16, 17, 16, 30),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(17, 17, 17, 30),
        ]);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task OffersNoFixForAConstrainedTypeInsideTheLambdaOrAThrowExpression()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;

            public sealed class Box<T>
                where T : notnull
            {
            }

            public static class C
            {
                public static void M(Option<string> option)
                {
                    var a = option.Select(_ => new Box<string?>());
                    var b = option.Select<string?>(_ => throw new System.InvalidOperationException());
                }
            }
            """;
        DiagnosticResult[] expectedDiagnostics =
        [
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(14, 44, 14, 51),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(15, 17, 15, 39),
        ];
        var test = new VerifyCS.Test
        {
            TestCode = inputCode + Environment.NewLine + OptionStubCode,
            FixedCode = inputCode + Environment.NewLine + OptionStubCode,
        };
        test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
        test.FixedState.ExpectedDiagnostics.AddRange(expectedDiagnostics);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task OffersNoFixForEitherAndResultOrForMethodGroups()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;

            public sealed class User
            {
                public string? MiddleName { get; set; }
            }

            public static class C
            {
                public static void M(Option<User> option, Either<string, User> either, Result<User> result)
                {
                    var a = either.Select(user => user.MiddleName);
                    var b = result.Select(user => user.MiddleName);
                    var c = option.Select(GetMiddleName);
                }

                private static string? GetMiddleName(User user) => user.MiddleName;
            }
            """;
        DiagnosticResult[] expectedDiagnostics =
        [
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(14, 17, 14, 30),
            DiagnosticResult.CompilerError(LambdaReturnTypeMismatch).WithSpan(14, 31, 14, 38),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(15, 17, 15, 30),
            DiagnosticResult.CompilerError(LambdaReturnTypeMismatch).WithSpan(15, 31, 15, 38),
            DiagnosticResult.CompilerError(NotNullConstraintViolation).WithSpan(16, 17, 16, 30),
        ];
        var test = new VerifyCS.Test
        {
            TestCode = inputCode + Environment.NewLine + OptionStubCode,
            FixedCode = inputCode + Environment.NewLine + OptionStubCode,
        };
        test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
        test.FixedState.ExpectedDiagnostics.AddRange(expectedDiagnostics);
        await test.RunAsync(CancellationToken.None);
    }
}
