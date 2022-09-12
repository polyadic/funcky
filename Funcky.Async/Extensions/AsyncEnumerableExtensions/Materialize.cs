namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Materializes all the items of a lazy <see cref="IEnumerable{TItem}" />. If the underlying sequence is a collection type we do not actively enumerate them.
    /// </summary>
    /// <typeparam name="TItem">Type of the items in the source sequence.</typeparam>
    /// <param name="source">The source sequence can be any <see cref="IEnumerable{TItem}" />.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of the enumerated items.</returns>
    public static async ValueTask<IReadOnlyCollection<TItem>> MaterializeAsync<TItem>(this IAsyncEnumerable<TItem> source, CancellationToken cancellationToken = default)
        => await source.MaterializeAsync(DefaultMaterializationAsync, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Materializes all the items of a lazy <see cref="IEnumerable{TItem}" />. If the underlying sequence is a collection type we do not actively enumerate them.
    /// Via the materialize function you can chose how the enumeration is done when it is needed.
    /// </summary>
    /// <typeparam name="TItem">Type of the items in the source sequence.</typeparam>
    /// <typeparam name="TMaterialization">The type of the materialization target.</typeparam>
    /// <param name="source">The source sequence can be any <see cref="IEnumerable{TItem}" />.</param>
    /// <param name="materializer">A function which materializes a given sequence into a collection.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of the enumerated items.</returns>
    public static async ValueTask<IReadOnlyCollection<TItem>> MaterializeAsync<TItem, TMaterialization>(
        this IAsyncEnumerable<TItem> source,
        Func<IAsyncEnumerable<TItem>, CancellationToken, ValueTask<TMaterialization>> materializer,
        CancellationToken cancellationToken = default)
        where TMaterialization : IReadOnlyCollection<TItem>
        => source switch
        {
            _ => await materializer(source, cancellationToken).ConfigureAwait(false),
        };

    private static async ValueTask<IReadOnlyCollection<TItem>> DefaultMaterializationAsync<TItem>(IAsyncEnumerable<TItem> source, CancellationToken cancellationToken = default)
        => await source.ToListAsync(cancellationToken).ConfigureAwait(false);
}
