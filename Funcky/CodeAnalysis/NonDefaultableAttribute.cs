namespace Funcky.CodeAnalysis;

/// <summary>Structs annotated with this attribute should not be instantiated with <see langword="default"/>.</summary>
[AttributeUsage(AttributeTargets.Struct)]
internal sealed class NonDefaultableAttribute : Attribute;
