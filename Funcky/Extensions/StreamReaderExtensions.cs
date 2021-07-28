using System;
using System.Collections.Generic;
using System.IO;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class StreamReaderExtensions
    {
        public static IEnumerable<string> ReadLines(this StreamReader streamReader)
            => Sequence
                .Generate(GetNextLine(streamReader));

        private static Func<Option<string>> GetNextLine(StreamReader streamReader)
            => ()
                => Option.FromNullable(streamReader.ReadLine());
    }
}
