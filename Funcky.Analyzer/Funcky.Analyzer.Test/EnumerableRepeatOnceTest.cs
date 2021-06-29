using System.Linq;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = Funcky.Analyzer.Test.CSharpCodeFixVerifier<Funcky.Analyzer.EnumerableRepeatOnceAnalyzer, Funcky.Analyzer.EnumerableRepeatOnceCodeFix>;

namespace Funcky.Analyzer.Test
{
    public class EnumerableRepeatOnceTest
    {
        [Fact]
        public async Task EnumerableRepeatWithAnyNumberButOneIssuesNoDiagnostic()
        {
            var test = @"
    using System;
    using System.Linq;
    
    namespace ConsoleApplication1
    {
        class Program
        {
            private void Syntax()
            {
                var single = Enumerable.Repeat(1337, 2);
            }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task UsingEnumerableRepeatOnceShowsTheSequenceReturnDiagnostic()
        {
            var test = @"
    using System;
    using System.Linq;

    namespace ConsoleApplication1
    {
        class Sequence {
            public static int Return(int value) => value;           
        }
        class Program
        {
            private void Syntax()
            {
                var single = Enumerable.Repeat(1337, 1);
            }
        }
    }";

            var fixtest = @"
    using System;
    using System.Linq;

    namespace ConsoleApplication1
    {
        class Sequence {
            public static int Return(int value) => value;           
        }
        class Program
        {
            private void Syntax()
            {
                var single = Sequence.Return(1337);
            }
        }
    }";
            var expected = VerifyCS.Diagnostic("EnumerableRepeatOnceAnalyzer").WithSpan(14, 30, 14, 56);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        private void Syntax()
        {
            var single =
                Enumerable
                .Repeat("Test", 1)
                .Select(s => s + "!");

            var s2 = Sequence.Return("Test");
        }
    }
}
