using System.Threading.Tasks;
using Xunit;
using Verify = Funcky.Analyzers.Test.CSharpVerifier<Funcky.Analyzers.IdentityAnalyzer, Funcky.Analyzers.IdentityCodeFixProvider, Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace Funcky.Analyzers.Test
{
    public class IdentityAnalyzerTest
    {
        [Fact]
        public async Task IgnoresLinqExpression()
        {
            const string source = @"public class Foo
{
    public void Bar()
    {
        System.Linq.Expressions.Expression<System.Func<int, int>> func = x => x;
    }
}";
            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task IgnoresNestedFuncWithParameterReferenceToParentFunc()
        {
            const string source = @"public class Foo
{
    public static System.Func<object, T> Const<T>(T value) => _ => value;
}";
            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task IgnoresIdentityFunctionWithImplicitCast()
        {
            const string source = @"public class Foo
{
    public void Bar()
    {
        System.Func<int, int?> func1 = x => x;
        System.Func<int, double> func2 = x => x;
    }
}";
            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task IgnoresIdentityFunctionWithReturnTypeDifferingOnlyInNullability()
        {
            const string source = @"public class Foo
{
    public void Bar()
    {
        System.Func<string, string?> func = x => x;
    }
}";
            await Verify.VerifyAnalyzerAsync(source);
        }
    }
}
