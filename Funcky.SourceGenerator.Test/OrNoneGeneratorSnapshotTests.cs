namespace Funcky.SourceGenerator.Test
{
    [UsesVerify] // 👈 Adds hooks for Verify into XUnit
    public class OrNoneGeneratorSnapshotTests
    {
        [Fact]
        public Task GenerateSingleMethodWithTheSingleArgumentCandidate()
        {
            const string source = @"using System.Diagnostics.Contracts;
using Funcky.Internal;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(bool), nameof(bool.TryParse))]
        public static partial Option<bool> ParseBooleanOrNone(this string candidate);
    }
}";

            return TestHelper.Verify(source);
        }

        [Fact]
        public Task GenerateSingleMethodWithMultipleArgumentsToForward()
        {
            const string source = @"using System.Diagnostics.Contracts;
using Funcky.Internal;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this string candidate, IFormatProvider provider, DateTimeStyles styles);
    }
}";

            return TestHelper.Verify(source);
        }

        [Fact]
        public Task GenerateMethodWhichHasConstraints()
        {
            const string source = @"using System.Diagnostics.Contracts;
using Funcky.Internal;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(Enum), nameof(Enum.TryParse))]
        public static partial Option<TEnum> ParseEnumOrNone<TEnum>(this string candidate)
            where TEnum : struct;
    }
}";

            return TestHelper.Verify(source);
        }

        [Fact]
        public Task GenerateMultipleMethodsInASingleClass()
        {
            const string source = @"using System.Diagnostics.Contracts;
using Funcky.Internal;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(bool), nameof(bool.TryParse))]
        public static partial Option<bool> ParseBooleanOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this string candidate, IFormatProvider provider, DateTimeStyles styles);
    }
}";

            return TestHelper.Verify(source);
        }
    }
}
