namespace Funcky.CodeAnalysis;

/// <summary>The analyzer will suggest inlining lambda expressions passed to this method when the lambda is «simple».</summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class InlineSimpleLambdaExpressionsAttribute : Attribute
{
}
