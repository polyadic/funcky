namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// The IEnumerable version of foreach. You can apply an action to each element. This is only useful when you have side effects.
        /// </summary>
        /// <typeparam name="T">the inner type of the enumerable.</typeparam>
        public static Unit ForEach<T>(this IEnumerable<T> elements, Action<T> action)
            => elements
                .ForEach(ActionToUnit(action));

        /// <summary>
        /// The IEnumerable version of foreach. You can apply an <c><![CDATA[Func<T, Unit>]]></c> to each element. This is only useful when you have side effects.
        /// </summary>
        /// <typeparam name="T">the inner type of the enumerable.</typeparam>
        public static Unit ForEach<T>(this IEnumerable<T> elements, Func<T, Unit> action)
            => elements
                .Aggregate(Unit.Value, (_, element) => action(element));
    }
}
