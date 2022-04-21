namespace Funcky.CodeAnalysis;

/// <summary>Methods annotated with this attribute should be used with named arguments.</summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class UseWithArgumentNamesAttribute : Attribute
{
}
