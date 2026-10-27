using System.Globalization;
using System.Net.Http.Headers;

namespace Funcky.Test.Extensions.ParseExtensions;

public sealed class StaticParseExtensionsTest
{
    [Fact]
    public void TheGeneratedStaticFormParsesLikeTheStringExtension()
    {
        Assert.Some(42, int.ParseOrNone("42"));
        Assert.None(int.ParseOrNone("forty-two"));
        Assert.Some(255, int.ParseOrNone("ff", NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        Assert.Some(new Version(1, 2), Version.ParseOrNone("1.2"));
        Assert.Some(
            new DateTime(2026, 10, 21),
            DateTime.ParseExactOrNone("2026-10-21", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None));
    }

    [Fact]
    public void TheStaticFormIsAlsoAPlainStaticMethodOnTheEnclosingClass()
        => Assert.Some(42, Int32ParseExtensions.ParseOrNone("42"));

    [Fact]
    public void EnumsParseByNameAndIgnoreCaseOnRequest()
    {
        string? nothing = null;

        Assert.Some(DayOfWeek.Monday, DayOfWeek.ParseOrNone("Monday"));
        Assert.Some(DayOfWeek.Monday, DayOfWeek.ParseOrNone("1"));
        Assert.None(DayOfWeek.ParseOrNone("monday"));
        Assert.Some(DayOfWeek.Monday, DayOfWeek.ParseOrNone("monday", ignoreCase: true));
        Assert.None(DayOfWeek.ParseOrNone(nothing));
    }

#if NETCOREAPP
    [Fact]
    public void NonGenericEnumParsingReturnsTheBoxedMember()
    {
        Assert.Some((object)DayOfWeek.Monday, Enum.ParseOrNone(typeof(DayOfWeek), "Monday"));
        Assert.Some((object)DayOfWeek.Monday, Enum.ParseOrNone(typeof(DayOfWeek), "monday", ignoreCase: true));
        Assert.None(Enum.ParseOrNone(typeof(DayOfWeek), "Funday"));
    }
#endif

#if GENERIC_PARSABLE
    [Fact]
    public void AnyParsableTypeHasTheStaticForm()
    {
        Assert.Some(new Celsius(21.5), Celsius.ParseOrNone("21.5", CultureInfo.InvariantCulture));
        Assert.Some(new Celsius(21.5), Celsius.ParseOrNone("21.5".AsSpan(), CultureInfo.InvariantCulture));
        Assert.None(Celsius.ParseOrNone("warm", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void AnyNumberTypeHasTheStaticFormWithNumberStyles()
    {
        Assert.Some((Half)1.5, Half.ParseOrNone("1.5", NumberStyles.Float, CultureInfo.InvariantCulture));
        Assert.Some((Half)1.5, Half.ParseOrNone("1.5".AsSpan(), NumberStyles.Float, CultureInfo.InvariantCulture));
        Assert.None(Half.ParseOrNone("1.5", NumberStyles.Integer, CultureInfo.InvariantCulture));
    }
#endif

#if UTF8_SPAN_PARSABLE
    [Fact]
    public void AnyUtf8ParsableTypeHasTheStaticForm()
    {
        Assert.Some((Half)1.5, Half.ParseOrNone("1.5"u8, CultureInfo.InvariantCulture));
        Assert.None(Half.ParseOrNone("warm"u8, CultureInfo.InvariantCulture));
    }
#endif

    [Fact]
    public void CacheControlHeaderValueParsesTheEmptyString()
    {
        var value = Assert.Some(CacheControlHeaderValue.ParseOrNone(string.Empty));
        Assert.Equal(string.Empty, value.ToString());
    }

#if GENERIC_PARSABLE
    private readonly record struct Celsius(double Degrees) : ISpanParsable<Celsius>
    {
        public static Celsius Parse(string s, IFormatProvider? provider)
            => new(double.Parse(s, provider));

        public static bool TryParse(string? s, IFormatProvider? provider, out Celsius result)
            => TryParse(s.AsSpan(), provider, out result);

        public static Celsius Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            => new(double.Parse(s, provider));

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Celsius result)
        {
            var success = double.TryParse(s, NumberStyles.Float, provider, out var degrees);
            result = new Celsius(degrees);
            return success;
        }
    }
#endif
}
