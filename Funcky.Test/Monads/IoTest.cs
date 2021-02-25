using System;
using System.IO;
using System.Text;
using Funcky.Monads;
using Xunit;
using static Funcky.Monads.Factory;

namespace Funcky.Test.Monads
{
    public sealed class IoTest
    {
        [Fact]
        public void Test()
        {
            Io<int> query = from unit1 in Io(() => Console.WriteLine("File path:"))
                            from filePath in Io(Console.ReadLine)
                            from unit2 in Io(() => Console.WriteLine("File encoding:"))
                            from encodingName in Io(Console.ReadLine)
                            let encoding = Encoding.GetEncoding(encodingName)
                            from fileContent in Io(() => File.ReadAllText(filePath, encoding))
                            from unit3 in Io(() => Console.WriteLine("File content:"))
                            from unit4 in Io(() => Console.WriteLine(fileContent))
                            select fileContent.Length;

            int result = query(); // Execute query.
        }
    }
}
