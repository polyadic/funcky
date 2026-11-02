using Funcky.Monads;

namespace Funcky.Spike.Test;

public sealed class TraitsTest
{
    [Fact]
    public void OneTraverseServesEveryContainerAndEveryEffect()
    {
        // Seq × Option
        var numbers = new Seq<int>([1, 2, 3]);
        var allParsed = numbers.Traverse<SeqKind, OptionKind, int, string>(x => OptionKind.Pure(x.ToString()));
        Assert.Equal(["1", "2", "3"], SeqKind.As(OptionKind.As(allParsed).GetOrElse(new Seq<string>([]))));

        var oneFails = numbers.Traverse<SeqKind, OptionKind, int, int>(x => x == 2 ? OptionKind.Empty<int>() : OptionKind.Pure(x));
        Assert.Equal(Option<Seq<int>>.None, OptionKind.As(oneFails).Select(s => (Seq<int>)s));

        // Seq × Either<string, _>
        var eithers = numbers.Traverse<SeqKind, EitherKind<string>, int, int>(x => x > 2 ? new EitherK<string, int>(Either<string, int>.Left("too big")) : EitherKind<string>.Pure(x));
        Assert.Equal(Either<string, int>.Left("too big"), EitherKind<string>.As(eithers).Select(_ => 0));

        // Option × Either: today OptionExtensions.Traverse(Func<T, Either<L, R>>), here the same method as above.
        var some = OptionKind.Pure(5);
        var traversed = some.Traverse<OptionKind, EitherKind<string>, int, int>(x => EitherKind<string>.Pure(x * 2));
        Assert.Equal(Either<string, int>.Right(10), EitherKind<string>.As(traversed).Select(k => OptionKind.As(k).GetOrElse(0)));
    }

    [Fact]
    public void TheSameGenericCodeRunsOnAllThreeOptionShapes()
    {
        Assert.Equal(42, OptionKind.As(Program<OptionKind>()).GetOrElse(0));
        Assert.Equal(42, UnionOptionKind.As(Program<UnionOptionKind>()).GetOrElse(0));
        Assert.Equal(42, ClosedOptionKind.As(Program<ClosedOptionKind>()).GetOrElse(0));
        Assert.True(ClosedOptionDirectKind.As(Program<ClosedOptionDirectKind>()) is DirectOption<int>.Some(42));
    }

    [Fact]
    public void LiftIsWrittenOncePerArityNotOncePerMonad()
    {
        Assert.Equal(Option.Some(6), OptionKind.As(Generic.Lift<OptionKind, int, int, int, int>((a, b, c) => a + b + c, OptionKind.Pure(1), OptionKind.Pure(2), OptionKind.Pure(3))));
        Assert.Equal(Either<string, int>.Right(3), EitherKind<string>.As(Generic.Lift<EitherKind<string>, int, int, int>((a, b) => a + b, EitherKind<string>.Pure(1), EitherKind<string>.Pure(2))));
        Assert.Equal([11, 12, 21, 22], SeqKind.As(Generic.Lift<SeqKind, int, int, int>((a, b) => a + b, new Seq<int>([10, 20]), new Seq<int>([1, 2]))));
    }

    [Fact]
    public void KleisliCompositionWorksForAnyMonad()
    {
        Func<string, K<OptionKind, int>> parse = s => int.TryParse(s, out var n) ? OptionKind.Pure(n) : OptionKind.Empty<int>();
        Func<int, K<OptionKind, int>> half = n => n % 2 == 0 ? OptionKind.Pure(n / 2) : OptionKind.Empty<int>();
        var parseAndHalve = parse.Compose(half);
        Assert.Equal(Option.Some(21), OptionKind.As(parseAndHalve("42")));
        Assert.Equal(Option<int>.None, OptionKind.As(parseAndHalve("41")));
        Assert.Equal(Option<int>.None, OptionKind.As(parseAndHalve("x")));
    }

    [Fact]
    public void TheStructOptionBoxesOnEveryTraitCallTheDirectClosedRecordDoesNot()
    {
        static long Allocated<M>()
            where M : Monad<M>
        {
            Run<M>();
            var before = GC.GetAllocatedBytesForCurrentThread();
            Run<M>();
            return GC.GetAllocatedBytesForCurrentThread() - before;
        }

        static void Run<M>()
            where M : Monad<M>
        {
            var m = M.Pure(0);
            for (var i = 0; i < 1000; i++)
            {
                m = M.Map(m, static x => x + 1);
            }
        }

        var structOption = Allocated<OptionKind>();
        var unionOption = Allocated<UnionOptionKind>();
        var closedViaWrapper = Allocated<ClosedOptionKind>();
        var closedDirect = Allocated<ClosedOptionDirectKind>();

        // One Some object per step is the floor for any class-based shape; the wrappers add one box per step. The union
        // pays its boxed case where the closed record pays its Some object, so the two come out equal.
        // Exact numbers: TraitBoxingBenchmarks.
        Assert.True(structOption > closedDirect, $"struct {structOption} > direct {closedDirect}");
        Assert.True(closedViaWrapper > closedDirect, $"wrapper {closedViaWrapper} > direct {closedDirect}");
        Assert.Equal(closedViaWrapper, unionOption);
    }

    private static K<M, int> Program<M>()
        where M : Monad<M>
        => from a in M.Pure(20)
           from b in M.Pure(22)
           select a + b;
}
