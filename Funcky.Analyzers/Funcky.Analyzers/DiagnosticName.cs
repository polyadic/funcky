namespace Funcky.Analyzers;

internal static class DiagnosticName
{
    public const string Prefix = "λ";

    public const string Usage = "10";

    public const string FunctionalAssertUsage = "11";

    /// <summary>The rules that point at the C# 14 extension forms added in 3.8.0 (static extension members next to the classic API).</summary>
    public const string ExtensionForms = "12";

    /// <summary>The category of every opt-in rule: the flagged code works, but Funcky offers a more idiomatic form.
    /// The built-in analyzers shipped in the <c>Funcky</c> package use <c>Funcky.Correctness</c> instead.</summary>
    public const string Category = "Funcky.Style";

    public static string HelpLink(string diagnosticId) => $"https://polyadic.github.io/funcky/analyzer-rules/{diagnosticId}.html";
}
