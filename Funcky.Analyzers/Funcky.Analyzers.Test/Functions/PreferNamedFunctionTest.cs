using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.Functions.PreferNamedFunctionAnalyzer, Funcky.Analyzers.Functions.PreferNamedFunctionCodeFix>;

namespace Funcky.Analyzers.Test.Functions;

public sealed class PreferNamedFunctionTest
{
    // language=csharp
    private const string FunctionalStub =
        """

        namespace Funcky
        {
            public static class Functional
            {
                public static T Identity<T>(T value) => value;

                public static bool True() => true;

                public static bool True<T1>(T1 ω1) => true;

                public static bool True<T1, T2>(T1 ω1, T2 ω2) => true;

                public static bool False() => false;

                public static bool False<T1>(T1 ω1) => false;

                public static bool False<T1, T2>(T1 ω1, T2 ω2) => false;

                public static void NoOperation() { }

                public static void NoOperation<T1>(T1 ω1) { }

                public static void NoOperation<T1, T2>(T1 ω1, T2 ω2) { }
            }
        }
        """;

    [Fact]
    public async Task LambdasSpellingOutANamedFunctionAreReportedAndReplaced()
    {
        // language=csharp
        const string inputCode =
            """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(IEnumerable<int> items, List<int> list)
                {
                    _ = items.Select({|λ1017:x => x|});
                    _ = items.Select({|λ1017:x => { return x; }|});
                    _ = items.Any({|λ1017:_ => true|});
                    _ = items.All({|λ1017:_ => false|});
                    _ = items.Any({|λ1017:(_) => true|});
                    list.ForEach({|λ1017:_ => { }|});
                    Action action = {|λ1017:() => { }|};
                    Action<int, string> binary = {|λ1017:(_, _) => { }|};
                    Func<bool> predicate = {|λ1017:() => true|};
                    Func<string, string> same = {|λ1017:s => s|};
                    Func<string, string, bool> never = {|λ1017:(a, b) => false|};
                    action = {|λ1017:() => { }|};
                    Run(({|λ1017:x => x|}));
                }

                private static Func<int, int> F() => {|λ1017:x => x|};

                private static Func<int, int> G()
                {
                    return {|λ1017:x => x|};
                }

                private static void Run(Func<int, int> f) { }
            }
            """;

        // language=csharp
        const string fixedCode =
            """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(IEnumerable<int> items, List<int> list)
                {
                    _ = items.Select(Identity);
                    _ = items.Select(Identity);
                    _ = items.Any(True);
                    _ = items.All(False);
                    _ = items.Any(True);
                    list.ForEach(NoOperation);
                    Action action = NoOperation;
                    Action<int, string> binary = NoOperation;
                    Func<bool> predicate = True;
                    Func<string, string> same = Identity;
                    Func<string, string, bool> never = False;
                    action = NoOperation;
                    Run((Identity));
                }

                private static Func<int, int> F() => Identity;

                private static Func<int, int> G()
                {
                    return Identity;
                }

                private static void Run(Func<int, int> f) { }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunctionalStub, fixedCode + FunctionalStub);
    }

    [Fact]
    public async Task TheUsingStaticIsAddedWhenMissing()
    {
        // language=csharp
        const string inputCode =
            """
            using System.Collections.Generic;
            using System.Linq;

            public static class C
            {
                public static IEnumerable<int> M(IEnumerable<int> items)
                    => items.Select({|λ1017:x => x|});
            }
            """;

        // language=csharp
        const string fixedCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            using static Funcky.Functional;

            public static class C
            {
                public static IEnumerable<int> M(IEnumerable<int> items)
                    => items.Select(Identity);
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunctionalStub, fixedCode + FunctionalStub);
    }

    [Fact]
    public async Task TheUsingStaticIsAddedInsideTheNamespaceWhenTheUsingsAreThere()
    {
        // language=csharp
        const string inputCode =
            """
            namespace Test
            {
                using System.Collections.Generic;
                using System.Linq;

                public static class C
                {
                    public static IEnumerable<int> M(IEnumerable<int> items)
                        => items.Select({|λ1017:x => x|});
                }
            }
            """;

        // language=csharp
        const string fixedCode =
            """
            namespace Test
            {
                using System.Collections.Generic;
                using System.Linq;
                using static Funcky.Functional;

                public static class C
                {
                    public static IEnumerable<int> M(IEnumerable<int> items)
                        => items.Select(Identity);
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunctionalStub, fixedCode + FunctionalStub);
    }

    [Fact]
    public async Task TheQualifiedNameIsUsedWhenTheBareNameIsTaken()
    {
        // language=csharp
        const string inputCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky;

            public static class C
            {
                public static IEnumerable<int> M(IEnumerable<int> items)
                    => items.Select({|λ1017:x => x|});

                private static int Identity(int x) => x + 1;
            }
            """;

        // language=csharp
        const string fixedCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky;

            public static class C
            {
                public static IEnumerable<int> M(IEnumerable<int> items)
                    => items.Select(Functional.Identity);

                private static int Identity(int x) => x + 1;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunctionalStub, fixedCode + FunctionalStub);
    }

    [Fact]
    public async Task TheQualifiedNameIsUsedWhenTheImportWouldMakeAnotherNameAmbiguous()
    {
        // language=csharp
        const string inputCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky;
            using static Other;

            public static class Other
            {
                public static bool True() => true;
            }

            public static class C
            {
                public static bool M(IEnumerable<int> items)
                    => items.Any({|λ1017:_ => true|}) && True();
            }
            """;

        // language=csharp
        const string fixedCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky;
            using static Other;

            public static class Other
            {
                public static bool True() => true;
            }

            public static class C
            {
                public static bool M(IEnumerable<int> items)
                    => items.Any(Functional.True) && True();
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunctionalStub, fixedCode + FunctionalStub);
    }

    [Fact]
    public async Task TheUsingStaticIsGloballyQualifiedWhenANestedFunckyNamespaceIsVisible()
    {
        // language=csharp
        const string inputCode =
            """
            namespace App
            {
                using System.Collections.Generic;
                using System.Linq;

                namespace Funcky
                {
                    public static class Nested
                    {
                    }
                }

                public static class C
                {
                    public static IEnumerable<int> M(IEnumerable<int> items)
                        => items.Select({|λ1017:x => x|});
                }
            }
            """;

        // language=csharp
        const string fixedCode =
            """
            namespace App
            {
                using System.Collections.Generic;
                using System.Linq;
                using static global::Funcky.Functional;

                namespace Funcky
                {
                    public static class Nested
                    {
                    }
                }

                public static class C
                {
                    public static IEnumerable<int> M(IEnumerable<int> items)
                        => items.Select(Identity);
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunctionalStub, fixedCode + FunctionalStub);
    }

    [Fact]
    public async Task TheUsingStaticIsInsertedOutsideOfConditionalRegions()
    {
        // language=csharp
        const string inputCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            #if true
            using System.Text;
            #endif

            public static class C
            {
                public static IEnumerable<int> M(IEnumerable<int> items)
                    => items.Select({|λ1017:x => x|});
            }
            """;

        // language=csharp
        const string fixedCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            using static Funcky.Functional;
            #if true
            using System.Text;
            #endif

            public static class C
            {
                public static IEnumerable<int> M(IEnumerable<int> items)
                    => items.Select(Identity);
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunctionalStub, fixedCode + FunctionalStub);
    }

    [Fact]
    public async Task LambdasWhoseReplacementWouldBreakInferenceAreNotReported()
    {
        // language=csharp
        const string inputCode =
            """
            using System;
            using static Funcky.Functional;

            public static class C
            {
                public static void M()
                {
                    // Either lambda alone supplies T, both method groups together do not.
                    Both((int x) => x, (int y) => y);

                    // A method group has no natural type.
                    var f = (int x) => x;
                }

                private static void Both<T>(Func<T, T> a, Func<T, T> b) { }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + FunctionalStub);
    }

    [Fact]
    public async Task LambdasThatAreNotEquivalentToANamedFunctionAreNotReported()
    {
        // language=csharp
        const string inputCode =
            """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;
            using static Funcky.Functional;

            public static class C
            {
                private const bool Flag = true;

                public static void M(IEnumerable<int> items, IEnumerable<bool> flags)
                {
                    _ = items.Select(x => x + 1);
                    _ = items.Select(x => (long)x);
                    _ = items.Any(x => x == x);
                    _ = items.Any(_ => Flag);
                    _ = flags.Select(x => !x);
                    Func<int, object> boxed = x => x;
                    Func<int, int?> nullable = x => x;
                    Func<int, Task> asynchronous = async _ => { };
                    Action<int> sideEffect = x => { Console.WriteLine(x); };
                    Func<int> throws = () => throw new Exception();
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + FunctionalStub);
    }

    [Fact]
    public async Task LambdasWhoseReplacementWouldBindDifferentlyAreNotReported()
    {
        // language=csharp
        const string inputCode =
            """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Linq.Expressions;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(IEnumerable<int> items, IQueryable<int> queryable)
                {
                    // Where has an overload with an index, so the method group is ambiguous.
                    _ = items.Where(_ => true);
                    _ = items.Select((x, _) => x);

                    // A method group cannot be converted to an expression tree.
                    _ = queryable.Select(x => x);
                    Expression<Func<int, int>> expression = x => x;

                    // The overloads differ only in the delegate's arity.
                    Overloaded(() => { });
                    Overloaded(_ => true);
                }

                private static void Overloaded(Action action) { }

                private static void Overloaded(Action<int> action) { }

                private static void Overloaded(Func<bool> predicate) { }

                private static void Overloaded(Func<int, bool> predicate) { }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + FunctionalStub);
    }

    [Fact]
    public async Task BeforeCSharp11TheDiagnosticIsAnInfo()
    {
        // language=csharp
        const string inputCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            using static Funcky.Functional;

            public static class C
            {
                public static IEnumerable<int> M(IEnumerable<int> items)
                    => items.Select(x => x);
            }
            """;
        await new VerifyCS.Test
        {
            TestCode = inputCode + FunctionalStub,
            LanguageVersion = LanguageVersion.CSharp10,
            ExpectedDiagnostics =
            {
                VerifyCS.Diagnostic().WithSeverity(DiagnosticSeverity.Info).WithSpan(8, 25, 8, 31).WithArguments("Identity"),
            },
        }.RunAsync();
    }
}
