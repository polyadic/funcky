#if !INDEX_OF_CHAR_COMPARISONTYPE_SUPPORTED
using System.Globalization;
using static System.Globalization.CultureInfo;

namespace System
{
    internal static class StringExtensions
    {
        // Source: https://github.com/dotnet/runtime/blob/de591a85fb1ea1da2d60ec34b8f3accc12624486/src/libraries/System.Private.CoreLib/src/System/String.Searching.cs#L48-L69
        public static int IndexOf(this string input, char value, StringComparison comparisonType)
            => comparisonType switch
            {
                StringComparison.CurrentCulture => CurrentCulture.CompareInfo.IndexOf(input, value, GetCaseCompareOfComparisonCulture(comparisonType)),
                StringComparison.CurrentCultureIgnoreCase => CurrentCulture.CompareInfo.IndexOf(input, value, GetCaseCompareOfComparisonCulture(comparisonType)),
                StringComparison.InvariantCulture => InvariantCulture.CompareInfo.IndexOf(input, value, GetCaseCompareOfComparisonCulture(comparisonType)),
                StringComparison.InvariantCultureIgnoreCase => InvariantCulture.CompareInfo.IndexOf(input, value, GetCaseCompareOfComparisonCulture(comparisonType)),
                StringComparison.Ordinal => input.IndexOf(value),
                StringComparison.OrdinalIgnoreCase => InvariantCulture.CompareInfo.IndexOf(input, value, CompareOptions.OrdinalIgnoreCase),
                _ => throw new ArgumentException("The string comparison type passed in is currently not supported.", nameof(comparisonType)),
            };

        // Source: https://github.com/dotnet/runtime/blob/de591a85fb1ea1da2d60ec34b8f3accc12624486/src/libraries/System.Private.CoreLib/src/System/String.Comparison.cs#L1027-L1045
        private static CompareOptions GetCaseCompareOfComparisonCulture(StringComparison comparisonType)
            => (CompareOptions)((int)comparisonType & (int)CompareOptions.IgnoreCase);
    }
}
#endif
