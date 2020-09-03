using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers
{
    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor UseFunctionalIdentity = new DiagnosticDescriptor(
            id: "ƛ0001",
            title: "Use Functional.Identity",
            messageFormat: "Use Functional.Identity instead of writing your own identity function",
            category: "Style",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
}
