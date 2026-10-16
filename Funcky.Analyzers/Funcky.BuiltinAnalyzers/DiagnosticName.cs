namespace Funcky.BuiltinAnalyzers;

internal static class DiagnosticName
{
    public const string Prefix = "λ";

    public const string BuiltIn = "00";

    public static string HelpLink(string diagnosticId) => $"https://polyadic.github.io/funcky/analyzer-rules/{diagnosticId}.html";
}
