namespace Funcky.Test.Extensions;

public sealed class DictionaryExtensionTest
{
    [Fact]
    public void GivenADictionaryWhenWeLookForAnExistentValueWithGetValueOrNoneThenTheResultShouldBeASomeOfTheGivenType()
    {
        var dictionary = new Dictionary<string, string> { ["some"] = "value" };

        var maybe = dictionary.GetValueOrNone(key: "some");

        FunctionalAssert.Some("value", maybe);
    }

    [Fact]
    public void GivenADictionaryWhenWeLookForAnInexistentValueWithGetValueOrNoneThenTheResultShouldBeANoneOfTheGivenType()
    {
        var dictionary = new Dictionary<string, string> { ["some"] = "value" };

        var maybe = dictionary.GetValueOrNone(readOnlyKey: "none");

        FunctionalAssert.None(maybe);
    }
}
