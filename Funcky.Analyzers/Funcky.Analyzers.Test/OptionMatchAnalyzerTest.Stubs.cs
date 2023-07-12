namespace Funcky.Analyzers.Test;

public sealed partial class OptionMatchAnalyzerTest
{
    // language=csharp
    private const string OptionStubCode =
        """
        namespace Funcky.Monads
        {
            [Funcky.CodeAnalysis.AlternativeMonad(ReturnAlias = "Some")]
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

        namespace Funcky.CodeAnalysis
        {
            /// <summary>Types annotated with this attribute are alternative monads (e.g. <c>Option</c> or <c>Either</c>).</summary>
            [System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
            internal sealed class AlternativeMonadAttribute : System.Attribute
            {
                public bool MatchHasSuccessStateFirst { get; set; }

                public string ReturnAlias { get; set; } = null!;
            }
        }
        """;
}
