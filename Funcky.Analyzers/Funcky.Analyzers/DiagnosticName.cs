namespace Funcky.Analyzers;

internal static class DiagnosticName
{
    public const string Prefix = "λ";

    public const string Usage = "10";

    public const string FunctionalAssertUsage = "11";

    public static string HelpLink(string diagnosticId) => $"https://polyadic.github.io/funcky/analyzer-rules/{diagnosticId}.html";
}
