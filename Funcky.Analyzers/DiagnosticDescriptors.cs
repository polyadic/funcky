using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers
{
    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor UseFunctionalIdentity = new DiagnosticDescriptor(
            "ƛ0001",
            "Use Functional.Identity",
            "Use Functional.Identity instead of writing your own identity function",
            "Style",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
}
