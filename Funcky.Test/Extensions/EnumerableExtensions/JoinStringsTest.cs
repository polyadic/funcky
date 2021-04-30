// ReSharper disable PossibleMultipleEnumeration

using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class JoinStringsTest
    {
        [Fact]
        public void JoiningAnEmptySetOfStringsReturnsAnEmptyString()
        {
            var empty = Enumerable.Empty<string>();

            Assert.Equal(string.Empty, empty.JoinStrings(", "));
            Assert.Equal(string.Empty, empty.JoinStrings(','));
        }

        [Fact]
        public void JoiningASetWithExactlyOneElementReturnsTheElementWithoutASeparator()
        {
            var singleElement = Sequence.Return("Alpha");

            Assert.Equal("Alpha", singleElement.JoinStrings(", "));
            Assert.Equal("Alpha", singleElement.JoinStrings(','));
        }

        [Fact]
        public void JoiningAListOfStringsAddsSeparatorsBetweenTheElements()
        {
            var strings = new List<string> { "Alpha", "Beta", "Gamma" };

            Assert.Equal("Alpha, Beta, Gamma", strings.JoinStrings(", "));
            Assert.Equal("Alpha,Beta,Gamma", strings.JoinStrings(','));
        }

        [Fact]
        public void JoiningNonStringsReturnASeparatedListToo()
        {
            var strings = new List<int> { 1, 2, 3 };

            Assert.Equal("1, 2, 3", strings.JoinStrings(", "));
            Assert.Equal("1,2,3", strings.JoinStrings(','));
        }

        [Fact]
        public void NullsAreHandledAsEmptyStringsWhileJoining()
        {
            var strings = new List<string?> { "Alpha", null, "Gamma" };

            Assert.Equal("Alpha, , Gamma", strings.JoinStrings(", "));
            Assert.Equal("Alpha,,Gamma", strings.JoinStrings(','));
        }
    }
}
