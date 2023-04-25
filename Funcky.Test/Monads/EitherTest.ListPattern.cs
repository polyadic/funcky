#pragma warning disable SA1010 // list patterns are not supported yet
namespace Funcky.Test.Monads;

#if SYSTEM_INDEX_SUPPORTED
public partial class EitherTest
{
    [Fact]
    public void EitherRightCanBeTestedWithAListPattern()
    {
        var option = Either<string, int>.Right(1337);

        var test = option switch
        {
            [null, int right] => right is 1337,
            [_, null] => false,
            _ => false,
        };

        Assert.True(test);
    }

    [Fact]
    public void EitherLeftCanBeTestedWithAListPattern()
    {
        var option = Either<string, int>.Left("Alpha");

        var test = option switch
        {
            [null, _] => false,
            [string left, null] => left is "Alpha",
            _ => false,
        };

        Assert.True(test);
    }

    [Fact]
    public void EitherRightCanBeTestedWithAListPatternInAnIf()
    {
        var option = Either<string, int>.Right(1337);

        if (option is [null, int right])
        {
            Assert.Equal(1337, right);
        }
        else
        {
            Assert.Fail("Should be some");
        }
    }

    [Fact]
    public void EitherLeftCanBeTestedWithAListPatternInAnIf()
    {
        var option = Either<string, int>.Left("Alpha");

        if (option is [string left, null])
        {
            Assert.Equal("Alpha", left);
        }
        else
        {
            Assert.Fail("Should be some");
        }
    }
}
#endif
