namespace Funcky.Test.Monads;

public sealed partial class ReaderTest
{
    private string _sideEffect = string.Empty;

    [Fact]
    public void YouCanApplyEnvironmentToReaderMonad()
    {
        var quoteReader = Quote("test");

        Assert.Equal("'test'", quoteReader(new Configuration('\'')));
    }

    [Fact]
    public void ReaderMonadImplementsSelect()
    {
        var quoteReader = QuotedList(["Alpha", "Beta"]);

        Assert.Equal("`Alpha`, `Beta`", quoteReader(new Configuration('`')));
    }

    [Fact]
    public void ReaderMonadImplementsSelectMany()
    {
        var quoteReader = QuoteMany("start", "middle", "end");

        Assert.Equal(".start..middle..end.", quoteReader(new Configuration('.')));
    }

    [Fact]
    public void ReaderMonadWorksWithAction()
    {
        var unitReader = AffectMember("effect");

        Assert.Equal(string.Empty, _sideEffect);

        _ = unitReader(new Configuration('.'));

        Assert.Equal(".effect.", _sideEffect);
    }

    private Reader<Configuration, Unit> AffectMember(string quotable)
        => Reader<Configuration>.FromAction(config => _sideEffect = $"{config.QuoteChar}{quotable}{config.QuoteChar}");

    private static Reader<Configuration, string> QuoteMany(string start, string middle, string end)
        => from s in Quote(start)
           from m in Quote(middle)
           from e in Quote(end)
           select $"{s}{m}{e}";

    private static Reader<Configuration, string> QuotedList(IEnumerable<string> toQuote)
        => from q in Quote(toQuote)
           select string.Join(", ", q);

    private static Reader<Configuration, IEnumerable<string>> Quote(IEnumerable<string> toQuote)
        => toQuote
            .Select(Quote)
            .Sequence();

    private static Reader<Configuration, string> Quote(string quotable)
        => Reader<Configuration>.FromFunc(config => $"{config.QuoteChar}{quotable}{config.QuoteChar}");

    private record Configuration(char QuoteChar);
}
