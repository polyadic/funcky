using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions.StringExtensions;

public sealed class LazySplitTest
{
    [Property]
    public Property SplitLazyWithSingleCharacterWorksTheSameAsSplit(StringNoNullChar text, char separator)
        => (text.Get is null
           || text
            .Get
            .Split(separator)
            .SequenceEqual(text.Get.SplitLazy(separator)))
            .ToProperty();

    [Property]
    public Property SplitLazyWithMultipleCharactersWorksTheSameAsSplit(StringNoNullChar text, char separator, char separator2, char separator3)
        => (text.Get is null
            || text
                .Get
                .Split(separator, separator2, separator3)
                .SequenceEqual(text.Get.SplitLazy(separator, separator2, separator3)))
            .ToProperty();

#if SPLIT_ACCEPTS_STRING_SEPARATOR
    [Property]
    public Property SplitLazyWithSingleStringWorksTheSameAsSplit(StringNoNullChar text, StringNoNullChar separator)
        => (text.Get is null
            || separator.Get is null
            || text
                .Get
                .Split(separator.Get)
                .SequenceEqual(text.Get.SplitLazy(separator.Get)))
            .ToProperty();
#endif

    [Fact]
    public void SplitLazyWithSingleStringSplitsWithAMultiCharacterSeparator()
    {
        var text = "Picard and Kirk and Janeway and Archer";

        Assert.Equal(["Picard", "Kirk", "Janeway", "Archer"], text.SplitLazy("and").Select(Trim));
    }

    [Fact]
    public void SplitLazyWithMultipleStringsSplitsWithMultipleMultiCharacterSeparators()
    {
        // there is no equivalent Split on string which supports multiple strings as separators...
        var text = "Alpha and Beta or Gamma";

        Assert.Equal(["Alpha", "Beta", "Gamma"], text.SplitLazy("and", "or").Select(Trim));
    }

    [Fact]
    public void SplitLazyWithMultipleStringsDoesWorkWithEmptySeparator()
    {
        var text = "Something";

        Assert.Equal(["Something"], text.SplitLazy("and", "or", string.Empty).Select(Trim));
    }

    [Fact]
    public void SplitLazyWithMultipleStringsWorksWithSpecialUnicodeCharactersCorrectly()
    {
        var text = "Something";

        Assert.Equal(["Something"], text.SplitLazy("and", "\u0003"));
    }

    [Fact]
    public void SplitLazyWithMultipleStringsWorksWithAnEmptySource()
    {
        var text = string.Empty;

        Assert.Equal([string.Empty], text.SplitLazy("and", "or"));
    }

    private static string Trim(string s)
        => s.Trim();
}
