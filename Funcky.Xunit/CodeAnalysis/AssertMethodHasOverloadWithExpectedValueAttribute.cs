namespace Funcky.CodeAnalysis;

/// <summary>Marks the assertions on <c>Assert</c> that have an accompanying overload which
/// takes the expected value; the analyzer rule λ1101 prefers that overload.</summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class AssertMethodHasOverloadWithExpectedValueAttribute : Attribute;
