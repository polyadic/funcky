using System.Globalization;
using System.Net.Http.Headers;

namespace Funcky.Test.Extensions.ParseExtensions;

public sealed class StaticParseExtensionsTest
{
    [Fact]
    public void TheGeneratedStaticFormParsesLikeTheStringExtension()
    {
        FunctionalAssert.Some(42, int.ParseOrNone("42"));
        FunctionalAssert.None(int.ParseOrNone("forty-two"));
        FunctionalAssert.Some(255, int.ParseOrNone("ff", NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        FunctionalAssert.Some(new Version(1, 2), Version.ParseOrNone("1.2"));
        FunctionalAssert.Some(
            new DateTime(2026, 10, 21),
            DateTime.ParseExactOrNone("2026-10-21", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None));
    }

    [Fact]
    public void TheStaticFormIsAlsoAPlainStaticMethodOnTheEnclosingClass()
        => FunctionalAssert.Some(42, Int32ParseExtensions.ParseOrNone("42"));

    [Fact]
    public void EnumsParseByNameAndIgnoreCaseOnRequest()
    {
        string? nothing = null;

        FunctionalAssert.Some(DayOfWeek.Monday, DayOfWeek.ParseOrNone("Monday"));
        FunctionalAssert.Some(DayOfWeek.Monday, DayOfWeek.ParseOrNone("1"));
        FunctionalAssert.None(DayOfWeek.ParseOrNone("monday"));
        FunctionalAssert.Some(DayOfWeek.Monday, DayOfWeek.ParseOrNone("monday", ignoreCase: true));
        FunctionalAssert.None(DayOfWeek.ParseOrNone(nothing));
    }

#if NETCOREAPP
    [Fact]
    public void NonGenericEnumParsingReturnsTheBoxedMember()
    {
        FunctionalAssert.Some((object)DayOfWeek.Monday, Enum.ParseOrNone(typeof(DayOfWeek), "Monday"));
        FunctionalAssert.Some((object)DayOfWeek.Monday, Enum.ParseOrNone(typeof(DayOfWeek), "monday", ignoreCase: true));
        FunctionalAssert.None(Enum.ParseOrNone(typeof(DayOfWeek), "Funday"));
    }
#endif

#if GENERIC_PARSABLE
    [Fact]
    public void AnyParsableTypeHasTheStaticForm()
    {
        FunctionalAssert.Some(new Celsius(21.5), Celsius.ParseOrNone("21.5", CultureInfo.InvariantCulture));
        FunctionalAssert.Some(new Celsius(21.5), Celsius.ParseOrNone("21.5".AsSpan(), CultureInfo.InvariantCulture));
        FunctionalAssert.None(Celsius.ParseOrNone("warm", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void AnyNumberTypeHasTheStaticFormWithNumberStyles()
    {
        FunctionalAssert.Some((Half)1.5, Half.ParseOrNone("1.5", NumberStyles.Float, CultureInfo.InvariantCulture));
        FunctionalAssert.Some((Half)1.5, Half.ParseOrNone("1.5".AsSpan(), NumberStyles.Float, CultureInfo.InvariantCulture));
        FunctionalAssert.None(Half.ParseOrNone("1.5", NumberStyles.Integer, CultureInfo.InvariantCulture));
    }
#endif

#if UTF8_SPAN_PARSABLE
    [Fact]
    public void AnyUtf8ParsableTypeHasTheStaticForm()
    {
        FunctionalAssert.Some((Half)1.5, Half.ParseOrNone("1.5"u8, CultureInfo.InvariantCulture));
        FunctionalAssert.None(Half.ParseOrNone("warm"u8, CultureInfo.InvariantCulture));
    }
#endif

    [Fact]
    public void CacheControlHeaderValueParsesTheEmptyString()
    {
        var value = FunctionalAssert.Some(CacheControlHeaderValue.ParseOrNone(string.Empty));
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
