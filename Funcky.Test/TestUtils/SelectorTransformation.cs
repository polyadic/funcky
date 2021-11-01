using Funcky.Test.Extensions.EnumerableExtensions;

namespace Funcky.Test.TestUtils
{
    internal class SelectorTransformation
    {
        public static Func<Option<T>, Option<T>> TransformNullableSelector<T>(Func<T?, T?> selector)
            where T : struct
            => option
                => Option.FromNullable(selector(option.Match(none: (T?)null, some: value => value)));

        public static Func<Person?, Person?> TransformPersonSelector(Func<int?, int?> selector)
            => nullablePerson
                => CreatePersonOrNull(selector(nullablePerson?.Age));

        public static Func<Option<Person>, Option<Person>> TransformOptionPersonSelector(Func<int?, int?> selector)
            => optionalPerson
                => CreatePersonOrNone(selector(optionalPerson.Match(none: (int?)null, some: person => person.Age)));

        private static Person? CreatePersonOrNull(int? transformedAge)
            => transformedAge.HasValue
                ? new Person(transformedAge.Value)
                : null;

        private static Option<Person> CreatePersonOrNone(int? transformedAge)
            => transformedAge.HasValue
                ? Option.Some(new Person(transformedAge.Value))
                : Option<Person>.None;
    }
}
