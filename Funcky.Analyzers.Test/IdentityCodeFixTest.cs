using System.Threading.Tasks;
using Xunit;
using Verify = Funcky.Analyzers.Test.CSharpVerifier<Funcky.Analyzers.IdentityAnalyzer, Funcky.Analyzers.IdentityCodeFix, Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace Funcky.Analyzers.Test
{
    public sealed class IdentityCodeFixTest
    {
        [Fact]
        public async Task ReplacesLambdaInVariableAssignment()
        {
            const string source = @"public class Foo
{
    public void Bar()
    {
        System.Func<int, int> func = x => x;
    }
}";
            const string fixedSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int, int> func = Funcky.Functional.Identity;
    }
}";
            var expected = Verify.Diagnostic().WithSpan(5, 38, 5, 44);
            await Verify.VerifyCodeFixAsync(source, expected, fixedSource);
        }

        [Fact]
        public async Task ReplacesLambdaWhenPassedAsParameter()
        {
            const string source = @"public class Foo
{
    public void Bar() => Baz(x => x);

    public void Baz(System.Func<int, int> func) {}
}";
            const string fixedSource = @"public class Foo
{
    public void Bar() => Baz(Funcky.Functional.Identity);

    public void Baz(System.Func<int, int> func) {}
}";
            var expected = Verify.Diagnostic().WithSpan(3, 30, 3, 36);
            await Verify.VerifyCodeFixAsync(source, expected, fixedSource);
        }
    }
}
