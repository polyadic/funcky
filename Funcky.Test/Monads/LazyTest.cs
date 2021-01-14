using System;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Monads;

namespace Funcky.Test.Monads
{
    public sealed class LazyTest
    {
        [Property]
        public Property LeftIdentityHolds(int input, Func<int, int> func)
        {
            Lazy<int> LazyFunc(int x) => new (func(x));

            return (new Lazy<int>(input).SelectMany(LazyFunc).Value == LazyFunc(input).Value)
                .ToProperty();
        }

        [Property]
        public Property RightIdentityHolds(int input)
        {
            var lazyInput = new Lazy<int>(input);
            return (lazyInput.SelectMany(v => new Lazy<int>(v)).Value == lazyInput.Value)
                .ToProperty();
        }

        [Property]
        public Property AssociativityHolds(int input, Func<int, int> func1, Func<int, int> func2)
        {
            Lazy<int> LazyFunc1(int x) => new (func1(x));
            Lazy<int> LazyFunc2(int x) => new (func2(x));
            Lazy<int> Func1AndFunc2(int x) => LazyFunc1(x).SelectMany(LazyFunc2);

            var lazyInput = new Lazy<int>(input);
            return (lazyInput.SelectMany(LazyFunc1).SelectMany(LazyFunc2).Value == lazyInput.SelectMany(Func1AndFunc2).Value)
                .ToProperty();
        }
    }
}
