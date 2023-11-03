using System.Diagnostics.CodeAnalysis;

namespace Funcky;

/// <summary>Functions to upcast monad values.</summary>
public static class UpCast<TResult>
    where TResult : notnull
{
    /// <summary>Upcasts an <see cref="Option{TItem}"/>'s item to <typeparamref name="TResult"/>.</summary>
    /// <example>
    /// Upcasting an option's item from <see cref="string"/> to <see cref="object"/>:
    /// <code><![CDATA[
    /// Option<object> result = UpCast<object>.From(Option.Return("hello world"));
    /// ]]></code></example>
    public static Option<TResult> From<TItem>(Option<TItem> option)
        where TItem : TResult
        => option.Select(From);

    /// <summary>Upcasts the right side of an <see cref="Either{TLeft,TRight}"/> to <typeparamref name="TResult"/>.</summary>
    /// <example>
    /// Upcasting an either's right value from <see cref="string"/> to <see cref="object"/>:
    /// <code><![CDATA[
    /// Either<Error, object> result = UpCast<object>.From(Either<Error>.Return("hello world"));
    /// ]]></code></example>
    public static Either<TLeft, TResult> From<TLeft, TRight>(Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : TResult
        => either.Select(From);

    /// <summary>Upcasts the success result of a <see cref="Result{TValidResult}"/> to <typeparamref name="TResult"/>.</summary>
    /// <example>
    /// Upcasting a result's success value from <see cref="string"/> to <see cref="object"/>:
    /// <code><![CDATA[
    /// Result<object> result = UpCast<object>.From(Result.Return("hello world"));
    /// ]]></code></example>
    public static Result<TResult> From<TValidResult>(Result<TValidResult> result)
        where TValidResult : TResult
        => result.Select(From);

    /// <summary>Upcasts the value of a <see cref="Lazy{TValidResult}"/> to <typeparamref name="TResult"/>.</summary>
    /// <example>
    /// Upcasting a lazy's value from <see cref="string"/> to <see cref="object"/>:
    /// <code><![CDATA[
    /// Lazy<object> result = UpCast<object>.From(Lazy.Return("hello world"));
    /// ]]></code></example>
    [SuppressMessage("", "IL2091: DynamicallyAccessedMembersMismatchTypeArgumentTargetsGenericParameter", Justification = "Public parameterless constructor is only used when a Lazy is created without providing a value or func.")]
    public static Lazy<TResult> From<T>(Lazy<T> lazy)
        where T : TResult
        => lazy.Select(From);

    private static TResult From<TValue>(TValue value)
        where TValue : TResult
        => value;
}
