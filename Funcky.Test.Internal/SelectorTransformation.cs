using Funcky.Monads;
using Funcky.Test.Internal.Data;

namespace Funcky.Test.Internal;

public class SelectorTransformation
{
    public static Func<Option<T>, Option<T>> TransformNullableSelector<T>(Func<T?, T?> selector)
        where T : struct
        => option
            => Option.FromNullable(selector(option.Match(none: (T?)null, some: value => value)));

    public static Func<Option<T>, ValueTask<Option<T>>> TransformNullableSelector<T>(Func<T?, ValueTask<T?>> selector)
        where T : struct
        => async option
            => Option.FromNullable(await selector(option.Match(none: (T?)null, some: value => value)));

    public static Func<Option<T>, CancellationToken, ValueTask<Option<T>>> TransformNullableSelector<T>(Func<T?, CancellationToken, ValueTask<T?>> selector)
        where T : struct
        => async (option, cancellationToken)
            => Option.FromNullable(await selector(option.Match(none: (T?)null, some: value => value), cancellationToken));

    public static Func<Person?, Person?> TransformPersonSelector(Func<int?, int?> selector)
        => nullablePerson
            => CreatePersonOrNull(selector(nullablePerson?.Age));

    public static Func<Person?, ValueTask<Person?>> TransformPersonSelector(Func<int?, ValueTask<int?>> selector)
        => nullablePerson
            => new ValueTask<Person?>(CreatePersonOrNull(selector(nullablePerson?.Age).Result));

    public static Func<Person?, CancellationToken, ValueTask<Person?>> TransformPersonSelector(Func<int?, CancellationToken, ValueTask<int?>> selector)
        => (nullablePerson, cancellationToken)
            => new ValueTask<Person?>(CreatePersonOrNull(selector(nullablePerson?.Age, cancellationToken).Result));

    public static Func<Option<Person>, Option<Person>> TransformOptionPersonSelector(Func<int?, int?> selector)
        => optionalPerson
            => CreatePersonOrNone(selector(optionalPerson.Match(none: (int?)null, some: person => person.Age)));

    public static Func<Option<Person>, ValueTask<Option<Person>>> TransformOptionPersonSelector(Func<int?, ValueTask<int?>> selector)
        => optionalPerson
            => new ValueTask<Option<Person>>(CreatePersonOrNone(selector(optionalPerson.Match(none: (int?)null, some: person => person.Age)).Result));

    public static Func<Option<Person>, CancellationToken, ValueTask<Option<Person>>> TransformOptionPersonSelector(Func<int?, CancellationToken, ValueTask<int?>> selector)
        => (optionalPerson, cancellationToken)
            => new ValueTask<Option<Person>>(CreatePersonOrNone(selector(optionalPerson.Match(none: (int?)null, some: person => person.Age), cancellationToken).Result));

    private static Person? CreatePersonOrNull(int? transformedAge)
        => transformedAge.HasValue
            ? new Person(transformedAge.Value)
            : null;

    private static Option<Person> CreatePersonOrNone(int? transformedAge)
        => transformedAge.HasValue
            ? Option.Some(new Person(transformedAge.Value))
            : Option<Person>.None;
}
