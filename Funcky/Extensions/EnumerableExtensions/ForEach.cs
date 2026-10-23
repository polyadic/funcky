namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// The IEnumerable version of foreach. You can apply an action to each element. This is only useful when you have side effects.
        /// </summary>
        public Unit ForEach(Action<TSource> action)
            => source
                .ForEach(ActionToUnit(action));

        /// <summary>
        /// The IEnumerable version of foreach. You can apply an <c><![CDATA[Func<T, Unit>]]></c> to each element. This is only useful when you have side effects.
        /// </summary>
        public Unit ForEach(Func<TSource, Unit> action)
            => source
                .Aggregate(Unit.Value, (_, element) => action(element));
    }
}
