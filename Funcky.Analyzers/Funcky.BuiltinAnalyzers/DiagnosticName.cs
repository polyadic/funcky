namespace Funcky.BuiltinAnalyzers;

internal static class DiagnosticName
{
    public const string Prefix = "λ";

    public const string BuiltIn = "00";

    /// <summary>λ0004, shared by <c>DefaultInConversionAnalyzer</c> and its code fix, which live in different projects.</summary>
    public const string DefaultInConversionId = $"{Prefix}{BuiltIn}04";

    /// <summary>The category of every built-in rule: the flagged code is outside the supported surface of Funcky.
    /// The opt-in rules in the <c>Funcky.Analyzers</c> package use <c>Funcky.Style</c> instead.</summary>
    public const string Category = "Funcky.Correctness";

    /// <summary>λ0005, shared by <c>NullToMonadAnalyzer</c> and its code fix, which live in different projects.</summary>
    public const string NullToMonadId = $"{Prefix}{BuiltIn}05";

    public static string HelpLink(string diagnosticId) => $"https://polyadic.github.io/funcky/analyzer-rules/{diagnosticId}.html";
}
