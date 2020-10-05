using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class AdjacentGroupByTest
    {
        [Fact]
        public void GivenAnEmptyEnumerableChunkReturnsAnEmptyList()
        {
            var dates = DateGenerator(2020);

            var months = dates.AdjacentGroupBy(date => date.Month);

            Assert.Equal(366, dates.Count());
            Assert.Equal(12, months.Count());
            Assert.Equal(new[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }, months.Select(month => month.Count()));
        }

        private IEnumerable<DateTime> DateGenerator(int startYear)
        {
            var current = new DateTime(startYear, 1, 1);

            while (current.Year == startYear)
            {
                yield return current;
                current = current.AddDays(1);
            }
        }
    }
}
