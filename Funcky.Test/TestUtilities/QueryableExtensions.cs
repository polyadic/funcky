using System.Collections;
using System.Linq.Expressions;

namespace Funcky.Test.TestUtilities;

internal static class QueryableExtensions
{
    public static IQueryable<TSource> PreventAccidentalUseAsEnumerable<TSource>(this IQueryable<TSource> source)
        => new QueryableDisallowingUseAsEnumerable<TSource>(source);

    private sealed class QueryableDisallowingUseAsEnumerable<TSource>(IQueryable<TSource> queryable) : IQueryable<TSource>
    {
        public Type ElementType => queryable.ElementType;

        public Expression Expression => queryable.Expression;

        public IQueryProvider Provider => new QueryProvider(queryable.Provider);

        public IEnumerator<TSource> GetEnumerator() => throw new InvalidOperationException("Queryable should not be used as Enumerable");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class QueryProvider(IQueryProvider provider) : IQueryProvider
        {
            public IQueryable CreateQuery(Expression expression) => throw new NotImplementedException();

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
                => new QueryableDisallowingUseAsEnumerable<TElement>(provider.CreateQuery<TElement>(expression));

            public object? Execute(Expression expression) => throw new NotImplementedException();

            public TResult Execute<TResult>(Expression expression)
                => provider.Execute<TResult>(expression);
        }
    }
}
