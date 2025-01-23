using FsCheck;
using FsCheck.Fluent;

namespace Funcky.Test.TestUtils;

internal static class CheckAssert
{
    public static Property Some<TItem>(TItem expectedValue, Option<TItem> option)
        where TItem : notnull
        => option.Match(none: false, some: value => value.Equals(expectedValue))
            .ToProperty();

    public static Property None<TItem>(Option<TItem> option)
        where TItem : notnull
        => option.Match(none: true, some: False)
            .ToProperty();

    public static Property Equal<T>(T expected, T actual)
        where T : IEquatable<T>
        => expected
            .Equals(actual)
            .ToProperty();

    public static Property Equal<TItem>(Lazy<TItem> expected, Lazy<TItem> actual)
        where TItem : IEquatable<TItem>
        => expected.Value
            .Equals(actual.Value)
            .ToProperty();

    public static Property Equal<TItem>(Reader<TItem, TItem> expected, Reader<TItem, TItem> actual, TItem environment)
        where TItem : IEquatable<TItem>
        => Sequence
            .Return(environment)
            .All(EqualGivenEnvironment(expected, actual))
            .ToProperty();

    private static Func<TItem, bool> EqualGivenEnvironment<TItem>(Reader<TItem, TItem> expected, Reader<TItem, TItem> actual)
        where TItem : IEquatable<TItem>
        => environment
            => expected(environment).Equals(actual(environment));
}
