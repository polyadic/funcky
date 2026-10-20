namespace Funcky.BuiltinAnalyzers;

internal static class DiagnosticName
{
    public const string Prefix = "λ";

    public const string BuiltIn = "00";

    /// <summary>The category of every built-in rule: the flagged code is outside the supported surface of Funcky.
    /// The opt-in rules in the <c>Funcky.Analyzers</c> package use <c>Funcky.Style</c> instead.</summary>
    public const string Category = "Funcky.Correctness";

    public static string HelpLink(string diagnosticId) => $"https://polyadic.github.io/funcky/analyzer-rules/{diagnosticId}.html";
}
