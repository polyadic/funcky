namespace Funcky.BuiltinAnalyzers;

internal static class DiagnosticName
{
    public const string Prefix = "λ";

    public const string BuiltIn = "00";

    /// <summary>λ0004, shared by <c>DefaultInConversionAnalyzer</c> and its code fix, which live in different projects.</summary>
    public const string DefaultInConversionId = $"{Prefix}{BuiltIn}04";

    public static string HelpLink(string diagnosticId) => $"https://polyadic.github.io/funcky/analyzer-rules/{diagnosticId}.html";
}
