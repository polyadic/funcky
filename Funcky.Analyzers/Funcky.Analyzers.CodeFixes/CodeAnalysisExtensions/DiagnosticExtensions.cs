using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

internal static class DiagnosticExtensions
{
    // No static interfaces or IParsable<T> in .NET Standard 2.0...
    private delegate bool TryParseDelegate<T>(string? s, [NotNullWhen(true)] out T? value);

    public static bool TryGetIntProperty(this Diagnostic diagnostic, string propertyName, out int value)
        => TryGetProperty(diagnostic, propertyName, int.TryParse, out value);

    private static bool TryGetProperty<T>(this Diagnostic diagnostic, string propertyName, TryParseDelegate<T> parser, [NotNullWhen(true)] out T? value)
    {
        value = default;
        return diagnostic.Properties.TryGetValue(propertyName, out var stringValue) && parser(stringValue, out value);
    }
}
