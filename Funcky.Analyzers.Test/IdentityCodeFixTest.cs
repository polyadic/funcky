using System.Threading.Tasks;
using Xunit;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<Funcky.Analyzers.IdentityAnalyzer, Funcky.Analyzers.IdentityCodeFix, Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace Funcky.Analyzers.Test
{
    public sealed class IdentityCodeFixTest
    {
        [Fact]
        public async Task ReplacesLambdaInVariableAssignment()
        {
            const string source = @"namespace Funcky
{
    public static class Functional
    {
        public static T Identity<T>(T x) => x;
    }
}

public class Foo
{
    public void Bar()
    {
        System.Func<int, int> func = x => x;
    }
}";
            const string fixedSource = @"namespace Funcky
{
    public static class Functional
    {
        public static T Identity<T>(T x) => x;
    }
}

public class Foo
{
    public void Bar()
    {
        System.Func<int, int> func = Funcky.Functional.Identity;
    }
}";
            var expected = Verify.Diagnostic().WithSpan(13, 38, 13, 44);
            await Verify.VerifyCodeFixAsync(source, expected, fixedSource);
        }

        [Fact]
        public async Task ReplacesLambdaWhenPassedAsParameter()
        {
            const string source = @"namespace Funcky
{
    public static class Functional
    {
        public static T Identity<T>(T x) => x;
    }
}

public class Foo
{
    public void Bar() => Baz(x => x);

    public void Baz(System.Func<int, int> func) {}
}";
            const string fixedSource = @"namespace Funcky
{
    public static class Functional
    {
        public static T Identity<T>(T x) => x;
    }
}

public class Foo
{
    public void Bar() => Baz(Funcky.Functional.Identity);

    public void Baz(System.Func<int, int> func) {}
}";
            var expected = Verify.Diagnostic().WithSpan(11, 30, 11, 36);
            await Verify.VerifyCodeFixAsync(source, expected, fixedSource);
        }
    }
}
