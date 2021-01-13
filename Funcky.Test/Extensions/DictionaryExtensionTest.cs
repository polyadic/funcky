using System.Collections.Generic;
using Funcky.Extensions;
using Funcky.Xunit;
using Xunit;

namespace Funcky.Test.Extensions
{
    public class DictionaryExtensionTest
    {
        [Fact]
        public void GivenADictionaryWhenWeLookForAnExistentValueWithGetValueOrNoneThenTheResultShouldBeASomeOfTheGivenType()
        {
            var dictionary = new Dictionary<string, string> { ["some"] = "value" };

            var maybe = dictionary.GetValueOrNone(key: "some");

            FunctionalAssert.IsSome("value", maybe);
        }

        [Fact]
        public void GivenADictionaryWhenWeLookForAnInexistentValueWithGetValueOrNoneThenTheResultShouldBeANoneOfTheGivenType()
        {
            var dictionary = new Dictionary<string, string> { ["some"] = "value" };

            var maybe = dictionary.GetValueOrNone(readOnlyKey: "none");

            FunctionalAssert.IsNone(maybe);
        }
    }
}
