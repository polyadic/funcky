using System;
using System.Collections.Generic;
using Funcky.Monads;
using Funcky.Xunit;
using Xunit;

namespace Funcky.Test.Monads
{
    public class OptionImplicitConversionTest
    {
        [Fact]
        public void ImplicitFunctionParameterConversionDoesCompile()
        {
            AcceptStringOption("Hello world");
            AcceptIntOption(1337);
            AcceptCovariantOption(Comparer<int>.Default);
        }

        [Fact]
        public void ImplicitReturnDoesCompile()
        {
            ReturnStringOption();
            ReturnIntOption();
        }

        [Fact]
        public void OverloadResoltionPrefersTypeWithoutImplicitConversion()
        {
            Assert.True(OverloadResolution(42));
            Assert.False(OverloadResolution(Option.Some(42)));
        }

        [Fact]
        public void ImplicitAssignmentCompiles()
        {
            Option<int> number = 12;

            var v = FunctionalAssert.IsSome(number);
            Assert.Equal(12, v);
        }

        [Fact]
        public void NullAssignmentThrows()
        {
            Assert.Throws<ArgumentNullException>(TryNullAssignment);
        }

        private void TryNullAssignment()
        {
            Option<string> number = null!;
        }

        private void AcceptStringOption(Option<string> s)
        {
        }

        private void AcceptIntOption(Option<int> i)
        {
        }

        private void AcceptCovariantOption(Option<IComparer<int>> c = default)
        {
        }

        private Option<string> ReturnStringOption()
            => "Hello world!";

        private Option<int> ReturnIntOption()
            => 42;

        private bool OverloadResolution(int i)
            => true;

        private bool OverloadResolution(Option<int> i)
            => false;
    }
}
