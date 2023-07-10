namespace Funcky.CodeAnalysis;

/// <summary>Types annotated with this attribute are alternative monads (e.g. <c>Option</c> or <c>Either</c>).</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
internal sealed class AlternativeMonadAttribute : Attribute
{
    public bool MatchHasSuccessStateFirst { get; init; }

    public string? ReturnAlias { get; init; }
}
