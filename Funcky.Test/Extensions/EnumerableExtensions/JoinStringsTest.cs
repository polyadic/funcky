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

            Assert.Equal(string.Empty, empty.JoinToString(", "));
            Assert.Equal(string.Empty, empty.JoinToString(','));
        }

        [Fact]
        public void JoiningASetWithExactlyOneElementReturnsTheElementWithoutASeparator()
        {
            var singleElement = Sequence.Return("Alpha");

            Assert.Equal("Alpha", singleElement.JoinToString(", "));
            Assert.Equal("Alpha", singleElement.JoinToString(','));
        }

        [Fact]
        public void JoiningAListOfStringsAddsSeparatorsBetweenTheElements()
        {
            var strings = new List<string> { "Alpha", "Beta", "Gamma" };

            Assert.Equal("Alpha, Beta, Gamma", strings.JoinToString(", "));
            Assert.Equal("Alpha,Beta,Gamma", strings.JoinToString(','));
        }

        [Fact]
        public void JoiningNonStringsReturnASeparatedListToo()
        {
            var strings = new List<int> { 1, 2, 3 };

            Assert.Equal("1, 2, 3", strings.JoinToString(", "));
            Assert.Equal("1,2,3", strings.JoinToString(','));
        }

        [Fact]
        public void NullsAreHandledAsEmptyStringsWhileJoining()
        {
            var strings = new List<string?> { "Alpha", null, "Gamma" };

            Assert.Equal("Alpha, , Gamma", strings.JoinToString(", "));
            Assert.Equal("Alpha,,Gamma", strings.JoinToString(','));
        }
    }
}
