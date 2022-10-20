namespace Funcky.Analyzers.Test;

public sealed partial class OptionMatchAnalyzerTest
{
    private const string OptionStubCode =
        """
        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
                where TItem : notnull
            {
                public static Option<TItem> None => default;

                public static implicit operator Option<TItem>(TItem item) => default;

                public TResult Match<TResult>(TResult none, System.Func<TItem, TResult> some) => default!;

                public TResult Match<TResult>(System.Func<TResult> none, System.Func<TItem, TResult> some) => default!;

                public TItem GetOrElse(TItem fallback) => default!;

                public TItem GetOrElse(System.Func<TItem> fallback) => default!;

                public Option<TItem> OrElse(Option<TItem> fallback) => default!;

                public Option<TItem> OrElse(System.Func<Option<TItem>> fallback) => default!;

                public Option<TResult> SelectMany<TResult>(System.Func<TItem, Option<TResult>> selector) where TResult : notnull => default;
            }

            public static class OptionExtensions
            {
                public static TItem? ToNullable<TItem>(this Option<TItem> option, RequireStruct<TItem>? ω = null) where TItem : struct => default;

                public static TItem? ToNullable<TItem>(this Option<TItem> option, RequireClass<TItem>? ω = null) where TItem : class => default;
            }

            public static class Option
            {
                public static Option<TItem> Some<TItem>(TItem value) where TItem : notnull => default;

                public static Option<TItem> Return<TItem>(TItem value) where TItem : notnull => default;
            }

            public sealed class RequireStruct<T> where T : struct { }

            public sealed class RequireClass<T> where T : class { }
        }

        namespace Funcky
        {
            public static class Functional
            {
                public static T Identity<T>(T x) => x;
            }
        }
        """;
}
