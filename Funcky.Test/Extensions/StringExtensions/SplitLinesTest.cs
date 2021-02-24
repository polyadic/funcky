using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.StringExtensions
{
    public sealed class SplitLinesTest
    {
        [Fact]
        public void SplitLinesOnAnEmptyStringReturnsAnEmptySequence()
        {
            Assert.Equal(Enumerable.Empty<string>(), string.Empty.SplitLines());
        }

        [Fact]
        public void SplitLinesOnAStringWithoutANewLineCharacterReturnsTheString()
        {
            var text = "single line text";

            Assert.Equal(Sequence.Return(text), text.SplitLines());
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\r\n")]
        public void ASingleNewLineSplitsInASingleEmptyLine(string text)
        {
            Assert.Equal(Sequence.Return(string.Empty), text.SplitLines());
        }

        [Theory]
        [InlineData("Two\nlines\n")]
        [InlineData("Two\rlines\r")]
        [InlineData("Two\r\nlines\r\n")]
        [InlineData("Two\nlines\r\n")]
        public void IfANewLineIsAtTheEndOfTheLastLineNoEmptyLineIsAdded(string text)
        {
            Assert.Equal(new[] { "Two", "lines" }, text.SplitLines());
        }

        [Fact]
        public void SplitLinesSplitsOnAllValidNewLineCharacters()
        {
            var text = "this\ntext\r\nis\non\r\nmultiple\rlines";

            Assert.Equal(new[] { "this", "text", "is", "on", "multiple", "lines" }, text.SplitLines());
        }

        [Theory]
        [InlineData("\n\n")]
        [InlineData("\r\r")]
        [InlineData("\r\n\r\n")]
        [InlineData("\n\r\n")]
        public void TwoConsecutiveNewLinesAreNotSwallowed(string text)
        {
            Assert.Equal(Enumerable.Repeat(string.Empty, 2), text.SplitLines());
        }
    }
}
