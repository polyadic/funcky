#pragma warning disable SA1010 // list patterns are not supported yet
namespace Funcky.Test.Monads;

#if SYSTEM_INDEX_SUPPORTED
public sealed partial class OptionTest
{
    [Fact]
    public void OptionSomeCanBeTestedWithAListPattern()
    {
        var option = Option.Some(1337);

        var test = option switch
        {
            [var some] => some is 1337,
            _ => false,
        };

        Assert.True(test);
    }

    [Fact]
    public void OptionSomeCanBeTestedWithAListPatternInAnIf()
    {
        var option = Option.Some(1337);

        if (option is [_])
        {
        }
        else
        {
            Assert.Fail("Should be some");
        }

        if (option is [var some])
        {
            Assert.Equal(1337, some);
        }
        else
        {
            Assert.Fail("Should be some");
        }
    }

    [Fact]
    public void OptionNoneCanBeTestedWithAListPattern()
    {
        var option = Option<int>.None;

        var test = option switch
        {
            [_] => false,
            _ => true,
        };

        Assert.True(test);
    }

    [Fact]
    public void OptionNoneCanBeTestedWithAListPatternInAnIf()
    {
        var option = Option<int>.None;

        if (option is [])
        {
        }
        else
        {
            Assert.Fail("Should be none");
        }
    }
}
#endif
