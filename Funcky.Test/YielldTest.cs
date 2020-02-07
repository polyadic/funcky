using System;
using System.Collections.Generic;
using System.Text;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test
{
    public class YielldTest
    {
        [Fact]
        void GivenAnObjectWeCreateAnIEnumerableWithYield()
        {
            AcceptInts(42.Yield());

            Unit unit;
            AcceptUnits(unit.Yield());
        }

        void AcceptInts(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                Assert.Equal(42, value);
            }
        }

        void AcceptUnits(IEnumerable<Unit> units)
        {
            foreach (var unit in units)
            {
                Assert.Equal(new Unit(), unit);
            }
        }
    }
}
