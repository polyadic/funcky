using System.Linq;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.FunctionalClass
{
    public class IdentityTest
    {
        [Fact]
        public void IdentityReturnsPassedValue()
        {
            const string value = "foo";
            Assert.Equal(value, Identity(value));
        }

        [Fact]
        public void IdentityCanBeUsedInSelect()
        {
            var list = new[] { "foo", "bar", "baz" };
            var mappedList = list.Select(Identity).ToList();
            Assert.Equal(list, mappedList);
        }
    }
}
