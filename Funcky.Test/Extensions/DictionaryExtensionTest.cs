namespace Funcky.Test.Extensions;

public sealed class DictionaryExtensionTest
{
    [Fact]
    public void GivenADictionaryWhenWeLookForAnExistentValueWithGetValueOrNoneThenTheResultShouldBeASomeOfTheGivenType()
    {
        var dictionary = new Dictionary<string, string> { ["some"] = "value" };
        FunctionalAssert.Some("value", dictionary.GetValueOrNone(key: "some"));
    }

    [Fact]
    public void GivenADictionaryWhenWeLookForAnInexistentValueWithGetValueOrNoneThenTheResultShouldBeANoneOfTheGivenType()
    {
        var dictionary = new Dictionary<string, string> { ["some"] = "value" };
        FunctionalAssert.None(dictionary.GetValueOrNone(readOnlyKey: "none"));
    }
}
