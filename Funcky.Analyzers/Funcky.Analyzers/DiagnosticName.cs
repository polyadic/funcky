namespace Funcky.Analyzers;

internal static class DiagnosticName
{
    public const string Prefix = "λ";

    public const string Usage = "10";

    public const string FunctionalAssertUsage = "11";

    /// <summary>The category of every opt-in rule: the flagged code works, but Funcky offers a more idiomatic form.
    /// The built-in analyzers shipped in the <c>Funcky</c> package use <c>Funcky.Correctness</c> instead.</summary>
    public const string Category = "Funcky.Style";

    public static string HelpLink(string diagnosticId) => $"https://polyadic.github.io/funcky/analyzer-rules/{diagnosticId}.html";
}
