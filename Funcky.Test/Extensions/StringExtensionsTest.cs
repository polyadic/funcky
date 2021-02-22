using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Funcky.Test.Extensions
{
    public sealed class StringExtensionsTest
    {
        private const int NumberOfThisParametersInExtensionMethods = 1;

        private const string Haystack = "haystack";
        private const string NonExistingNeedle = "needle";
        private const char NonExistingNeedleChar = 'n';
        private const string ExistingNeedle = "ystack";
        private const char ExistingNeedleChar = 'y';
        private const int NeedlePosition = 2;

        private readonly ITestOutputHelper _testOutputHelper;

        public StringExtensionsTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [MemberData(nameof(InvalidIndexes))]
        public void ReturnsNoneIfNeedleIsNotFound(Option<int> index)
        {
            FunctionalAssert.IsNone(index);
        }

        public static TheoryData<Option<int>> InvalidIndexes()
            => new()
            {
                Haystack.IndexOfOrNone(NonExistingNeedleChar),
                Haystack.IndexOfOrNone(NonExistingNeedleChar, startIndex: 0),
#if INDEX_OF_CHAR_COMPARISONTYPE_SUPPORTED
                Haystack.IndexOfOrNone(NonExistingNeedleChar, StringComparison.InvariantCulture),
#endif
                Haystack.IndexOfOrNone(NonExistingNeedleChar, startIndex: 0, count: 1),
                Haystack.IndexOfOrNone(NonExistingNeedle),
                Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0),
                Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1),
                Haystack.IndexOfOrNone(NonExistingNeedle, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1, StringComparison.InvariantCulture),
                Haystack.IndexOfAnyOrNone(new[] { NonExistingNeedleChar }),
                Haystack.IndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0),
                Haystack.IndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0, count: 1),
                Haystack.LastIndexOfOrNone(NonExistingNeedleChar),
                Haystack.LastIndexOfOrNone(NonExistingNeedleChar, startIndex: 0),
                Haystack.LastIndexOfOrNone(NonExistingNeedleChar, startIndex: 0, count: 1),
                Haystack.LastIndexOfOrNone(NonExistingNeedle),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, StringComparison.InvariantCulture),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0, StringComparison.InvariantCulture),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1, StringComparison.InvariantCulture),
                Haystack.LastIndexOfAnyOrNone(new[] { NonExistingNeedleChar }),
                Haystack.LastIndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0),
                Haystack.LastIndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0, count: 1),
            };

        [Theory]
        [MemberData(nameof(ValidIndexes))]
        public void ReturnsIndexIfNeedleIsFound(Option<int> index)
        {
            FunctionalAssert.IsSome(NeedlePosition, index);
        }

        public static TheoryData<Option<int>> ValidIndexes()
            => new()
            {
                Haystack.IndexOfOrNone(ExistingNeedleChar),
                Haystack.IndexOfOrNone(ExistingNeedleChar, startIndex: 0),
#if INDEX_OF_CHAR_COMPARISONTYPE_SUPPORTED
                Haystack.IndexOfOrNone(ExistingNeedleChar, StringComparison.InvariantCulture),
#endif
                Haystack.IndexOfOrNone(ExistingNeedleChar, startIndex: 0, count: Haystack.Length),
                Haystack.IndexOfOrNone(ExistingNeedle),
                Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0),
                Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0, count: Haystack.Length),
                Haystack.IndexOfOrNone(ExistingNeedle, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0, count: Haystack.Length, StringComparison.InvariantCulture),
                Haystack.IndexOfAnyOrNone(new[] { ExistingNeedleChar }),
                Haystack.IndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: 0),
                Haystack.IndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: 0, count: Haystack.Length),
                Haystack.LastIndexOfOrNone(ExistingNeedleChar),
                Haystack.LastIndexOfOrNone(ExistingNeedleChar, startIndex: Haystack.Length - 1),
                Haystack.LastIndexOfOrNone(ExistingNeedleChar, startIndex: Haystack.Length - 1, count: Haystack.Length),
                Haystack.LastIndexOfOrNone(ExistingNeedle),
                Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1),
                Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1, count: Haystack.Length),
                Haystack.LastIndexOfOrNone(ExistingNeedle, StringComparison.InvariantCulture),
                Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1, StringComparison.InvariantCulture),
                Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1, count: Haystack.Length, StringComparison.InvariantCulture),
                Haystack.LastIndexOfAnyOrNone(new[] { ExistingNeedleChar }),
                Haystack.LastIndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: Haystack.Length - 1),
                Haystack.LastIndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: Haystack.Length - 1, count: Haystack.Length),
            };

        [Fact]
        public void ValidIndexesCoversAllOverloads()
        {
            var overloadCount = GetIndexOfExtensionMethods().Count();
            var validIndexesCount = ValidIndexes().Count();
            Assert.Equal(overloadCount, validIndexesCount);
        }

        [Fact]
        public void InvalidIndexesCoversAllOverloads()
        {
            var overloadCount = GetIndexOfExtensionMethods().Count();
            var invalidIndexesCount = InvalidIndexes().Count();
            Assert.Equal(overloadCount, invalidIndexesCount);
        }

        [SkipOnMonoFact]
        public void AllOverloadsOfIndexOfAreSupported()
        {
            GetIndexOfMethods()
                .Inspect(WriteToTestOutput)
                .Select(GetMatchingExtensionMethod)
                .ForEach(matchingExtensionMethod =>
                {
                    matchingExtensionMethod.AndThen(WriteToTestOutput);
                    FunctionalAssert.IsSome(matchingExtensionMethod);
                });
        }

        [Fact]
        public void SplitLinesOnAnEmptyStringReturnsAnEmptyIEnumerable()
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
        public void ASingleNewLineSplitsIn2EmptyLines()
        {
            var text = "\n";

            Assert.Equal(Enumerable.Repeat(string.Empty, 2), text.SplitLines());
            Assert.Equal(Enumerable.Empty<string>(), text.SplitLines(StringSplitOptions.RemoveEmptyEntries));
        }

        [Fact]
        public void SplitLinesSplitsOnValidNewLineCharacters()
        {
            var text = "this\ntext\r\nis\non\r\nmultiple\nlines";

            Assert.Equal(new[] { "this", "text", "is", "on", "multiple", "lines" }, text.SplitLines());
        }

        private static IEnumerable<MethodInfo> GetIndexOfMethods()
            => typeof(string).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(IsIndexOfMethod);

        private void WriteToTestOutput(object value) => _testOutputHelper.WriteLine(value.ToString());

        private static Option<MethodInfo> GetMatchingExtensionMethod(MethodInfo originalMethod)
        {
            const string extensionMethodSuffix = "OrNone";
            var expectedMethodName = originalMethod.Name + extensionMethodSuffix;
            return GetIndexOfExtensionMethods()
                .Where(m => m.Name == expectedMethodName)
                .Where(m => IsOptionWithItemType(m.ReturnType, originalMethod.ReturnType))
                .Where(m => ArityOfExtensionMethodMatchesRegularMethod(m, originalMethod))
                .FirstOrNone(m => originalMethod.GetParameters()
                    .Zip(m.GetParameters().Skip(NumberOfThisParametersInExtensionMethods), ValueTuple.Create)
                    .All(pair => AreParametersEqual(pair.Item1, pair.Item2)));
        }

        private static IEnumerable<MethodInfo> GetIndexOfExtensionMethods()
            => typeof(StringExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(IsIndexOfMethod);

        private static bool IsIndexOfMethod(MethodInfo method)
            => method.Name.Contains("IndexOf");

        private static bool AreParametersEqual(ParameterInfo expected, ParameterInfo actual)
            => expected.Name == actual.Name &&
               expected.ParameterType == actual.ParameterType &&
               expected.DefaultValue == actual.DefaultValue;

        private static bool IsOptionWithItemType(Type optionType, Type expectedItemType)
            => optionType.GetGenericTypeDefinition() == typeof(Option<>) &&
               optionType.GetGenericArguments().Single() == expectedItemType;

        private static bool ArityOfExtensionMethodMatchesRegularMethod(MethodInfo extensionMethod, MethodInfo regularMethod)
            => extensionMethod.GetParameters().Length - NumberOfThisParametersInExtensionMethods == regularMethod.GetParameters().Length;
    }
}
