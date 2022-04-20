using Microsoft.CodeAnalysis;
using Xunit;

namespace Funcky.Analyzers.Test;

public sealed class VerifierTests
{
    [Fact]
    public void CS8632AndCS8669AreNullableWarnings()
    {
        Assert.Equal(ReportDiagnostic.Error, CSharpVerifierHelper.NullableWarnings["CS8632"]);
        Assert.Equal(ReportDiagnostic.Error, CSharpVerifierHelper.NullableWarnings["CS8669"]);
    }
}
