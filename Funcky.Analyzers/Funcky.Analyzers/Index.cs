namespace System;

// Roslyn claims to require System.Index for list patterns, but doesn't use it in the generated code.
internal readonly struct Index
{
    public Index(int value, bool fromEnd = false) => throw null!;

    public int GetOffset(int length) => throw null!;
}
