using Funcky.Extensions;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test.Extensions
{
    public sealed partial class ParseExtensionsTest
    {
        public enum MyEnum
        {
            Cool,
            Warp,
            FortyTwo,
        }

        [Theory]
        [MemberData(nameof(BooleanStrings))]
        public void GivenAStringParseBooleanOrNoneReturnsTheCorrectValue(Option<bool> expected, string input)
        {
            Assert.Equal(expected, input.ParseBooleanOrNone());
        }

        [Theory]
        [MemberData(nameof(EnumStrings))]
        public void GivenAStringParseEnumOrNoneReturnsTheCorrectValue(Option<MyEnum> expected, string input)
        {
            Assert.Equal(expected, input.ParseEnumOrNone<MyEnum>());
        }

        private static TheoryData<Option<bool>, string> BooleanStrings()
            => new()
            {
                { Option<bool>.None(), string.Empty },
                { Option.Some(true), "true" },
                { Option.Some(false), "false" },
                { Option.Some(true), "TrUe" },
                { Option.Some(false), "FalsE" },
                { Option<bool>.None(), "0" },
                { Option<bool>.None(), "1" },
                { Option<bool>.None(), "T" },
                { Option<bool>.None(), "F" },
                { Option<bool>.None(), "falsch" },
                { Option<bool>.None(), "bool" },
                { Option<bool>.None(), "none" },
            };

        private static TheoryData<Option<MyEnum>, string> EnumStrings()
            => new()
            {
                { Option<MyEnum>.None(), string.Empty },
                { Option.Some(MyEnum.Cool), "Cool" },
                { Option.Some(MyEnum.FortyTwo), "FortyTwo" },
                { Option.Some(MyEnum.Warp), "Warp" },
                { Option<MyEnum>.None(), "NotCool" },
                { Option<MyEnum>.None(), "WarpCool" },
                { Option<MyEnum>.None(), "MyEnum.Cool" },
                { Option<MyEnum>.None(), "fortytwo" },
            };
    }
}
