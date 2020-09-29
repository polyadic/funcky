using System;
using System.Collections.Generic;
using System.Text;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class WhereNotNullTest
    {
        [Fact]
        public void WhereNotNullRemovesNullReferenceValues()
        {
            var input = new[]
            {
                null,
                "foo",
                null,
                "bar",
                null,
            };
            var expectedResult = new[]
            {
                "foo",
                "bar",
            };
            Assert.Equal(expectedResult, input.WhereNotNull());
        }

        [Fact]
        public void WhereNotNullRemovesNullValueTypeValues()
        {
            var input = new int?[]
            {
                null,
                10,
                null,
                20,
                null,
            };
            var expectedResult = new[]
            {
                10,
                20,
            };
            Assert.Equal(expectedResult, input.WhereNotNull());
        }
    }
}
