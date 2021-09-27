namespace Funcky.Test.FunctionalClass
{
    public sealed class IdentityTest
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
