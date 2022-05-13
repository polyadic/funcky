// ReSharper disable UnusedMember.Global
using FsCheck;
using Result = Funcky.Monads.Result;

namespace Funcky.Test;

internal static class FunckyGenerators
{
    public static void Register() => Arb.Register(typeof(FunckyGenerators));

    public static Arbitrary<Either<int, int>> ArbitraryEitherOfInt()
        => Arb.From(Gen.OneOf(
            Arb.Generate<int>().Select(Either<int, int>.Left),
            Arb.Generate<int>().Select(Either<int, int>.Right)));

    public static Arbitrary<Result<int>> ArbitraryResultOfInt() => Arb.From(Gen.OneOf(
        Arb.Generate<int>().Select(Result.Ok),
        Arb.Generate<string>().Select(message => Result<int>.Error(new EquatableException(message)))));

    public static Arbitrary<(T1 Item1, T2 Item2)> ArbitraryTuple2<T1, T2>()
        => GenerateValueTuple2<T1, T2>()
            .ToArbitrary();

#if PRIORITY_QUEUE
    public static Arbitrary<PriorityQueue<TElement, TPriority>> GeneratePriorityQueue<TElement, TPriority>()
        => GeneratePriorityQueueGenerator<TElement, TPriority>()
            .ToArbitrary();

    private static Gen<PriorityQueue<TElement, TPriority>> GeneratePriorityQueueGenerator<TElement, TPriority>()
         => from values in Arb.Generate<List<(TElement, TPriority)>>()
            select new PriorityQueue<TElement, TPriority>(values);
#endif

    private static Gen<(T1 Item1, T2 Item2)> GenerateValueTuple2<T1, T2>()
        => from value1 in Arb.Generate<T1>()
            from value2 in Arb.Generate<T2>()
            select (value1, value2);

    private sealed class EquatableException : Exception, IEquatable<EquatableException>
    {
        public EquatableException(string? message)
            : base(message)
        {
        }

        public override bool Equals(object? obj)
            => ReferenceEquals(this, obj) || (obj is EquatableException other && Equals(other));

        public bool Equals(EquatableException? other)
            => other is not null && Message == other.Message;

        public override int GetHashCode() => HashCode.Combine(Message);
    }
}
