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

    [Fact]
    public void CallingGetValueOrNoneOnADictionaryThatImplementsBothReadonlyAndNonReadonlyInterfacesIsNotACompileError()
    {
        var dictionary = new Dictionary<string, string> { ["some"] = "value" };
        _ = dictionary.GetValueOrNone("some");
    }

#if REMOVE_EXTENSION
    [Fact]
    public void RemoveOrNoneOfAnExistingItemRemovesItemFromDictionaryAndReturnsTheValue()
    {
        IDictionary<string, string> dictionary = new Dictionary<string, string> { ["some"] = "value" };

        var value = dictionary.RemoveOrNone("some");

        Assert.Empty(dictionary);
        FunctionalAssert.Some("value", value);
    }

    [Fact]
    public void RemoveOrNoneOfANonExistingItemReturnsNoneAndLeavesTheDictionaryUnchanegd()
    {
        IDictionary<string, string> dictionary = new Dictionary<string, string> { ["some"] = "value" };

        var value = dictionary.RemoveOrNone("none");

        Assert.NotEmpty(dictionary);
        FunctionalAssert.None(value);
    }
#endif
}
