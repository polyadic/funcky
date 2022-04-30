using FsCheck;
using FsCheck.Xunit;
using Funcky.FsCheck;

namespace Funcky.Test.Monads;

public sealed partial class EitherTest
{
    public EitherTest()
        => FunckyGenerators.Register();

    [Property]
    public Property AssociativityHolds(
        Either<int, int> input,
        Func<int, Either<int, int>> selectorOne,
        Func<int, Either<int, int>> selectorTwo)
    {
        Either<int, int> CombinedSelector(int x) => selectorOne(x).SelectMany(selectorTwo);

        return (input.SelectMany(selectorOne).SelectMany(selectorTwo) == input.SelectMany(CombinedSelector))
            .ToProperty();
    }

    [Property]
    public Property RightIdentityHolds(Either<int, int> input)
        => (input.SelectMany(Either<int>.Return) == input).ToProperty();

    [Property]
    public Property LeftIdentityHolds(int input, Func<int, Either<int, int>> selector)
        => (Either<int>.Return(input).SelectMany(selector) == selector(input)).ToProperty();
}
