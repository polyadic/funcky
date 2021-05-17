using System;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Monads;

namespace Funcky.Test.Monads
{
    public sealed partial class ReaderTest
    {
        [Property]
        public Property AssociativityHolds(int environment, Func<int, int> readerFunction)
        {
            var reader = Reader<int>.Return(readerFunction);
            static Reader<int, int> Combined(int number) => Add(number).SelectMany(Times);

            return (reader.SelectMany(Add).SelectMany(Times).Invoke(environment)
                    == reader.SelectMany(Combined).Invoke(environment)).ToProperty();
        }

        [Property]
        public Property RightIdentityHolds(int environment, Func<int, int> readerFunction)
        {
            var reader = Reader<int>.Return(readerFunction);

            return (reader.SelectMany(Reader<int>.Return).Invoke(environment) == reader.Invoke(environment)).ToProperty();
        }

        [Property]
        public Property LeftIdentityHolds(int environment, int value)
        {
            var reader = value.Reader<int, int>();

            return (reader.SelectMany(Add).Invoke(environment) == Add(value).Invoke(environment)).ToProperty();
        }

        private static Reader<int, int> Add(int number)
            => Reader<int>.Return(config => number + config);

        private static Reader<int, int> Times(int number)
            => Reader<int>.Return(config => number * config);
    }
}
