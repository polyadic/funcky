namespace Funcky.Spike.Unions;

/// <summary>The "has a value" case shared by the union-shaped options. A struct, so that the case itself never allocates;
/// the union still boxes it, because a C# 15 union stores its case in a single <c>object</c> field.</summary>
public readonly record struct Some<T>(T Value)
    where T : notnull;

/// <summary>The "no value" case shared by the union-shaped options.</summary>
public readonly record struct None;

public readonly record struct Left<T>(T Value)
    where T : notnull;

public readonly record struct Right<T>(T Value)
    where T : notnull;

public readonly record struct Ok<T>(T Value)
    where T : notnull;
