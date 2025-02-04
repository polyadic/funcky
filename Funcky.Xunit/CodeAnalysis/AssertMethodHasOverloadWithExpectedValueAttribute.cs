namespace Funcky.CodeAnalysis;

/// <summary>Marks <c>FunctionalAssert</c> methods that have an accompanying overload that
/// takes the expected value.</summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class AssertMethodHasOverloadWithExpectedValueAttribute : Attribute;
