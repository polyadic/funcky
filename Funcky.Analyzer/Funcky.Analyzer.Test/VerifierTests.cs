using Microsoft.CodeAnalysis;
using Xunit;

namespace Funcky.Analyzer.Test
{
    public class VerifierTests
    {
        [Fact]
        public void CS8632AndCS8669AreNullableWarnings()
        {
            Assert.Equal(ReportDiagnostic.Error, CSharpVerifierHelper.NullableWarnings["CS8632"]);
            Assert.Equal(ReportDiagnostic.Error, CSharpVerifierHelper.NullableWarnings["CS8669"]);
        }
    }
}
