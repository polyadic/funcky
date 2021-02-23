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

        [Fact]
        public void ASingleNewLineSplitsInASingleEmptyLine()
        {
            var text = "\n";

            Assert.Equal(Sequence.Return(string.Empty), text.SplitLines());
        }

        [Fact]
        public void IfANewLineIsAtTheEndOfTheLastLineNoEmptyLineIsAdded()
        {
            var text = "Two\nlines\n";

            Assert.Equal(new[] { "Two", "lines" }, text.SplitLines());
        }

        [Fact]
        public void SplitLinesSplitsOnAllValidNewLineCharacters()
        {
            var text = "this\ntext\r\nis\non\r\nmultiple\rlines";

            Assert.Equal(new[] { "this", "text", "is", "on", "multiple", "lines" }, text.SplitLines());
        }
    }
}
