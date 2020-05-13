using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class ReaderServiceProviderTest
    {
        private interface IGreater
        {
            string GreetUser(string userName);
        }

        [Fact]
        public static async Task BasicTest()
        {
            var sp = new MockServiceProvider();
            sp.RegisterInstance<IGreater>(new Greater());

            var result = await Greet("John").Apply(sp);

            Assert.Equal("Hello, John!", result);
        }

        private static async Reader<string> Greet(string userName)
        {
            var service = await Reader.GetService<IGreater>();
            return service.GreetUser(userName);
        }

        private class Greater : IGreater
        {
            public string GreetUser(string userName)
                => $"Hello, {userName}!";
        }

        private class MockServiceProvider : IServiceProvider
        {
            private readonly Dictionary<Type, object> _storage = new Dictionary<Type, object>();

            public void RegisterInstance<T>(T instance)
                => _storage.Add(typeof(T), instance);

            public object GetService(Type t)
                => _storage.TryGetValue(t, out var result)
                    ? result
                    : throw new Exception($"Type '{t.Name}' is not registered");
        }
    }
}