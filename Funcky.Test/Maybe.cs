using System.Net.Sockets;
using Xunit;
using System.Linq.Expressions;

namespace Funcky.Test
{
    public class Maybe
    {
        [Fact]
        public void MaybeMonadMatchesNothing()
        {
            var maybe = TryParseHelper.TryParseInt("no number");

            Maybe<bool> isLeet = maybe.Select(m => m == 1337);

            Assert.False(isLeet.Match(false, b => true));
        }

        [Fact]
        public void MaybeMonadJust1337()
        {
            var maybe = TryParseHelper.TryParseInt("1337");

            Assert.Equal(1337, maybe.Match(0, m => m));
        }
    }
}
